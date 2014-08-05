
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{


    public class GCNewPlayer : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;

                 
            if(!m_posWorld.readFromBuff(ref buff)) return false;


            if (buff.ReadFloat(ref m_fDir) != sizeof(float)) return false;

            short equipVer= 0;
            if (buff.ReadShort(ref equipVer) != sizeof(short)) return false;
            m_wEquipVer = (ushort)EquipVer;
            if (buff.ReadFloat(ref m_fMoveSpeed) != sizeof(float)) return false;

        
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

//             buff.WriteInt(m_ObjID);
//             //byte[] bytes = NET_DEFINE.StructToBytes(m_posWorld);
//             //buff.Write(ref bytes, Marshal.SizeOf(m_posWorld));
//             buff.WriteStruct(m_posWorld);
//             buff.WriteInt((int)m_fDir);
//             buff.WriteInt((int)m_fMoveSpeed);
//             buff.WriteByte(m_IsNPC);
// 
             return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_NEWPLAYER;
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
        public int ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }
        public float Dir{
            get { return this.m_fDir; }
            set { m_fDir = value; }
        }
        public float MoveSpeed{
            get { return this.m_fMoveSpeed; }
            set { m_fMoveSpeed = value; }
        }
        public ushort EquipVer
        {
            get { return this.m_wEquipVer; }
            set { m_wEquipVer = value; }
        }

        //数据
   
        private int m_ObjID;		// ObjID
        WORLD_POS m_posWorld;		// 位置
        private float m_fDir;			// 方向
        ushort m_wEquipVer;	// 玩家的装备版本号
        private float m_fMoveSpeed;	// 移动速度


    };
    public class GCNewPlayerFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCNewPlayer(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_NEWPLAYER; }
        public override int GetPacketMaxSize()
        {
            return WORLD_POS.GetMaxSize() +
            sizeof(int) +
            sizeof(float) * 2 +
             sizeof(ushort);
        }
    };
}