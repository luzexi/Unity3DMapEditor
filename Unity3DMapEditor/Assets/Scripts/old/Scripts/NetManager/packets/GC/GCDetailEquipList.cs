using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public enum ASK_EQUIP_MODE
    {
        ASK_EQUIP_ALL,
        ASK_EQUIP_SET
    };

    public class GCDetailEquipList : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_DETAILEQUIPLIST;
        }

        public override int getSize()
        {
            int uSize = sizeof(uint) + sizeof(int) + sizeof(uint); //[2010-11-04] cfp+ 16位不够用改成32位
            for (int i = 0; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
            {
                if ((m_wPartFlags & (1 << i) ) != 0) 
                    uSize += m_ItemList[i].getSize();
            }

            return uSize;
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

        _ITEM[] m_ItemList = new _ITEM[(int)HUMAN_EQUIP.HEQUIP_NUMBER];
        public void SetEquipData(HUMAN_EQUIP Point, _ITEM pEquip)
        {
            m_ItemList[(int)Point] = pEquip;
            m_wPartFlags |= (uint)(1 << (int)Point);
        }

        public _ITEM GetEquipData(HUMAN_EQUIP Point)
        {
            return m_ItemList[(int)Point];
        }

        public int GetItemCount()
        {
            if (m_Mode == ASK_EQUIP_MODE.ASK_EQUIP_ALL)
                return (int)HUMAN_EQUIP.HEQUIP_NUMBER;

            int nItemNum = 0;
            for (int i = 0; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
            {
                if ((m_wPartFlags & (1 << i)) != 0) 
                    nItemNum++;
            }
            return nItemNum;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref objId);
            
            int tempMode = 0;
            buff.ReadInt(ref tempMode);
            m_Mode = (ASK_EQUIP_MODE)tempMode;
            
            buff.ReadUint(ref m_wPartFlags);
            for (int i = 0; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
            {
                if ((m_wPartFlags & (1 << i)) != 0)
                {
                    m_ItemList[i] = new _ITEM();
                    m_ItemList[i].readFromBuff(ref buff);

                    if ( ( (ITEM_CLASS)(m_ItemList[i].ItemClass()) != ITEM_CLASS.ICLASS_EQUIP ) 
                        && (!m_ItemList[i].IsNullType()) )
                    {
                        LogManager.LogError("equip type error");
                    }
                }
            }
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }
    }

    public class GCDetailEquipListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCDetailEquipList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_DETAILEQUIPLIST; }
        public override int GetPacketMaxSize()
        {
            int uSize = sizeof(uint) + sizeof(int) + sizeof(uint); //[2010-11-04] cfp+ 16位不够用改成32位
            uSize += _ITEM.GetMaxSize() * (int)HUMAN_EQUIP.HEQUIP_NUMBER;

            return uSize;
        }
    };
}