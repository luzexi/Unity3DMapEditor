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
    class GCUseEquipResult : PacketBase
    {
        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_USEEQUIPRESULT;
        }

        public override int getSize()
        {
            return sizeof(byte) * 3 + _ITEM_GUID.GetMaxSize() * 2 + sizeof(uint) * 2;
        }


        byte m_Result;		//结果信息
        public byte Result
        {
            get { return m_Result; }
            set { m_Result = value; }
        }
        byte m_EquipPoint;	//装配点
        public byte EquipPoint
        {
            get { return m_EquipPoint; }
            set { m_EquipPoint = value; }
        }
        byte m_BagIndex;		//存放的位置
        public byte BagIndex
        {
            get { return m_BagIndex; }
            set { m_BagIndex = value; }
        }
        _ITEM_GUID m_ItemID;		//
        public _ITEM_GUID ItemID
        {
            get { return m_ItemID; }
            set { m_ItemID = value; }
        }
        _ITEM_GUID m_EquipID;
        public _ITEM_GUID EquipID
        {
            get { return m_EquipID; }
            set { m_EquipID = value; }
        }
        uint m_ItemResId;	//BagIndex 对应的资源
        public uint ItemResId
        {
            get { return m_ItemResId; }
            set { m_ItemResId = value; }
        }
        uint m_EquipResId;  //EquipPoint对应的资源
        public uint EquipResId
        {
            get { return m_EquipResId; }
            set { m_EquipResId = value; }
        }
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            buff.ReadByte(ref m_Result);
            buff.ReadByte(ref m_EquipPoint);
            buff.ReadByte(ref m_BagIndex);
            m_ItemID.readFromBuff(ref buff);
            m_EquipID.readFromBuff(ref buff);
            buff.ReadUint(ref m_ItemResId);
            buff.ReadUint(ref m_EquipResId);
            
            return true;
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
            throw new NotImplementedException();
        }
    }

    public class GCUseEquipResultFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCUseEquipResult(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_USEEQUIPRESULT; }
        public override int GetPacketMaxSize()
        {
            return sizeof(byte) * 3 + _ITEM_GUID.GetMaxSize() * 2 + sizeof(uint) * 2;
        }
    };
}