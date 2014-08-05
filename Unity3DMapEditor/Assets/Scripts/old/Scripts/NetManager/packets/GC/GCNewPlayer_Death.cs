
using System;
using System.Collections.Generic;

using Network;

namespace Network.Packets
{
    
    public class GCNewPlayer_Death : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_ObjID) != sizeof(int)) return false;


            if (!m_posWorld.readFromBuff(ref buff)) return false;


            if (buff.ReadFloat(ref m_fDir) != sizeof(float)) return false;

            short equipVer = 0;
            if (buff.ReadShort(ref equipVer) != sizeof(short)) return false;
            m_wEquipVer = (ushort)EquipVer;
            if (buff.ReadFloat(ref m_fMoveSpeed) != sizeof(float)) return false;


            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
 
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_NEWPLAYER_DEATH;
        }
        public override int getSize()
        {
            return m_posWorld.getSize() +
                sizeof(int) +
                sizeof(float) * 2 +
                sizeof(ushort);
        }

        //public interface
        public WORLD_POS Position
        {
            get { return this.m_posWorld; }
            set { m_posWorld = value; }
        }
        public uint ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }
        public float Dir
        {
            get { return this.m_fDir; }
            set { m_fDir = value; }
        }
        public float MoveSpeed
        {
            get { return this.m_fMoveSpeed; }
            set { m_fMoveSpeed = value; }
        }
        public ushort EquipVer
        {
            get { return this.m_wEquipVer; }
            set { m_wEquipVer = value; }
        }

        //数据

        private uint m_ObjID;		// ObjID
        private WORLD_POS m_posWorld;		// 位置
        private float m_fDir;			// 方向
        private ushort m_wEquipVer;	// 玩家的装备版本号
        private float m_fMoveSpeed;	// 移动速度


    };
    public class GCNewPlayer_DeathFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCNewPlayer_Death(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_NEWPLAYER_DEATH; }
        public override int GetPacketMaxSize()
        {
            return WORLD_POS.GetMaxSize() +
            sizeof(int) +
            sizeof(float) * 2 +
             sizeof(ushort);
        }
    };
}