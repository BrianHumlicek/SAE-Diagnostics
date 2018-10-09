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

namespace RealtimeQueue
{
    public partial class RealtimeQueue<T>
    {
        private class RealtimeQueueNode
        {
            public T Item { get; set; }
            private event EventHandler<SequenceChangedEventArgs> SequenceChanged;

            private RealtimeQueue<T> rootList;
            public RealtimeQueueNode next;
            private int EnumeratorsHoldingAReference;

            public RealtimeQueueNode(RealtimeQueue<T> Root)
            {
                rootList = Root;
            }

            public void EnumeratorHold()
            {
                EnumeratorsHoldingAReference++;
            }

            public void EnumeratorRelease()
            {
                EnumeratorsHoldingAReference--;
                //If nothing is holding a reference to this node, clear and recycle it.
                if (EnumeratorsHoldingAReference == 0 && SequenceChanged == null)
                {
                    Next = null;    //This unregisters the ForwardReferenceChanged Event in the property setter
                    Item = default(T);  //Release reference to item, to assist GC
                    rootList.nodePool.Add(this);
                }
            }

            internal RealtimeQueueNode Next
            {
                get { return next; }
                set
                {
                    if (next != null) next.SequenceChanged -= OnSequenceChanged;
                    next = value;
                    if (next != null) next.SequenceChanged += OnSequenceChanged;
                }
            }

            internal void NotifySequenceChanged(RealtimeQueueNode NewReference, RealtimeQueueNode ExcludedNode = null)
            {
                lock (rootList.sync)
                    SequenceChanged?.Invoke(this, new SequenceChangedEventArgs(NewReference, ExcludedNode));
            }

            internal void Remove()
            {
                NotifySequenceChanged(NewReference: Next);
            }

            private void OnSequenceChanged(object sender, SequenceChangedEventArgs args)
            {
                if (!this.Equals(args.ExcludedNode))
                {
                    Next = args.NewNodeReference;
                }
            }
        }
    }
}
