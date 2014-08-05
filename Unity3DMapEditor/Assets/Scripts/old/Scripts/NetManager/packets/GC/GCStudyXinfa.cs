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
    class GCStudyXinfa : PacketBase
    {
        public struct _STUDERESULT_INFO : ClassCanbeSerialized   //学习后的结果（改变了什么）
        {
            public uint m_uSpareMoney;			//	升级后玩家剩余的金钱
            public uint m_uSpareExperience;		//	升级后玩家剩余的经验

            public short m_idXinfa;
            public short m_StudedLevel;			// 升级之后的等级

            public static int getMaxSize()
            {
                return sizeof(uint) * 2 + sizeof(short)*2;
            }
            public  Boolean readFromBuff(ref Network.NetInputBuffer buff)
            {
                buff.ReadUint(ref m_uSpareMoney);
                buff.ReadUint(ref m_uSpareExperience);
                buff.ReadShort(ref m_idXinfa);
                buff.ReadShort(ref m_StudedLevel);

                return true;
            }

            public  Int32 getSize()
            {
                return sizeof(uint) * 2 + sizeof(short) * 2;
            }

            public  Int32 writeToBuff(ref Network.NetOutputBuffer buff)
            {
                throw new Exception("The method or operation is not implemented.");
            }
        };

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_STUDYXINFA_H;
        }

        public override int getSize()
        {
            return m_StudeResult.getSize();
        }

      
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            m_StudeResult.readFromBuff(ref buff);

            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }

        public _STUDERESULT_INFO StudyResult
        {
            get { return m_StudeResult; }
            set { m_StudeResult = value; }
        }
        private _STUDERESULT_INFO m_StudeResult;
    }

    public class GCStudyXinfaFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCStudyXinfa(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_STUDYXINFA_H; }
        public override int GetPacketMaxSize()
        {
            return GCStudyXinfa._STUDERESULT_INFO.getMaxSize();
        }
    };
}