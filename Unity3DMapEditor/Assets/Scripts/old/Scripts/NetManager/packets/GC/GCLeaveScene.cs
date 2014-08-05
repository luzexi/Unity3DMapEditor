
using System;
using System.Collections.Generic;

using Network;

namespace Network.Packets
{
    public class GCLeaveScene : PacketBase
    {

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_LEAVESCENE;
        }

        public override int getSize()
        {
            return sizeof(byte) + sizeof(uint);
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //包内容, 10为包头偏移
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            // 包内容
            if (buff.ReadUint(ref m_ObjID) != sizeof(int)) return false;
            if (buff.ReadByte(ref m_LeaveType) != sizeof(byte)) return false;
            return true;
        }
 
        public uint ObjectID
        {
            get { return m_ObjID; }
        }
        public byte LeaveType
        {
            get { return m_LeaveType; }
        }
        private uint m_ObjID;
        private byte m_LeaveType; //enum	LEVELUP_TYPE 用于判断升级的类型，0为角色等级，1为境界等级 [2011-1-5] by: cfp+
    }


    public class GCLeaveSceneFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCLeaveScene(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_LEAVESCENE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) + sizeof(uint);
        }
    };
}