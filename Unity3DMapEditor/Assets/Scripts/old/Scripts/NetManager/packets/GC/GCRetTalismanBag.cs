using Network;
using Network.Handlers;
using System;

namespace Network.Packets
{
    public class GCRetTalismanBag : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_RET_TALISMANBAG;
        }

        public override int getSize()
        {
            int structSize = 0;
            if (ucTMItemCount != 0)
            {
                structSize = ucTMItemCount * m_TMBagItem[0].getSize();
            }
            if (ucTMEquipItemCount != 0)
            {
                structSize += ucTMEquipItemCount * m_TmEquipItem[0].getSize();
            }
            return sizeof(byte) * 4 + structSize;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadByte(ref ucTMBagCount) != sizeof(byte)) return false;
            if (buff.ReadByte(ref ucTMItemCount) != sizeof(byte)) return false;
	
            if (buff.ReadByte(ref ucTMEquipCount) != sizeof(byte)) return false;
            if (buff.ReadByte(ref ucTMEquipItemCount) != sizeof(byte)) return false;	
			
            for (byte i = 0; i < ucTMItemCount; i++)
            {
                _TALISMAN_ITEM item = new _TALISMAN_ITEM();
                m_TMBagItem[i] = item;
                if (!m_TMBagItem[i].readFromBuff(ref buff)) return false;
            }

            for (byte i = 0; i < ucTMEquipItemCount; i++)
            {
                _TALISMAN_ITEM item = new _TALISMAN_ITEM();
                m_TmEquipItem[i] = item;
                if (!m_TmEquipItem[i].readFromBuff(ref buff)) return false;
            }
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public byte BagItemCount
        {
            get { return ucTMItemCount; }
            set { ucTMItemCount = value; }
        }
        public byte BagCount
        {
            get { return ucTMBagCount; }
            set { ucTMBagCount = value; }
        }
        public byte EquipCount
        {
            get { return ucTMEquipCount; }
            set { ucTMEquipCount = value; }
        }
        public byte EquipItemCount
        {
            get { return ucTMEquipItemCount; }
            set { ucTMEquipItemCount = value; }
        }

        public _TALISMAN_ITEM[] BagItems
        {
            get { return m_TMBagItem; }
        }

        public _TALISMAN_ITEM[] EquipItems
        {
            get { return m_TmEquipItem; }
        }

        private byte ucTMBagCount;       //未装备法宝包的可用大小
        private byte ucTMItemCount;      //未装备法宝在包里的数量
        private _TALISMAN_ITEM[] m_TMBagItem = new _TALISMAN_ITEM[GAMEDEFINE.MAX_TALISMAN_SIZE_UNEQUIP];

        //法宝包（已装备）
        private byte ucTMEquipCount;     //已装备法宝包的可用大小
        private byte ucTMEquipItemCount; //已装备法宝在包里的数量
        _TALISMAN_ITEM[] m_TmEquipItem = new _TALISMAN_ITEM[GAMEDEFINE.MAX_TALISMAN_SIZE_EQUIP];
    }

    public class GCRetTalismanBagFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCRetTalismanBag(); }
        public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_RET_TALISMANBAG; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 4 +
                   _TALISMAN_ITEM.getMaxSize() *
                   (GAMEDEFINE.MAX_TALISMAN_SIZE_UNEQUIP + GAMEDEFINE.MAX_TALISMAN_SIZE_EQUIP);
        }
    }
}