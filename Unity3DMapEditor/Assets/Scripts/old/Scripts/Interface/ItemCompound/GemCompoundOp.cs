using System;
using System.Collections.Generic;

using Network;
using Network.Packets;

namespace Interface
{
    public class GemCompoundOp : ICompoundOperator
    {
        public static string Name = "GemCompoundOp";
        public override string GetName()
        {
            return Name;
        }
        public override bool isCanOperated(out string oResult)
        {
            oResult = null;
            if (mStuffs == null || mStuffs.Count < 3)
            {
                oResult = "#{NoEnoughStuff}";
                return false;
            }
            int nType = -1;
            foreach (CObject_Item stuff in mStuffs)
            {
                //是否同种材料
                if(nType == -1)
                    nType = stuff.GetIdTable();
                else if (nType != stuff.GetIdTable())
                {
                    oResult = "#{NotSameType}";
                    return false;
                }
                if (stuff.GetItemTableQuality() >= 9)
                {
                    oResult = "#{MaxLevel}";
                    return false;
                }

            }
            return true;

        }

        public override bool isMaxLevel()
        {
            //在isCanOperated检测
            return false;
        }

        public override bool checkDeplete(out string oResult)
        {
            oResult = null;
            //消耗未定
            return true;
        }

        public override int checkCoolDown()
        {
            return 0;
        }

        public override float getSucessRate()
        {
            string error;
            if (isCanOperated(out error))
            {
                if(mStuffs.Count >= 3)
                    return 25 * (mStuffs.Count - 1);
            }
            return 0;
        }
        public override void reset()
        {
            if(mStuffs != null)
                mStuffs.Clear();
        }
        public override void setItem(CObject_Item item)
        {
            if (item == null)
                throw new NullReferenceException("Stuff is null");
            if (mStuffs == null)
                mStuffs = new List<CObject_Item>();
            mStuffs.Add(item);
        }

        public override void sendOperation()
        {
            int guidPlatform = MacroDefine.INVALID_ID;
            CGGemCompound Msg = new CGGemCompound();
            
            for(byte i = 0; i < CGGemCompound.GEMBAGINDEX; i++)
            {
                if(i < mStuffs.Count && mStuffs[i] != null)
                {
                    Msg.SetGemBagIndex((sbyte)mStuffs[i].GetPosIndex(), i);
                }
                else
                {
                    Msg.SetGemBagIndex(-1, i);
                }
            }

            Msg.PlatformId = guidPlatform;
            NetManager.GetNetManager().SendPacket(Msg);
        }

        List<CObject_Item> mStuffs;
    }
}