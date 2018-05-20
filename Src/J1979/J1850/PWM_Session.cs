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

 namespace SAE.J1979.J1850
{
    public class PWM_Session : Session
    {
        protected override SessionChannel sessionChannel { get; }
        public PWM_Session(J2534.Device Device)
        {
            sessionChannel = SessionChannelFactory.GetSessionChannel(Device, J2534.Protocol.J1850PWM, J2534.Baud.J1850PWM, J2534.ConnectFlag.NONE);
            initializeDefaults();
        }
        protected override void initializeDefaults()
        {
            base.initializeDefaults();
            header.K_bit = false;
            header.Priority = 6;
            Channel.AddToFunctMsgLookupTable(0x6B);
            Channel.SetConfig(J2534.Parameter.NODE_ADDRESS, header.Source);
            Channel.StartMsgFilter(new J2534.MessageFilter()
            {
                Mask = new byte[] { 0x00, 0xFF, 0x00 },
                Pattern = new byte[] { 0x00, (byte)header.Source, 0x00 },
                FilterType = J2534.Filter.PASS_FILTER
            });

        }
    }
}
