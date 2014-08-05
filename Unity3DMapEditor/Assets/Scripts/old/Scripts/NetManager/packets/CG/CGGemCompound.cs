
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Network;

namespace Network.Packets
{

    public class CGGemCompound : PacketBase
    {
        public static int GEMBAGINDEX = 6;

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            for (byte i = 0; i < GEMBAGINDEX; i++ )
            {
                buff.WriteSByte(m_GemBagIndex[i]);
            }
            buff.WriteInt(m_Platform);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_GEM_COMPOUND;
        }
        public override int getSize()
        {
            return sizeof(sbyte)*GEMBAGINDEX + sizeof(int);
        }

        //public interface
        public void SetGemBagIndex(sbyte bagIndex, byte index)
        {
            m_GemBagIndex[index] = bagIndex;
        }
        public sbyte GetGemBagIndex(byte index)
        {
            return m_GemBagIndex[index];
        }
        public int PlatformId
        {
            set { m_Platform = value; }
        }
        //{----------------------------------------------------------------------------
		// [2010-11-23] by: cfp+ 兼容现在的宝石合成脚本
		// 原
		//BYTE					m_GemBagIndex1;
		//BYTE					m_GemBagIndex2;

		// 现
		sbyte[]					m_GemBagIndex=new sbyte[GEMBAGINDEX];
		//----------------------------------------------------------------------------}
		int					    m_Platform;

    };
    public class CGGemCompoundFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGGemCompound(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_GEM_COMPOUND; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte)*CGGemCompound.GEMBAGINDEX + sizeof(int);
        }
    };
}