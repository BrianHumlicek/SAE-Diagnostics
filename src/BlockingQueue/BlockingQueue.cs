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

namespace SAE.BlockingQueue
{
    public partial class BlockingQueue<T> : IEnumerable<T>
    {
        readonly BlockingQueue<T> root_queue;
        readonly object sync;
        readonly Hashtable nodemap;
        readonly ActionMultiplex addactionmux;
        bool consuming;
        BlockingQueueNode nodezero;
        BlockingQueueNode lastnode;

        BlockingQueueNode LastNode { get => root_queue.lastnode; set => root_queue.lastnode = value; }

        public BlockingQueue()
        {
            root_queue = this;
            sync = new object();
            nodemap = new Hashtable();            
            nodezero = new BlockingQueueNode();
            nodezero.Next = nodezero;
            LastNode = nodezero;
        }

        public BlockingQueue(Action AddAction) : this()
        {
            addactionmux = new ActionMultiplex(AddAction);
        }

        protected BlockingQueue(BlockingQueue<T> Root_Queue)
        {
            root_queue = Root_Queue;
            sync = root_queue.sync;
            nodemap = root_queue.nodemap;
            addactionmux = root_queue.addactionmux;
            nodezero = root_queue.nodezero;
        }

        public void Add(T Item)
        {
            AppendNode(CreateNode(Item));
        }

        public void AddRange(IEnumerable<T> ItemRange)
        {
            var ItemRangeEnumerator = ItemRange?.GetEnumerator();

            if (ItemRangeEnumerator?.MoveNext() ?? false)
            {
                BlockingQueueNode RangeFirstNode = CreateNode(ItemRangeEnumerator.Current);
                BlockingQueueNode RangeLastNode = RangeFirstNode;
                while (ItemRangeEnumerator.MoveNext())
                {
                    BlockingQueueNode RangeNextNode = CreateNode(ItemRangeEnumerator.Current);
                    RangeLastNode.Next = RangeNextNode;
                    RangeNextNode.Previous = RangeLastNode;
                    RangeLastNode = RangeNextNode;
                }
                AppendNode(RangeFirstNode, RangeLastNode);
            }
        }

        private void AppendNode(BlockingQueueNode NewNode, BlockingQueueNode LastNode = null)
        {
            lock (sync)
            {
                this.LastNode.Next = NewNode;
                NewNode.Previous = this.LastNode;
                this.LastNode = LastNode ?? NewNode;
            }
        }

        public int BlockingTimeout { get; set; }

        public BlockingQueue<T> GetConsumingEnumerable()
        {
            return new BlockingQueue<T>(this) { consuming = true };
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return new BlockingQueueEnumerator(this, consuming, BlockingTimeout);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Remove(T obj)
        {
            lock (sync)
            {
                BlockingQueueNode node = (BlockingQueueNode)nodemap[obj];
                if (node != null)
                {
                    if (node == LastNode)
                    {
                        LastNode = node.Previous;
                    }

                    node.Remove();
                    nodemap.Remove(obj);
                }
            }
        }

        public void Unconsume()
        {
            lock (sync)
            {
                this.nodezero = root_queue.nodezero;
            }
        }
    }
}
