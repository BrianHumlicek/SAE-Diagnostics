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

 namespace SAE.ISO15765
{
    class Session : J1979.Session
    {
        protected override SessionChannel sessionChannel { get; }
        public Session(J2534.Device Device) : base(new Header())
        {
            sessionChannel = SessionChannelFactory.GetSessionChannel(Device, J2534.Protocol.ISO15765, J2534.Baud.ISO15765, J2534.ConnectFlag.NONE);
            InitializeDefaultConfigs();
        }

        private void InitializeDefaultConfigs()
        {
            Channel.ClearMsgFilters();
            for (int i = 0; i < 8; i++)
                Channel.StartMsgFilter(new J2534.MessageFilter(J2534.UserFilterType.STANDARDISO15765,
                                                         new byte[4] { 0x00, 0x00, 0x07, (byte)(0xE0 + i) }));
            Channel.SetConfig(J2534.Parameter.LOOP_BACK, 0);
        }
    }
}
