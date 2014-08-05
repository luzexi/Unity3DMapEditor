using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Network;
using Network.Handlers;
using UnityEngine;
namespace Network.Packets
{
    public class GCDetailHealsAndDamages : PacketBase
    {
        public enum DETAIL_TYPE
		{
			IDX_HP_MODIFICATION = 0,
			IDX_MP_MODIFICATION,
			IDX_RAGE_MODIFICATION,
			IDX_STRIKE_POINT_MODIFICATION,
			NUM_OF_FLAGS,
		};
        
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            byte[] val = m_DirtyFlags.GetFlags();
            for (uint i = 0; i < m_DirtyFlags.GetByteSize(); i++)
            {
                if (buff.ReadByte(ref val[i]) != sizeof(byte)) return false;
            }
            if (buff.ReadInt(ref m_nReceiverID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_nSenderID) != sizeof(int)) return false;
			if (buff.ReadShort(ref m_nSkillID) != sizeof(short))return false;
            if (buff.ReadInt(ref m_nSenderLogicCount) != sizeof(int)) return false;
            if (IsHpModificationDirty())
            {
                if (buff.ReadInt(ref m_nHpModification) != sizeof(int)) return false;
            }
            if(IsMpModificationDirty())
            {
                if (buff.ReadInt(ref m_nMpModification) != sizeof(int)) return false;
            }
            if (IsStrikePointModificationDirty())
            {
                if (buff.ReadInt(ref m_nStrikePointModification) != sizeof(int)) return false;
            }

            if(buff.ReadByte(ref m_bIsCriticalHit) != sizeof(byte))return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
     
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }


        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_DETAIL_HEALS_AND_DAMAGES;
        }

        public override int getSize()
        {
            int nSize =  (int)m_DirtyFlags.GetByteSize() + sizeof(int) + sizeof(int) + sizeof(short) + sizeof(int);
			for(int nIdx = (int)DETAIL_TYPE.IDX_HP_MODIFICATION; (int)DETAIL_TYPE.NUM_OF_FLAGS>nIdx; ++nIdx)
			{
				if(m_DirtyFlags.GetFlagByIndex(nIdx))
				{
					nSize += sizeof(int);
				}
			}
			nSize += sizeof(byte);
			return nSize;
        }

        public int SenderID
        {
            get { return this.m_nSenderID; }
            set { m_nSenderID = value; }
        }

        public int RecieverID
        {
            get { return this.m_nReceiverID; }
            set { m_nReceiverID = value; }
        }

        public int SenderLogicCount
        {
            get { return this.m_nSenderLogicCount; }
            set { m_nSenderLogicCount = value; }
        }

        public short SkillID
        {
            get { return this.m_nSkillID; }
            set { m_nSkillID = value; }
        }

        public bool CriticalHit
        {
            get{return (m_bIsCriticalHit != 0);}
        }

        public int HPModification
        {
            get{return m_nHpModification;}
            set { m_nHpModification = value; }
        }

        public int MPModification
        {
            get { return m_nMpModification;}
            set { m_nMpModification = value; }
        }

        public int RageModification
        {
            get { return m_nRageModification; }
            set { m_nRageModification = value;}
        }

        public int StrikePointModification
        {
            get { return m_nStrikePointModification; }
            set { m_nStrikePointModification = value; }
        }


        public bool	IsHpModificationDirty(){return m_DirtyFlags.GetFlagByIndex((int)DETAIL_TYPE.IDX_HP_MODIFICATION);}
		public bool	IsMpModificationDirty(){return m_DirtyFlags.GetFlagByIndex((int)DETAIL_TYPE.IDX_MP_MODIFICATION);}
		public bool	IsRageModificationDirty(){return m_DirtyFlags.GetFlagByIndex((int)DETAIL_TYPE.IDX_RAGE_MODIFICATION);}
		public bool	IsStrikePointModificationDirty(){return m_DirtyFlags.GetFlagByIndex((int)DETAIL_TYPE.IDX_STRIKE_POINT_MODIFICATION);}

        private int m_nReceiverID;	// 效果接受者的ID
        private int m_nSenderID;	// 效果发送者的ID
        private short m_nSkillID;   //产生这次伤害的技能ID
        private int m_nSenderLogicCount; //效果创建者的逻辑计数
        private int m_nHpModification; //生命变化量
        private int m_nMpModification; //内力变化量
        private int m_nRageModification; //怒气变化量
        private int m_nStrikePointModification; //连击点变化量
        private byte m_bIsCriticalHit;	//是否是会心一击
        GameUtil.BitFlagSet_T m_DirtyFlags = new GameUtil.BitFlagSet_T((uint)DETAIL_TYPE.NUM_OF_FLAGS);
    };

    public class GCDetailHealsAndDamagesFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCDetailHealsAndDamages(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_DETAIL_HEALS_AND_DAMAGES; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + sizeof(int) + sizeof(short) + sizeof(int)*5 + GameUtil.BitFlagSet_T.getMaxSize((uint)GCDetailHealsAndDamages.DETAIL_TYPE.NUM_OF_FLAGS) + sizeof(byte);
        }
    };
}