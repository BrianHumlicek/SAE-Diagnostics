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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockingQueue
{
    public class BlockingLinkedList<T> : LinkedList<T>
    {
        public BlockingLinkedList() : base() { }
        public BlockingLinkedList(IEnumerable<T> enumerable) : base(enumerable) { }

        public IEnumerable<T> GetBlockingEnumerable(int blockingTimeout = 0)
        {
            var blockingTimer = new System.Diagnostics.Stopwatch();
            blockingTimer.Start();
            do
            {
                //Get a message for 10ms
                var node = First;
                while (node != null)
                {
                    yield return node.Value;
                    node = node.Next;
                }

            } while (blockingTimer.ElapsedMilliseconds < blockingTimeout);
        }
    }
    public static class BlockingLinkedlistIEnumerableExtension
    {
        public static IEnumerable<T> ConsumeFrom<T>(this IEnumerable<T> enumerable, BlockingLinkedList<T> queue)
        {
            foreach (T obj in enumerable)
            {
                queue.Remove(obj);
                yield return obj;
            }
        }
    }
}
