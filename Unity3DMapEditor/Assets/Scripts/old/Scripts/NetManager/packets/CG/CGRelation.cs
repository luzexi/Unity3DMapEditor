
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGRelation :  PacketBase 
    {

	    //公用继承接口
	    public override bool readFromBuff(ref NetInputBuffer buff)
        {
            m_Relation.readFromBuff(ref buff);
            return true;
        }
	    public override int writeToBuff(ref NetOutputBuffer buff)
        {
            m_Relation.writeToBuff(ref buff);
             return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

	    public override short getPacketID() { return (short)PACKET_DEFINE.PACKET_CG_RELATION ; }
	    public override int getSize()
	    { 
		    return m_Relation.getSize();
	    }
    	
	    //使用数据接口
	   public CG_RELATION			GetRelation( ){ return m_Relation ; }
	   public void					SetRelation( CG_RELATION pRelation ){ m_Relation = pRelation ; } 

	    //数据
	    CG_RELATION				m_Relation = new  CG_RELATION();

    };


    public class CGRelationFactory : PacketFactory
    {
	    public override PacketBase CreatePacket() {   return new CGRelation() ; }
	    public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_RELATION ; }
	    public override int GetPacketMaxSize(){ return CG_RELATION.getMaxSize() ; }
    };


 
}

