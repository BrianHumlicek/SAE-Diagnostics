#region Copyright
/* Copyright(c) 2018, Brian Humlicek
 * https://github.com/BrianHumlicek
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sub-license, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
 #endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;

namespace SAE.J1979.J2190
{
    public class Ford_CAN_Session : J1979.ISO15765.Session
    {

        public Ford_CAN_Session(J2534.Device Device) : base(Device)
        {

        }
        public void FEPSOn()
        {
            Channel.SetProgrammingVoltage(J2534.Pin.PIN_13, 18000);
        }

        public void FEPSOff()
        {
            Channel.SetProgrammingVoltage(J2534.Pin.PIN_13, -1);
        }
        public void StartTesterPresentMessage()
        {
            //0x3E 0x01 gets a response from the module.  Anything else does not.
            J2534.PeriodicMessage TesterPresent = new J2534.PeriodicMessage(3000, header.Tx.Concat(new byte[2] { 0x3E, 0x02 }).ToArray(), J2534.TxFlag.NONE);
            Channel.StartPeriodicMessage(TesterPresent);
        }
        /// <summary>
        /// Initiate diagnostic operation
        /// </summary>
        /// <param name="SessionState">Operational state to enter</param>
        /// <returns></returns>
        public ServiceResult Mode10(DiagnosticState SessionState)
        {
            try
            {
                return serviceTransaction((byte)Mode.INITIATE_DIAG_OP, 1, new byte[] { (byte)SessionState });
            }
            catch
            {
                if (SessionState == DiagnosticState.IVFER) return null;
                throw;
            }
        }
        /// <summary>
        /// Module Reset
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public ServiceResult Mode11(int Address)
        {
            throw new NotImplementedException();
            return serviceTransaction((byte)Mode.DATA_BY_ADDRESS, 2, new byte[] { Address.Byte3(), Address.Byte2(), Address.Byte1(), Address.Byte0() });
        }
        /// <summary>
        /// Request data by memory address
        /// </summary>
        /// <param name="Address">Address (24bit)</param>
        /// <returns></returns>
        public ServiceResult Mode23(int Address, int Length)
        {
            return serviceTransaction((byte)Mode.DATA_BY_ADDRESS, 2, new byte[] { Address.Byte3(), Address.Byte2(), Address.Byte1(), Address.Byte0(), Length.Byte1(), Length.Byte0() });
        }
        /// <summary>
        /// Request security access
        /// </summary>
        /// <param name="Step">Step in sequence</param>
        /// <param name="Keys">Key bytes (optional)</param>
        /// <returns></returns>
        public ServiceResult Mode27(byte Step, byte[] Keys = null)
        {
            IEnumerable<byte> Data = new byte[] { Step };
            if (Keys != null) Data = Data.Concat(Keys);

            return serviceTransaction((byte)Mode.REQ_SECURITY_ACCESS, 1, Data);
        }
        /// <summary>
        /// Start diagnostic routine
        /// </summary>
        /// <param name="Routine">Routine to execute</param>
        /// <param name="RoutineData">Data (optional)</param>
        /// <returns></returns>
        public ServiceResult Mode31(DiagRoutine Routine, byte[] RoutineData = null)
        {
            IEnumerable<byte> Data = new byte[] { (byte)Routine };
            if (RoutineData != null) Data = Data.Concat(RoutineData);

            return serviceTransaction((byte)Mode.START_DIAG_ROUTINE_BY_NUMBER, 1, Data);
        }
        /// <summary>
        /// Stop diagnostic routine
        /// </summary>
        /// <param name="Routine">Routine to stop</param>
        /// <returns></returns>
        public ServiceResult Mode32(DiagRoutine Routine)
        {
            return serviceTransaction((byte)Mode.STOP_DIAG_ROUTINE_BY_NUMBER, 1, new byte[] { (byte)Routine });
        }
        /// <summary>
        /// Request tool to module transfer
        /// </summary>
        /// <param name="Length">Length in bytes of transfer</param>
        /// <param name="Address">Starting address of transfer block</param>
        /// <returns></returns>
        public ServiceResult Mode34(int Address, int Length)
        {
            return serviceTransaction((byte)Mode.REQ_DOWNLOAD, 2, new byte[] { 0x80 /*Only format 0x80 is supported*/, Length.Byte1(), Length.Byte0(), Address.Byte2(), Address.Byte1(), Address.Byte0() });
        }
        /// <summary>
        /// Request module to tool transfer
        /// </summary>
        /// <param name="Length">Length in bytes of transfer</param>
        /// <param name="Address">Starting address of transfer block</param>
        /// <returns></returns>
        public ServiceResult Mode35(int Address, int Length)
        {
            return serviceTransaction((byte)Mode.REQ_UPLOAD, 2, new byte[] { 0x80, Length.Byte1(), Length.Byte0(), Address.Byte2(), Address.Byte1(), Address.Byte0() });
        }
        /// <summary>
        /// Data transfer receive
        /// </summary>
        /// <param name="NumOfMessages">Number of expected messages</param>
        /// <returns></returns>
        public IEnumerable<ArraySegment<byte>> Mode36(int NumOfMessages)
        {
            var ResponseQuery = queryCache.Resolve((byte)Mode.DATA_TRANSFER, () => rxQueue.DecueueWhere(successPredicate((byte)Mode.DATA_TRANSFER)));

            lock (ResponseQuery)
            {
                var result = ResponseQuery.Take(NumOfMessages).Select(msg => new ArraySegment<byte>(msg.Data, header.Rx.Length + 1, 6)).ToArray();
                if (result.Length == NumOfMessages) return result;
            }

            throw new Exception($"Too few messages in block data transfer!");
        }
        /// <summary>
        /// Data transfer transmit
        /// </summary>
        /// <param name="Data">Data to send</param>
        public void Mode36(ArraySegment<byte>[] Data)
        {
            Channel.SendMessages(Data.Select(dat => header.Tx.ConcatByte((byte)Mode.DATA_TRANSFER).Concat(dat)).ToArray());
        }
        public ServiceResult Mode37()
        {
            return serviceTransaction((byte)Mode.TRANSFER_ROUTINE_EXIT, 1, new byte[] { 0x80 });
        }
        public ServiceResult ModeA0(IEnumerable<byte[]> PacketList)
        {
            throw new NotImplementedException();
        }
        public ServiceResult ModeA0()
        {
            throw new NotImplementedException();
        }
        public ServiceResult ModeA1(IEnumerable<byte[]> PacketList)
        {
            throw new NotImplementedException();
        }
    }
    public enum DiagnosticState
    {
        Normal = 0x81,
        STATE82 = 0x82, //Not sure what this is, but I know its necessary
        IVFER = 0x85
    }
}
