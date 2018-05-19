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

namespace SAE
{
    internal class SessionChannelFactory
    {
        private static Dictionary<string, SessionChannel> ChannelCache = new Dictionary<string, SessionChannel>();
        public static SessionChannel GetSessionChannel(J2534.Device Device, J2534.Protocol ProtocolID, J2534.Baud Baud, J2534.ConnectFlag Flags)
        {
            string Key = $"{Device}:{ProtocolID}:{Baud}";
            lock (ChannelCache)
            {
                SessionChannel result;
                if (ChannelCache.ContainsKey(Key))
                {
                    result = ChannelCache[Key];
                }
                else
                {
                    result = new SessionChannel(Device.GetChannel(ProtocolID, Baud, Flags));
                    ChannelCache[Key] = result;
                    result.OnDisposing += () =>
                    {
                        lock (ChannelCache)
                        {
                            if (result.References-- == 0)
                                ChannelCache.Remove(Key);
                        }
                    };
                }
                result.References++;
                return result;
            }
        }
    }
}
