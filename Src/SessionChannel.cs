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
using System.Threading;

using SAE.BlockingQueue;

namespace SAE.J1979
{
    //This class provides a mechanism for synchronizing initialization of a channel by potentially competing threads.
    //Whoever gets there first will initialize it, while the other waits for it to finish.
    //It also wires up the BlockingQueue
    public partial class SessionChannel : Common.ManagedDisposable
    {
        private Common.BoolInterlock initOneShot = new Common.BoolInterlock();
        private EventWaitHandle initWaiter = new EventWaitHandle(false, EventResetMode.ManualReset);
        private int refCount;
        public J2534.Channel Channel { get; }
        public BlockingQueue<J2534.Message> RxQueue { get; }
        public SessionChannel(J2534.Channel Channel)
        {
            this.Channel = Channel;
            RxQueue = new BlockingQueue<J2534.Message>(new Action(() =>
            {
                RxQueue.AddRange(Channel.GetMessages(200, 0).Messages);
            }));
        }
        public bool IsInitialized
        {
            get
            {
                if (initOneShot.Enter())
                {
                    return false;
                }
                initWaiter.WaitOne();
                return true;
            }
            set
            {
                if (value == true)
                {
                    initWaiter.Set();
                }
                else
                {
                    initWaiter.Reset();
                    initOneShot.Exit();
                }
            }
        }
        protected override void DisposeManaged()
        {
            if (refCount == 0)
            {
                Channel.Dispose();
            }
        }
    }
}
