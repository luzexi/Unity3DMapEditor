using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{
	public class CGUseAbility:	PacketBase
	{
		//公用接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadShort(ref m_Ability) != sizeof(short)) return false;
            if (buff.ReadInt(ref m_Prescription) != sizeof(int)) return false;
            if (buff.ReadUint(ref m_Platform) != sizeof(uint)) return false;
            if (buff.ReadUint(ref m_SpecialFlag) != sizeof(uint)) return false;
            return true;
        }
		public override int writeToBuff(ref NetOutputBuffer buff) 
        {
            buff.WriteShort(m_Ability);
            buff.WriteInt(m_Prescription);
            buff.WriteUint(m_Platform);
            buff.WriteUint(m_SpecialFlag);
            return   NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

		public override short getPacketID() { return (short)PACKET_DEFINE.PACKET_CG_USEABILITY; }
		public override int getSize() { return sizeof(short)+
															  sizeof(int)+
															  sizeof(uint)+
															  sizeof(uint); }

        public void SetAbilityID(short aid) { m_Ability = aid; }
        public short GetAbilityID() { return m_Ability; }

        public void SetPrescriptionID(int pid) { m_Prescription = pid; }
        public int GetPrescriptionID() { return m_Prescription; }

        public void SetPlatformGUID(uint guid) { m_Platform = guid; }
        public uint GetPlatformGUID() { return m_Platform; }

        public void SetSpecialFlag(uint uFlag) { m_SpecialFlag = uFlag; }
        public uint GetSpecialFlag() { return m_SpecialFlag; }

        
        public void SetOwnerType(int nType) 
        {
            m_SpecialFlag = (uint)m_SpecialFlag & 0xffffff00 + (uint)nType & 0xff;
        }
      

        //public void SetBind(bool bBind) { if (bBind) m_SpecialFlag |= 0x1 << 8; }
        //public bool IsBind() { return (m_SpecialFlag & (0x1 << 8)) != 0; }

        public void SetItemIndex(int nIndex) { m_SpecialFlag |= (uint)nIndex << 16; }
        public ushort GetItemIndex() { return (ushort)(m_SpecialFlag >> 16); }

        public enum OWNER_TYPE
        {
            ROLE_EQUIPT= 0,
            PACKAGE,
            PET_EQUIPT,
        }

        public void SetOwnerType(OWNER_TYPE type)
        {
            m_SpecialFlag = (m_SpecialFlag & 0xffffff00) | (0xff & (uint)type);
        }

        public OWNER_TYPE GetOwnerType()
        {
            return (OWNER_TYPE)((uint)(m_SpecialFlag & 0xff));
        }

        public void SetPetIndex(int index)
        {
            m_SpecialFlag = (m_SpecialFlag & 0xffff00ff) | (uint)index << 8;
        }

        public int GetPetIndex()
        {
            return (int)((m_SpecialFlag & 0x0000ff00) >> 8);
        }

        public short m_Ability;
        public int m_Prescription;
        public uint m_Platform;
		// 添加一个特殊标记，用于新增功能 [10/8/2011 Sun]
		// 0-7bit，1背包，0身上装备
		// 16-31bit, 道具位置索引
        public uint m_SpecialFlag = 0;
	};

	public class CGUseAbilityFactory: 	PacketFactory
	{
		public override PacketBase CreatePacket()  { return new CGUseAbility() ; }
		 public override int GetPacketID()			{  return (int)PACKET_DEFINE.PACKET_CG_USEABILITY; }
		public override int GetPacketMaxSize()		{ return sizeof(short)
														+sizeof(int)
														+sizeof(uint)
														+sizeof(uint);}
	};
}
