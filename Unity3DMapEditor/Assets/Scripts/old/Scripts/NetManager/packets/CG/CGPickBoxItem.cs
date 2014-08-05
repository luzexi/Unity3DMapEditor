using System;
using System.Collections.Generic;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class CGPickBoxItem : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_PICKBOXITEM;
        }

        public override int getSize()
        {
            return sizeof(uint) + m_ItemId.getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            throw new NotImplementedException();
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteUint(m_ObjID);
            m_ItemId.writeToBuff(ref buff);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
        public uint ObjectId
        {
            set { m_ObjID = value; }
        }
        public _ITEM_GUID ItemGUID
        {
            set { m_ItemId = value; }
        }

        uint m_ObjID;
        _ITEM_GUID m_ItemId;
    }
    public class CGPickBoxItemFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGPickBoxItem(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_PICKBOXITEM; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint) + _ITEM_GUID.GetMaxSize();
        }
    };
}
