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
class GCCharEquipment : PacketBase
{
	public GCCharEquipment( ){
		Reset( );
	}

	void Reset(){
		m_ObjID			= MacroDefine.UINT_MAX;
		m_wFlags		= 0;
	}

	//公用继承接口
    public override bool readFromBuff(ref NetInputBuffer iStream)
    {
        iStream.ReadUint( ref m_ObjID);

        iStream.ReadUShort(ref m_wFlags);
     if ( (m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_WEAPON)) != 0)
	{
		iStream.ReadUint( ref m_WeaponID);
        iStream.ReadUint(ref m_WeaponGemID);
	}

    if ( (m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_CAP)) != 0)
	{
        iStream.ReadUint(ref m_CapID);
        iStream.ReadUint(ref m_CapGemID);
	}

    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_ARMOR)) != 0)
	{
        iStream.ReadUint(ref m_ArmourID);
        iStream.ReadUint(ref m_ArmourGemID);
	}

    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_CUFF)) != 0)
	{
        iStream.ReadUint(ref m_CuffID);
        iStream.ReadUint(ref m_CuffGemID);
	}

    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_BOOT)) != 0)
	{
        iStream.ReadUint(ref m_BootID);
        iStream.ReadUint(ref m_BootGemID);
	}

    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_BACK)) != 0)
	{
        iStream.ReadUint(ref m_BackID);
        iStream.ReadUint(ref m_BackGemID);
	}

    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_SASH)) != 0)           // Allan_Tao 27/4/2011 
    {
        iStream.ReadUint(ref m_SashID);
        iStream.ReadUint(ref m_SashGemID);

    }
	return true ;

    }
	public override int writeToBuff(ref NetOutputBuffer buff)
    {
         return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
    }

    public override short getPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHAREQUIPMENT; }
	public override int getSize() {
								    uint uAttribSize = 0;
								    uint i;
								    for ( i = 0; i < (uint)HUMAN_EQUIP.HEQUIP_NUMBER; i++ )
								    {
                                        if ((m_wFlags & (1 << (int)i)) != 0)
									    {
										    uAttribSize += sizeof( uint ) * 2;
									    }
								    }
								    uAttribSize +=	sizeof(uint) + sizeof(ushort);
                                    return (int)uAttribSize;
							}

	//使用数据接口
public	void			setObjID(uint id) { m_ObjID = id; }
public	uint			getObjID()  { return m_ObjID; }

public	uint			getFlags(  ) { return (uint)(m_wFlags); }

public	bool			IsUpdateAttrib( HUMAN_EQUIP eAttrib ) { return ((m_wFlags & (1<<(int)eAttrib)) != 0)?(true):(false); }
public	void			SetUpdateAttrib( HUMAN_EQUIP eAttrib, bool bUpdate ){
						if ( bUpdate )
                            m_wFlags = (ushort)(m_wFlags | (1 << (int)eAttrib));
						else
                            m_wFlags &= (ushort)(~(1 << (int)eAttrib));
					}

public	void			setWeaponID( uint ID ){ m_WeaponID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_WEAPON,true); }
public	uint			getWeaponID(  ) { return m_WeaponID; }

public	void			setWeaponGemID( uint ID ){ m_WeaponGemID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_WEAPON,true); }
public	uint			getWeaponGemID(  ) { return m_WeaponGemID; }

public	void			setCapID( uint ID ){ m_CapID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_CAP,true); }
public	uint			getCapID(  ) { return m_CapID; }

public	void			setCapGemID( uint ID ){ m_CapGemID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_CAP,true); }
public	uint			getCapGemID(  ) { return m_CapGemID; }

public	void			setArmourID( uint ID ){ m_ArmourID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_ARMOR,true); }
public	uint			getArmourID(  ) { return m_ArmourID; }

public	void			setArmourGemID( uint ID ){ m_ArmourGemID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_ARMOR,true); }
public	uint			getArmourGemID(  ) { return m_ArmourGemID; }

public	void			setCuffID( uint ID ){ m_CuffID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_CUFF,true); }
public	uint			getCuffID(  ) { return m_CuffID; }

public	void			setCuffGemID( uint ID ){ m_CuffGemID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_CUFF,true); }
public	uint			getCuffGemID(  ) { return m_CuffGemID; }

public	void			setBootID( uint ID ){ m_BootID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_BOOT,true); }
public	uint			getBootID(  ) { return m_BootID; }

