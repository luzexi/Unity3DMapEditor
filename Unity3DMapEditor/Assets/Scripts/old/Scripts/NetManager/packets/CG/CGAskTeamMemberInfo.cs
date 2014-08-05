
using System;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    
    public class CGAskTeamMemberInfo : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_ObjID);
            buff.WriteUint(m_GUID);
            buff.WriteShort(m_SceneID);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ASKTEAMMEMBERINFO;
        }
        public override int getSize()
        {
            return sizeof(short) + sizeof(uint)*2;
        }

        public short SceneID
        {
            set { m_SceneID = value; }
        }
        public uint GUID
        {
            set { m_GUID = value; }
        }
        public uint ObjID
        {
            set { m_ObjID = value; }
        }

        //数据
        short m_SceneID;	//对方所在的场景ID
        uint m_GUID;		//对方的GUID
        uint m_ObjID;	//对方的ObjID
    };

    public class CGAskTeamMemberInfoFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGAskTeamMemberInfo(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASKTEAMMEMBERINFO; }
        public override int GetPacketMaxSize()
        {
            return sizeof(short) + sizeof(uint)*2;
        }
    };
}