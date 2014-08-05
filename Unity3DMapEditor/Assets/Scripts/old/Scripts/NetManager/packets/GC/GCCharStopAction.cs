using System;

using Network;
using Network.Handlers;
namespace Network.Packets
{
    public class GCCharStopAction : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_ObjID) != sizeof(uint)) return false;
            if (buff.ReadInt(ref m_nLogicCount) != sizeof(int)) return false;
            if (buff.ReadUint(ref m_uStopTime) != sizeof(uint)) return false;

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
     
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARSTOPACTION;
        }

        public override int getSize()
        {
            return sizeof(int) * 3;
        }

        public uint ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }

        public int LogicCount
        {
            get { return this.m_nLogicCount; }
            set { m_nLogicCount = value; }
        }

        public uint StopTime
        {
            get { return this.m_uStopTime; }
            set { m_uStopTime = value; }
        }


        uint m_ObjID;			// ObjID
        int  m_nLogicCount;		// 逻辑计数
        uint m_uStopTime;
    };

    public class GCCharStopActionFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharStopAction(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARSTOPACTION; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 3;
        }
    };
}