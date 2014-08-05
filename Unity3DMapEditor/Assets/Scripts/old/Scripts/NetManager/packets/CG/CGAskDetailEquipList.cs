using System;
using System.Collections.Generic;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGAskDetailEquipList : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKDETAILEQUIPLIST; 
        }

        public override int getSize()
        {
            return sizeof(uint) + sizeof(int) + sizeof(uint);
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        uint objId;
        public uint ObjId
        {
            get { return objId; }
            set { objId = value; }
        }

        ASK_EQUIP_MODE m_Mode;
        public Network.Packets.ASK_EQUIP_MODE Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }
        /*
		|  ref [HUMAN_EQUIP]
		|	 00000000 xxxxxxxx
		|             ||||||||__ 武器  WEAPON
		|             |||||||___ 帽子 	DEFENCE
		|             ||||||____ 衣服  DEFENCE
		|             |||||_____ 护腕  DEFENCE
		|             ||||______ 靴子  DEFENCE
		|             |||_______ 腰带	ADORN
		|             ||________ 戒子	ADORN
		|             |_________ 项链	ADORN
		|
		*/
        uint m_wPartFlags;		// 每个位表示一个属性是否要刷新 HUMAN_EQUIP
        public uint PartFlags
        {
            get { return m_wPartFlags; }
            set { m_wPartFlags = value; }
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(objId);
            buff.WriteInt((int)m_Mode);
            buff.WriteUint(m_wPartFlags);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
    }

    class CGAskDetailEquipListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskDetailEquipList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASKDETAILEQUIPLIST; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint) + sizeof(int) + sizeof(uint);
        }
    }
}