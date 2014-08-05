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
	public class GCCharSkillSend : PacketBase
    {
		public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadInt(ref m_ObjID) != sizeof(int)) return false;
            if (buff.ReadInt(ref m_nLogicCount) != sizeof(int)) return false;
            if (buff.ReadShort(ref m_SkillDataID) != sizeof(short)) return false;
			if (!m_posUser.readFromBuff(ref buff))return false;
            if (buff.ReadInt(ref m_TargetID) != sizeof(int)) return false;
        	if (!m_posTarget.readFromBuff(ref buff))return false;
			if (buff.ReadFloat(ref m_fDir)!= sizeof(float))return false;
			if (buff.ReadInt(ref m_nTotalTime)!= sizeof(int))return false;
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            buff.WriteInt(m_ObjID);
            buff.WriteInt(m_nLogicCount);
         	buff.WriteShort(m_SkillDataID);
            m_posUser.writeToBuff(ref buff);
            buff.WriteInt(m_TargetID);
            m_posTarget.writeToBuff(ref buff);
            buff.WriteFloat(m_fDir);
            buff.WriteInt(m_nTotalTime);
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }
		
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_CHARSKILL_SEND;
        }
		
        public override int getSize()
        {
            return sizeof(int)*4 + sizeof(short) + 
                m_posUser.getSize() + 
				m_posTarget.getSize() + m_posUser.getSize() +
				sizeof(float);
        }

        public int ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }
		
        public int nLogicCount
        {
            get { return this.m_nLogicCount; }
            set { m_nLogicCount = value; }
        }
        public short SkillDataID
        {
            get { return this.m_SkillDataID; }
            set { m_SkillDataID = value;}
        }
        public WORLD_POS posUser
        {
            get { return this.m_posUser; }
            set { m_posUser = value;}
        }
        public  int TargetID
        {
            get { return this.m_TargetID; }
            set { m_TargetID = value;}
        }
        public WORLD_POS posTarget
        {
            get { return this.m_posTarget; }
            set { m_posTarget = value;}
        }
		
        public float Dir
        {
            get { return this.m_fDir; }
            set { m_fDir = value; }
        }
		
        public int TotalTime
        {
            get { return this.m_nTotalTime; }
            set { m_nTotalTime = value;}
        }
       

		private int					m_ObjID;			// ObjID		
		private int					m_nLogicCount;	    //逻辑计数
		private short				m_SkillDataID;		//技能的资源id
		private WORLD_POS			m_posUser;			
		private int					m_TargetID;			
		private WORLD_POS			m_posTarget;		
		private float				m_fDir;				
		private int					m_nTotalTime;		
      };
	
	  public class GCCharSkillSendFactory : PacketFactory
	  {
		public override PacketBase CreatePacket() { return new GCCharSkillSend(); }
		public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_CHARSKILL_SEND; }
		public override int GetPacketMaxSize()
		{
			return sizeof(int)*4 + WORLD_POS.GetMaxSize()*2 + sizeof(float) + sizeof(short);
		}
	  };
}