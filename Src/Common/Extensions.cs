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
using System.Collections.Generic;

namespace Common.Extensions
{
    public static class Extensions
    {
        public static byte Byte0(this int Integer32)
        {
            return (byte)Integer32;
        }
        public static byte Byte1(this int Integer32)
        {
            return (byte)(Integer32 >> 8);
        }
        public static byte Byte2(this int Integer32)
        {
            return (byte)(Integer32 >> 16);
        }
        public static byte Byte3(this int Integer32)
        {
            return (byte)(Integer32 >> 24);
        }
        public static bool IsBitSet(this byte InByte, int Bit)
        {
            if (((InByte >> Bit) & 1) == 1) return true;
            return false;
        }
        public static IEnumerable<byte>ConcatByte(this IEnumerable<byte> Enumerable, byte Byte)
        {
            foreach(var EnumerableByte in Enumerable)
            {
                yield return EnumerableByte;
            }
            yield return Byte;
        }
    }
}
