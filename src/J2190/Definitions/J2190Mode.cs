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
namespace SAE.J1979.J2190
{
    public enum J2190Mode : byte
    {
        INITIATE_DIAG_OP = 0x10,
        MODULE_RESET = 0x11,
        REQ_FREEZE_FRAME = 0x12,
        REQ_DTC = 0x13,
        CLEAR_DIAG_INFO = 0x14,
        DTC_STATUS = 0x17,
        REQ_DTC_BY_STATUS = 0x18,
        NORMAL_OPERATION = 0x20,
        DATA_BY_OFFSET = 0x21,
        DATA_BY_PID = 0x22,
        DATA_BY_ADDRESS = 0x23,
        REQ_PID_SCALING = 0x24,
        STOP_DATA_TX = 0x25,
        SET_DATA_RATE = 0x26,
        REQ_SECURITY_ACCESS = 0x27,
        STOP_NORMAL_TRAFFIC = 0x28,
        RESUME_NORMAL_TRAFFIC = 0x29,
        REQ_DATA_PACKET = 0x2A,
        DEFINE_PACKET_BY_OFFSET = 0x2B,
        DEFINE_PACKET = 0x2C,
        IOCTL_BY_PID = 0x2F,
        IOCTL_BY_VALUE = 0x30,
        START_DIAG_ROUTINE_BY_NUMBER = 0x31,
        STOP_DIAG_ROUTINE_BY_NUMBER = 0x32,
        DIAG_ROUTINE_RESULTS_BY_NUMBER = 0x33,
        REQ_DOWNLOAD = 0x34,    //tool to module
        REQ_UPLOAD = 0x35,  //module to tool
        DATA_TRANSFER = 0x36,
        TRANSFER_ROUTINE_EXIT = 0x37,
        START_DIAG_ROUTINE_BY_ADDRESS = 0x38,
        STOP_DIAG_ROUTINE_BY_ADDRESS = 0x39,
        DIAG_ROUTINE_RESULTS_BY_ADDRESS = 0x3A,
        WRITE_DATA_BLOCK = 0x3B,
        READ_DATA_BLOCK = 0x3C,
        DIAG_HEARTBEAT = 0x3F
    }
}
