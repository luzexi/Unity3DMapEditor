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
    class GCUseItemResult : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_USEITEMRESULT;
        }

        public override int getSize()
        {
            return sizeof(int);
        }

        USEITEM_RESULT m_Result;
        public USEITEM_RESULT Result
        {
            get { return m_Result; }
            set { m_Result = value; }
        }
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            int iTemp = 0;
            buff.ReadInt(ref iTemp);
            m_Result = (USEITEM_RESULT)iTemp;

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }
    }

    public class GCUseItemResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCUseItemResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_USEITEMRESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int);
        }
    };
}