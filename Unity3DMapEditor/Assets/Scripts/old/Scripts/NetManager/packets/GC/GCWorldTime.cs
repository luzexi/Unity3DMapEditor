
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class GCWorldTime : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if(buff.ReadInt(ref m_Time) != sizeof(int)) return false;

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            buff.WriteInt(m_Time);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_WORLDTIME;
        }
        public override int getSize()
        {
            return sizeof(int) + sizeof(short); ;
        }

        //public interface
    
        public short SceneID
        {
            get { return this.m_SceneID; }
            set { m_SceneID = value; }
        }
        public int Time
        {
            get { return this.m_Time; }
            set { m_Time = value; }
        }


        //数据
        private int m_Time; //WORLD_TIME
        private short m_SceneID;

    };
    public class GCWorldTimeFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCWorldTime(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_WORLDTIME; }
        public override int GetPacketMaxSize()
        {
            return  sizeof(int) + sizeof(short);
        }
    };
}