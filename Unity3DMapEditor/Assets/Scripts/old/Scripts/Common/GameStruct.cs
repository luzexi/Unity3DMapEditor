using System;
using System.Runtime.InteropServices;

using Network;
using Network.Packets;
//[Serializable] //可序列化
//[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
//public struct DB_CHAR_EQUIP_LIST
//{
//    //public DB_CHAR_EQUIP_LIST()
//    //{
//    //    CleanUp();
//    //}

//    //void	CleanUp()
//    //{
		
//    //}
//    //uint[]	m_Equip = new uint[HUMAN_EQUIP.HEQUIP_NUMBER];			//装备
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)HUMAN_EQUIP.HEQUIP_NUMBER)]
//    uint[] m_Equip;
//};


[Serializable] //可序列化
[StructLayout(LayoutKind.Sequential, Pack=1)] //按1字节对齐
public struct DB_CHAR_BASE_INFO: ClassCanbeSerialized
{

    //public DB_CHAR_BASE_INFO()
    //{
    //    CleanUp();
    //}

    //void	CleanUp()
    //{
    //    m_EquipList = new DB_CHAR_EQUIP_LIST();
    //}

	public int				m_GUID;							//角色全局编号
    public byte m_Sex;							//性别替换为种族，奇数代表男性，偶数代表女性 [12/14/2010 ivan edit]
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_DEFINE.MAX_CHARACTER_NAME)]
    public byte[] m_Name;		//角色名字
    public int m_Level;						//角色等级
    public byte m_Ambit;                        //角色境界 [2011-8-10] by: cfp+
    public uint m_HairColor;					//头发颜色	
    public byte m_FaceColor;					//脸形颜色
    public byte m_HairModel;					//头发模型
    public byte m_FaceModel;					//脸形模型
    public short m_StartScene;					//角色所在场景
    public short m_nClientResID;		            //客户端资源索引 [2011-10-26] by: cfp+
    public int m_Menpai;						//角色门派
    public int m_HeadID;						//头部编号
    public short m_Camp;							//阵营编号
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)HUMAN_EQUIP.HEQUIP_NUMBER)]
    public uint[] m_EquipList;					            //装备列表


    public static int getMaxSize(){
         return sizeof(int)*(5 + (int)HUMAN_EQUIP.HEQUIP_NUMBER)+
            sizeof(byte)*(5+NET_DEFINE.MAX_CHARACTER_NAME) +
            sizeof(short)*3; 
    }
    //////////////////////////////////////////////////////////////////////////
    //from interface
    // 取得包大小
    public int getSize(){

        return sizeof(int)*(5 + (int)HUMAN_EQUIP.HEQUIP_NUMBER)+
            sizeof(byte)*(5+NET_DEFINE.MAX_CHARACTER_NAME) +
            sizeof(short)*3;

    }

    // 将包内容转换为byte流 return byte流长度
    public int writeToBuff(ref NetOutputBuffer buff){
        //不用实现写
        return getSize();
    }
    
    // 将byte流转换为包内容
    public bool readFromBuff(ref NetInputBuffer buff){

        if(buff.ReadInt(ref m_GUID) != sizeof(int)) return false;
        if(buff.ReadByte(ref m_Sex) != sizeof(byte)) return false;
        if (m_Name == null)
        {
            m_Name = new byte[NET_DEFINE.MAX_CHARACTER_NAME];
           
        }
        if(buff.Read(ref m_Name, NET_DEFINE.MAX_CHARACTER_NAME)!=NET_DEFINE.MAX_CHARACTER_NAME) return false;
        //temp code 去除服务器发送的无用字节
		int zeroIndex = -1;
		for(int i=0; i < NET_DEFINE.MAX_CHARACTER_NAME; ++i)
		{
			if(m_Name[i] == 0)
			{
				zeroIndex = i;
				break;
			}
		}
		for(int i= zeroIndex; i < NET_DEFINE.MAX_CHARACTER_NAME; ++i)
		{
			m_Name[i] = 0;
		}//end temp code
		
        if(buff.ReadInt(ref m_Level)!=sizeof(int))return false;
        if(buff.ReadByte(ref m_Ambit) != sizeof(byte)) return false;
        int n = 0;
        if(buff.ReadInt(ref n) != sizeof(int)) return false;
        m_HairColor = (uint)n;
        if(buff.ReadByte(ref m_FaceColor) != sizeof(byte)) return false;
        if (buff.ReadByte(ref m_HairModel) != sizeof(byte)) return false;
        if (buff.ReadByte(ref m_FaceModel) != sizeof(byte)) return false;
        if(buff.ReadShort(ref m_StartScene) != sizeof(short)) return false;
        if(buff.ReadShort(ref m_nClientResID) != sizeof(short)) return false;
        if(buff.ReadInt(ref m_Menpai) != sizeof(int))return false;
        if(buff.ReadInt(ref m_HeadID) != sizeof(int))return false;
        if(buff.ReadShort(ref m_Camp) != sizeof(short)) return false;

        if(m_EquipList == null)
            m_EquipList = new uint[(int)HUMAN_EQUIP.HEQUIP_NUMBER];
        for(int i =0; i < (int)HUMAN_EQUIP.HEQUIP_NUMBER; i++)
        {
            if(buff.ReadInt(ref n) != sizeof(int)) return false;
            m_EquipList[i] = (uint)n;
        }
        
        return true;
    }
};

