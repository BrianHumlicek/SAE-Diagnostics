using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.J1979.ISO14229.Subfunctions
{
    public enum Mode10 : byte
    {
        defaultSession = 0x01,
        programmingSession = 0x02,
        extendedDiagnosticSession = 0x03,
        safetySystemDiagnosticSession = 0x04
    }
}
