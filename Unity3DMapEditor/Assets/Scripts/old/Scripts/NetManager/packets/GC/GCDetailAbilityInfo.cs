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
    public class GCDetailAbilityInfo : PacketBase
    {
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_ObjID) != sizeof(uint)) return false;
            if (buff.ReadByte(ref m_wNumAbility) != sizeof(byte)) return false;

            buff.Read(ref m_uAbilityIDList, m_wNumAbility);
            for (byte i = 0; i < m_wNumAbility; i++)
            {
                if (!m_aAbility[i].readFromBuff(ref buff)) return false;
            }
            buff.Read(ref m_aPrescr, GAMEDEFINE.MAX_CHAR_PRESCRIPTION_BYTE);

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
 
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_DETAILABILITYINFO;
        }

        public override int getSize()
        {
            return sizeof(uint) +
                   sizeof(byte) +
                   _OWN_ABILITY.getMaxSize() * m_wNumAbility +
                   sizeof(byte) * m_wNumAbility+
                   sizeof(byte) * GAMEDEFINE.MAX_CHAR_PRESCRIPTION_BYTE;
        }

        public int ObjectID
        {
            get { return (int)this.m_ObjID; }
            set { m_ObjID = (uint)value; }
        }
        public byte NumAbility
        {
            get { return m_wNumAbility; }
            set { m_wNumAbility = value; }
        }

        public _OWN_ABILITY[] Ability
        {
            get { return m_aAbility; }
        }
        public byte[] AbilityIDList
        {
            get { return m_uAbilityIDList; }
        }
        public byte[] PrescrList
        {
            get { return m_aPrescr; }
        }

        uint					m_ObjID;	// 所有Obj类型的ObjID
	    byte					m_wNumAbility;
	    byte[]					m_uAbilityIDList = new byte[GAMEDEFINE.MAX_CHAR_ABILITY_NUM];
	    _OWN_ABILITY[]			m_aAbility= new _OWN_ABILITY[GAMEDEFINE.MAX_CHAR_ABILITY_NUM];
	    byte[]					m_aPrescr= new byte[GAMEDEFINE.MAX_CHAR_PRESCRIPTION_BYTE];
    };

    public class GCDetailAbilityInfoFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCDetailAbilityInfo(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_DETAILABILITYINFO; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) + sizeof(byte) +
                   _OWN_ABILITY.getMaxSize() * GAMEDEFINE.MAX_CHAR_ABILITY_NUM +
                   sizeof(byte) * GAMEDEFINE.MAX_CHAR_ABILITY_NUM +
                   sizeof(byte) * GAMEDEFINE.MAX_CHAR_PRESCRIPTION_BYTE;
        }
    };
}