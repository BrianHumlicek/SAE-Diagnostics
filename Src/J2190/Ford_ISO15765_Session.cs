﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;

namespace SAE.J2190
{
    class Ford_ISO15765_Session : J1979.ISO15765.Session
    {

        public Ford_ISO15765_Session(J2534.Device Device) : base(Device)
        {

        }
        public virtual bool BroadcastListen(int Timeout = 200)
        {
            byte[] InviteRequest = header.Tx.Concat(new byte[] { 0x10, 0x85 }).ToArray();
            var BroadcastMessages = rxQueue.GetEnumerable(Timeout: 10, BookmarkEnumeration: true).DecueueWhere(msg =>
            {
                if (msg.Data.Length < 5) return false;
                if (msg.Data[1] != 0x05) return false;
                if (msg.Data[2] != 0x10) return false;
                if (msg.Data[3] != 0x04) return false;
                if (msg.Data[4] != 0x00) return false;
                return true;
            });
            byte[] broadcastmessage;
            do
            {
                Channel.SendMessage(InviteRequest);
                broadcastmessage = BroadcastMessages.FirstOrDefault()?.Data;
            } while (broadcastmessage == null);

            return true;
        }

        public void FEPSOn()
        {
            Channel.SetProgrammingVoltage(J2534.Pin.PIN_13, 18000);
        }

        public void FEPSOff()
        {
            Channel.SetProgrammingVoltage(J2534.Pin.PIN_13, -1);
        }
        public void StartTesterPresentMessage()
        {
            //0x3E 0x01 gets a response from the module.  Anything else does not.
            J2534.PeriodicMessage TesterPresent = new J2534.PeriodicMessage(3000, header.Tx.Concat(new byte[2] { 0x3E, 0x02 }).ToArray(), J2534.TxFlag.NONE);
            Channel.StartPeriodicMessage(TesterPresent);
        }
        /// <summary>
        /// Request data by memory address
        /// </summary>
        /// <param name="Address">Address (24bit)</param>
        /// <returns></returns>
        public ServiceResult Mode23(int Address)
        {
            return serviceHandler((byte)Mode.DATA_BY_ADDRESS, 2, new byte[] { Address.Byte2(), Address.Byte1(), Address.Byte0() });
        }
        /// <summary>
        /// Request security access
        /// </summary>
        /// <param name="Step">Step in sequence</param>
        /// <param name="Keys">Key bytes (optional)</param>
        /// <returns></returns>
        public ServiceResult Mode27(byte Step, byte[] Keys = null)
        {
            IEnumerable<byte> Data = new byte[] { Step };
            if (Keys != null) Data = Data.Concat(Keys);

            return serviceHandler((byte)Mode.REQ_SECURITY_ACCESS, 1, Data);
        }
        /// <summary>
        /// Start diagnostic routine
        /// </summary>
        /// <param name="Routine">Routine to execute</param>
        /// <param name="RoutineData">Data (optional)</param>
        /// <returns></returns>
        public ServiceResult Mode31(DiagRoutine Routine, byte[] RoutineData = null)
        {
            IEnumerable<byte> Data = new byte[] { (byte)Routine };
            if (RoutineData != null) Data = Data.Concat(RoutineData);

            return serviceHandler((byte)Mode.START_DIAG_ROUTINE_BY_NUMBER, 1, Data);
        }
        /// <summary>
        /// Stop diagnostic routine
        /// </summary>
        /// <param name="Routine">Routine to stop</param>
        /// <returns></returns>
        public ServiceResult Mode32(DiagRoutine Routine)
        {
            return serviceHandler((byte)Mode.STOP_DIAG_ROUTINE_BY_NUMBER, 1, new byte[] { (byte)Routine });
        }
        /// <summary>
        /// Request tool to module transfer
        /// </summary>
        /// <param name="Length">Length in bytes of transfer</param>
        /// <param name="Address">Starting address of transfer block</param>
        /// <returns></returns>
        public ServiceResult Mode34(int Length, int Address)
        {
            return serviceHandler((byte)Mode.REQ_DOWNLOAD, 2, new byte[] { 0x80 /*Only format 0x80 is supported*/, Length.Byte1(), Length.Byte0(), Address.Byte2(), Address.Byte1(), Address.Byte0() });
        }
        /// <summary>
        /// Request module to tool transfer
        /// </summary>
        /// <param name="Length">Length in bytes of transfer</param>
        /// <param name="Address">Starting address of transfer block</param>
        /// <returns></returns>
        public ServiceResult Mode35(int Length, int Address)
        {
            return serviceHandler((byte)Mode.REQ_UPLOAD, 2, new byte[] { 0x80, Length.Byte1(), Length.Byte0(), Address.Byte2(), Address.Byte1(), Address.Byte0() });
        }
        /// <summary>
        /// Data transfer receive
        /// </summary>
        /// <param name="NumOfMessages">Number of expected messages</param>
        /// <returns></returns>
        public IEnumerable<ArraySegment<byte>> Mode36(int NumOfMessages)
        {
            var ResponseQuery = queryCache.Resolve((byte)Mode.DATA_TRANSFER, () => rxQueue.DecueueWhere(successPredicate((byte)Mode.DATA_TRANSFER)));

            lock (ResponseQuery)
            {
                var result = ResponseQuery.Take(NumOfMessages).Select(msg => new ArraySegment<byte>(msg.Data, header.Rx.Length + 1, 6)).ToArray();
                if (result.Length == NumOfMessages) return result;
            }

            throw new Exception($"Too few messages in block data transfer!");
        }
        /// <summary>
        /// Data transfer transmit
        /// </summary>
        /// <param name="Data">Data to send</param>
        public void Mode36(ArraySegment<byte>[] Data)
        {
            Channel.SendMessages(Data.Select(dat => header.Tx.ConcatByte((byte)Mode.DATA_TRANSFER).Concat(dat)).ToArray());
        }
        public ServiceResult Mode37()
        {
            return serviceHandler((byte)Mode.TRANSFER_ROUTINE_EXIT, 1, new byte[] { 0x80 });
        }
        public ServiceResult ModeA0(IEnumerable<byte[]> PacketList)
        {
            throw new NotImplementedException();
        }
        public ServiceResult ModeA0()
        {
            throw new NotImplementedException();
        }
        public ServiceResult ModeA1(IEnumerable<byte[]> PacketList)
        {
            throw new NotImplementedException();
        }

    }
}