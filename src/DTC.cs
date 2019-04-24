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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SAE.J1979
{
    public class DTC
    {
        public string Description { get; }
        public DTCSystem SystemId { get; }
        public int Code { get; }

        public DTC(byte[] TwoBytes)
        {
            this.SystemId = (DTCSystem)((TwoBytes[0] >> 6) & 0x03);
            this.Code = TwoBytes[1] + (TwoBytes[0] & 0xCF);
            this.Description = GetDescription();
        }

        protected string GetDescription()
        {
            string DTCString = this.ToString();
            string result = LookUpDescription(DTCString);


            if (String.IsNullOrWhiteSpace(result))
            {
                result = ParseDescription(DTCString);
            }
            return result;
        }

        protected string LookUpDescription(string DTCString)
        {
            return String.Empty;
        }

        protected string ParseDescription(string DTCString)
        {
                string CodeClass = "Unknownn";
                string System = "Unknownn";
                string SubSystem = "Unknownn";

                System = SystemId.GetDescription();

                switch (DTCString[1])
                {
                    case '0':
                        CodeClass = "Generic";
                        break;
                    case '1':
                        CodeClass = "Manufacturer Specific";
                        break;
                    case '2':
                    case '3':
                        break;
                }

                switch (DTCString[2])
                {
                    case '0':
                    case '1':
                        SubSystem = "Air/Fuel metering";
                        break;
                    case '2':
                        SubSystem = "Injector Circuit";
                        break;
                    case '3':
                        SubSystem = "Ignition System/Misfire";
                        break;
                    case '4':
                        SubSystem = "Auxillary Emissions";
                        break;
                    case '5':
                        SubSystem = "Vehicle Speed/Idle control";
                        break;
                    case '6':
                        SubSystem = "Computer and outputs";
                        break;
                    case '7':
                    case '8':
                    case '9':
                        SubSystem = "Transmission";
                        break;
                    case 'A':
                    case 'B':
                    case 'C':
                        SubSystem = "Hybrid Propulsion";
                        break;
                    case 'D':
                    case 'E':
                    case 'F':
                        break;

                }
                return $"Class: {CodeClass} Type: {System} Subsystem: {SubSystem}";
        }

        public override string ToString()
        {
            return $"{SystemId.ToString()}{Code: D4}";
        }
    }

    public enum DTCSystem
    {
        [Description("Powertrain")]
        P = 0,
        [Description("Chassis")]
        C = 1,
        [Description("Body")]
        B = 2,
        [Description("Network")]
        U = 3
    }

    public static class SystemIdExtensions
    {
        public static string GetDescription(this DTCSystem code)
        {
            var members = typeof(DTCSystem).GetMember(code.ToString());
            var attributes = members[0]?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((DescriptionAttribute)attributes[0])?.Description ?? String.Empty;
        }
    }
}
