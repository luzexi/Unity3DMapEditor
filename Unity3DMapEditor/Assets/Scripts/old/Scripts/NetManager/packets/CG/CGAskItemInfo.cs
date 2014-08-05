
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Packets;

namespace Network.Packets
{


public class CGAskItemInfo : PacketBase
{
    //公用继承接口
    public override bool readFromBuff(ref NetInputBuffer buff)
    {
        return true;
    }
    public override int writeToBuff(ref NetOutputBuffer buff)
    {
        buff.WriteShort(m_BagIndex);
        return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
    }

    public override short getPacketID()
    {
        return (short)PACKET_DEFINE.PACKET_CG_ASKITEMINFO;
    }
    public override int getSize()
    {
        return sizeof(short);
    }

    //public
    public short BagIndex
    {
        get { return this.m_BagIndex; }
        set { m_BagIndex = value; }
    }

    //数据
    private short m_BagIndex;		//item 在包中的位置

};
public class CGAskItemInfoFactory : PacketFactory
{
    public override PacketBase CreatePacket() { return new CGAskItemInfo(); }
    public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ASKITEMINFO; }
    public override int GetPacketMaxSize()
    {
        return sizeof(short);
    }
};
}