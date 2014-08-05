using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

using UnityEngine;

namespace Network.Packets
{
    public class GCDetailAttrib : PacketBase
    {

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_DETAILATTRIB;
        }

        bool IsSetBit(int bit)
        {
            if (bit < 32)
            {
                return (m_uLowFlags & (1 << bit)) != 0;
            }
            else
            {
                bit -= 32;
                return (m_uHighFlags & (1 << bit)) != 0;
            }
        }

        public bool IsSetBit(ENUM_DETAIL_ATTRIB i)
        {
            return IsSetBit((int)i);
        }

        public override int getSize()
        {
            int uAttribSize = sizeof(int) + sizeof(uint)*6;
            int maxSize = (int)ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_NUMBERS;
            for (int i = 0; i < maxSize; i++)
            {
                if (IsSetBit(i))
                {
                    switch (i)
                    {
                        case (int)ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CAMP:
                            uAttribSize += m_CampData.getSize();
                            break;
                        //                         case DETAIL_ATTRIB_CURRENT_PET_GUID:
                        //                             uSize += sizeof(PET_GUID_t);
                        //                             break;
                        case (int)ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_AMBIT: //角色境界大小为BYTE [2011-8-10] by: cfp+
                            uAttribSize += sizeof(Byte);
                            break;
                        default:
                            uAttribSize += sizeof(uint);
                            break;
                    }
                }
            }
            return uAttribSize;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_ObjID) != sizeof(int)) return false;

            buff.ReadUint(ref m_uLowFlags);
            buff.ReadUint(ref m_uHighFlags);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_LEVEL))
                buff.ReadUint(ref m_uLevel);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_AMBIT)) //角色境界 [2011-8-10] by: cfp+
                buff.ReadByte(ref m_ucAmbit);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_HP))
                buff.ReadInt(ref m_nHP);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MP))
                buff.ReadInt(ref m_nMP);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MAXHP))
                buff.ReadInt(ref m_nMAXHP);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MAXMP))
                buff.ReadInt(ref m_nMAXMP);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MOVESPEED))
                buff.ReadFloat(ref m_fMoveSpeed);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_EXP))
                buff.ReadInt(ref m_nExp);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MONEY))
                buff.ReadInt(ref m_uMoney);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_STR))
                buff.ReadInt(ref m_Str);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_SPR))
                buff.ReadInt(ref m_Spr);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CON))
                buff.ReadInt(ref m_Con);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_INT))
                buff.ReadInt(ref m_Int);
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEX))
                buff.ReadInt(ref m_Dex);
            if(IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_STR_RANDOM_POINT))					//力量 力量
                buff.ReadInt(ref m_StrRandomPoint);
            if(IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_SPR_RANDOM_POINT))					//灵气 灵力
                buff.ReadInt(ref m_SprRandomPoint);
            if(IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CON_RANDOM_POINT))					//体制 体制
                buff.ReadInt(ref m_ConRandomPoint);
            if(IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_INT_RANDOM_POINT))					//定力 智力
                buff.ReadInt(ref m_IntRandomPoint);
            if(IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEX_RANDOM_POINT))					//身法 敏捷
                buff.ReadInt(ref m_DexRandomPoint);
			
			if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_POINT_REMAIN))
                buff.ReadInt(ref m_nPoint_Remain);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_HP_RESPEED))
                buff.ReadInt(ref m_nHP_ReSpeed);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MP_RESPEED))
                buff.ReadInt(ref m_nMP_ReSpeed);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ATT_PHYSICS))
                buff.ReadInt(ref m_nAtt_Physics);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ATT_MAGIC))
                buff.ReadInt(ref m_nAtt_Magic);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEF_PHYSICS))
                buff.ReadInt(ref m_nDef_Physics);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEF_MAGIC))
                buff.ReadInt(ref m_nDef_Magic);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_HIT))
                buff.ReadInt(ref m_nHit);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MISS))
                buff.ReadInt(ref m_nMiss);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CRITRATE))
                buff.ReadInt(ref m_nCritic);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEFENCE_C))	//抗暴率 2011-11-16 ZL+
                buff.ReadInt(ref m_nDefenceCritic);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_RAGE))
                buff.ReadInt(ref m_nRage);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_STRIKE_POINT))
                buff.ReadInt(ref m_nStrikePoint);


            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ATTACKSPEED))
                buff.ReadInt(ref m_nAttackSpeed);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ATTACKCOLD))
                buff.ReadInt(ref m_nAttCold);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEFENCECOLD))
                buff.ReadInt(ref m_nDefCold);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ATTACKFIRE))
                buff.ReadInt(ref m_nAttFire);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEFENCEFIRE))
                buff.ReadInt(ref m_nDefFire);


            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ATTACKLIGHT))
                buff.ReadInt(ref m_nAttLight);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEFENCELIGHT))
                buff.ReadInt(ref m_nDefLight);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ATTACKPOISON))
                buff.ReadInt(ref m_nAttPoison);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEFENCEPOISON))
                buff.ReadInt(ref m_nDefPoison);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ATTACKEARTH))
                buff.ReadInt(ref m_nAttEarth);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DEFENCEEARTH))
                buff.ReadInt(ref m_nDefEarth);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MENPAI))
                buff.ReadInt(ref m_nMenPai);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_GUILD))
                buff.ReadInt(ref m_nGuild);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CAMP))
                m_CampData.readFromBuff(ref buff);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DATAID))
                buff.ReadInt(ref m_nDataID);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_PORTRAITID))
                buff.ReadInt(ref m_nPortraitID);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MODELID))
                buff.ReadInt(ref m_nModelID);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MOUNTID))
                buff.ReadInt(ref m_nMountID);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CURRENT_PET_GUID))
            {
                for (int i = 0; i < GAMEDEFINE.MAX_CURRENT_PET; i++)
                {
                    m_guidCurrentPet[i].readFromBuff(ref buff);
                }
            }

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_LIMIT_MOVE))
                buff.ReadByte(ref m_bLimitMove);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CAN_ACTION1))
                buff.ReadByte(ref m_bCanActionFlag1);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CAN_ACTION2))
                buff.ReadByte(ref m_bCanActionFlag2);

            //{----------------------------------------------------------------------------
            // [2010-12-1] by: cfp+ 活力and精力
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_VIGOR))
                buff.ReadInt(ref m_Vigor);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MAX_VIGOR))
                buff.ReadInt(ref m_MaxVigor);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_ENERGY))
                buff.ReadInt(ref m_Energy);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_MAX_ENERGY))
                buff.ReadInt(ref m_MaxEnergy);
            //----------------------------------------------------------------------------}

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_RMB))
                buff.ReadInt(ref m_RMBMoney);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_BANK_RMB))
                buff.ReadInt(ref m_BankMoney);

            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_DOUBLEEXPTIME))
                buff.ReadInt(ref m_DoubleExpTime_Num);

            //新手引导掩码 [2011-8-10] by: cfp+
            if (IsSetBit(ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_HELPMASK))
                buff.ReadUint(ref m_uHelpMask);

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //客户端不需要实现写入功能
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        ///////////////////////////////////////////////////////////////////////////////
        //属性
        uint m_ObjID;	// 所有Obj类型的ObjID
        public uint ObjID
        {
            get { return m_ObjID; }
        }
        // 每个位表示一个属性是否要刷新 ENUM_DETAIL_ATTRIB
        uint m_uLowFlags;
        uint m_uHighFlags;

        uint m_uLevel;		// 等级
        public uint LV
        {
            get{return m_uLevel;}
        }

        Byte m_ucAmbit;		// 角色境界 [2011-8-10] by: cfp+
        
        int m_nExp;			// 经验值
        public int Exp
        {
            get { return m_nExp; }
        }

        int m_nHP;			//生命点
        public int HP 
        { 
            get { return m_nHP; } 
        }

        int m_nMP;			//魔法点
        public int MP
        {
            get { return m_nMP; }
        }

        int m_nMAXHP;		//最大生命点
        public int MaxHP
        {
            get { return m_nMAXHP; }
        }
        
        int m_nMAXMP;		//最大魔法点
        public int MaxMP
        {
            get { return m_nMAXMP; }
        }
        
        float m_fMoveSpeed;	//移动速度
        public float MoveSpeed
        {
            get { return m_fMoveSpeed; }
        }

        int m_uMoney;		// 游戏币数
        public int Money
        {
            get { return m_uMoney; }
        }
        //一级战斗属性
       
        int m_Str;					//力量 力量
        public int Str
        {
            get { return m_Str; }
        }

        
        int m_Spr;					//灵气 灵力
        public int SPR
        {
            get { return m_Spr; }
        }

        int m_Con;					//体制 体制
        public int CON
        {
            get { return m_Con; }
        }

        int m_Int;					//定力 智力
        public int INT
        {
            get { return m_Int;}
        }

        int m_Dex;					//身法 敏捷
        public int DEX
        {
            get { return m_Dex;}
        }

        //// 基础一级战斗属性的培养点数[02/23/2012 zzh+]
        int m_StrRandomPoint;					//力量 力量
        public int StrRandomPoint
        {
            get { return m_StrRandomPoint; }
        }
        int m_SprRandomPoint;					//灵气 灵力
        public int SprRandomPoint
        {
            get { return m_SprRandomPoint; }
        }
        int m_ConRandomPoint;					//体制 体制
        public int ConRandomPoint
        {
            get { return m_ConRandomPoint; }
        }
        int m_IntRandomPoint;					//定力 智力
        public int IntRandomPoint
        {
            get { return m_IntRandomPoint; }
        }
        int m_DexRandomPoint;					//身法 敏捷
        public int DexRandomPoint
        {
            get { return m_DexRandomPoint; }
        }

        int m_nPoint_Remain;		//剩余待分配点数
        public int Remain
        {
            get { return m_nPoint_Remain; }
        }

        //二级战斗属性

        int m_nHP_ReSpeed;		//HP恢复速度  点/秒
        public int HPReSpeed
        {
            get { return m_nHP_ReSpeed; }
        }

        int m_nMP_ReSpeed;		//MP恢复速度  点/秒
        public int MPReSpeed
        {
            get { return m_nMP_ReSpeed; }
        }

        int m_nAtt_Physics;		//物理攻击力
        public int PhysicsAttk
        {
            get { return m_nAtt_Physics; }
        }

        int m_nAtt_Magic;		//魔法攻击力
        public int MagicAttk
        {
            get { return m_nAtt_Magic; }
        }

        int m_nDef_Physics;		//物理防御力
        public int PhysicsDef
        {
            get { return m_nDef_Physics; }
        }

        int m_nDef_Magic;		//魔法防御力
        public int MagicDef
        {
            get { return m_nDef_Magic; }
        }

        int m_nHit;				//命中率
        public int Hit
        {
            get { return m_nHit; }
        }

        int m_nMiss;			//闪避率
        public int Miss
        {
            get { return m_nMiss; }
        }
        int m_nCritic;			//致命一击率
        public int CriticalRate
        {
            get { return m_nCritic; }
        }
        int m_nDefenceCritic;	//抗暴率 2011-11-16 ZL+
        public int CriticalDef
        {
            get { return m_nDefenceCritic; }
        }

        int m_nRage;			//怒气
        public int Rage
        {
            get { return m_nRage; }
        }

        int m_nStrikePoint;		//连技点
        public int StrikePoint
        {
            get { return m_nStrikePoint; }
        }

        int m_nAttackSpeed;		//攻击速度
        public int AttkSpeed    
        {
            get { return m_nAttackSpeed; }
        }

        int m_nAttCold;			//冰攻击 //水
        public int ColdAttk
        {
            get { return m_nAttCold; }
        }
        int m_nDefCold;			//冰防御
        public int ColdDef
        {
            get { return m_nDefCold; }
        }
        int m_nAttFire;			//火攻击 //火
        public int FireAttk
        {
            get { return m_nAttFire; }
        }
        int m_nDefFire;			//火防御
        public int FireDef
        {
            get { return m_nDefFire; }
        }
        int m_nAttLight;		//电攻击 //金
        public int LightAttk
        {
            get { return m_nAttLight; }
        }
        int m_nDefLight;		//电防御
        public int LightDef
        {
            get { return m_nDefLight; }
        }
        int m_nAttPoison;		//毒攻击 //木
        public int PoisonAttk
        {
            get { return m_nAttPoison; }
        }
        int m_nDefPoison;		//毒防御
        public int PoisonDef
        {
            get { return m_nDefPoison; }
        }
        int m_nAttEarth;		//土攻击
        public int EarthAttk
        {
            get { return m_nAttEarth; }
        }
        int m_nDefEarth;		//土防御
        public int EarthDef
        {
            get { return m_nDefEarth; }
        }

        int m_nMenPai;			//门派		-> 五行灵根值
        public int MenPai
        {
            get { return m_nMenPai; }
        }
        int m_nGuild;			//帮派


        _CAMP_DATA m_CampData;			// 阵营	
        int m_nDataID;			// DataID
        public int DataID
        {
            get { return m_nDataID; }
        }
        public bool LimitMove
        {
            get { return (m_bLimitMove !=0); }
        }
        public int MountID
        {
            get { return m_nMountID; }
        }
        int m_nPortraitID;		// 头像ID
        int m_nModelID;			// 外形
        public int ModelID
        {
            get { return m_nModelID; }
        }
        int m_nMountID;			//座骑
        //暂时屏蔽
        PET_GUID_t[] m_guidCurrentPet = new PET_GUID_t[GAMEDEFINE.MAX_CURRENT_PET];	//当前宠物
        public PET_GUID_t[] CurrentPet
        {
            get { return m_guidCurrentPet; }
        }

        byte m_bLimitMove;		//是否限制不能移动
        byte m_bCanActionFlag1;	//技能受限标记1,用于昏迷催眠
        byte m_bCanActionFlag2;	//技能受限标记2,用于沉默

        int m_Vigor;			// 活力 
        int m_MaxVigor;			// 活力上限

        int m_Energy;			// 精力
        int m_MaxEnergy;		// 精力上限

        int m_RMBMoney;			//元宝
        int m_BankMoney;		//银行中的元宝
        
        int m_DoubleExpTime_Num;	// 双倍经验时间和倍数;
        public int DoubleExpTimeNum
        {
            get { return m_DoubleExpTime_Num; }
        }
        
        int m_GmRight;			// GM权限
        uint m_uHelpMask;         //新手引导掩码 [2011-8-10] by: cfp+
        public uint HelpMask
        {
            get { return m_uHelpMask; }
        }
		
		public int MaxEnergy
		{
			get { return m_MaxEnergy; }
		}

        public int RMBMoney
        {
            get { return m_RMBMoney; }
        }
    }

    public class GCDetailAttribFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCDetailAttrib(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_DETAILATTRIB; }
        public override int GetPacketMaxSize()
        {
            int uAttribSize = sizeof(int) + sizeof(uint)*6;
            int maxSize = (int)ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_NUMBERS;
            for (int i = 0; i < maxSize; i++)
            {
                switch (i)
                {
                    case (int)ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_CAMP:
                        uAttribSize += _CAMP_DATA.getMaxSize();
                        break;
                    //                         case DETAIL_ATTRIB_CURRENT_PET_GUID:
                    //                             uSize += sizeof(PET_GUID_t);
                    //                             break;
                    case (int)ENUM_DETAIL_ATTRIB.DETAIL_ATTRIB_AMBIT: //角色境界大小为BYTE [2011-8-10] by: cfp+
                        uAttribSize += sizeof(Byte);
                        break;
                    default:
                        uAttribSize += sizeof(uint);
                        break;
                }
            }
            return uAttribSize;

        }
    };
}
