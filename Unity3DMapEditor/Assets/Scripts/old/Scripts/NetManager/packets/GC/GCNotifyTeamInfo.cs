
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class GCNotifyTeamInfo : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_ObjID) != sizeof(uint)) return false;
            if (buff.ReadByte(ref m_uTeamFlag) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_uTeamLeaderFlag) != sizeof(byte)) return false;
            if (buff.ReadByte(ref m_uTeamFullFlag) != sizeof(byte)) return false;

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            //todo
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_NOTIFYTEAMINFO;
        }
        public override int getSize()
        {
            return sizeof(uint) + sizeof(byte) * 3;
        }

        //public interface
        public uint ObjectId
        {
            get { return m_ObjID; }
        }
        public byte TeamFlag
        {
            get { return m_uTeamFlag; }
        }
        public byte TeamLeaderFlag
        {
            get { return m_uTeamLeaderFlag; }
        }
        public byte TeamFullFlag
        {
            get { return m_uTeamFullFlag; }
        }

        //数据

        uint m_ObjID;	// 玩家 ID 号
        byte m_uTeamFlag; // 队伍标记
        byte m_uTeamLeaderFlag; // 队长标记
        byte m_uTeamFullFlag; // 队伍满员标记
        //UCHAR				m_uTeamFollowFlag; // 是否处于组队跟随状态

    };
    public class GCNotifyTeamInfoFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCNotifyTeamInfo(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_NOTIFYTEAMINFO; }
        public override int GetPacketMaxSize()
        {
            return sizeof(uint) + sizeof(byte) * 3;
        }
    };
}