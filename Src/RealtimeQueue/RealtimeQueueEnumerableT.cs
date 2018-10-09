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

namespace RealtimeQueue
{
    public class RealtimeQueueEnumerable<EnumerableT> : Common.LiteDisposable, IEnumerable<EnumerableT>
    {
        public RealtimeQueue<EnumerableT> RootQueue { get; protected set; }
        public object Bookmark { private get; set; }
        public int Timeout { get; private set; }
        public bool BookmarkEnumeration { get; private set; }
        
        protected RealtimeQueueEnumerable()
        {
            Timeout = 0;
        }

        public RealtimeQueueEnumerable(RealtimeQueue<EnumerableT> RootQueue, int Timeout, bool BookmarkEnumeration)
        {
            this.RootQueue = RootQueue;
            this.Timeout = Timeout;
            this.BookmarkEnumeration = BookmarkEnumeration;
        }

        public IEnumerator<EnumerableT> GetEnumerator()
        {
            return new RealtimeQueue<EnumerableT>.RealtimeQueueEnumerator<EnumerableT>(this, Bookmark);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using (var Enumerator = new RealtimeQueue<EnumerableT>.RealtimeQueueEnumerator<EnumerableT>(this, Bookmark))
            {
                while (Enumerator.MoveNext())
                {
                    yield return Enumerator.Current;
                }
            }
        }

        public IEnumerable<EnumerableT> Decueue()
        {
            using (var Enumerator = new RealtimeQueue<EnumerableT>.RealtimeQueueEnumerator<EnumerableT>(this, Bookmark))
            {
                if (Enumerator.MoveWhere(null, true))
                {
                    yield return Enumerator.Current;
                }
            }
        }

        public IEnumerable<EnumerableT> DecueueWhere(Predicate<EnumerableT> Predicate)
        {
            using (var Enumerator = new RealtimeQueue<EnumerableT>.RealtimeQueueEnumerator<EnumerableT>(this, Bookmark))
            {
                if (Enumerator.MoveWhere(Predicate, Decueue: true))
                {
                    yield return Enumerator.Current;
                }
            }
        }

        //Optimized version of WHERE.
        public IEnumerable<EnumerableT> Where(Predicate<EnumerableT> Predicate)
        {
            using (var Enumerator = new RealtimeQueue<EnumerableT>.RealtimeQueueEnumerator<EnumerableT>(this, Bookmark))
            {
                if (Enumerator.MoveWhere(Predicate))
                {
                    yield return Enumerator.Current;
                }
            }
        }
    }
}
