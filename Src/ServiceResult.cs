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
using System.Collections.Generic;

namespace SAE.J1979
{
    public class ServiceResult
    {
        public ExitCode ExitCode { get; }
        public IList<byte> Data { get; }
        public ServiceResult(ExitCode ExitCode)
        {
            this.ExitCode = ExitCode;
            Data = new ArraySegment<byte>();
        }
        public ServiceResult(ArraySegment<byte> Data)
        {
            ExitCode = ExitCode.NONE;
            this.Data = Data;
        }
        public ServiceResult(ExitCode ExitCode, ArraySegment<byte> Data)
        {
            this.ExitCode = ExitCode;
            this.Data = Data;
        }
        /// <summary>
        /// Checks the response exit code to the expected exit code.  Throws an exception if they don't match.
        /// </summary>
        /// <param name="FailureMessage">Message to include in the exception</param>
        /// <param name="ExpectedExitCode">Exit code to test for</param>
        /// <returns></returns>
        public ServiceResult CheckExitCode(string FailureMessage, ExitCode ExpectedExitCode = ExitCode.NONE)
        {
            if (ExitCode != ExpectedExitCode)
            {
                //throw new FordDiagnosticException(Result.Response, FailureMessage, Result.Data);
            }
            return this;
        }

        /// <summary>
        /// Compares the first data byte to the expected exit code.  Throws an exception if they don't match
        /// </summary>
        /// <param name="FailureMessage">Message to include in the exception</param>
        /// <param name="ExpectedExitCode">Exit code to test for</param>
        /// <returns></returns>
        public ServiceResult CheckData(string FailureMessage, ExitCode ExpectedExitCode = ExitCode.AFFIRMITIVE_RESPONSE)
        {
            if ((Data?.Count ?? 0) < 1 || Data[0] != (byte)ExpectedExitCode)
            {
                //throw new FordDiagnosticException(Result.Response, FailureMessage, Result.Data);
            }
            return this;
        }
    }
}
