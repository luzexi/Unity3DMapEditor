using System;
using Network;
using Network.Handlers;

namespace Network.Packets
{
    public class GCAbilityAction : PacketBase
    {
        public enum ActionState
	    {
		    ABILITY_BEGIN = 1,
		    ABILITY_END,
	    };
        public override bool readFromBuff(ref NetInputBuffer buff)
        {

            buff.ReadUint(ref m_ObjID);
            buff.ReadInt(ref m_nLogicCount);
            buff.ReadShort(ref m_AbilityID);
            buff.ReadInt(ref m_PrescriptionID);
            buff.ReadUint(ref m_TargetID);
            buff.ReadByte(ref m_BeginOrEnd);
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_ABILITY_ACTION;
        }

        public override int getSize()
        {
            return sizeof(int) * 4 + sizeof(short) + sizeof(byte);
        }

        public uint ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }

        public int LogicCount
        {
            get { return this.m_nLogicCount; }
            set { m_nLogicCount = value; }
        }
        public short AbilityID
        {
            get { return m_AbilityID; }
        }
        public int PrescriptionID
        {
            get { return m_PrescriptionID; }
        }
        public uint TargetID
        {
            get { return this.m_TargetID; }
            set { m_TargetID = value; }
        }

        public byte BeginOrEnd
        {
            get { return this.m_BeginOrEnd; }
            set { m_BeginOrEnd = value; }
        }
        //数据
        uint m_ObjID;			// 所有Obj类型的ObjID
        int m_nLogicCount;		// 逻辑计数
        short m_AbilityID;		// 生活技能ID
        int m_PrescriptionID;	// 配方 ID
        uint m_TargetID;			// 操作台（对象）的 ObjID
        byte m_BeginOrEnd;		// 
    };

    public class GCAbilityActionFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCAbilityAction(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_ABILITY_ACTION; }
        public override int GetPacketMaxSize()
        {
            return sizeof(int) * 4 + sizeof(short) + sizeof(byte);
        }
    };
}