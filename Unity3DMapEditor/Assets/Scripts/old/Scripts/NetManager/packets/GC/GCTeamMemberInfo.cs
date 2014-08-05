using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCTeamMemberInfo : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_TEAMMEMBERINFO;
        }

        public override int getSize()
        {
            int uSize = sizeof(uint) + sizeof(int);

			for(int i=0; i<(int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_NUMBERS; ++i)
			{
				if( IsUpdateAttrib(i) )
				{
					switch( (ENUM_TEAM_MEMBER_ATT)i )
					{
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_FAMILY:
                            uSize += sizeof(uint);
                            break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_LEVEL:
                            uSize += sizeof(uint);
                            break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_MAX_HP:
                            uSize += sizeof(uint);
                            break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_MAX_MP:
                            uSize += sizeof(uint);
                            break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_WEAPON:
                            uSize += sizeof(uint);
                            break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_CAP:
                            uSize += sizeof(uint);
                            break;
					    case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_ARMOR:
                            uSize += sizeof(uint);
                            break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_CUFF:
                            uSize += sizeof(uint);
                            break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_BOOT:
                            uSize += sizeof(uint);
                            break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_FACEMESH:		// 面部模型
                            uSize += sizeof(uint);
                            break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_HAIRMESH:		// 头发模型
                            uSize += sizeof(uint);
                            break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_HAIRCOLOR:		// 头发颜色
                            uSize += sizeof(uint);
                            break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_BACK:			// 增加背饰 [12/14/2010 ivan edit]
						    uSize += sizeof(uint);
						    break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_POSITION:
						uSize += m_WorldPos.getSize();
						break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_HP:
                        uSize += sizeof(int);
                        break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_MP:
                        uSize += sizeof(int);
                        break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_ANGER:
						uSize += sizeof(int);
						break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_DEADLINK:
                        uSize += sizeof(byte);
                        break;
                        case ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_DEAD:
						uSize += sizeof(byte);
						break;
					default:
						// size 不变，例如两个标记量
						break;
					}
				}
			}

			return uSize;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadUint(ref m_GUID);
            buff.ReadInt(ref m_Flags);
            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_FAMILY))
            {
                buff.ReadUint(ref m_uFamily);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_LEVEL))
            {
                buff.ReadUint(ref m_uLevel);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_POSITION))
            {
                m_WorldPos.readFromBuff(ref buff);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_HP))
            {
                
                buff.ReadInt(ref m_nHP);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_MAX_HP))
            {
                
                buff.ReadUint(ref m_uMaxHP);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_MP))
            {
                
                buff.ReadInt(ref m_nMP);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_MAX_MP))
            {
                buff.ReadUint(ref m_uMaxMP);
                
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_ANGER))
            {
                
                buff.ReadInt(ref m_nAnger);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_WEAPON))
            {
                
                buff.ReadUint(ref m_WeaponID);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_CAP))
            {
                
                buff.ReadUint(ref m_CapID);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_ARMOR))
            {
                
                buff.ReadUint(ref m_ArmourID);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_CUFF))
            {
                
                buff.ReadUint(ref m_CuffID);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_BOOT))
            {
                
                buff.ReadUint(ref m_FootID);
            }


            /*
                if( IsUpdateAttrib(TEAM_MEMBER_ATT_BUFF) )
                {
                }
            */

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_DEADLINK))
            {
                
                buff.ReadByte(ref m_DeadLinkFlag);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_DEAD))
            {
                
                buff.ReadByte(ref m_DeadFlag);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_FACEMESH))
            {
                
                buff.ReadUint(ref m_uFaceMeshID);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_HAIRMESH))
            {
                
                buff.ReadUint(ref m_uHairMeshID);
            }

            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_HAIRCOLOR))
            {
                
                buff.ReadUint(ref m_uHairColor);
            }

            // 增加背饰 [12/14/2010 ivan edit]
            if (IsUpdateAttrib((int)ENUM_TEAM_MEMBER_ATT.TEAM_MEMBER_ATT_BACK))
            {
                buff.ReadUint(ref m_BackID);
            }

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }
        public bool IsUpdateAttrib( int eAttrib )
		{
			if( (m_Flags & (1 << eAttrib)) != 0 )
            {
                return true; 
            }

			return false;
		}
   
        public uint GUID
        {
            get { return m_GUID; }
        }
        public int Flag
        {
            get { return m_Flags; }
        }
        public uint Family
        {
            get { return m_uFamily; }
        }
        public uint Level
        {
            get { return m_uLevel; }
        }

        public WORLD_POS WorldPos
        {
            get { return m_WorldPos; }
        }
        public int HP
        {
            get { return m_nHP; }
        }
        public uint MaxHP
        {
            get { return m_uMaxHP; }
        }
        public int MP
        {
            get { return m_nMP; }
        }
        public uint MaxMP
        {
            get { return m_uMaxMP; }
        }
        public int Anger
        {
            get { return m_nAnger; }
        }
        public uint WeaponID
        {
            get { return m_WeaponID; }
        }
        public uint CapID
        {
            get { return m_CapID; }
        }
        public uint ArmourID
        {
            get { return m_ArmourID; }
        }
        public uint CuffID
        {
            get { return m_CuffID; }
        }
        public uint FootID
        {
            get { return m_FootID; }
        }
        public byte DeadLinkFlag
        {
            get { return m_DeadLinkFlag; }
        }
        public byte DeadFlag
        {
            get { return m_DeadFlag; }
        }
        public uint FaceMeshID
        {
            get { return m_uFaceMeshID; }
        }
        public uint HairMeshID
        {
            get { return m_uHairMeshID; }
        }
        public uint HairColor
        {
            get { return m_uHairColor; }
        }
        public uint BackID
        {
            get { return m_BackID; }
        }

        //数据
        uint m_GUID;

        int  m_Flags;	// 每个位表示以下值是否要刷新 ENUM_DETAIL_ATTRIB
        uint m_uFamily;						// 1.门派
        uint m_uLevel;						// 2.等级
        WORLD_POS m_WorldPos;						// 3.位置（坐标）
        int  m_nHP;							// 4.HP
        uint m_uMaxHP;						// 5.HP上限
        int  m_nMP;							// 6.MP
        uint m_uMaxMP;						// 7.MP上限
        int  m_nAnger;						// 8.怒气
        uint m_WeaponID;						// 9.武器
        uint m_CapID;						// 10.帽子
        uint m_ArmourID;						// 11.衣服
        uint m_CuffID;						// 12.护腕
        uint m_FootID;						// 13.靴子
        // 14.buff，暂时不考虑
        byte m_DeadLinkFlag;					// 15.断线
        byte m_DeadFlag;						// 16.死亡
        uint m_uFaceMeshID;					// 17.面部模型
        uint m_uHairMeshID;					// 18.头发模型
        uint m_uHairColor;					// 19.头发颜色
        uint m_BackID;						// 20.背饰 [8/30/2010 Sun]
    }

    public class GCTeamMemberInfoFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCTeamMemberInfo(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_TEAMMEMBERINFO; }
        public override int GetPacketMaxSize()
        {
            return  sizeof(byte) * 2 + sizeof(uint) * 18 + WORLD_POS.GetMaxSize();
        }
    }
}