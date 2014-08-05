
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Network;

namespace Network.Packets
{


    public class CGExecuteScript : PacketBase
    {

        //公用继承接口
        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            return true;
        }
        public override int writeToBuff(ref NetOutputBuffer buff)
        {

            x_Script.writeToBuff(ref buff);

            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_CG_EXECUTESCRIPT;
        }
        public override int getSize()
        {
            return x_Script.getSize();
        }

        //public interface
       public X_SCRIPT x_Script
       {
           get {
               if (x_script == null)
                   throw new NullReferenceException("The Script is null : CGExecuteScript");
               return x_script; 
           }
           set
           {
               x_script = value;
           }
       }

        //数据
        private X_SCRIPT x_script;

    };
    public class CGExecuteScriptFactory : PacketFactory
    {
        public override PacketBase CreatePacket() { return new CGExecuteScript(); }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_CG_EXECUTESCRIPT; }
        public override int GetPacketMaxSize()
        {
            return X_SCRIPT.getMaxSize();
        }
    };
}