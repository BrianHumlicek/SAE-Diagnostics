using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAE.J1979.ISO14229
{
    public enum Mode : byte
    {
        DiagnosticSessionControl = 0x10,
        ECUReset = 0x11,
        ReadMemoryByAddress = 0x23,
        SecurityAccess = 0x27,
        CommunicationControl =0x28,
        ReadDataByPeriodicIdentifier = 0x2A,
        DynamicallyDefineDataIdentifier = 0x2C,
        RoutineControl = 0x31,
        RequestDownload = 0x34,
        RequestUpload = 0x35,
        TransferData = 0x36,
        RequestTransferExit = 0x37,
        TesterPresent = 0x3E
    }
}