public	void			setBootGemID( uint ID ){ m_BootGemID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_BOOT,true); }
public	uint			getBootGemID(  ) { return m_BootGemID; }

public	void			setBackID(uint ID) { m_BackID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_BACK,true);}
public	uint			getBackID()   {return m_BackID;}

public	void			setBackGemID(uint ID) {m_BackGemID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_BACK,true);}
public	uint			getBackGemID() {return m_BackGemID;}

 public   void            setSashID(uint ID) { m_SashID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_SASH, true); }           // Allan_Tao 27/4/2011 
  public  uint            getSashID() { return m_SashID;}                                                 // Allan_Tao 27/4/2011 

  public  void            setSashGemID(uint ID){ m_SashGemID = ID; SetUpdateAttrib(HUMAN_EQUIP.HEQUIP_SASH, true); }      // Allan_Tao 27/4/2011 
public    uint            getSashGemID() { return m_SashGemID; }                                          // Allan_Tao 27/4/2011 

public	void			setID(HUMAN_EQUIP EquipPoint, uint ID, uint uGemID)
	{
			switch(EquipPoint) {
			case HUMAN_EQUIP.HEQUIP_WEAPON:
				{
					setWeaponID(ID);
					setWeaponGemID(uGemID);
				}
				break;
			case HUMAN_EQUIP.HEQUIP_CAP:
				{
					setCapID(ID);
					setCapGemID(uGemID);
				}
				break;
			case HUMAN_EQUIP.HEQUIP_ARMOR:
				{
					setArmourID(ID);
					setArmourGemID(uGemID);
				}
				break;
			case HUMAN_EQUIP.HEQUIP_CUFF:
				{
					setCuffID(ID);
					setCuffGemID(uGemID);
				}
				break;
			case HUMAN_EQUIP.HEQUIP_BOOT:
				{
					setBootID(ID);
					setBootGemID(uGemID);
				}
				break;
			case HUMAN_EQUIP.HEQUIP_BACK:
				{
					setBackID(ID);
					setBackGemID(uGemID);
                   
				}
                break;
            case HUMAN_EQUIP.HEQUIP_SASH:                               // Allan_Tao 27/4/2011 
                {
                    setSashID(ID);
                    setSashGemID(uGemID);
                }
                break;
			default:
				break;
			}
	}

public	void FillParamBuf(  object pBuf )
{
    uint [] aBuf	= (uint[])(pBuf);

	uint uParamIndex;
	uParamIndex = 0;

    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_WEAPON)) != 0)
		aBuf[uParamIndex++] = m_WeaponID;

    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_CAP)) != 0)
		aBuf[uParamIndex++] = m_CapID;

    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_ARMOR)) != 0)
		aBuf[uParamIndex++] = m_ArmourID;

    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_CUFF)) != 0)
		aBuf[uParamIndex++] = m_CuffID;

    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_BOOT)) != 0)
		aBuf[uParamIndex++] = m_BootID;

    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_BACK)) != 0)
	{
		aBuf[uParamIndex++] = m_BackID;
	}
    if ((m_wFlags & (1 << (int)HUMAN_EQUIP.HEQUIP_SASH)) != 0)        // Allan_Tao 27/4/2011 
    {
        aBuf[uParamIndex++] = m_SashID;
    }
}

	// 当前装备相关接口 [10/25/2011 edit by ZL]
public	byte		getWeaponEnhance()	  { return (byte)((m_WeaponGemID>>24) &0xFF); }
public	byte		getCapEnhance   ()	  { return (byte)((m_CapGemID   >>24) &0xFF); }
public	byte		getArmourEnhance()	  { return (byte)((m_ArmourGemID>>24) &0xFF); }
public	byte		getCuffEnhance  ()	  { return (byte)((m_CuffGemID  >>24) &0xFF); }
public	byte		getBootEnhance  ()	  { return (byte)((m_BootGemID  >>24) &0xFF); }
public	byte		getBackEnhance  ()	  { return (byte)((m_BackGemID  >>24) &0xFF); }
public	byte		getSashEnhance  ()	  { return (byte)((m_SashGemID  >>24) &0xFF); }

	// gemPos = [0, 2], 装备0,1,2位置的宝石等级 = 目前ID的[16,21], [8,13], [0,5]位的数值[10/25/2011 edit by ZL]
