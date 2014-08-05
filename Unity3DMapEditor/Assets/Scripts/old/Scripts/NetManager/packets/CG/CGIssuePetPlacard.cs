
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{


    public class CGIssuePetPlacard : PacketBase
    {
        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            //todo
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            m_GUID.writeToBuff(ref buff);
            buff.WriteUint(m_idNpc);
            buff.WriteByte(m_byMessageSize);
            buff.Write(ref m_szMessage, m_byMessageSize);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_ISSUEPETPLACARD;
        }

        public override int getSize()
        {
            return sizeof(uint) + sizeof(byte) *(m_byMessageSize + 1)+ m_GUID.getSize();
        }


        //public
        public PET_GUID_t GUID
        {
            get { return this.m_GUID; }
            set { m_GUID = value; }
        }
        public uint idNpc
        {
            get { return this.m_idNpc; }
            set { m_idNpc = value; }
        }

        public string Message
        {
            set { 
                    m_byMessageSize = (byte)value.Length;
                    for (int i = 0; i < m_byMessageSize; i++)
                    {
                        m_szMessage[i] = (byte)value[0];
                    }
                }
        }

        //数据
        private PET_GUID_t m_GUID;// ID
        private uint m_idNpc;// 目标NPC ID
        private byte m_byMessageSize;
        private byte[] m_szMessage = new byte[GAMEDEFINE.PET_PLACARD_ITEM_MESSAGE_SIZE];
    };
    public class CGIssuePetPlacardFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGIssuePetPlacard(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_ISSUEPETPLACARD; }
        public override int GetPacketMaxSize()
        {
            return PET_GUID_t.getMaxSize() + sizeof(uint) + sizeof(byte) * (GAMEDEFINE.PET_PLACARD_ITEM_MESSAGE_SIZE + 1);
        }
    };
}