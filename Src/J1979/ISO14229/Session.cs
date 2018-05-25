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

namespace SAE.J1979.ISO14229
{
    public class Session : J1979.ISO15765.Session
    {
        public Session(J2534.Device Device) : base(Device)
        {

        }
        public ServiceResult Mode10(Subfunctions.Mode10 Subfunction, bool SuppressPositiveResponse = false)
        {
            return ISOserviceHandler((byte)Mode.DiagnosticSessionControl, 1, new byte[] { (byte)Subfunction }, SuppressPositiveResponse);
        }
        protected ServiceResult ISOserviceHandler(byte Mode, int NumOfParams, IEnumerable<byte> Data, bool SuppressPositiveResponse)
        {
            if (SuppressPositiveResponse)
                Mode |= 0x80;
            else
                Mode &= 0x7F;

            return serviceHandler(Mode, NumOfParams, Data);
        }
    }
}
