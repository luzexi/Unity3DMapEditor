
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Handlers;

namespace Network.Packets
{


    public class CGCommand : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            //todo
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            buff.WriteByte(m_CommandSize);
            if(m_CommandSize > 0 && m_CommandSize < 127)
                buff.Write(ref m_Command, m_CommandSize);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_COMMAND;
        }
        public override int getSize()
        {
            return sizeof(byte) * (m_CommandSize + 1);
        }


        //public
        public byte CommandSize
        {
            get { return this.m_CommandSize; }
            set { m_CommandSize = value; }
        }
        public byte[] Command
        {
            get { return this.m_Command; }
            set { m_Command = value; }
        }

        //数据

	    private byte					m_CommandSize ;					//当前命令的长度
	    private byte[]					m_Command = new byte[128] ;	//命令数据

    };
    public class CGCommandFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGCommand(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_COMMAND; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * (128 + 1);
        }
    };
}