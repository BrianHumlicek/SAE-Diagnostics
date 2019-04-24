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
using System.Collections.Concurrent;

namespace SAE.BlockingQueue
{
    public partial class BlockingQueue<T>
    {
        protected class BlockingQueueReusableNode : BlockingQueueNode
        {
            ConcurrentBag<BlockingQueueNode> nodepool;
            int references;

            public BlockingQueueReusableNode(ConcurrentBag<BlockingQueueNode> NodePool)
            {
                this.nodepool = NodePool;
            }

            public override void EnumeratorHold()
            {
                references++;
            }

            public override void EnumeratorRelease()
            {
                references--;
                tryreturntopool();
            }

            public override void Remove()
            {
                base.Remove();
                tryreturntopool();
            }

            private void tryreturntopool()
            {
                if (references < 1                 //If nothing is holding a reference to this node,
                    && IsForwardSequenceChangedEventNull) //and nothing is looking to this node as 'Next'
                {
                    Next = null;    //This unregisters the ForwardReferenceChanged Event in the property setter
                    Item = default(T);  //Release reference to item, to assist GC
                    references = 0;
                    nodepool.Add(this);
                }
            }
        }
    }
}
