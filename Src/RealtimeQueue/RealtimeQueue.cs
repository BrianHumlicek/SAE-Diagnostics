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
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace RealtimeQueue
{
    public partial class RealtimeQueue<T> : RealtimeQueueEnumerable<T>
    {
        private RealtimeQueueNode linkNode;
        private object sync = new object();
        private ConcurrentBag<RealtimeQueueNode> nodePool = new ConcurrentBag<RealtimeQueueNode>();
        private ActionMultiplex addActionMultiplex;

        public int Count { get; private set; }

        public RealtimeQueue(Action AddAction)
        {
            base.RootQueue = this;
            linkNode = new RealtimeQueueNode(this);
            linkNode.Next = linkNode;
            addActionMultiplex = new ActionMultiplex(AddAction);
        }

        public void AddRange(IEnumerable<T> Range)
        {
            var Enumerator = Range?.GetEnumerator();

            if (Enumerator?.MoveNext() ?? false)
            {
                var AddRangeCount = 1;
                var FirstNode = GetNode(Enumerator.Current);
                var LastNode = FirstNode;
                while (Enumerator.MoveNext())
                {
                    AddRangeCount++;
                    LastNode.Next = GetNode(Enumerator.Current);
                    LastNode = LastNode.Next;
                }
                lock (sync)
                {
                    linkNode.NotifySequenceChanged(NewReference: FirstNode);
                    LastNode.Next = linkNode;
                    Count += AddRangeCount;
                }
            }
        }

        public void Add(T Item)
        {
            var NewNode = GetNode(Item);
            lock (sync)
            {
                linkNode.NotifySequenceChanged(NewReference: NewNode);
                NewNode.Next = linkNode;
                Count++;
            }
        }

        public T DecueueOne()
        {
            var item = default(T);
            lock (sync)
            {
                if (!linkNode.Next.Equals(linkNode))
                {
                    var FirstNode = linkNode.Next; //
                    FirstNode.EnumeratorHold();
                    FirstNode.Remove();
                    item = FirstNode.Item;
                    FirstNode.EnumeratorRelease();
                    Count--;
                }
            }
            return item;
        }

        public RealtimeQueueEnumerable<T> GetEnumerable(int Timeout, bool BookmarkEnumeration = false)
        {
            return new RealtimeQueueEnumerable<T>(this, Timeout, BookmarkEnumeration);
        }

        private RealtimeQueueNode GetNode(T Item = default(T))
        {
            RealtimeQueueNode node;
            nodePool.TryTake(out node);
            if (node == null) node = new RealtimeQueueNode(this);
            node.Item = Item;
            return node;
        }
    }
}
