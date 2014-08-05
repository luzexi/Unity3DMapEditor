
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Network;
using Network.Handlers;

namespace Network.Packets
{

    public class GCNewItemBox : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            if (buff.ReadUint(ref m_ObjID) != sizeof(uint)) return false;
            if (buff.ReadUint(ref m_idOwner) != sizeof(uint)) return false;
            if (buff.ReadUint(ref m_MonsterID) != sizeof(uint)) return false;
            //int size = Marshal.SizeOf(typeof(WORLD_POS));
            //byte[] bytes = new byte[size];
            //if (buff.Read(ref bytes, size) != size) return false;
            //m_posWorld = (WORLD_POS)NET_DEFINE.BytesToStruct(bytes, typeof(WORLD_POS));
         
            if(!m_posWorld.readFromBuff(ref buff))return false;
            if (buff.ReadInt(ref m_OBJType) != sizeof(int)) return false;
           

            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            buff.WriteUint(m_ObjID);
            buff.WriteUint(m_idOwner);
            buff.WriteUint(m_MonsterID);
            //byte[] bytes = NET_DEFINE.StructToBytes(m_posWorld);
            //buff.Write(ref bytes, Marshal.SizeOf(m_posWorld));
            buff.WriteStruct(m_posWorld);
            buff.WriteInt(m_OBJType);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_GC_NEWITEMBOX;
        }
        public override int getSize()
        {
            return m_posWorld.getSize() +
                sizeof(int)*4;
        }

        //public interface
         public WORLD_POS Position
        {
            get { return this.m_posWorld; }
            set { m_posWorld = value; }
        }
         public int ObjectType
        {
            get { return this.m_OBJType; }
            set { m_OBJType = value; }
        }
        public uint ObjectID
        {
            get { return this.m_ObjID; }
            set { m_ObjID = value; }
        }
        public uint OwnerID
        {
            get { return this.m_idOwner; }
            set { m_idOwner = value; }
        }
        public uint MonsterID
        {
            get { return this.m_MonsterID; }
            set { m_MonsterID = value; }
        }
        

        //数据
        private int m_OBJType;		//ItemBox的类型
        private uint m_ObjID;		//物品的ObjID
        private uint m_idOwner;		//物品主人的ObjID,或者组队的ID
        private uint m_MonsterID;	//产生掉落包的怪物id
        private WORLD_POS m_posWorld;		//物品的位置

    };
    public class GCNewItemBoxFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new GCNewItemBox(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_GC_NEWITEMBOX; }
        public override int GetPacketMaxSize()
        {
            return WORLD_POS.GetMaxSize() +
                sizeof(int) * 4;
        }
    };
}