// 用于测试与简易服务端相连 [12/15/2011 ZL]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public enum  ENUM_PET_ATTR_TYPE
	{
		TYPE_NORMAL = 0, 
		TYPE_EXCHANGE,
		TYPE_STALL,
		TYPE_PLAYERSHOP,
		TYPE_CONTEX_MENU_OTHER_PET,
	};

	public enum ENUM_PET_DETAIL_ATTR
	{
		PET_DETAIL_ATTR_INVALID		= -1,
		PET_DETAIL_ATTR_OBJ_ID,				// ObjID
		PET_DETAIL_ATTR_DATA_ID,			// DataID
		PET_DETAIL_ATTR_NAME,				// 名称
		PET_DETAIL_ATTR_AI_TYPE,			// 性格
		PET_DETAIL_ATTR_SPOUSE_GUID,		// 配偶ID
		PET_DETAIL_ATTR_LEVEL,				// 等级
		PET_DETAIL_ATTR_EXP,				// 经验
		PET_DETAIL_ATTR_HP,					// 血当前值
		PET_DETAIL_ATTR_HPMAX,				// 血最大值

		PET_DETAIL_ATTR_LIFE,				// 当前寿命
		PET_DETAIL_ATTR_GENERATION,			// 几代宠
		PET_DETAIL_ATTR_HAPPINESS,			// 快乐度

		PET_DETAIL_ATTR_ATT_PHYSICS,		// 物理攻击力
		PET_DETAIL_ATTR_ATT_MAGIC,			// 魔法攻击力
		PET_DETAIL_ATTR_DEF_PHYSICS,		// 物理防御力
		PET_DETAIL_ATTR_DEF_MAGIC,			// 魔法防御力

		PET_DETAIL_ATTR_HIT,				// 命中率
		PET_DETAIL_ATTR_MISS,				// 闪避率
		PET_DETAIL_ATTR_CRITICAL,			// 会心率
        PET_DETAIL_ATTR_DEF_CRITICAL,       //抗暴

		PET_DETAIL_ATTR_MODELID,			// 外形
		PET_DETAIL_ATTR_MOUNTID,			// 座骑ID
		
		PET_DETAIL_ATTR_STRPERCEPTION,		// 力量资质
		PET_DETAIL_ATTR_CONPERCEPTION,		// 体力资质
		PET_DETAIL_ATTR_DEXPERCEPTION,		// 身法资质
		PET_DETAIL_ATTR_SPRPERCEPTION,		// 灵气资质
		PET_DETAIL_ATTR_INTPERCEPTION,		// 定力资质

		PET_DETAIL_ATTR_STR,				// 力量
		PET_DETAIL_ATTR_CON,				// 体力
		PET_DETAIL_ATTR_DEX,				// 身法
		PET_DETAIL_ATTR_SPR,				// 灵气
		PET_DETAIL_ATTR_INT,				// 定力
		PET_DETAIL_ATTR_GENGU,				// 根骨

        PET_DETAIL_ATTR_RANDOM_STR,			// 力量
        PET_DETAIL_ATTR_RANDOM_CON,			// 体力
        PET_DETAIL_ATTR_RANDOM_DEX,			// 身法
        PET_DETAIL_ATTR_RANDOM_SPR,			// 灵气
        PET_DETAIL_ATTR_RANDOM_INT,			// 定力

		PET_DETAIL_ATTR_POINT_REMAIN,		// 潜能点

		PET_DETAIL_ATTR_SKILL_0,			// 技能0
		PET_DETAIL_ATTR_SKILL_1,			// 技能1
		PET_DETAIL_ATTR_SKILL_2,			// 技能2
		PET_DETAIL_ATTR_SKILL_3,			// 技能3
		PET_DETAIL_ATTR_SKILL_4,			// 技能4
		PET_DETAIL_ATTR_SKILL_5,			// 技能5


		PET_DETAIL_ATTR_NUMBERS
	};

    public class GCDetailAttrib_Pet : PacketBase
    {

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_DETAILATTRIB_PET;
        }

        public override int getSize()
        {
            int uSize = PET_GUID_t.getMaxSize() + sizeof(uint)*2 + sizeof(int);
            uint i;
            for (i = 0; i < (uint)ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_NUMBERS; i++)
            {
                if (IsSetBit((ENUM_PET_DETAIL_ATTR)i))
                {
                    switch ((ENUM_PET_DETAIL_ATTR)i)
                    {
                        case ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SPOUSE_GUID:
                            uSize += PET_GUID_t.getMaxSize();
                            break;
                        case ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_OBJ_ID:
                            uSize += sizeof(uint);
                            break;
                        case ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_NAME:
                            uSize += sizeof(byte) + m_byNameSize;
                            break;
                        case ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_GENERATION:
                        case ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_HAPPINESS:
                            uSize += sizeof(byte);
                            break;

                        case ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_0:
                        case ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_1:
                        case ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_2:
                        case ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_3:
                        case ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_4:
                        case ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_5:
                            uSize += _OWN_SKILL.GetMaxSize();
                            break;
                        default:
                            uSize += sizeof(uint);
                            break;
                    }
                }
            }

            uSize += sizeof(byte);
            uSize += m_ExtraInfoLength;
            return uSize;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            m_GUID.readFromBuff(ref buff);
            buff.ReadInt(ref m_nTradeIndex);
            buff.ReadUint(ref m_uLowFlags);
            buff.ReadUint(ref m_uHighFlags);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_OBJ_ID))
                buff.ReadUint(ref m_ObjID);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DATA_ID))
                buff.ReadInt(ref m_nDataID);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_NAME))
            {
                buff.ReadByte(ref m_byNameSize);
                buff.Read(ref m_szName, m_byNameSize);
            }
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_AI_TYPE))
                buff.ReadInt(ref m_nAIType);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SPOUSE_GUID))
                m_SpouseGUID.readFromBuff(ref buff);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_LEVEL))
                buff.ReadInt(ref m_nLevel);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_EXP))
                buff.ReadInt(ref m_nExp);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_HP))
                buff.ReadInt(ref m_nHP);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_HPMAX))
                buff.ReadInt(ref m_nHPMax);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_LIFE))
                buff.ReadInt(ref m_nLife);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_GENERATION))
                buff.ReadByte(ref m_byGeneration);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_HAPPINESS))
                buff.ReadByte(ref m_byHappiness);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_ATT_PHYSICS))
                buff.ReadInt(ref m_nAtt_Physics);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_ATT_MAGIC))
                buff.ReadInt(ref m_nAtt_Magic);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DEF_PHYSICS))
                buff.ReadInt(ref m_nDef_Physics);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DEF_MAGIC))
                buff.ReadInt(ref m_nDef_Magic);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_HIT))
                buff.ReadInt(ref m_nHit);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_MISS))
                buff.ReadInt(ref m_nMiss);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_CRITICAL))
                buff.ReadInt(ref m_nCritical);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DEF_CRITICAL))
                buff.ReadInt(ref m_nDefCritical);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_MODELID))
                buff.ReadInt(ref m_nModelID);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_MOUNTID))
                buff.ReadInt(ref m_nMountID);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_STRPERCEPTION))
                buff.ReadInt(ref m_StrPerception);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_CONPERCEPTION))
                buff.ReadInt(ref m_ConPerception);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DEXPERCEPTION))
                buff.ReadInt(ref m_DexPerception);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SPRPERCEPTION))
                buff.ReadInt(ref m_SprPerception);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_INTPERCEPTION))
                buff.ReadInt(ref m_IntPerception);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_STR))
                buff.ReadInt(ref m_Str);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_CON))
                buff.ReadInt(ref m_Con);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_DEX))
                buff.ReadInt(ref m_Dex);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SPR))
                buff.ReadInt(ref m_Spr);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_INT))
                buff.ReadInt(ref m_Int);
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_GENGU))
                buff.ReadInt(ref m_GenGu);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_RANDOM_STR))
                buff.ReadInt(ref m_StrBring);							// 力量
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_RANDOM_CON))
                buff.ReadInt(ref m_ConBring);							// 体力
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_RANDOM_DEX))
                buff.ReadInt(ref m_DexBring);							// 身法
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_RANDOM_SPR))
                buff.ReadInt(ref m_SprBring);							// 灵气
            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_RANDOM_INT))
                buff.ReadInt(ref m_IntBring);							// 定力


            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_POINT_REMAIN))
                buff.ReadInt(ref m_nRemainPoint);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_0))
                m_aSkill[0].readFromBuff(ref buff);
               

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_1))
                m_aSkill[1].readFromBuff(ref buff);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_2))
                m_aSkill[2].readFromBuff(ref buff);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_3))
                m_aSkill[3].readFromBuff(ref buff);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_4))
                m_aSkill[4].readFromBuff(ref buff);

            if (IsSetBit(ENUM_PET_DETAIL_ATTR.PET_DETAIL_ATTR_SKILL_5))
                m_aSkill[5].readFromBuff(ref buff);

            buff.ReadByte(ref m_ExtraInfoLength);
            buff.Read(ref m_ExtraInfoData, m_ExtraInfoLength);
            return true;
        }

        public PET_GUID_t GUID
        {
            get { return m_GUID; }
        }
        public int TradeIndex
        {
            get { return m_nTradeIndex; }
        }
        public uint LowFlag
        {
            get { return m_uLowFlags; }
        }
        public uint HighFlag
        {
            get { return m_uHighFlags; }
        }
        bool IsSetBit(int bit)
        {
            if (bit < 32)
            {
                return (m_uLowFlags & (1 << bit)) != 0;
            }
            else
            {
                return (m_uHighFlags & (1 << bit)) != 0;
            }
        }

        public bool IsSetBit(ENUM_PET_DETAIL_ATTR i)
        {
            return IsSetBit((int)i);
        }
        public uint ObjectID { get { return m_ObjID; } }
        public int DataID { get { return m_nDataID; } }
        public byte NameSize { get { return m_byNameSize; } }
        public byte[] Name { get { return m_szName; } }
        public int AiType { get { return m_nAIType; } }
        public PET_GUID_t SpouseGUID { get { return m_SpouseGUID; } }
        public int Level { get { return m_nLevel; } }
        public int Exp { get { return m_nExp; } }
        public int HP { get { return m_nHP; } }
        public int HPMax { get { return m_nHPMax; } }
        public int Life { get { return m_nLife; } }
        public byte Generation { get { return m_byGeneration; } }
        public byte Happiness { get { return m_byHappiness; } }
        public int AttPhysics { get { return m_nAtt_Physics; } }
        public int AttMagic { get { return m_nAtt_Magic; } }
        public int DefPhysics { get { return m_nDef_Physics; } }
        public int DefMagic { get { return m_nDef_Magic; } }
        public int Hit { get { return m_nHit; } }
        public int Miss { get { return m_nMiss; } }
        public int Critical { get { return m_nCritical; } }
        public int DefCritical { get { return m_nDefCritical; } }
        public int ModelID { get { return m_nModelID; } }
        public int MountID { get { return m_nMountID; } }
        public int StrPerception { get { return m_StrPerception; } }
        public int ConPerception { get { return m_ConPerception; } }
        public int DexPerception { get { return m_DexPerception; } }
        public int SprPerception { get { return m_SprPerception; } }
        public int IntPerception { get { return m_IntPerception; } }
        public int Str { get { return m_Str; } }
        public int Con { get { return m_Con; } }
        public int Dex { get { return m_Dex; } }
        public int Spr { get { return m_Spr; } }
        public int Int { get { return m_Int; } }
        public int StrBring { get { return m_StrBring; } }
        public int ConBring { get { return m_ConBring; } }
        public int DexBring { get { return m_DexBring; } }
        public int SprBring { get { return m_SprBring; } }
        public int IntBring { get { return m_IntBring; } }
        public int GenGu { get { return m_GenGu; } }
        public int RemainPoint { get { return m_nRemainPoint; } }
        public _OWN_SKILL[] PetSkill { get { return m_aSkill; } }
        public byte ExtraInfoLength { get { return m_ExtraInfoLength; } }
        public byte[] ExtraInfoData { get { return m_ExtraInfoData; } }

        //数据
        private PET_GUID_t		m_GUID;							// ID
		private int				m_nTradeIndex;					// 交易用到的索引值
		private uint m_uLowFlags;
        private uint m_uHighFlags;
		uint			m_ObjID;						// 所有Obj类型的ObjID
		int				m_nDataID;						// 模型ID,宠物类型
		byte			m_byNameSize;					// 名称长度,不包括最后的'\0'
		byte[]			m_szName = new byte[NET_DEFINE.MAX_CHARACTER_NAME];	// 名称
		int				m_nAIType;						// 性格
		PET_GUID_t		m_SpouseGUID;					// 配偶的GUID
		int				m_nLevel;						// 等级
		int				m_nExp;							// 经验
		int				m_nHP;							// 血当前值
		int				m_nHPMax;						// 血最大值
		int				m_nLife;						// 当前寿命
		byte			m_byGeneration;					// 几代宠
		byte			m_byHappiness;					// 快乐度
		int				m_nAtt_Physics;					// 物理攻击力
		int				m_nAtt_Magic;					// 魔法攻击力
		int				m_nDef_Physics;					// 物理防御力
		int				m_nDef_Magic;					// 魔法防御力
		int				m_nHit;							// 命中率
		int				m_nMiss;						// 闪避率
		int				m_nCritical;					// 会心率
        int             m_nDefCritical;                 //抗暴
		int				m_nModelID;						// 外形
		int				m_nMountID;						// 座骑ID
		int				m_StrPerception;				// 力量资质
		int				m_ConPerception;				// 体力资质
		int 			m_DexPerception;				// 身法资质
		int				m_SprPerception;				// 灵气资质
		int 			m_IntPerception;				// 定力资质
		int				m_Str;							// 力量
		int				m_Con;							// 体力
		int 			m_Dex;							// 身法
		int				m_Spr;							// 灵气
		int 			m_Int;							// 定力
		int 			m_GenGu;						// 根骨
        int             m_StrBring;							// 力量
        int             m_ConBring;							// 体力
        int             m_DexBring;							// 身法
        int             m_SprBring;							// 灵气
        int             m_IntBring;							// 定力
		int				m_nRemainPoint;					// 潜能点
		_OWN_SKILL[]	m_aSkill = new _OWN_SKILL[GAMEDEFINE.MAX_PET_SKILL_COUNT];	// 技能列表
		byte			m_ExtraInfoLength;							// 附加信息长度
		byte[]			m_ExtraInfoData = new byte[GAMEDEFINE.MAX_EXTRAINFO_LENGTH];		// 附加信息内容

    }


    public class GCDetailAttrib_PetFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCDetailAttrib_Pet(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_DETAILATTRIB_PET; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) + sizeof(byte) * 256;
        }
    };
}