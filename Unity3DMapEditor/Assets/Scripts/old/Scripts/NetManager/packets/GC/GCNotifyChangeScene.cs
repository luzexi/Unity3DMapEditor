
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class GCNotifyChangeScene : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadShort(ref m_CurrentSceneID) != sizeof(short)) return false;
            if (buff.ReadShort(ref m_TargetSceneID) != sizeof(short)) return false;
   
      
            if (!m_TargetPos.readFromBuff(ref buff)) return false;
            if (buff.ReadByte(ref m_TargetDir) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_Flag) != sizeof(byte)) return false;
            if (buff.ReadShort(ref m_nResID) != sizeof(short)) return false;

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //todo
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_NOTIFYCHANGESCENE;
        }
        public override int getSize()
        {
            return m_TargetPos.getSize() +
                sizeof(short) * 3 +
                sizeof(byte) * 2;
        }

        //public interface
        public WORLD_POS Position
        {
            get { return this.m_TargetPos; }
            set { m_TargetPos = value; }
        }
        public short CurrentSceneID
        {
            get { return this.m_CurrentSceneID; }
        }
        public short TargetSceneID
        {
            get { return this.m_TargetSceneID; }
      
        }
        public byte TargetDir
        {
            get { return this.m_TargetDir; }
        }
        public byte Flag
        {
            get { return this.m_Flag; }
        }
        public short ResID
        {
            get { return this.m_nResID; }
        }

        //数据

        private short m_CurrentSceneID;		// ObjID
        private short m_TargetSceneID;		// ObjID
        private WORLD_POS m_TargetPos;			//目标场景位置
        private byte m_TargetDir;			//目标场景方向
        private byte m_Flag;
        private short m_nResID;              //目标场景的客户端场景资源索引 [2011-10-25] by: cfp+

    };
    public class GCNotifyChangeSceneFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCNotifyChangeScene(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_NOTIFYCHANGESCENE; }
        public override int GetPacketMaxSize()
        {
            return WORLD_POS.GetMaxSize() +
                 sizeof(short) * 3 +
                 sizeof(byte) * 2;
        }
    };
}