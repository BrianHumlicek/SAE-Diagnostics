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
using Common.Extensions;

namespace SAE.J1979.J1850
{
    public class J1850Header : IHeader
    {
        private byte[] tx, rx;
        private byte zz = 0;
        private byte priority = 7;
        private byte source = 0xF1;
        private byte target = 0x10;
        private bool h_bit = false;
        private bool k_bit = true;
        private bool y_bit = true;
        public J1850Header()
        {
            components2Array();
        }
        public byte[] Tx
        {
            get { return tx; }
            set
            {
                validateLength(value);
                if (value.Length == 3)
                {
                    target = value[1];
                    source = value[2];
                }
                array2Components(value[0]);
            }
        }
        public byte[] Rx
        {
            get { return rx; }
            set
            {
                validateLength(value);
                if (value.Length == 3)
                {
                    source = value[1];
                    target = value[2];
                }
                array2Components(value[0]);
            }
        }
        public int MaxLength { get { return 3; } }
        /// <summary>
        /// This is the physical or functional address of the tool (this)
        /// </summary>
        public int Source
        {
            get { return source; }
            set
            {
                source = (byte)value;
                components2Array();
            }
        }
        /// <summary>
        /// This is the physical or functional address of the downstream device
        /// </summary>
        public int Target
        {
            get { return target; }
            set
            {
                target = (byte)value;
                components2Array();
            }
        }
        /// <summary>
        /// Message priority 0-7
        /// </summary>
        public int Priority
        {
            get { return priority; }
            set
            {
                priority = (byte)(value & 0x07);
                components2Array();
            }
        }
        /// <summary>
        /// <para>true  = single byte header</para>
        /// </summary>
        public bool H_bit   //1 = single byte header
        {
            get { return h_bit; }
            set
            {
                h_bit = value;
                components2Array();
            }
        }
        /// <summary>
        /// true  = IFR not allowed
        /// </summary>
        public bool K_bit   //1 = IFR not allowed
        {
            get { return k_bit; }
            set
            {
                k_bit = value;
                components2Array();
            }
        }
        /// <summary>
        /// true = physical addressing
        /// </summary>
        public bool Y_bit    //1 = Physical addressing
        {
            get { return y_bit; }
            set
            {
                y_bit = value;
                components2Array();
            }
        }
        /// <summary>
        /// Functional address in single byte header mode
        /// </summary>
        public int ZZ  //message type/single byte header address
        {
            get { return zz; }
            set
            {
                zz = (byte)(value & 0x03);
                components2Array();
            }
        }
        private void array2Components(byte byte0)
        {
            priority = (byte)(byte0 >> 5);
            h_bit = byte0.IsBitSet(4);
            k_bit = byte0.IsBitSet(3);
            y_bit = byte0.IsBitSet(2);
            zz = (byte)(byte0 & 0x03);
            if (h_bit) target = zz;
        }
        private void components2Array()
        {
            byte byte0 = (byte)(priority << 5);
            if (h_bit) byte0 |= 0x10;
            if (k_bit) byte0 |= 0x08;
            if (y_bit) byte0 |= 0x04;
            if (h_bit)
            {
                rx = new byte[1] { (byte)(byte0 | (source & 0x03)) };
                tx = new byte[1] { (byte)(byte0 | (target & 0x03)) };

            }
            else
            {
                byte0 |= zz;
                rx = new byte[3] { byte0, source, target };
                tx = new byte[3] { byte0, target, source };
            }
        }
        private void validateLength(byte[] value)
        {
            if (value.Length != 1 && value.Length != 3) throw new FormatException($"Array length of {value.Length} is invalid.  Length must be either 1 or 3");

            if (value.Length == 1)
            {
                if (!value[0].IsBitSet(4)) throw new FormatException($"Array length mismatch!  Length is 1 and H_bit is 0");
            }
            else
            {
                if (value[0].IsBitSet(4)) throw new FormatException($"Array length mismatch with H_bit! Length is 3 and H_bit is 1");
            }
        }
    }
}