public	byte		getWeaponGemLevel(byte gemPos)	  { return (byte)((m_WeaponGemID>>(16-(gemPos<<3))) &0x3F); }
public	byte		getCapGemLevel   (byte gemPos)    { return (byte)((m_CapGemID   >>(16-(gemPos<<3))) &0x3F); }
public	byte		getArmourGemLevel(byte gemPos)	  { return (byte)((m_ArmourGemID>>(16-(gemPos<<3))) &0x3F); }
public	byte		getCuffGemLevel  (byte gemPos)	  { return (byte)((m_CuffGemID  >>(16-(gemPos<<3))) &0x3F); }
public	byte		getBootGemLevel  (byte gemPos)	  { return (byte)((m_BootGemID  >>(16-(gemPos<<3))) &0x3F); }
public	byte		getBackGemLevel  (byte gemPos)	  { return (byte)((m_BackGemID  >>(16-(gemPos<<3))) &0x3F); }
public	byte		getSashGemLevel  (byte gemPos)	  { return (byte)((m_SashGemID  >>(16-(gemPos<<3))) &0x3F); }

	// gemPos = [0, 2], 装备0,1,2位置的宝石镶嵌状态 = 目前ID的[22,23], [14,15], [6,7]位的数值[10/25/2011 edit by ZL]
	/*
	  return
	  0----普通
	  1----完美
	  2----极致
	*/
public	byte		getWeaponGemState(byte gemPos)	  { return (byte)((m_WeaponGemID>>(22-(gemPos<<3))) &0x03); }
public	byte		getCapGemState   (byte gemPos)    { return (byte)((m_CapGemID   >>(22-(gemPos<<3))) &0x03); }
public	byte		getArmourGemState(byte gemPos)	  { return (byte)((m_ArmourGemID>>(22-(gemPos<<3))) &0x03); }
public	byte		getCuffGemState  (byte gemPos)	  { return (byte)((m_CuffGemID  >>(22-(gemPos<<3))) &0x03); }
public	byte		getBootGemState  (byte gemPos)	  { return (byte)((m_BootGemID  >>(22-(gemPos<<3))) &0x03); }
public	byte		getBackGemState  (byte gemPos)	  { return (byte)((m_BackGemID  >>(22-(gemPos<<3))) &0x03); }
public	byte		getSashGemState  (byte gemPos)	  { return (byte)((m_SashGemID  >>(22-(gemPos<<3))) &0x03); }


	uint			m_ObjID;		// ObjID

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

	// 根据策划需求 需要 传给客户端装备的 当前强化等级、三个宝石的镶嵌状态以及宝石等级 [10/25/2011 edit by ZL]
	/*
	|  ref [现GemID修改如下]
	|  uint GemID xxxxxxxx
	|              |||||||__ 前2位镶嵌状态（后6位宝石等级）共8位宝石信息
	|              |||||
	|              |||||____ 前2位镶嵌状态（后6位宝石等级）共8位宝石信息
	|              ||| 
	|              |||______ 前2位镶嵌状态（后6位宝石等级）共8位宝石信息
	|              | 
	|              |________ 8位强化等级
	|              
	*/
	ushort			m_wFlags;		// 每个位表示一个属性是否要刷新 HUMAN_EQUIP

	uint			m_WeaponID;		// 武器 - 资源表ID
	uint			m_WeaponGemID;	// 武器宝石 - 资源表ID
	uint			m_CapID;		// 帽子 - 资源表ID
	uint			m_CapGemID;		// 帽子宝石 - 资源表ID
	uint			m_ArmourID;		// 衣服 - 资源表ID
	uint			m_ArmourGemID;	// 衣服宝石 - 资源表ID
	uint			m_CuffID;		// 护腕 - 资源表ID
	uint			m_CuffGemID;	// 护腕宝石 - 资源表ID
	uint			m_BootID;		// 靴子 - 资源表ID
	uint			m_BootGemID;	// 靴子宝石 - 资源表ID
	uint			m_BackID;		// 背饰 - 资源表ID
	uint			m_BackGemID;	// 背部宝石，必须要有，计算packet size [8/30/2010 Sun]
    uint            m_SashID;       // Allan_Tao 27/4/2011 肩带 资源ID
    uint            m_SashGemID;    // Allan_Tao 27/4/2011 肩带宝石. 暂时没用
};


class GCCharEquipmentFactory : PacketFactory
{
    public override PacketBase CreatePacket() { return new GCCharEquipment(); }

    public override int GetPacketID() { return (int)PACKET_DEFINE.PACKET_GC_CHAREQUIPMENT; }
	public override int		GetPacketMaxSize()  { return	sizeof(uint) +
													sizeof(ushort) +
													sizeof(uint) * 12; }
};



}
