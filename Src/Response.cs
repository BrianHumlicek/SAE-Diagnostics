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
    public enum Response
    {
        AFFIRMITIVE_RESPONSE = 0x00,
        GENERAL_REJECT = 0x10,
        MODE_NOT_SUPPORTED = 0x11,
        INVALID_SUBFUNCTION = 0x12,
        REPEAT_REQUEST = 0x21,
        CONDITIONS_NOT_CORRECT = 0x22,
        ROUTINE_NOT_COMPLETE = 0x23,
        REQUEST_OUT_OF_RANGE = 0x31,
        SECURITY_ACCESS_DENIED = 0x33,
        SECURITY_ACCESS_ALLOWED = 0x34,
        INVALID_KEY = 0x35,
        EXCEED_NUMBER_OF_ATTEMPTS = 0x36,
        TIME_DELAY_NOT_EXPIRED = 0x37,
        DOWNLOAD_NOT_ACCEPTED = 0x40,
        IMPROPER_DOWNLOAD_TYPE = 0x41,
        DOWNLOAD_ADDRESS_UNAVAILABLE = 0x42,
        IMPROPER_DOWNLOAD_LENGTH = 0x43,
        READY_FOR_DOWNLOAD = 0x44,
        UPLOAD_NOT_ACCEPTED = 0x50,
        IMPROPER_UPLOAD_TYPE = 0x51,
        UPLOAD_ADDRESS_UNAVAILABLE = 0x52,
        IMPROPER_UPLOAD_LENGTH = 0x53,
        READY_FOR_UPLOAD = 0x54,
        PASS_WITH_RESULTS = 0x61,
        PASS_WITHOUT_RESULTS = 0x62,
        FAIL_WITH_RESULTS = 0x63,
        FAIL_WITHOUT_RESULTS = 0x64,
        TRANSFER_SUSPENDED = 0x71,
        TRANSFER_ABORTED = 0x72,
        TRANSFER_COMPLETE = 0x73,
        TRANSFER_ADDRESS_UNAVAILABLE = 0x74,
        IMPROPER_TRANSFER_LENGTH = 0x75,
        IMPROPER_TRANSFER_TYPE = 0x76,
        TRANSFER_CHECKSUM_FAIL = 0x77,
        TRANSFER_RECEIVED = 0x78,
        TRANSFER_BYTE_COUNT_MISMATCH = 0x79,
        MANUFACTURER_SPECIFIC = 0x100,
        NONE = 0x101
    }
}
