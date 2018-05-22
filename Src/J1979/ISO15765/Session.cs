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

 namespace SAE.J1979.ISO15765
{
    public class Session : J1979.Session
    {
        protected override SessionChannel sessionChannel { get; }

        public Session(J2534.Device Device) : base(new Header())
        {
            sessionChannel = SessionChannelFactory.GetSessionChannel(Device, J2534.Protocol.ISO15765, J2534.Baud.ISO15765, J2534.ConnectFlag.NONE);
            if (!sessionChannel.IsInitialized)
            {
                InitializeDefaults();
                sessionChannel.IsInitialized = true;
            }
        }
        public override void InitializeDefaults()
        {
            base.InitializeDefaults();
            for (int i = 0; i < 8; i++)
                Channel.StartMsgFilter(new J2534.MessageFilter(J2534.UserFilterType.STANDARDISO15765,
                                                               new byte[4] { 0x00, 0x00, 0x07, (byte)(0xE0 + i) }));
            Channel.SetConfig(J2534.Parameter.LOOP_BACK, 0);
            //Are these ISO15765 or Ford specific?
            Channel.SetConfig(J2534.Parameter.ISO15765_BS, 0);
            Channel.SetConfig(J2534.Parameter.ISO15765_STMIN, 0);
        }
        protected new Header header
        {
            get { return (Header)base.header; }
        }
        protected override Predicate<J2534.Message> successPredicate(byte Mode)
        {
            return new Predicate<J2534.Message>(Message =>
            {
                if ((Message?.Data?.Length ?? 0) > 4 &&
                    Message.Data[4] == (Mode ^ 0x40) &&
                    Message.Data[3] == base.header.Rx[3] &&
                    Message.Data[2] == base.header.Rx[2]) return true;
                return false;
            });
        }
        protected override Predicate<J2534.Message> failPredicate(byte Mode)
        {
            return new Predicate<J2534.Message>(Message =>
            {
                if ((Message?.Data?.Length ?? 0) > 5 &&
                    Message.Data[5] == (byte)Mode &&
                    Message.Data[4] == (byte)J1979.Mode.GENERAL_RESPONSE &&
                    Message.Data[3] == base.header.Rx[3] &&
                    Message.Data[2] == base.header.Rx[2]) return true;
                return false;
            });
        }

    }
}
