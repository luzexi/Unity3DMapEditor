using System;

using Network;
using Network.Packets;

namespace Interface
{
    public class EquipEnchaseGemOp : ICompoundOperator
    {
        public static string Name = "EnchaseGem";
        public override string GetName()
        {
            return Name;
        }
        public override bool isCanOperated(out string oResult)
        {
            oResult = null;
            //检测宝石孔是否具备
            if (mEquip == null)
                throw new Exception("Equip is Null! in EquipEnchaseGemOp.isCanOperated()");

            if (mGemIndex.Length != mEquip.GetGemMaxCount())
            {
                oResult = "Gem number is wrong";
                return false;
            }

            return true;
        }

        public override bool isMaxLevel()
        {
            //任意等级宝石都可镶嵌
            return false;
        }

        public override bool checkDeplete(out string oResult)
        {
            oResult = null;
            if (mEquip == null) 
                throw new NullReferenceException("equip is null. in EquipEnchaseGemOp.checkDeplete()");

            //金钱消耗
            //TODO:

            return true;
        }

        public override int checkCoolDown()
        {
            return 0;
        }

        public override float getSucessRate()
        {
            return 100;
        }
        public override void reset()
        {
            mEquip = null;
            mGem = null;
            mGemIndex = null;
        }

        public override void setItem(CObject_Item item)
        {
            if (ObjectSystem.Instance.IsGemItem(item.GetIdTable()))
                mGem = item as CObject_Item_Gem;
            else if (ObjectSystem.Instance.IsEquip(item.GetIdTable()))
                mEquip = item as CObject_Item_Equip;
            else
                throw new Exception("item type is error!");

        }

        public override void setRoleIndex(int index)
        {
            mRoleIndex = index;
        }
    
        public override void sendOperation()
        {
            if (mEquip == null )
                return;

            CGUseGem msg = new CGUseGem();

            msg.EquipBagIndex = (byte)mEquip.GetPosIndex();
            if (mEquip.TypeOwner == ITEM_OWNER.IO_MYSELF_EQUIP)
            {
                msg.isInBag = 0;
            }
            else if (mEquip.TypeOwner == ITEM_OWNER.IO_MYSELF_PACKET)
            {
                msg.isInBag = 1;
            }
            else if (mEquip.TypeOwner == ITEM_OWNER.IO_PET_EQUIPT)
            {
                msg.isInBag = 2;
                msg.petIndex = (byte)(mRoleIndex - 1);
            }
            msg.SetGemState(mGemIndex);
            NetManager.GetNetManager().SendPacket(msg);
        }

        public void setEnchaseIndex(short[] indexs)
        {
            mGemIndex = indexs;
        }

        CObject_Item_Equip mEquip;
        CObject_Item_Gem mGem;
        int mRoleIndex = 0;
        short[] mGemIndex;
    }
}