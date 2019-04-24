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
using System.Collections.Generic;

namespace SAE.J1979
{
    public partial class SessionChannel
    {
        public class Factory
        {
            private static Dictionary<string, SessionChannel> channelcache = new Dictionary<string, SessionChannel>();
            public static SessionChannel Create(J2534.Device Device, J2534.Protocol ProtocolID, J2534.Baud Baud, J2534.ConnectFlag Flags)
            {
                string Key = $"{Device}:{ProtocolID}:{Baud}";
                lock (channelcache)
                {
                    SessionChannel result;

                    if (!channelcache.ContainsKey(Key))
                    {
                        var NewChannel = new SessionChannel(Device.GetChannel(ProtocolID, Baud, Flags));
                        NewChannel.OnDisposing += () =>
                        {
                            lock (channelcache)
                            {
                                NewChannel.refCount--;
                                if (NewChannel.refCount < 1)
                                {
                                    channelcache.Remove(Key);
                                }
                            }
                        };
                        channelcache[Key] = NewChannel;
                    }
                    result = channelcache[Key];
                    result.refCount++;
                    return result;
                }
            }
        }
    }
}
