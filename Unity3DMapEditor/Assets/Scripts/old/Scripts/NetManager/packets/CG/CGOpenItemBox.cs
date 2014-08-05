using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{
		public class CGOpenItemBox: PacketBase
		{
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

			public override short getPacketID()  { return (short)PACKET_DEFINE.PACKET_CG_OPENITEMBOX ; }
			public override int getSize() { return	sizeof(uint);}

			public void			setObjID(uint id) { m_ObjID = id; }
			public uint			getObjID()  { return m_ObjID; }

			uint			m_ObjID;		//Obj_ItemBoxµÄObjID
		}
		
		public class CGOpenItemBoxFactory:	PacketFactory
		{
			public override PacketBase CreatePacket() { return new CGOpenItemBox() ; }
			public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_OPENITEMBOX; }
			public override int GetPacketMaxSize(){ return	sizeof(uint); }
		}
}