//[Serializable] //可序列化
//[StructLayout(LayoutKind.Sequential, Pack=1)] //按1字节对齐
public struct WORLD_POS : ClassCanbeSerialized
{
    public float m_fX;
    public float m_fZ;

    public int getSize(){
        return sizeof(float) * 2;
    }
    public static int GetMaxSize() {
        return sizeof(float) * 2;
    }
    public int writeToBuff(ref NetOutputBuffer buff) {

        buff.WriteFloat(m_fX);
        buff.WriteFloat(m_fZ);

        return getSize();
    }
    public bool readFromBuff(ref NetInputBuffer buff){

        if (buff.ReadFloat(ref m_fX) != sizeof(float)) return false;
        if (buff.ReadFloat(ref m_fZ) != sizeof(float)) return false;

        return true;
    }
}

//一级战斗属性结构
[Serializable] //可序列化
[StructLayout(LayoutKind.Sequential, Pack=1)] //按1字节对齐
public struct _ATTR_LEVEL1 : ClassCanbeSerialized
{
    public int[] m_pAttr;

    
    public int this[int index]
    {
        get{
            if(index <0 || index >=(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_NUMBER)
                throw new ArgumentOutOfRangeException("Attr_level1 get");
            return m_pAttr[index];
        }
        set{
            if(index <0 || index >=(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_NUMBER)
                throw new ArgumentOutOfRangeException("Attr_level1 set");
            m_pAttr[index] = value;
        }
    }
    public static int getMaxSize()
    {
        return sizeof(int) * (int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_NUMBER;
    }

    #region ClassCanbeSerialized Members

    public int getSize()
    {
        return sizeof(int) * m_pAttr.Length;
    }

    public bool readFromBuff(ref NetInputBuffer buff)
    {
        if (m_pAttr == null)
            m_pAttr = new int[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_NUMBER];
        for (int i = 0; i < (int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_NUMBER; i++ )
        {
            buff.ReadInt(ref m_pAttr[i]);
        }
        return true;
    }

    public int writeToBuff(ref NetOutputBuffer buff)
    {
        for (int i = 0; i < (int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_NUMBER; i++)
        {
            buff.WriteInt(m_pAttr[i]);
        }
        return getSize();
    }

    #endregion

};
//一级战斗属性结构
[Serializable] //可序列化
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public struct _OWN_ABILITY:ClassCanbeSerialized
{
    // AbilityID_t	m_Ability_ID; 不需要 ID，索引就是 ID
    public short m_Level; // 技能等级
    public short m_Exp; // 技能熟练度

    public static int getMaxSize()
    {
        return sizeof(short) * 2;
    }
    #region ClassCanbeSerialized Members

    public int getSize()
    {
        return sizeof(short) *2;
    }

    public bool readFromBuff(ref NetInputBuffer buff)
    {
        buff.ReadShort(ref m_Level);
        buff.ReadShort(ref m_Exp);
        return true;
    }

    public int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteShort(m_Level);
        buff.WriteShort(m_Exp);
        return getSize();
    }

    #endregion
};

[Serializable] //可序列化
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public struct _ObjID_List: ClassCanbeSerialized
{
	public const int MAX_LIST_SIZE = 512;
    public int getSize()
    {
        return sizeof(int) + sizeof(int)*m_nCount;
    }
    public int writeToBuff(ref NetOutputBuffer buff) 
    {
        buff.WriteInt(m_nCount);
        for(int i = 0; i < m_nCount; i++)
        {
            buff.WriteInt(m_aIDs[i]);
        }
        return getSize();
    }
    public bool readFromBuff(ref NetInputBuffer buff)
    {
        if (m_aIDs == null)
            m_aIDs = new int[MAX_LIST_SIZE];
        if(buff.ReadInt(ref m_nCount) != sizeof(int))return false;
        for(int i = 0;i < m_nCount;i++)
        {
            if(buff.ReadInt(ref m_aIDs[i]) != sizeof(int))return false;
        }
        return true;
    }
    
    public static int getMaxSize()
    {
        return sizeof(int) * MAX_LIST_SIZE;
    }
	public int m_nCount;
	public int[] m_aIDs;
}
[Serializable] //可序列化
[StructLayout(LayoutKind.Sequential, Pack = 1)] //按1字节对齐
public class RELATION_MEMBER
{
    public uint m_MemberGUID;
    public byte[] m_szMemberName = new byte[GAMEDEFINE.MAX_CHARACTER_NAME];
    public int m_nLevel;							//角色等级
    public int m_nMenPai;							//门派 MENPAI_ATTRIBUTE
    public int m_nPortrait;						// 头像
    public short m_GuildID;							//帮会ID
    public static void copy(RELATION_MEMBER src, RELATION_MEMBER dest)
    {
        dest.m_MemberGUID = src.m_MemberGUID;
        Array.Copy(src.m_szMemberName, dest.m_szMemberName, GAMEDEFINE.MAX_CHARACTER_NAME);
        dest.m_nLevel = src.m_nLevel;
        dest.m_nMenPai = src.m_nMenPai;
        dest.m_nPortrait = src.m_nPortrait;
        dest.m_GuildID = src.m_GuildID;
    }
	public class  ReMember_ExtData
	{
		public int			m_nLevel;							//角色等级
		public int			m_nMenPai;							//门派 MENPAI_ATTRIBUTE
		public int			m_nPortrait;						//头像
		public short	m_GuildID;							//帮会ID
	};

	public RELATION_MEMBER( )
	{
		CleanUp( );
	}

	public void CleanUp( )
	{
		m_MemberGUID = 0xFFFFFFFF/*INVALID_ID*/; // UINT类型没有负数 [2010-12-14] by: cfp+
        for(int i=0; i<m_szMemberName.Length; ++i)
        {
            m_szMemberName[i] = 0;
        }
		m_nLevel = 0;
		m_nMenPai = 9;
		m_nPortrait = -1;
		m_GuildID = MacroDefine.INVALID_ID;
	}

	public ReMember_ExtData	GetExtData()
	{
		ReMember_ExtData ExtData = new ReMember_ExtData();
		
		ExtData.m_nLevel = m_nLevel;
		ExtData.m_nMenPai = m_nMenPai;
		ExtData.m_nPortrait = m_nPortrait;
		ExtData.m_GuildID = m_GuildID;
		return ExtData;
	}

	public void			 SetExtData( ReMember_ExtData ExtData)
	{
		m_nLevel = ExtData.m_nLevel;
		m_nMenPai = ExtData.m_nMenPai;
		m_nPortrait = ExtData.m_nPortrait;
		m_GuildID = ExtData.m_GuildID;
	}
};
