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
    public class GCSkillClass : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (buff.ReadShort(ref m_wNumXinFa) != sizeof(short)) return false;
            for(short i = 0; i < m_wNumXinFa; i++)
            {
                m_aXinFa[i].readFromBuff(ref buff);
            }
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            buff.WriteShort(m_wNumXinFa);
            for(short i = 0; i < m_wNumXinFa; i++)
            {
                m_aXinFa[i].writeToBuff(ref buff);
            }
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_DETAILXINFALIST;
        }

        public override int getSize()
        {
            return sizeof(int)  + 
                   sizeof(short) +
                   _OWN_XINFA.GetMaxSize()*m_wNumXinFa;
        }

        public int ObjectID
        {
            get { return m_ObjID; }
            set { m_ObjID = value; }
        }

        public short numXinFa
        {
            get { return m_wNumXinFa; }
            set { m_wNumXinFa = value; }
        }

        public _OWN_XINFA[] XinFa
        {
            get { return m_aXinFa; }
        }

        int 		m_ObjID;	// 所有Obj类型的ObjID
	    short		m_wNumXinFa;
	    _OWN_XINFA[]	m_aXinFa = new _OWN_XINFA[GAMEDEFINE.MAX_CHAR_XINFA_NUM];
    };

    public class GCDetailXinFaListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCSkillClass(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_DETAILXINFALIST; }
        public override int GetPacketMaxSize()
        { 
             return sizeof(int)  + 
                   sizeof(short) +
                   _OWN_XINFA.GetMaxSize()*GAMEDEFINE.MAX_CHAR_XINFA_NUM;
        }
    };
}