
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{


    public class GCItemInfo : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadShort(ref m_BagIndex) != sizeof(short)) return false;
            if(buff.ReadInt(ref m_nsNull) != sizeof(int)) return false;
            if (!m_ITEM.readFromBuff(ref buff)) return false;
       
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_ITEMINFO;
        }
        public override int getSize()
        {
            return sizeof(short)+
                m_ITEM.getSize() +
                sizeof(bool);
        }

        //public interface
        public short BagIndex
        {
            get { return this.m_BagIndex; }
            set { m_BagIndex = value; }
        }
        public int IsNull
        {
            get { return this.m_nsNull; }
            set { m_nsNull = value; }
        }
        public _ITEM Item
        {
            get { return this.m_ITEM; }
            set { m_ITEM = value; }
        }

        //数据
        private short m_BagIndex;		//item 的BagIndex
        private int m_nsNull;		//物品是否为空		TRUE 代表没有Item,FALSE 代表有Item	
        private _ITEM m_ITEM = new _ITEM();


    };
    public class GCItemInfoFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCItemInfo(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_ITEMINFO; }
        public override int GetPacketMaxSize()
        {
            return  sizeof(short)+
                _ITEM.GetMaxSize() +
                sizeof(bool);
        }
    };
}