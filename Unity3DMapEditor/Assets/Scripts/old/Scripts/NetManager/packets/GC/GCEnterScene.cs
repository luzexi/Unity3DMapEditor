
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{


    public class GCEnterScene : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref m_byRet) != sizeof(byte)) return false;
            if (buff.ReadShort(ref m_nSceneID) != sizeof(short)) return false;
            //int size = Marshal.SizeOf(typeof(WORLD_POS));
            //byte[] bytes = new byte[size];
            //if (buff.Read(ref bytes, size) != size) return false;
            //m_posWorld = (WORLD_POS)NET_DEFINE.BytesToStruct(bytes, typeof(WORLD_POS));
          
            if(!m_posWorld.readFromBuff(ref buff)) return false;
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (buff.ReadByte(ref m_bIsCity) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_nCityLevel) != sizeof(byte)) return false;
            if (buff.ReadShort(ref m_nResID) != sizeof(short)) return false;
        

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            buff.WriteByte(m_byRet);
            buff.WriteShort(m_nSceneID);
//             byte[] bytes = NET_DEFINE.StructToBytes(m_posWorld);
//             buff.Write(ref bytes, Marshal.SizeOf(m_posWorld));
            m_posWorld.writeToBuff(ref buff);
            buff.WriteInt(m_ObjID);
            buff.WriteByte(m_bIsCity);
            buff.WriteByte(m_nCityLevel);
            buff.WriteShort(m_nResID);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_ENTERSCENE;
        }
        public override int getSize()
        {
            return sizeof(byte) * 3 +
                sizeof(short) * 2 +
                m_posWorld.getSize() +
                sizeof(int);
        }

        //public interface
        public byte Result
        {
            get { return this.m_byRet; }
            set { m_byRet = value; }
        }
        public short SceneID
        {
            get { return this.m_nSceneID; }
            set { m_nSceneID = value; }
        }
        public short ResID
        {
            get { return this.m_nResID; }
            set { m_nResID = value; }
        }
        public WORLD_POS Position
        {
            get { return this.m_posWorld; }
            set { m_posWorld = value; }
        }
        public int ObjectID{
            get { return this.m_ObjID; }
            set { m_ObjID = value; } 
        }
        public byte City
        {
            get { return this.m_bIsCity; }
            set { m_bIsCity = value; }
        }
        public byte CityLevel
        {
            get { return this.m_nCityLevel; }
            set { m_nCityLevel = value; }
        }
     
        //数据
        private byte m_byRet;		//服务器返回结果
        //	0 - 确认可以进入该场景
        //  1 - 玩家没有进入该场景的许可
        //  2 - 非法的场景ID
        //  3 - 场景人数已经满了
        //  ....
        private short m_nSceneID;		//场景ID
        private short m_nResID;		//客户端资源索引 [2011-10-25] by: cfp+
        private WORLD_POS m_posWorld;		//进入点的X,Z坐标点
        private int m_ObjID;
        private byte m_bIsCity;		//是否是城市
        private byte m_nCityLevel;	//城市级别

    };
    public class GCEnterSceneFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCEnterScene(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_ENTERSCENE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 3 +
                sizeof(short) * 2 +
                WORLD_POS.GetMaxSize() +
                sizeof(int);
        }
    };
}