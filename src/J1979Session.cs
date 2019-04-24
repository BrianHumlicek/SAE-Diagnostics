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
using RealtimeQueue;
using SAE.BlockingQueue;

namespace SAE.J1979
{
    public abstract class J1979Session : Common.ManagedDisposable
    {
        protected SessionChannel sessionChannel { get; }
        protected IHeader header { get; }
        public J2534.Channel Channel { get { return sessionChannel.Channel; } }
        protected BlockingQueue<J2534.Message> rxQueue { get; }

        protected J1979Session(IHeader header, SessionChannel sessionChannel)
        {
            this.header = header;
            this.sessionChannel = sessionChannel;
            rxQueue = sessionChannel.RxQueue.GetConsumingEnumerable();
            rxQueue.BlockingTimeout = 250;
        }
        protected virtual void initializeChannel()
        {
            Channel.ClearMsgFilters();
            Channel.ClearPeriodicMsgs();
            //Channel.ClearFunctMsgLookupTable();
            Channel.ClearRxBuffer();
            Channel.ClearTxBuffer();
        }
        /// <summary>
        /// Request current powertrain diagnostic data
        /// </summary>
        /// <param name="PID">Parameter ID</param>
        /// <returns>32bit bitmap of supported PIDs</returns>
        public ServiceResult Mode01(byte PID)
        {
            return serviceTransaction((byte)J1979Mode.REQ_DIAG_DATA, 1, new byte[] { PID });
        }
        /// <summary>
        /// Request powertrain freeze frame data request message definition
        /// </summary>
        /// <param name="PID">Parameter ID</param>
        /// <param name="FRNO">Frame number</param>
        /// <returns>Data record of supported pids</returns>
        public ServiceResult Mode02(byte PID, byte FRNO)
        {
            return serviceTransaction((byte)J1979Mode.REQ_FREEZE_FRAME_DATA, 2, new byte[] { PID, FRNO });
        }
        /// <summary>
        /// Request emission-related DTC response message definition
        /// </summary>
        /// <returns>DTC array</returns>
        public ServiceResult Mode03()
        {
            return serviceTransaction((byte)J1979Mode.REQ_EMISSION_DIAG_DATA);
        }
        /// <summary>
        /// Clear/reset emission-related diagnostic information response SID
        /// </summary>
        /// <returns>void</returns>
        public ServiceResult Mode04()
        {
            return serviceTransaction((byte)J1979Mode.CLEAR_EMISSION_DIAG_DATA);
        }
        /// <summary>
        /// Request oxygen sensor monitoring test results request SID
        /// </summary>
        /// <param name="TID">Test ID</param>
        /// <param name="O2SNO">o2 sensor number</param>
        /// <returns>Data record of test ID</returns>
        public ServiceResult Mode05(TID TID, O2 O2SNO)
        {
            return serviceTransaction((byte)J1979Mode.REQ_O2_MON_RESULTS, 2, new byte[] { (byte)TID, (byte)O2SNO });
        }
        /// <summary>
        /// Request on-board monitoring test results for specific monitored systems request SID
        /// </summary>
        /// <param name="TID">Test ID</param>
        /// <returns>Data record of test ID</returns>
        public ServiceResult Mode06(byte TID)
        {
            return serviceTransaction((byte)J1979Mode.REQ_O2_MON_RESULTS, 1, new byte[] { TID });
        }
        /// <summary>
        /// Request emission-related diagnostic trouble codes detected during current or last completed driving cycle request SID
        /// </summary>
        /// <returns>DTC array</returns>
        public ServiceResult Mode07()
        {
            return serviceTransaction((byte)J1979Mode.REQ_CURRENT_DTC);
        }
        /// <summary>
        /// Request control of on-board device request SID
        /// </summary>
        /// <param name="TID">Test ID</param>
        /// <param name="TIDREC">Data record of test ID</param>
        /// <returns>Data record of test ID</returns>
        public ServiceResult Mode08(byte TID, byte[] TIDREC)
        {
            return serviceTransaction(Mode:             (byte)J1979Mode.REQ_SYSTEM_CTL,
                                      ResultDataOffset: 1,
                                      Data:             new byte[] { TID }.Concat(TIDREC));
        }
        /// <summary>
        /// Request vehicle information response SID
        /// </summary>
        /// <param name="INFTYP">Info type</param>
        /// <returns></returns>
        public ServiceResult Mode09(InfoType INFTYP)
        {
            return serviceTransaction(Mode:             (byte)J1979Mode.REQ_VEHICLE_INFO,
                                      ResultDataOffset: 1,
                                      Data:             new byte[] { (byte)INFTYP });
        }
        protected virtual ServiceResult serviceTransaction(byte Mode, int ResultDataOffset = 0, IEnumerable<byte> Data = null)
        {
            var Message = header.Tx.ConcatByte(Mode);
            if (Data != null) Message = Message.Concat(Data);

            Channel.SendMessage(Message);

            var Response = rxQueue.Where(successMessage(Mode)).RemoveFrom(rxQueue).FirstOrDefault();

            if (Response != null)
            {
                var Offset = header.Rx.Length + 1 + ResultDataOffset;
                return new ServiceResult(new ArraySegment<byte>(Response.Data, Offset, Response.Data.Length - Offset));
            }

            rxQueue.Unconsume();

            Response = rxQueue.Where(failMessage(Mode)).RemoveFrom(rxQueue).FirstOrDefault();

            if (Response != null)
            {
                return new ServiceResult((ExitCode)Response.Data.Last());
            }

            throw new Exception($"No response from module for service mode {Mode}!");
        }

        protected abstract Func<J2534.Message, bool> successMessage(byte Mode);
        protected abstract Func<J2534.Message, bool> failMessage(byte Mode);

        public override string ToString()
        {
            return Channel.ToString();
        }

        protected override void DisposeManaged()
        {
            sessionChannel?.Dispose();
        }
    }
}
