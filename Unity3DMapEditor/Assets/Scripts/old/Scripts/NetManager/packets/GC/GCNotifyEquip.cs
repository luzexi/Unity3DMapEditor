
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class GCNotifyEquip : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadShort(ref m_BagIndex) != sizeof(short)) return false;
            if (!m_Item.readFromBuff(ref buff)) return false;

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //todo
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_NOTIFYEQUIP;
        }
        public override int getSize()
        {
            return  m_Item.getSize() +
                    sizeof(short);
        }

        //public interface
       
        public short BagIndex
        {
            get { return this.m_BagIndex; }
        }
        public _ITEM Item
        {
            get { return this.m_Item; }
        }

        //数据

        private short m_BagIndex;
        private _ITEM m_Item = new _ITEM();

    };
    public class GCNotifyEquipFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCNotifyEquip(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_NOTIFYEQUIP; }
        public override int GetPacketMaxSize()
        {
            return _ITEM.GetMaxSize() +
                 sizeof(short);
        }
    };
}