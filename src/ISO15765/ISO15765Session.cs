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

 namespace SAE.J1979.ISO15765
{
    public class ISO15765Session : J1979Session
    {
        public ISO15765Session(J2534.Device Device) : base(new ISO15765Header(), SessionChannel.Factory.Create(Device, J2534.Protocol.ISO15765, J2534.Baud.ISO15765, J2534.ConnectFlag.NONE))
        {
            if (!sessionChannel.IsInitialized)
            {
                initializeChannel();
                sessionChannel.IsInitialized = true;
            }
        }
        protected override void initializeChannel()
        {
            base.initializeChannel();
            for (byte addrLow = 0xE0; addrLow < 0xE1; addrLow++)
            {
                Channel.StartMsgFilter(new J2534.MessageFilter(J2534.UserFilterType.STANDARDISO15765,
                                                               new byte[4] { 0x00, 0x00, 0x07, addrLow }));
                //Channel.StartMsgFilter(new J2534.MessageFilter(J2534.UserFilterType.PASS,
                //                                               new byte[4] { 0x00, 0x00, 0x07, addrLow }));

            }
            Channel.SetConfig(J2534.Parameter.LOOP_BACK, 0);
            //Are these ISO15765 or Ford specific?
            //Channel.SetConfig(J2534.Parameter.ISO15765_BS, 0);
            //Channel.SetConfig(J2534.Parameter.ISO15765_STMIN, 0);
            Channel.DefaultTxFlag = J2534.TxFlag.ISO15765_FRAME_PAD;
        }
        protected new ISO15765Header header
        {
            get { return (ISO15765Header)base.header; }
        }
        protected override Func<J2534.Message, bool> successMessage(byte Mode)
        {
            return new Func<J2534.Message, bool>(Message =>
            {
                if ((Message?.Data?.Length ?? 0) > 4 &&
                    Message.Data[4] == (Mode ^ 0x40) &&
                    Message.Data[3] == base.header.Rx[3] &&
                    Message.Data[2] == base.header.Rx[2]) return true;
                return false;
            });
        }
        protected override Func<J2534.Message, bool> failMessage(byte Mode)
        {
            return new Func<J2534.Message, bool>(Message =>
            {
                if ((Message?.Data?.Length ?? 0) > 5 &&
                    Message.Data[5] == (byte)Mode &&
                    Message.Data[4] == (byte)J1979.J1979Mode.GENERAL_RESPONSE &&
                    Message.Data[3] == base.header.Rx[3] &&
                    Message.Data[2] == base.header.Rx[2]) return true;
                return false;
            });
        }

    }
}
