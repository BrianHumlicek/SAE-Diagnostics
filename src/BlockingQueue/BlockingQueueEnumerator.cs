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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SAE.BlockingQueue
{
    public partial class BlockingQueue<T>
    {
        protected struct BlockingQueueEnumerator : IEnumerator<T>
        {
            BlockingQueue<T> blockingqueue;
            int timeout;
            bool consuming;
            BlockingQueueNode currentnode;

            public BlockingQueueEnumerator(BlockingQueue<T> BlockingQueue, bool Consuming, int Timeout)
            {
                this.blockingqueue = BlockingQueue;
                this.timeout = Timeout;
                this.consuming = Consuming;
                this.currentnode = null;
                lock (BlockingQueue.sync)
                {
                    CurrentNode = this.blockingqueue.nodezero;
                }
            }

            public T Current => currentnode == null ? default : currentnode.Item;

            object IEnumerator.Current => Current;

            BlockingQueueNode CurrentNode
            {
                get => currentnode;
                set
                {
                    if (currentnode != value)
                    {
                        currentnode?.EnumeratorRelease();
                        currentnode = value;
                        currentnode?.EnumeratorHold();
                    }
                }
            }
            public void Dispose()
            {
                CurrentNode = null;
            }

            public bool MoveNext()
            {
                lock (blockingqueue.sync)
                {
                    if (TryMove())
                    {
                        return true;
                    }

                    Stopwatch TimeoutClock = new Stopwatch();
                    TimeoutClock.Start();
                    do
                    {
                        blockingqueue.addactionmux?.Invoke();
                        if (TryMove())
                        {
                            return true;
                        }
                    } while (timeout > TimeoutClock.ElapsedMilliseconds);

                    return false;
                }
            }

            private Boolean TryMove()
            {
                if (CurrentNode != blockingqueue.LastNode)
                {
                    CurrentNode = CurrentNode.Next;
                    if (consuming)
                    {
                        blockingqueue.nodezero = CurrentNode;
                    }
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
