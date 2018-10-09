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
namespace SAE.J1979.ISO15765.ISO14229
{
    public enum Mode : byte
    {
        DiagnosticSessionControl = 0x10,
        ECUReset = 0x11,
        ReadMemoryByAddress = 0x23,
        SecurityAccess = 0x27,
        CommunicationControl =0x28,
        ReadDataByPeriodicIdentifier = 0x2A,
        DynamicallyDefineDataIdentifier = 0x2C,
        RoutineControl = 0x31,
        RequestDownload = 0x34,
        RequestUpload = 0x35,
        TransferData = 0x36,
        RequestTransferExit = 0x37,
        TesterPresent = 0x3E
    }
}
