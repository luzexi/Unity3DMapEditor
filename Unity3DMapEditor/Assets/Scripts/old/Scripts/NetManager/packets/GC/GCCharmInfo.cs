using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{
    // 刷新符印信息 [3/23/2012 ZZY]
    public class GCCharmInfoFlush : PacketBase
	{
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARMINFOFLUSH;
        }

        public override int getSize()
        {
            int uSize = sizeof(byte)*GAMEDEFINE.CHARM_ATTR_NUM*GAMEDEFINE.CHARM_LEVEL_NUM;;
            return uSize;
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            for (int i = 0; i < GAMEDEFINE.CHARM_ATTR_NUM; ++i)//读取每个属性每个等级使用的符印数量
            {
                for (int j = 0; j < GAMEDEFINE.CHARM_LEVEL_NUM; ++j)
                {
                    buff.ReadByte(ref mCharmInfo[i,j]);
                }
            }
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
        public  byte[,] CharmInfo
        {
            set
            {
                mCharmInfo = value;
            }
            get
            {
                return mCharmInfo;
            }
        }

        byte[,] mCharmInfo = new byte[GAMEDEFINE.CHARM_ATTR_NUM,GAMEDEFINE.CHARM_LEVEL_NUM];
    };

    public class GCCharmInfoFlushFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCCharmInfoFlush(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARMINFOFLUSH; }
        public override int GetPacketMaxSize()
        {
            int uSize = sizeof(byte)*GAMEDEFINE.CHARM_ATTR_NUM*GAMEDEFINE.CHARM_LEVEL_NUM;
            return uSize;
        }
    };
}
