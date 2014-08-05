using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCTargetListAndHitFlags : PacketBase
    {

        public enum TARGETTYPE
		{
			UNIT_USE_SKILL = 0,
			SPECIAL_OBJ_ACTIVATE,
		};
		public enum MAXTARGETLIST
		{
			MAX_TARGET_LIST_SIZE = 64,
		};
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_TARGET_LIST_AND_HIT_FLAGS;
        }

        public override int getSize()
        {
            return sizeof(byte) +
                sizeof(int) *3 +
                sizeof(short) +
                WORLD_POS.GetMaxSize()*2+
                sizeof(float) + 
                (int)m_HitFlagList.GetByteSize() +
                sizeof(int) * m_nTargetNum;
         
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if(buff.ReadByte(ref m_nDataType)!= sizeof(byte))return false;
            if(buff.ReadInt(ref m_nObjID) != sizeof(int)) return false;
            if(buff.ReadInt(ref m_nLogicCount) != sizeof(int)) return false;
            if(buff.ReadShort(ref m_nSkillOrSpecialObjDataID)!= sizeof(short))return false;
            if(!m_posUser.readFromBuff(ref buff))return false;
            if(buff.ReadInt(ref m_nTargetID) != sizeof(int)) return false;
            if(!m_posTarget.readFromBuff(ref buff)) return false;
            if(buff.ReadFloat(ref m_fDir)!=sizeof(float))return false;
            if (buff.ReadByte(ref m_nTargetNum) != sizeof(byte)) return false;
            byte[] val = m_HitFlagList.GetFlags();
            for (uint i = 0; i < m_HitFlagList.GetByteSize(); i++)
            {
                if (buff.ReadByte(ref val[i]) != sizeof(byte)) return false;
            }
            for (byte i = 0; i < m_nTargetNum; i++)
            {
                if (buff.ReadInt(ref m_aTargetList[i]) != sizeof(int))return false;
            }
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public byte DataType
        {
            get { return m_nDataType; }
        }

        public int ObjID
        {
            get { return m_nObjID; }
        }

        public int LogicCount
        {
            get { return m_nLogicCount; }
        }

        public short SkillOrSpecialObjDataID
        {
            get { return m_nSkillOrSpecialObjDataID; }
        }

        public WORLD_POS PosUser
        {
            get { return m_posUser; }
        }

        public int TargetID
        {
            get { return m_nTargetID; }
        }

        public WORLD_POS PosTarget
        {
            get { return m_posTarget; }
        }

        public float Dir
        {
            get { return m_fDir; }
        }

        public GameUtil.BitFlagSet_T HitFlagList
        {
            get { return m_HitFlagList; }
        }

        public byte TargetNum
        {
            get { return m_nTargetNum;}
        }

        public int[] TargetList
        {
            get { return m_aTargetList; }
        }
        private byte		                m_nDataType;		// 是角色使用技能还是陷阱之类的特殊对象爆炸或激活
		private int 		                m_nObjID;			// ObjID， 技能使用者或正在激活的对象
		private int 		                m_nLogicCount;		// 逻辑计数， 技能使用者或正在激活的对象的逻辑计数
		private short		                m_nSkillOrSpecialObjDataID;		// 技能或特殊对象的资源ID
		private WORLD_POS	                m_posUser;			// 使用者坐标或正在激活的对象的坐标
		private int			                m_nTargetID;			// 目标角色，主要是使用者需要面向的目标
		private WORLD_POS	                m_posTarget;		// 目标坐标，主要是使用者需要面向的位置
		private float		                m_fDir;				// 技能的方向，主要是使用者需要面向的方向
		private GameUtil.BitFlagSet_T		m_HitFlagList = new GameUtil.BitFlagSet_T((int)MAXTARGETLIST.MAX_TARGET_LIST_SIZE);		// 目标被击中与否的标记列表，一个标记对应下面的目标列表中的一个目标对象，用索引对应。
		private byte				        m_nTargetNum;		// 影响的目标数目
		private int[]				        m_aTargetList = new int[(int)MAXTARGETLIST.MAX_TARGET_LIST_SIZE];		// 影响的目标ID列表
    };

    public class GCTargetListAndHitFlagsFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCTargetListAndHitFlags(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_TARGET_LIST_AND_HIT_FLAGS; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) +
              sizeof(int) * 3 +
              sizeof(short) +
              WORLD_POS.GetMaxSize() * 2 +
              sizeof(float) +
              GameUtil.BitFlagSet_T.getMaxSize((uint)GCTargetListAndHitFlags.MAXTARGETLIST.MAX_TARGET_LIST_SIZE) +
              sizeof(int) * (int)(GCTargetListAndHitFlags.MAXTARGETLIST.MAX_TARGET_LIST_SIZE);
        }
    };
}
