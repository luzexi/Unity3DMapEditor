
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;


namespace Network.Packets
{
    public class GCConnect : PacketBase
    {
  

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CONNECT;
        }

        public override int getSize()
        {
            return sizeof(short) * 2 +
                sizeof(byte) +
                m_Position.getSize();
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            
            buff.WriteShort(m_ServerID);
            buff.WriteShort(m_SceneID);
            //byte[] bytes = NET_DEFINE.StructToBytes(m_Position);
            m_Position.writeToBuff(ref buff);
            //buff.Write(ref bytes, bytes.Length);
            buff.WriteByte(m_Estate);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadShort(ref m_ServerID) != sizeof(short)) return false;
            if (buff.ReadShort(ref m_SceneID) != sizeof(short)) return false;
            if(!m_Position.readFromBuff(ref buff)) return false;
     
//             int size = Marshal.SizeOf(typeof(WORLD_POS));
//             byte[] bytes = new byte[size];
//             if (buff.Read(ref bytes, size) != size) return false;
//             m_Position = (WORLD_POS)NET_DEFINE.BytesToStruct(bytes, typeof(WORLD_POS));
            if (buff.ReadByte(ref m_Estate) != sizeof(byte)) return false;
        
            return true;
        }

        //public interface
        public short ServerID{
            get { return this.m_ServerID; }
            set { m_ServerID = value; }
        }
        public short SceneID{
            get { return this.m_SceneID; }
            set { m_SceneID = value; }
        }
        public WORLD_POS Position{
            get { return this.m_Position; }
            set { m_Position = value; }
            
        }
        public byte Estate{
            get { return this.m_Estate; }
            set { m_Estate = value; }
        }
        //数据
        private short m_ServerID;
        private short m_SceneID;
        private WORLD_POS m_Position;
        private byte m_Estate;//1表示服务器正在存盘当前玩家，请玩家等待
    }


    public class GCConnectFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCConnect(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CONNECT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(short) * 2 +
                sizeof(byte) +
                WORLD_POS.GetMaxSize();
        }
    };
}