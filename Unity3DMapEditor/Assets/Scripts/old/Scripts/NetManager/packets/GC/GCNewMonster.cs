
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{


    public class GCNewMonster : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;

                 
            if(!m_posWorld.readFromBuff(ref buff)) return false;


            if (buff.ReadFloat(ref m_fDir) != sizeof(float)) return false;
         
            if (buff.ReadFloat(ref m_fMoveSpeed) != sizeof(float)) return false;

            if(buff.ReadByte(ref m_IsNPC) != sizeof(byte)) return false;
        
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            buff.WriteInt(m_ObjID);
            m_posWorld.writeToBuff(ref buff);
            buff.WriteInt((int)m_fDir);
            buff.WriteInt((int)m_fMoveSpeed);
            buff.WriteByte(m_IsNPC);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_NEWMONSTER;
        }
        public override int getSize()
        {
            return m_posWorld.getSize() +
                sizeof(int) +
                sizeof(float) * 2 +
                sizeof(byte);
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
        public byte IsNpc{
            get { return this.m_IsNPC; }
            set { m_IsNPC = value; }
        }

        //数据
   
        private int m_ObjID;		// ObjID
        WORLD_POS m_posWorld;		// 位置
        private float m_fDir;			// 方向
        private float m_fMoveSpeed;	// 移动速度
        private byte m_IsNPC;

    };
    public class GCNewMonsterFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCNewMonster(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_NEWMONSTER; }
        public override int GetPacketMaxSize()
        {
            return WORLD_POS.GetMaxSize() +
               sizeof(int) +
               sizeof(float) * 2 +
               sizeof(byte);
        }
    };
}