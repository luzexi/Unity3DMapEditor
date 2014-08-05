using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGCharAskEquipment: PacketBase
    {
        //公用继承接口
            public override bool readFromBuff(ref NetInputBuffer buff)
            {
                buff.ReadUint(ref m_ObjID);
                return true;
            }
            public override int writeToBuff(ref NetOutputBuffer buff)
            {
                buff.WriteUint(m_ObjID);
                return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
            }

	        public override short getPacketID()
            {
                return (short)PACKET_DEFINE.PACKET_CG_CHARASKEQUIPMENT ; 
            }
	         public override int getSize() 
             { 
                 return sizeof(uint) ; 
             }

        //使用数据接口
	    public uint					getObjID() { return m_ObjID; }
	    public void					setObjID(uint idObjID) { m_ObjID = idObjID; }

	    //数据
	    uint					m_ObjID;	//对方的ObjID
    };


    public class CGCharAskEquipmentFactory :  PacketFactory 
    {
	    public override   PacketBase		CreatePacket() { return new CGCharAskEquipment() ; }
	    public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_CHARASKEQUIPMENT ; }
	    public override int GetPacketMaxSize() { return sizeof(uint) ; }
    };

}

