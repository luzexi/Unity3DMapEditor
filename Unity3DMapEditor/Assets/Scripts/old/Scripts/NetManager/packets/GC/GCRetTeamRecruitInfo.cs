
using System;
using System.Collections.Generic;

using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class GCRetTeamRecruitInfo : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadByte(ref m_nTeamCount);
            for (byte i = 0; i < m_nTeamCount; i++)
            {
                if(recruitInfo[i] == null)
                    recruitInfo[i] = new RECRUIT_INFO();
                recruitInfo[i].readFromBuff(ref buff);
            }

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //todo
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_RET_RECRUITINFO;
        }
        public override int getSize()
        {
            int length = 0;
            for (byte i = 0; i < m_nTeamCount; i++)
            {
                length += recruitInfo[i].getSize();
            }
            return sizeof(byte)  + length;
        }

        //public interface

        public byte TeamCount
        {
            get{return m_nTeamCount;}
        }
        public RECRUIT_INFO[] RecruitInfo
        {
            get { return recruitInfo; }
        }

        byte                    m_nTeamCount;   //队伍数量
        RECRUIT_INFO[]          recruitInfo = new RECRUIT_INFO[5]; //返回的队伍信息

    };
    public class GCRetTeamRecruitInfoFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCRetTeamRecruitInfo(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_RET_RECRUITINFO; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) + 5 * RECRUIT_INFO.getMaxSize();
        }
    };
}