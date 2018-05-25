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
using Common.Extensions;

namespace SAE.J1979.ISO15765
{
    public class Header : IHeader
    {
        private byte[] tx;
        private byte[] rx;
        public Header(int Target = 0x7E0)
        {
            this.Target = Target;
        }
        public int Target
        {
            set
            {
                int Source = value + 0x08;
                tx = new byte[4] { 0x00, 0x00, value.Byte1(), value.Byte0() };
                rx = new byte[4] { 0x00, 0x00, Source.Byte1(), Source.Byte0() };
            }
        }
        public int MaxLength => 4;
        public byte[] Rx => rx;
        public byte[] Tx => tx;
    }
}
