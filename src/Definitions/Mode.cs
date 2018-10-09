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
namespace SAE.J1979
{
    public enum Mode : byte
    {
        REQ_DIAG_DATA = 0x01,
        REQ_FREEZE_FRAME_DATA = 0x02,
        REQ_EMISSION_DIAG_DATA = 0x03,
        CLEAR_EMISSION_DIAG_DATA = 0x04,
        REQ_O2_MON_RESULTS = 0x05,
        REQ_SYSTEM_MON_RESULTS = 0x06,
        REQ_CURRENT_DTC = 0x07,
        REQ_SYSTEM_CTL = 0x08,
        REQ_VEHICLE_INFO = 0x09,
        REQ_PERMANENT_EMISSION_DTC = 0x0A,
        GENERAL_RESPONSE = 0x7F
    }
}
