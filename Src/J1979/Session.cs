#region License
/* Copyright(c) 2018, Brian Humlicek
 * https://github.com/BrianHumlicek
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

namespace SAE.J1979
{
    public abstract class Session : Common.LiteDisposable
    {
        protected virtual SessionChannel sessionChannel { get; }
        protected virtual IHeader header { get; }
        public J2534.Channel Channel { get { return sessionChannel.Channel; } }
        protected RealtimeQueue<J2534.Message> rxQueue { get { return sessionChannel.RxQueue; } }
        protected IEnumerableCache queryCache { get; } = new IEnumerableCache();
        protected Session(IHeader Header)
        {
            header = Header;
        }
        protected virtual void initializeDefaults()
        {
            Channel.ClearMsgFilters();
        }
        /// <summary>
        /// Request current powertrain diagnostic data
        /// </summary>
        /// <param name="PID">Parameter ID</param>
        /// <returns>32bit bitmap of supported PIDs</returns>
        public ServiceResult Mode01(byte PID)
        {
            return serviceHandler((byte)Mode.REQ_DIAG_DATA, 1, new byte[] { PID });
        }
        /// <summary>
        /// Request powertrain freeze frame data request message definition
        /// </summary>
        /// <param name="PID">Parameter ID</param>
        /// <param name="FRNO">Frame number</param>
        /// <returns>Data record of supported pids</returns>
        public ServiceResult Mode02(byte PID, byte FRNO)
        {
            return serviceHandler((byte)Mode.REQ_FREEZE_FRAME_DATA, 2, new byte[] { PID, FRNO });
        }
        /// <summary>
        /// Request emission-related DTC response message definition
        /// </summary>
        /// <returns>DTC array</returns>
        public ServiceResult Mode03()
        {
            return serviceHandler((byte)Mode.REQ_EMISSION_DIAG_DATA);
        }
        /// <summary>
        /// Clear/reset emission-related diagnostic information response SID
        /// </summary>
        /// <returns>void</returns>
        public ServiceResult Mode04()
        {
            return serviceHandler((byte)Mode.CLEAR_EMISSION_DIAG_DATA);
        }
        /// <summary>
        /// Request oxygen sensor monitoring test results request SID
        /// </summary>
        /// <param name="TID">Test ID</param>
        /// <param name="O2SNO">o2 sensor number</param>
        /// <returns>Data record of test ID</returns>
        public ServiceResult Mode05(TID TID, O2 O2SNO)
        {
            return serviceHandler((byte)Mode.REQ_O2_MON_RESULTS, 2, new byte[] { (byte)TID, (byte)O2SNO });
        }
        /// <summary>
        /// Request on-board monitoring test results for specific monitored systems request SID
        /// </summary>
        /// <param name="TID">Test ID</param>
        /// <returns>Data record of test ID</returns>
        public ServiceResult Mode06(byte TID)
        {
            return serviceHandler((byte)Mode.REQ_O2_MON_RESULTS, 1, new byte[] { TID });
        }
        /// <summary>
        /// Request emission-related diagnostic trouble codes detected during current or last completed driving cycle request SID
        /// </summary>
        /// <returns>DTC array</returns>
        public ServiceResult Mode07()
        {
            return serviceHandler((byte)Mode.REQ_CURRENT_DTC);
        }
        /// <summary>
        /// Request control of on-board device request SID
        /// </summary>
        /// <param name="TID">Test ID</param>
        /// <param name="TIDREC">Data record of test ID</param>
        /// <returns>Data record of test ID</returns>
        public ServiceResult Mode08(byte TID, byte[] TIDREC)
        {
            return serviceHandler((byte)Mode.REQ_SYSTEM_CTL, 1, new byte[] { TID }.Concat(TIDREC));
        }
        /// <summary>
        /// Request vehicle information response SID
        /// </summary>
        /// <param name="INFTYP">Info type</param>
        /// <returns></returns>
        public ServiceResult Mode09(InfoType INFTYP)
        {
            return serviceHandler((byte)Mode.REQ_VEHICLE_INFO, 1, new byte[] { (byte)INFTYP });
        }
        protected virtual ServiceResult serviceHandler(byte Mode, int NumOfParams = 0, IEnumerable<byte> Data = null)
        {
            var Message = header.Tx.ConcatByte(Mode);
            if (Data != null) Message = Message.Concat(Data);

            var ResponseQuery = queryCache.Resolve(Mode, () => rxQueue.GetEnumerable(300, true).DecueueWhere(successPredicate(Mode)));

            lock (ResponseQuery)
            {
                Channel.SendMessage(Message);
                //var t = Message.ToArray();
                //if (t[3] == 0x27 && t[4] == 0x02)
                //{
                //    var a = 5;
                //}
                var Response = ResponseQuery.FirstOrDefault();

                var Offset = header.Rx.Length + 1 + NumOfParams;

                if (Response != null) return new ServiceResult(new ArraySegment<byte>(Response.Data, Offset, Response.Data.Length - Offset).ToArray());

                Response = rxQueue.GetEnumerable(0).DecueueWhere(failPredicate(Mode)).FirstOrDefault();

                if (Response != null) return new ServiceResult((Response)Response.Data.Last());
            }

            throw new Exception($"No response from module for service mode {Mode}!");
        }
        protected virtual Predicate<J2534.Message> successPredicate(byte Mode)
        {
            throw new NotImplementedException();
        }
        protected virtual Predicate<J2534.Message> failPredicate(byte Mode)
        {
            throw new NotImplementedException();
        }
        protected override void DisposeManaged()
        {
            sessionChannel?.Dispose();
        }
        protected class IEnumerableCache
        {
            private Dictionary<byte, IEnumerable<J2534.Message>> cache { get; } = new Dictionary<byte, IEnumerable<J2534.Message>>();
            public IEnumerable<J2534.Message> Resolve(byte Mode, Func<IEnumerable<J2534.Message>> NewEnumerable)
            {
                if (!cache.ContainsKey(Mode))
                    cache.Add(Mode, NewEnumerable());
                return cache[Mode];
            }
        }
    }
}
