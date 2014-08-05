using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

using UnityEngine;

namespace Network.Packets
{
    public class GCUseSymbolResult : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_USECHARMRESULT;
        }

        public override int getSize()
        {
            return sizeof(byte);
        }

        byte m_Result;
        public byte Result
        {
            get { return m_Result; }
            set { m_Result = value; }
        }
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadByte(ref m_Result);
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }
    }

    public class GCUseSymbolResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCUseSymbolResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_USECHARMRESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte);
        }
    };
}