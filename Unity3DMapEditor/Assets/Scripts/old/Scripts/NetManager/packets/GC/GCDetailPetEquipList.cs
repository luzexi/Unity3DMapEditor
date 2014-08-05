using System;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCDetailPetEquipList : PacketBase
    {

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if(buff.ReadInt(ref m_ObjID)!=sizeof(int))return false;
            if (!m_PetGUID.readFromBuff(ref buff)) return false;
            if(buff.ReadUint(ref m_wPartFlags)!= sizeof(uint))return false;

            for (int i = 0; i < (int)PET_EQUIP.PEQUIP_NUMBER; i++)
            {
                if (((1 << i & m_wPartFlags) != 0)) 
                {
                    if (m_ItemList[i] == null) m_ItemList[i] = new _ITEM();

                    if (!m_ItemList[i].readFromBuff(ref buff)) return false;
                    if ((m_ItemList[i].ItemClass() != (byte)ITEM_CLASS.ICLASS_EQUIP)
                        &&(!m_ItemList[i].IsNullType()))
                    {
                        LogManager.LogError("GCDetailPetEquipList " + i + "is incorrect");
                        return false;
			        }
		        }
            }
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_DETAIL_PET_EQUIPLIST;
        }

        public override int getSize()
        {
            int uSize = PET_GUID_t.getMaxSize() + sizeof(uint) + sizeof(int);
            for (int i = 0; i < (int)PET_EQUIP.PEQUIP_NUMBER; i++)
            {
                if ((m_wPartFlags & (1 << i)) != 0)
                    uSize += m_ItemList[i].getSize();
            }
            return uSize;
        }

        public int ObjID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }

        public PET_GUID_t GUID
        {
            get { return this.m_PetGUID; }
            set { m_PetGUID = value; }
        }

        public _ITEM[] Items
        {
            get { return this.m_ItemList; }
            set { m_ItemList = value; }
        }

        public uint PartFlags
        {
            get { return m_wPartFlags; }
            set { m_wPartFlags = value; }
        }
        int         m_ObjID;
        PET_GUID_t  m_PetGUID;  //宠物的GUID
		/*
		|  ref [PET_EQUIP]
		|	 00000000 xxxxxxxx
		|               ||||||__ 利爪
		|               |||||___ 骨刺
		|               ||||____ 头角
		|               |||_____ 兽环
		|               ||______ 躯干
		|               |_______ 兽纹
		*/
		uint    m_wPartFlags;		// 每个位表示一个属性是否要刷新 PET_EQUIP
        _ITEM[] m_ItemList = new _ITEM[(int)PET_EQUIP.PEQUIP_NUMBER];
    };

    public class GCDetailPetEquipListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCDetailPetEquipList(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_DETAIL_PET_EQUIPLIST; }
        public override int GetPacketMaxSize()
        {
            return PET_GUID_t.getMaxSize() + sizeof(uint) + _ITEM.GetMaxSize()*(int)PET_EQUIP.PEQUIP_NUMBER + sizeof(int);
        }
    };
}