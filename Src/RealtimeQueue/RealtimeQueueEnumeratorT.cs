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

namespace RealtimeQueue
{
    public partial class RealtimeQueue<T>
    {
        public class RealtimeQueueEnumerator<EnumeratorT> : Common.LiteDisposable, IEnumerator<EnumeratorT>
        {
            private RealtimeQueue<EnumeratorT>.RealtimeQueueNode currentNode;
            private bool useBookmark;
            private RealtimeQueueEnumerable<EnumeratorT> rootEnumerable;
            private int timeout;

            private Stopwatch TimeoutClock = new Stopwatch();

            public RealtimeQueueEnumerator(RealtimeQueueEnumerable<EnumeratorT> RootEnumerable, object Bookmark)
            {
                rootEnumerable = RootEnumerable;
                timeout = RootEnumerable.Timeout;
                if (useBookmark = RootEnumerable.BookmarkEnumeration && Bookmark != null)
                {
                    if (!(Bookmark is RealtimeQueue<EnumeratorT>.RealtimeQueueNode))
                        throw new ArgumentException($"Enumerator was initialized with a type other than {typeof(RealtimeQueue<EnumeratorT>.RealtimeQueueNode).ToString()}");
                    currentNode = Bookmark as RealtimeQueue<EnumeratorT>.RealtimeQueueNode;
                }
                else
                {
                    currentNode = rootEnumerable.RootQueue.linkNode;
                }
                TimeoutClock.Reset();
                TimeoutClock.Start();
            }

            public EnumeratorT Current
            {
                get { return currentNode.Item; }
            }

            object IEnumerator.Current
            {
                get { return currentNode.Item; }
            }

            public bool MoveNext()
            {
                return MoveWhere(null);
            }

            public bool MoveWhere(Predicate<EnumeratorT> Predicate, bool Decueue = false)
            {
                if (TryMove(Predicate, Decueue)) return true;
                do
                {
                    rootEnumerable.RootQueue.addActionMultiplex.Invoke();
                    if (TryMove(Predicate, Decueue)) return true;
                    //System.Threading.Thread.Sleep(20);   //This line will throttle the rate the API is polled for data
                } while (TimeoutClock.ElapsedMilliseconds < timeout);
                return false;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            private bool TryMove(Predicate<EnumeratorT> Predicate = null, bool Decueue = false)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(GetType().ToString());

                lock (rootEnumerable.RootQueue.sync)
                {
                    var StartNode = currentNode;

                    //CurrentNode.Next should never be null
                    while (!currentNode.Next.Equals(rootEnumerable.RootQueue.linkNode))
                    {
                        rootEnumerable.Bookmark = currentNode = currentNode.Next;

                        if (Predicate?.Invoke(currentNode.Item) ?? true)
                        {
                            StartNode.EnumeratorRelease();
                            currentNode.EnumeratorHold();
                            if (Decueue)
                            {
                                rootEnumerable.Bookmark = currentNode.Next;
                                currentNode.Remove();
                                rootEnumerable.RootQueue.Count--;
                            }
                            return true;
                        }
                    }
                }
                return false;
            }

            protected override void DisposeManaged()
            {
                if (!useBookmark)
                    currentNode?.EnumeratorRelease();
            }
        }
    }
}
