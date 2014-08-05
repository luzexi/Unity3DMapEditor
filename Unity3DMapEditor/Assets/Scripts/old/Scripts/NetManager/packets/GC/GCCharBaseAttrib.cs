
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


    public class GCCharBaseAttrib : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if(buff.ReadInt(ref m_ObjID)!= sizeof(int))return false;
            int n = 0;
            if(buff.ReadInt(ref n)!= sizeof(int))return false;
            m_uFlags =(uint) n;


            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_DATA_ID)) != 0)
            {
                if (buff.ReadShort(ref m_wDataID) != sizeof(short)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_NAME)) != 0)
            {

                if(buff.ReadByte(ref m_byNameSize) != sizeof(byte)) return false;
                if (buff.Read(ref m_szName, sizeof(byte) * m_byNameSize) != m_byNameSize) return false;
                m_szName[m_byNameSize] = 0;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_TITLE)) != 0)
            {
                if (buff.ReadByte(ref m_TitleType) != sizeof(byte)) return false;
                if(buff.ReadByte(ref m_byTitleSize) != sizeof(byte)) return false;
                if(buff.Read(ref m_szTitle, m_byTitleSize) != m_byTitleSize) return false;
                m_szTitle[m_byTitleSize] = 0;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_LEVEL)) != 0)
            {
                if(buff.ReadByte(ref m_Level)!= sizeof(byte)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_HP_PERCENT)) != 0)
            {
                if(buff.ReadByte(ref m_HPPercent) != sizeof(byte)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MP_PERCENT)) != 0)
            {
                if (buff.ReadByte(ref m_MPPercent) != sizeof(byte)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_RAGE)) != 0)
            {
                if (buff.ReadInt(ref m_nRage) != sizeof(int)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_STEALTH_LEVEL)) != 0)
            {
                if (buff.ReadInt(ref m_nStealthLevel) != sizeof(int)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_SIT)) != 0)
            {
                if (buff.ReadByte(ref m_cMoodState) != sizeof(byte)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MOVE_SPEED)) != 0)
            {
                buff.ReadFloat(ref m_fMoveSpeed);
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_ATTACK_SPEED)) != 0)
            {
                buff.ReadFloat(ref m_fAttackSpeed);
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_CAMP_ID)) != 0)
            {
//                 int size = Marshal.SizeOf(m_CampData);
//                 byte[] bytes = new byte[size];
//                 if (buff.Read(ref bytes, size) != size) return false;
//                 m_CampData = (_CAMP_DATA)NET_DEFINE.BytesToStruct(bytes, typeof(_CAMP_DATA));
//                 LogManager.Log("CharBase: obj=" + m_ObjID + " Flag=" + m_uFlags + " size=" + size);

                //object camp = buff.ReadStruct(typeof(_CAMP_DATA));
                //if (camp == null) return false;
                //m_CampData = (_CAMP_DATA)camp;
                m_CampData.readFromBuff(ref buff);
               
            }



            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PORTRAIT_ID)) != 0)
            {
                
                if (buff.ReadInt(ref m_nPortraitID) != sizeof(int)) return false;
            }


            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MODEL_ID)) != 0)
            {

                if (buff.ReadInt(ref m_nModelID) != sizeof(int)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MOUNT_ID)) != 0)
            {

                if (buff.ReadInt(ref m_nMountID) != sizeof(int)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_AITYPE)) != 0)
            {

                if (buff.ReadInt(ref m_nAIType) != sizeof(int)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PLAYER_DATA)) != 0)
            {
                if (buff.ReadInt(ref m_uPlayerData) != sizeof(int)) return false;
                if (buff.ReadUint(ref m_HairColor) != sizeof(int)) return false;

            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_IS_IN_STALL)) != 0)
            {
                if (buff.ReadByte(ref m_bStallIsOpen) != sizeof(byte)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_STALL_NAME)) != 0)
            {
        
                if(buff.ReadByte(ref m_nStallNameSize) != sizeof(byte)) return false;
                //Assert(m_nStallNameSize < MAX_STALL_NAME);
                if(buff.Read(ref m_szStallName, m_nStallNameSize) != m_nStallNameSize) return false;
                m_szStallName[m_nStallNameSize] = 0;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_OCCUPANT)) !=0)
            {

                if (buff.ReadInt(ref m_OccupantGUID) != sizeof(int)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_OWNER)) !=0)
            {

                if (buff.ReadInt(ref m_OwnerID) != sizeof(int)) return false;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ISNPC)) !=0)
            {
                if (buff.ReadByte(ref m_IsNPC) != sizeof(byte)) return false;
            }
         
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {


            buff.WriteInt( m_ObjID);
            buff.WriteInt((int)m_uFlags) ;
            

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_DATA_ID)) !=0)
            {
                buff.WriteShort( m_wDataID);
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_NAME)) !=0)
            {

                buff.WriteByte(m_byNameSize) ;
                buff.Write(ref m_szName, sizeof(byte) * m_byNameSize) ;

            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_TITLE)) !=0)
            {
                buff.WriteByte( m_TitleType);
                buff.WriteByte( m_byTitleSize);
                buff.Write(ref m_szTitle, m_byTitleSize) ;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_LEVEL)) !=0)
            {
                buff.WriteByte( m_Level);
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_HP_PERCENT)) !=0)
            {
                buff.WriteByte( m_HPPercent);
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MP_PERCENT)) !=0)
            {
                buff.WriteByte( m_MPPercent);
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_RAGE)) !=0)
            {
                buff.WriteInt( m_nRage) ;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_STEALTH_LEVEL)) !=0)
            {
               buff.WriteInt( m_nStealthLevel);
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_SIT)) !=0)
            {
                buff.WriteByte( m_cMoodState);
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MOVE_SPEED)) !=0)
            {
                buff.WriteInt((int)m_fMoveSpeed);
              
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_ATTACK_SPEED)) !=0)
            {
                buff.WriteInt((int)m_fAttackSpeed) ;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_CAMP_ID)) !=0)
            {
//                int size = Marshal.SizeOf(m_CampData);
//                byte[] bytes = NET_DEFINE.StructToBytes(m_CampData);
//                buff.Write(ref bytes, size) ;
               //buff.WriteStruct(m_CampData);
                m_CampData.writeToBuff(ref buff);

            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PORTRAIT_ID)) !=0)
            {

                buff.WriteInt( m_nPortraitID) ;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MODEL_ID)) !=0)
            {

               buff.WriteInt( m_nModelID);
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MOUNT_ID)) !=0)
            {

                buff.WriteInt( m_nMountID) ;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_AITYPE)) !=0)
            {

                buff.WriteInt( m_nAIType) ;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PLAYER_DATA)) !=0)
            {
                buff.WriteInt( m_uPlayerData);
               buff.WriteUint( m_HairColor);

            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_IS_IN_STALL)) !=0)
            {
                buff.WriteByte( m_bStallIsOpen) ;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_STALL_NAME)) !=0)
            {

                buff.WriteByte( m_nStallNameSize);
                //Assert(m_nStallNameSize < MAX_STALL_NAME);
                buff.Write(ref m_szStallName, m_nStallNameSize) ;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_OCCUPANT)) !=0)
            {

               buff.WriteInt( m_OccupantGUID) ;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_OWNER)) !=0)
            {

               buff.WriteInt( m_OwnerID) ;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ISNPC)) !=0)
            {
                buff.WriteByte( m_IsNPC) ;
            }

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARBASEATTRIB;
        }
        public override int getSize()
        {
            int uAttribSize = sizeof(int) + sizeof(uint);

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_DATA_ID)) !=0)
                uAttribSize += sizeof(short);//m_dwDataID

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_NAME)) !=0)
            {
                uAttribSize += sizeof(byte);
                uAttribSize += sizeof(byte) * m_byNameSize;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_TITLE)) !=0)
            {
                uAttribSize += sizeof(byte);
                uAttribSize += sizeof(byte);//m_byTitleSize
                uAttribSize += sizeof(byte) * m_byTitleSize;
            }
            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_LEVEL)) !=0)
                uAttribSize += sizeof(byte);//m_Level
            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_HP_PERCENT)) !=0)
                uAttribSize += sizeof(byte);//m_HPPercent

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MP_PERCENT)) !=0)
                uAttribSize += sizeof(byte);//m_MPPercent

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_RAGE)) !=0)
                uAttribSize += sizeof(int);//m_nRage

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_STEALTH_LEVEL)) !=0)
                uAttribSize += sizeof(int);//m_nStealthLevel

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_SIT)) !=0)
                uAttribSize += sizeof(byte);//m_cMoodState


            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MOVE_SPEED)) !=0)
                uAttribSize += sizeof(float);//m_fMoveSpeed

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_ATTACK_SPEED)) !=0)
                uAttribSize += sizeof(float);//m_fAttackSpeed

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_CAMP_ID)) != 0)
                uAttribSize += m_CampData.getSize();//Marshal.SizeOf(typeof(_CAMP_DATA));

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PORTRAIT_ID)) !=0)
                uAttribSize += sizeof(int);//m_nPortraitID

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MODEL_ID)) !=0)
                uAttribSize += sizeof(int);//m_nModelID

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MOUNT_ID)) !=0)
                uAttribSize += sizeof(int);//m_nMountID

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_AITYPE)) !=0)
                uAttribSize += sizeof(int);//m_nAIType

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PLAYER_DATA)) !=0)
            {
                uAttribSize += sizeof(int);//m_uPlayerData
                uAttribSize += sizeof(int);//m_HairColor
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_IS_IN_STALL)) !=0)
                uAttribSize += sizeof(byte);//m_bStallIsOpen

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_STALL_NAME)) !=0)
            {
                uAttribSize += sizeof(byte) + sizeof(byte) * m_nStallNameSize;
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_OCCUPANT)) !=0)
            {
                uAttribSize += sizeof(int);
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_OWNER)) !=0)
            {
                uAttribSize += sizeof(int);
            }

            if ((m_uFlags & (1 << (int)ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ISNPC)) !=0)
            {
                uAttribSize += sizeof(byte);
            }

            return uAttribSize;
        }

        //public interface

        public bool IsUpdateAttrib(ENUM_UPDATE_CHAR_ATT eAttrib) { return (m_uFlags & (1 << (int)eAttrib)) != 0; }
		public void			SetUpdateAttrib( ENUM_UPDATE_CHAR_ATT eAttrib, bool bUpdate )
		{
			if ( bUpdate )
				m_uFlags |= (uint)(1<<(int)eAttrib);
			else
				m_uFlags &= (uint)(~(1<<(int)eAttrib));
		}

        public int ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }
        public uint Flag
        {
            get { return this.m_uFlags; }
            set { m_uFlags = value; }
        }
        public short DataID
        {
            get { return this.m_wDataID; }
            set { m_wDataID = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_DATA_ID, true); }
        }
        public byte Level
        {
            get { return this.m_Level; }
            set { m_Level = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_LEVEL, true); }
        }
        public byte HPPercent
        {
            get { return this.m_HPPercent; }
            set { m_HPPercent = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_HP_PERCENT, true); }
        }
        public byte MPPercent
        {
            get { return this.m_MPPercent; }
            set { m_MPPercent = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MP_PERCENT, true); }
        }
        public int Rage
        {
            get { return this.m_nRage; }
            set { m_nRage = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_RAGE, true);  }
        }
        public int StealthLevel
        {
            get { return this.m_nStealthLevel; }
            set { m_nStealthLevel = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_STEALTH_LEVEL, true); }
        }
        public byte MoodState
        {
            get { return this.m_cMoodState; }
            set { m_cMoodState = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_SIT, true); }
        }
        public float MoveSpeed
        {
            get { return this.m_fMoveSpeed; }
            set { m_fMoveSpeed = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MOVE_SPEED, true); }
        }
        public float AttackSpeed
        {
            get { return this.m_fAttackSpeed; }
            set { m_fAttackSpeed = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_ATTACK_SPEED, true); }
        }
        public _CAMP_DATA CampData
        {
            get { return this.m_CampData; }
            set { m_CampData = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_CAMP_ID, true); }
        }
        public int PortraitID
        {
            get { return this.m_nPortraitID; }
            set { m_nPortraitID = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PORTRAIT_ID, true); }
        }
        public int ModelID
        {
            get { return this.m_nModelID; }
            set { m_nModelID = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MODEL_ID, true); }
        }
        public int MountID
        {
            get { return this.m_nMountID; }
            set { m_nMountID = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_MOUNT_ID, true); }
        }
        public int AIType
        {
            get { return this.m_nAIType; }
            set { m_nAIType = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_AITYPE, true);  }
        }
        public byte NameSize
        {
            get { return this.m_byNameSize; }
            
        }
        public byte[] Name
        {
            get { return this.m_szName; }
            set { 
                m_szName = value;
                m_byNameSize = (byte)m_szName.Length;//(byte)Marshal.SizeOf(m_szName);
                SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_NAME, true); 
            }
        }
        public byte TitleSize
        {
            get { return this.m_byTitleSize; }
            //set { m_byTitleSize = value; }
        }
        public byte[] Title
        {
            get { return this.m_szTitle; }
            set {
                m_szTitle = value;
                m_byTitleSize = (byte)m_szTitle.Length;//(byte)Marshal.SizeOf(m_szTitle);
                SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_TITLE, true); 
            }
        }
        public byte TitleType
        {
            get { return this.m_TitleType; }
            set { m_TitleType = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_TITLE, true); }
        }
        public int PlayerData
        {
            get { return this.m_uPlayerData; }
            //set { m_uPlayerData = value; }
        }
        public uint HairColor
        {
            get { return this.m_HairColor; }
            set { m_HairColor = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PLAYER_DATA, true); }
        }
        public int FaceMesh{
            get{
                int uFaceMeshID = (m_uPlayerData & 0x000000FF);
			    if(uFaceMeshID == 0xFF)
			    {
				    uFaceMeshID = -1;
			    }
			    return uFaceMeshID;
            }
            set{
                m_uPlayerData |= value & 0xFF;
                SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PLAYER_DATA, true);
            }
        }
        public int HairMesh{
            get{
                int uHairMeshID = ((m_uPlayerData>>8) & 0x000000FF);
			    if(uHairMeshID == 0xFF)
			    {
				    uHairMeshID = -1;
			    }
			    return uHairMeshID;
            }
            set{
                m_uPlayerData |= ((value & 0XFF)<<8);
			    SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ATT_PLAYER_DATA,true);
            }
        }
 
 
        public byte StallIsOpen
        {
            get { return this.m_bStallIsOpen; }
            set { m_bStallIsOpen = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_IS_IN_STALL, true); }
        }
        public byte StallNameSize
        {
            get { return this.m_nStallNameSize; }
            //set { m_nStallNameSize = value; }
        }
        public byte[] StallName
        {
            get { return this.m_szStallName; }
            set { 
                m_szStallName = value;
                m_nStallNameSize = (byte)m_szStallName.Length;//Marshal.SizeOf(m_szStallName);
                SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_STALL_NAME, true);
            }
        }
        public int OccupantGUID
        {
            get { return this.m_OccupantGUID; }
            set { m_OccupantGUID = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_OCCUPANT, true); }
        }
        public int OwnerID
        {
            get { return this.m_OwnerID; }
            set { m_OwnerID = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_OWNER, true); }
        }
        public byte IsNPC
        {
            get { return this.m_IsNPC; }
            set { m_IsNPC = value; SetUpdateAttrib(ENUM_UPDATE_CHAR_ATT.UPDATE_CHAR_ISNPC, true); }
        }
        //数据

       private int			    m_ObjID;		// ObjID

		// 每个位表示一个属性是否要刷新 ENUM_UPDATE_CHAR_ATT
		private uint			m_uFlags;		

		//数据部分	
		private short			m_wDataID;			// 怪物的数据资源ID, 如果是玩家，则是性别
		private byte			m_Level;			// 等级
		private byte			m_HPPercent;		// 生命值百分比
		private byte			m_MPPercent;		// 魔法值百分比
		private int				m_nRage;			// 怒气
		private int				m_nStealthLevel;	// 隐身级别
		private byte			m_cMoodState;		// 表情状态
		private float			m_fMoveSpeed;		// 移动的速度
		private float			m_fAttackSpeed;		// 攻击速度
		private _CAMP_DATA		m_CampData;			// 阵营
		//INT				m_nObjectCampType;	// 阵营类型
		private int				m_nPortraitID;		// 头像ID
		private int				m_nModelID;			// 变形
		private int				m_nMountID;			// 座骑
		private int				m_nAIType;			// AI类型
		// Player专有
		private byte			m_byNameSize;					// 玩家姓名长度,不包括最后的'\0'
		private byte[]			m_szName = new byte[NET_DEFINE.MAX_CHARACTER_NAME];	// 玩家姓名
		private byte			m_byTitleSize;					// 玩家头衔长度,不包括最后的'\0'
		private byte[]			m_szTitle = new byte[NET_DEFINE.MAX_CHARACTER_TITLE];	// 玩家头衔
		private byte			m_TitleType;					// 称号类型

		private int			    m_uPlayerData;					//   FFFF|FF|FF
														//        |   |   
														//      头发 脸型
														//      模型 模型

		private uint			    m_HairColor;					// 头发颜色

		//摊位状态
		private byte			m_bStallIsOpen;					// 摊位是否已经打开
		private byte			m_nStallNameSize;				// 摊位名长度
		private byte[]			m_szStallName = new byte[NET_DEFINE.MAX_STALL_NAME];	// 摊位名

		private int			    m_OccupantGUID ;
		private int			    m_OwnerID ;
		private byte			m_IsNPC;

    };
    public class GCCharBaseAttribFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharBaseAttrib(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARBASEATTRIB; }
        public override int GetPacketMaxSize()
        {
            int uAttribSize = sizeof(int) + sizeof(uint);

                uAttribSize += sizeof(short);//m_dwDataID

                uAttribSize += sizeof(byte);
                uAttribSize += sizeof(byte) * NET_DEFINE.MAX_CHARACTER_NAME;

                uAttribSize += sizeof(byte);
                uAttribSize += sizeof(byte);//m_byTitleSize
                uAttribSize += sizeof(byte) * NET_DEFINE.MAX_CHARACTER_TITLE;
                uAttribSize += sizeof(byte);//m_Level
                uAttribSize += sizeof(byte);//m_HPPercent

                uAttribSize += sizeof(byte);//m_MPPercent

                uAttribSize += sizeof(int);//m_nRage

                uAttribSize += sizeof(int);//m_nStealthLevel

                uAttribSize += sizeof(byte);//m_cMoodState


                uAttribSize += sizeof(float);//m_fMoveSpeed

                uAttribSize += sizeof(float);//m_fAttackSpeed

                uAttribSize += _CAMP_DATA.getMaxSize();//Marshal.SizeOf(typeof(_CAMP_DATA));

                uAttribSize += sizeof(int);//m_nPortraitID

                uAttribSize += sizeof(int);//m_nModelID

                uAttribSize += sizeof(int);//m_nMountID

                uAttribSize += sizeof(int);//m_nAIType

                uAttribSize += sizeof(int);//m_uPlayerData
                uAttribSize += sizeof(int);//m_HairColor

                uAttribSize += sizeof(byte);//m_bStallIsOpen

                uAttribSize += sizeof(byte) + sizeof(byte) * NET_DEFINE.MAX_STALL_NAME;

                uAttribSize += sizeof(int);
                uAttribSize += sizeof(int);

                uAttribSize += sizeof(byte);

            return uAttribSize;
          
        }
    };
}