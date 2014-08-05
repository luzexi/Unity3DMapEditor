
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{

    public enum ENTER_TYPE{
        ENTER_TYPE_FIRST=0,			//0 登录后第一次进入场景
        ENTER_TYPE_FROM_OTHER		//1 从其他场景转入
    }

    public class CGEnterScene: PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_byEnterType) != sizeof(byte)) return false;
            if (buff.ReadShort(ref m_nSceneID) != sizeof(short)) return false;
     
            return m_posWorld.readFromBuff(ref buff);
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            buff.WriteByte(m_byEnterType);
            buff.WriteShort(m_nSceneID);
            //byte[] bytes = NET_DEFINE.StructToBytes(m_posWorld);
            //buff.Write(ref bytes, Marshal.SizeOf(m_posWorld));
            //buff.WriteStruct(m_posWorld);
            m_posWorld.writeToBuff(ref buff);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ENTERSCENE;
        }
        public override int getSize()
        {
            return sizeof(byte) +
                sizeof(short) +
                m_posWorld.getSize();
        }

        //public interface
        public byte EnterType{
            get { return this.m_byEnterType; }
            set { m_byEnterType = value; }
        }
        public short SceneID{
            get { return this.m_nSceneID; }
            set { m_nSceneID = value; }
        }
        public WORLD_POS Position{
            get { return this.m_posWorld; }
            set { m_posWorld = value; }
        }

        //数据
        private byte m_byEnterType;	//进入类型
        //#define ENTER_TYPE_FIRST		(0)	 登录后第一次进入场景
        //#define ENTER_TYPE_FROM_OTHER	(1)	 从其他场景转入
        //...
        private short m_nSceneID;		//场景ID
        private WORLD_POS m_posWorld;		//进入点的X,Z坐标点

    };
    public class CGEnterSceneFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGEnterScene(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ENTERSCENE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) +
                sizeof(short) +
                WORLD_POS.GetMaxSize();
        }
    };
}