
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class GCRetChangeScene : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {

            if (buff.ReadByte(ref m_Return) != sizeof(byte)) return false;
           
            if (m_Return == (byte)CHANGESCENERETURN.CSR_SUCCESS_DIFFSERVER)
            {
                if(buff.Read(ref m_IP, NET_DEFINE.IP_SIZE) != NET_DEFINE.IP_SIZE) return false;
                if(buff.ReadShort(ref m_Port) != sizeof(short)) return false;
            }
            if(buff.ReadInt(ref m_uKey) != sizeof(int)) return false;

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //todo
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_RETCHANGESCENE;
        }
        public override int getSize()
        {
            return sizeof(int) +
                sizeof(short)  +
                sizeof(byte) * (NET_DEFINE.IP_SIZE + 1);
        }


        public enum CHANGESCENERETURN
        {
            CSR_SUCCESS = 0,		//成功, 本场景
            CSR_SUCCESS_DIFFSERVER,//成功，其他服务器端程序
            CSR_ERROR,				//失败
        };

        //public interface

        public short Port
        {
            get { return this.m_Port; }
        }

        public byte Return
        {
            get { return this.m_Return; }
        }
        public int Key
        {
            get { return this.m_uKey; }
        }
        public byte[] IP
        {
            get { return this.m_IP; }
        }

 
        //数据
	    private byte					m_Return ;	//enum CHANGESCENERETURN
	    private byte[] m_IP = new byte[NET_DEFINE.IP_SIZE];
	    private short					m_Port ;
	    private int					m_uKey ;

    };
    public class GCRetChangeSceneFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCRetChangeScene(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_RETCHANGESCENE; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) +
                sizeof(short) +
                sizeof(byte) * (NET_DEFINE.IP_SIZE + 1);
        }
    };
}