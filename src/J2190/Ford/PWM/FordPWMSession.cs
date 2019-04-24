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
using System.Threading;
using SAE.BlockingQueue;
using Common.Extensions;
//using SAE.J1979.J2190.Ford;
//using SAE.J1979.J2190.Ford.PWM.Definitions;

namespace SAE.J1979.J2190.Ford.PWM
{
    public class FordPWMSession : J1979.J1850.PWMSession
    {
        private byte[] broadcastmessage;

        public FordPWMSession(J2534.Device Device) : base(Device)
        {

        }
        /// <summary>
        /// Initialize for in-vehicle flash eeprom reprogramming
        /// </summary>
        public void InitializeIVFER()
        {
            Channel.SetConfig(J2534.Parameter.DATA_RATE, (int)J2534.Baud.J1850PWM_41600);
            Channel.ClearMsgFilters();
            Channel.ClearFunctMsgLookupTable();
            Channel.AddToFunctMsgLookupTable(0x05);
            Channel.StartMsgFilter(new J2534.MessageFilter()
            {
                Mask = new byte[] { 0x00, 0x00, 0x00, 0xFF, 0xFF },
                Pattern = new byte[] { 0x00, 0x00, 0x00, 0x7F, 0x3F },
                FilterType = J2534.Filter.BLOCK_FILTER
            });
            Channel.StartMsgFilter(new J2534.MessageFilter()
            {
                Mask = new byte[] { 0x00, 0xFF, 0x00 },
                Pattern = new byte[] { 0x00, 0x05, 0x00 },
                FilterType = J2534.Filter.PASS_FILTER
            });
            Channel.StartMsgFilter(new J2534.MessageFilter()
            {
                Mask = new byte[] { 0x00, 0xFF, 0x00 },
                Pattern = new byte[] { 0x00, 0xF1, 0x00 },
                FilterType = J2534.Filter.PASS_FILTER
            });
            Channel.ClearRxBuffer();
        }

        public bool BroadcastListen()
        {
            var broadcastMessageFilter = rxQueue.Where(msg =>
            {
                if (msg.Data.Length < 5) return false;
                if (msg.Data[1] != 0x05) return false;
                if (msg.Data[2] != 0x10) return false;
                if (msg.Data[3] != 0x04) return false;
                if (msg.Data[4] != 0x00) return false;
                return true;
            });

            do
            {
                broadcastmessage = broadcastMessageFilter.FirstOrDefault()?.Data;
            } while (broadcastmessage == null);

            return true;
        }

        public void HighSpeedMode()
        {
            Channel.SetConfig(J2534.Parameter.DATA_RATE, (int)J2534.Baud.J1850PWM_83200);
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
            J2534.PeriodicMessage TesterPresent = new J2534.PeriodicMessage(3000, header.Tx.ConcatByte(0x3F).ToArray(), J2534.TxFlag.NONE);
            Channel.StartPeriodicMessage(TesterPresent);
        }

        /// <summary>
        /// Request data by memory address
        /// </summary>
        /// <param name="Address">Address (24bit)</param>
        /// <returns></returns>
        public ServiceResult Mode23(int Address)
        {
            return serviceTransaction((byte)J2190Mode.DATA_BY_ADDRESS, 2, new byte[] { Address.Byte2(), Address.Byte1(), Address.Byte0() });
        }
        /// <summary>
        /// Request security access
        /// </summary>
        /// <param name="Routine">Step in sequence</param>
        /// <param name="Keys">Key bytes (optional)</param>
        /// <returns></returns>
        public ServiceResult Mode27(SecurityChallenge Routine, byte[] Keys = null)
        {
            IEnumerable<byte> Data = new byte[] { (byte)Routine };
            if (Keys != null) Data = Data.Concat(Keys);

            return serviceTransaction((byte)J2190Mode.REQ_SECURITY_ACCESS, 1, Data);
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

            return serviceTransaction((byte)J2190Mode.START_DIAG_ROUTINE_BY_NUMBER, 1, Data);
        }
        /// <summary>
        /// Stop diagnostic routine
        /// </summary>
        /// <param name="Routine">Routine to stop</param>
        /// <returns></returns>
        public ServiceResult Mode32(DiagRoutine Routine)
        {
            return serviceTransaction((byte)J2190Mode.STOP_DIAG_ROUTINE_BY_NUMBER, 1, new byte[] { (byte)Routine });
        }
        /// <summary>
        /// Request tool to module transfer
        /// </summary>
        /// <param name="Length">Length in bytes of transfer</param>
        /// <param name="Address">Starting address of transfer block</param>
        /// <returns></returns>
        public ServiceResult Mode34(int Length, int Address)
        {
            return serviceTransaction((byte)J2190Mode.REQ_DOWNLOAD, 2, new byte[] { 0x80 /*Only format 0x80 is supported*/, Length.Byte1(), Length.Byte0(), Address.Byte2(), Address.Byte1(), Address.Byte0() });
        }
        /// <summary>
        /// Request module to tool transfer
        /// </summary>
        /// <param name="Length">Length in bytes of transfer</param>
        /// <param name="Address">Starting address of transfer block</param>
        /// <returns></returns>
        public ServiceResult Mode35(int Length, int Address)
        {
            return serviceTransaction((byte)J2190Mode.REQ_UPLOAD, 2, new byte[] { 0x80, Length.Byte1(), Length.Byte0(), Address.Byte2(), Address.Byte1(), Address.Byte0() });
        }
        /// <summary>
        /// Data transfer receive
        /// </summary>
        /// <param name="NumOfMessages">Number of expected messages</param>
        /// <returns></returns>
        public IEnumerable<ArraySegment<byte>> Mode36(int NumOfMessages)
        {
            var result = rxQueue.Where(successMessage((byte)J2190Mode.DATA_TRANSFER))
                                .Take(NumOfMessages)
                                .RemoveFrom(rxQueue)
                                .Select(msg => new ArraySegment<byte>(msg.Data, header.Rx.Length + 1, 6))
                                .ToArray();
            if (result.Length == NumOfMessages) return result;

            throw new Exception($"Too few messages in block data transfer!");
        }
        /// <summary>
        /// Data transfer transmit
        /// </summary>
        /// <param name="Data">Data to send</param>
        public void Mode36(IEnumerable<ArraySegment<byte>> Data)
        {
            Channel.SendMessages(Data.Select(dat => header.Tx.ConcatByte((byte)J2190Mode.DATA_TRANSFER).Concat(dat)).ToArray());
        }
        public ServiceResult Mode37()
        {
            return serviceTransaction((byte)J2190Mode.TRANSFER_ROUTINE_EXIT, 1, new byte[] { 0x80 });
        }
        public ServiceResult Mode2A(IEnumerable<byte[]> PacketList)
        {
            throw new NotImplementedException();
        }
        public ServiceResult Mode2A()
        {
            throw new NotImplementedException();
        }
        public ServiceResult Mode2C(IEnumerable<byte[]> PacketList)
        {
            throw new NotImplementedException();
        }
    }
    public class RapidPacketList
    {
        PID[] PayloadList;
        //public IEnumerable<byte[]> Mode2ADefinitions()
        //{
        //    return new byte[] { 0x00 };
        //}
    }
}
