
using System;
using System.Collections.Generic;

using Network;

namespace Network.Packets
{
    public class GCLevelUp : PacketBase
    {

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_LEVELUP;
        }

        public override int getSize()
        {
            return sizeof(byte) + sizeof(uint) *2;
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
            if (buff.ReadUint(ref m_Level) != sizeof(uint)) return false;
            if (buff.ReadByte(ref m_LevelType) != sizeof(byte)) return false;
            return true;
        }
        public uint Level
        {
            get { return m_Level; }
        }
        public uint ObjectID
        {
            get { return m_ObjID; }
        }
        public byte LevelType
        {
            get { return m_LevelType; }
        }
        private uint m_Level;
        private uint m_ObjID;
        private byte m_LevelType; //enum	LEVELUP_TYPE 用于判断升级的类型，0为角色等级，1为境界等级 [2011-1-5] by: cfp+
    }


    public class GCLevelUpFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCLevelUp(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_LEVELUP; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) + sizeof(uint)*2;
        }
    };
}