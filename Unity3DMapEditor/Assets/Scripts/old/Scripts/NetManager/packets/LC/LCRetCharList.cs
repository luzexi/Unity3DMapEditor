
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
    public class LCRetCharList : PacketBase
    {

         public override short getPacketID()
        {
            return (short)PACKET_DEFINE.PACKET_LC_RETCHARLIST;
        }

         public override int getSize()
        {
            if (Result == NET_RESULT_DEFINE.ASKCHARLIST_RESULT.ASKCHARLIST_SUCCESS)
            {
                return sizeof(NET_RESULT_DEFINE.ASKCHARLIST_RESULT) +
                    sizeof(byte) +
                    CharList[0].getSize()* uCharNumber +
                    sizeof(byte) * NET_DEFINE.MAX_ACCOUNT;
            }
            else
            {
                return sizeof(NET_RESULT_DEFINE.ASKCHARLIST_RESULT);
            }
        }

        public override int writeToBuff(ref NetOutputBuffer buff)
        {
           
            buff.WriteInt((int)Result);

            if (Result == NET_RESULT_DEFINE.ASKCHARLIST_RESULT.ASKCHARLIST_SUCCESS)
            {
                buff.Write(ref szAccount, NET_DEFINE.MAX_ACCOUNT);
                if (uCharNumber > NET_DEFINE.DB_CHAR_NUMBER)
                {
                    int RealNumber = NET_DEFINE.DB_CHAR_NUMBER;
                    buff.WriteByte((byte)RealNumber);
                    for (int i = 0; i < RealNumber; i++)
                    {
                         CharList[i].writeToBuff(ref buff);
                    }
                }
                else
                {
                    buff.WriteByte(uCharNumber);
                    for (byte i = 0; i < uCharNumber; i++)
                    {
                  					
						//buff.WriteStruct(CharList[i]);
                        CharList[i].writeToBuff(ref buff);
                    }
                }
            }
          
            return NET_DEFINE.PACKET_HEADER_SIZE + getSize();
        }

        public override bool readFromBuff(ref NetInputBuffer buff)
        {
            int n = -1;
           // LogManager.Log("Read LCRetCharList");

            if (buff.ReadInt(ref n) != 4) return false;
            result = (NET_RESULT_DEFINE.ASKCHARLIST_RESULT) n;

            if (Result == NET_RESULT_DEFINE.ASKCHARLIST_RESULT.ASKCHARLIST_SUCCESS)
            {
                buff.Read(ref szAccount, sizeof(byte) * NET_DEFINE.MAX_ACCOUNT);
                buff.ReadByte(ref uCharNumber);
                if (uCharNumber > NET_DEFINE.DB_CHAR_NUMBER)
                    uCharNumber = (byte)NET_DEFINE.DB_CHAR_NUMBER;
                if (CharList == null)
                    CharList = new DB_CHAR_BASE_INFO[uCharNumber];

                for (int i = 0; i < uCharNumber; i++)
                {
                    //int size = Marshal.SizeOf(typeof(DB_CHAR_BASE_INFO));
                    //byte[] bytes = new byte[size];
                    //buff.Read(ref bytes, size);
                    //CharList[i] = (DB_CHAR_BASE_INFO)NET_DEFINE.BytesToStruct(bytes, typeof(DB_CHAR_BASE_INFO));
					
		
					CharList[i].readFromBuff(ref buff);
                                
                    //CharList[i] = (DB_CHAR_BASE_INFO)buff.ReadStruct(typeof(DB_CHAR_BASE_INFO) );

                    //LogManager.Log("DBINFO size=" + size);
					
//                    LogManager.Log(" m_GUID:"+BitConverter.ToString( BitConverter.GetBytes(CharList[i].m_GUID) )
//                              +" ,m_Sex:"+BitConverter.ToString( BitConverter.GetBytes(CharList[i].m_Sex) )
//                              );
//					
//					string temp = string.Empty;
//					for(int j=0; j< CharList[i].m_Name.Length ;j++ ){
//						
//						temp += BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_Name[j] ) );
//					}
                    //					LogManager.Log(" ,name:"+ temp );
                    //                    LogManager.Log("m_Level:"+BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_Level ) )
                    //					          +",m_Ambit"+BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_Ambit ) )
                    //					          +",m_HairColor"+BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_HairColor ) )
                    //					          +",m_FaceColor"+BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_FaceColor ) )
                    //					          +",m_HairModel"+BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_HairModel ) )
                    //					          +",m_FaceModel"+BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_FaceModel ) )
                    //					          +" ,m_StartScene:"+BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_StartScene ) )
                    //					          +",m_nClientResID"+BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_nClientResID ) )
                    //					          +",m_Menpai"+BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_Menpai ) )
                    //					          +",m_HeadID"+BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_HeadID ) )
                    //                              +" ,m_Camp:"+BitConverter.ToString( BitConverter.GetBytes( CharList[i].m_Camp ) )
                    //					          );
//					
                    //LogManager.Log(" m_GUID:" + CharList[i].m_GUID
                    //          + " ,m_Sex:" + CharList[i].m_Sex
                    //          + " ,name:" + Encoding.ASCII.GetString(CharList[i].m_Name)
                    //          );
                    //LogManager.Log("m_Level:" + CharList[i].m_Level
                    //          + ",m_Ambit" + CharList[i].m_Ambit
                    //          + ",m_HairColor" + CharList[i].m_HairColor
                    //          + ",m_FaceColor" + CharList[i].m_FaceColor
                    //          + ",m_HairModel" + CharList[i].m_HairModel
                    //          + ",m_FaceModel" + CharList[i].m_FaceModel
                    //          + " ,m_StartScene:" + CharList[i].m_StartScene
                    //          + ",m_nClientResID" + CharList[i].m_nClientResID
                    //          + ",m_Menpai" + CharList[i].m_Menpai
                    //          + ",m_HeadID" + CharList[i].m_HeadID
                    //          + " ,m_Camp:" + CharList[i].m_Camp
                    //          );
					
                }
            }
   
            return true;
        }

        //public interface
        public NET_RESULT_DEFINE.ASKCHARLIST_RESULT Result
        {
            get
            {
                return this.result;
            }
            set{
                result = value;
            }
        }
        public byte CharNumber{
            get{
                return this.uCharNumber;
            }
            set{
                uCharNumber = value;
            }
        }

        public DB_CHAR_BASE_INFO[] CharsList{
            get{
                return this.CharList;
            }
           
        }
        public DB_CHAR_BASE_INFO GetCharBaseInfo(int index)
        {
            if (index >= NET_DEFINE.DB_CHAR_NUMBER)
                index = NET_DEFINE.DB_CHAR_NUMBER - 1;
            return CharList[index];
        }
        public void SetCharBaseInfo(ref DB_CHAR_BASE_INFO info, byte index){
            if(index < NET_DEFINE.DB_CHAR_NUMBER)
            {
                CharList[index] = info;
            }
        }
        public byte[] SzAccount{
            get{
                return this.szAccount;
            }
            set{
                szAccount = value;
            }
        }


        private NET_RESULT_DEFINE.ASKCHARLIST_RESULT			result;
		private byte						                    uCharNumber;
		private DB_CHAR_BASE_INFO[]			                    CharList = new DB_CHAR_BASE_INFO[NET_DEFINE.DB_CHAR_NUMBER];
		private byte[]						                    szAccount = new byte[NET_DEFINE.MAX_ACCOUNT+1] ;	//用户名称		
    }


    public class LCRetCharListFactory : PacketFactory
    {
        public override PacketBase CreatePacket() {
            //LogManager.Log("Create Msg LCRetCharList");
            return new LCRetCharList(); 
        }
        public override int GetPacketID() { return (short)PACKET_DEFINE.PACKET_LC_RETCHARLIST; }
        public override int GetPacketMaxSize()
        {
            int size =sizeof(NET_RESULT_DEFINE.ASKCHARLIST_RESULT) +
                    sizeof(byte) +
                    DB_CHAR_BASE_INFO.getMaxSize() * NET_DEFINE.DB_CHAR_NUMBER +
                    sizeof(byte) * NET_DEFINE.MAX_ACCOUNT;
            //LogManager.Log("PacketSize=" + size);
            return size;
        }
    };
}