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
using RealtimeQueue;

namespace SAE
{
    public class SessionChannel : Common.LiteDisposable
    {
        public int References;
        public J2534.Channel Channel { get; }
        public RealtimeQueue<J2534.Message> RxQueue { get; }
        public SessionChannel(J2534.Channel Channel)
        {
            this.Channel = Channel;
            RxQueue = new RealtimeQueue<J2534.Message>(new Action(() =>
            {
                RxQueue.AddRange(Channel.GetMessages(200, 0).Messages);
            }));
        }
        protected override void DisposeManaged()
        {
            if (References == 0)
            {
                RxQueue.Dispose();
                Channel.Dispose();
            }
        }
    }
}
