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
        public Response Response { get; }
        public IList<byte> Data { get; }
        public ServiceResult(Response Response)
        {
            this.Response = Response;
            Data = new ArraySegment<byte>();
        }
        public ServiceResult(ArraySegment<byte> Data)
        {
            Response = Response.NONE;
            this.Data = Data;
        }
        public ServiceResult(Response Response, ArraySegment<byte> Data)
        {
            this.Response = Response;
            this.Data = Data;
        }
        public ServiceResult CheckResponse(string FailureMessage, Response ExpectedResponse = Response.NONE)
        {
            if (Response != ExpectedResponse)
            {
                //throw new FordDiagnosticException(Result.Response, FailureMessage, Result.Data);
            }
            return this;
        }
        public ServiceResult CheckData(string FailureMessage, Response ExpectedResponse = Response.AFFIRMITIVE_RESPONSE)
        {
            if ((Data?.Count ?? 0) < 1 || Data[0] != (byte)ExpectedResponse)
            {
                //throw new FordDiagnosticException(Result.Response, FailureMessage, Result.Data);
            }
            return this;
        }
    }
}
