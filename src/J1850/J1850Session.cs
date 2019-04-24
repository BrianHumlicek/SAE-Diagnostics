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

namespace SAE.J1979.J1850
{
    public abstract class J1850Session : J2190.J2190Session
    {
        int source_filter_index = -1;

        protected J1850Session(SessionChannel sessionChannel) : base(new J1850.J1850Header(), sessionChannel)
        {
        }

        protected override void initializeChannel()
        {
            base.initializeChannel();
        }
        protected new J1850Header header
        {
            get { return (J1850Header) base.header; }
        }
        public int SourceAddress
        {
            get { return header.Source; }
            set
            {
                if (header.Source != value)
                {
                    header.Source = value;
                    setSourceFilter();
                }
            }
        }
        public int TargetAddress
        {
            get { return header.Target; }
            set { header.Target = value; }
        }

        protected override Func<J2534.Message, bool> successMessage(byte Mode)
        {
            if (!header.H_bit)  //3 byte headers
            {
                return new Func<J2534.Message, bool>(Message =>
                {
                    return (Message?.Data?.Length ?? 0) > 3
                           && Message.Data[3] == (Mode ^ 0x40)
                           && Message.Data[2] == base.header.Rx[2]
                           && Message.Data[1] == base.header.Rx[1];
                });
            }
            throw new NotImplementedException("Single byte headers are not yet supported!");
        }

        protected override Func<J2534.Message, bool> failMessage(byte Mode)
        {
            if (!header.H_bit)  //3 byte headers
            {
                return new Func<J2534.Message, bool>(Message =>
                {
                    return (Message?.Data?.Length ?? 0) > 4
                           && Message.Data[4] == (byte)Mode
                           && Message.Data[3] == (byte)J1979.J1979Mode.GENERAL_RESPONSE
                           && Message.Data[2] == base.header.Rx[2]
                           && Message.Data[1] == base.header.Rx[1];
                });
            }
            throw new NotImplementedException("Single byte headers are not yet supported!");
        }

        protected void setSourceFilter()
        {
            if (source_filter_index != -1)
            {
                Channel.StopMsgFilter(source_filter_index);
            }
            source_filter_index = Channel.StartMsgFilter(new J2534.MessageFilter() { Mask = new byte[] { 0x00, 0xFF, 0x00 },
                                                                                     Pattern = new byte[] { 0x00, (byte)header.Source, 0x00 },
                                                                                     FilterType = J2534.Filter.PASS_FILTER });
        }
    }
}
