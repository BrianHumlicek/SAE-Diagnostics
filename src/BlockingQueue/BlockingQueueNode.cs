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

namespace SAE.BlockingQueue
{
    public partial class BlockingQueue<T>
    {
        protected class BlockingQueueNode
        {
            event EventHandler<SequenceChangedEventArgs> forwardsequencechangedevent;
            BlockingQueueNode next;

            //public DateTime CreateTime 
            public virtual void EnumeratorHold()
            {
            }

            public virtual void EnumeratorRelease()
            {
            }

            public T Item { get; set; }

            public BlockingQueueNode Next
            {
                get { return next; }
                set
                {
                    if (next != value)
                    {
                        if (next != null) next.forwardsequencechangedevent -= OnForwardSequenceChanged;
                        next = value;
                        if (next != null) next.forwardsequencechangedevent += OnForwardSequenceChanged;
                    }
                }
            }

            protected void NotifyForwardSequenceChanged(BlockingQueueNode UpdatedNextNode)
            {
                forwardsequencechangedevent?.Invoke(this, new SequenceChangedEventArgs(UpdatedNextNode));
            }

            protected void OnForwardSequenceChanged(object sender, SequenceChangedEventArgs args)
            {
                Next = args.UpdatedNextNode;
            }

            public BlockingQueueNode Previous { get; set; }

            public virtual void Remove()
            {
                NotifyForwardSequenceChanged(Next);
                if (Next != null)
                {
                    Next.Previous = Previous;
                }
                Previous = null;
            }

            protected bool IsForwardSequenceChangedEventNull { get => forwardsequencechangedevent == null; }
        }
    }
}
