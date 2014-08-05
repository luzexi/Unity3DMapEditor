using System;

namespace Interface
{
    public class EquipStrengthenOp : ICompoundOperator
    {
        #region ICompoundOperateor
        public static string Name = "StrengthenOp";
        public override string GetName()
        {
            return Name;
        }
        /// <summary>
        /// 是否可以被强化，一般都是道具的硬条件，例如品质等
        /// </summary>
        /// <param name="oResult">错误信息，如果成功为空</param>
        /// <returns>true 可以升级， false 不可升级</returns>
        public override bool isCanOperated(out string oResult)
        {
            if (mEquip == null)
                throw new NullReferenceException("The equip is null. in " + "EquipStrendthen:isCanOperated()");

            if (mEquip.GetEquipQuantity() == EQUIP_QUALITY.WHITE_EQUIP)
            {
                oResult = " The equip must be at least of green ";
                return false;
            }

            oResult = null;
            return true;
        }

        /// <summary>
        /// 是否已经到达最高等级
        /// </summary>
        /// <returns>true, 已达最高等级</returns>
        public override bool isMaxLevel()
        {
            if (mEquip == null)
                throw new NullReferenceException("The equip is null. in " + "EquipStrendthen:isMaxLevel()");
            if (mEquip.GetStrengthLevel() >= CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level())
                return true;
            else
                return false;
        }

        /// <summary>
        /// 消耗道具是否足够
        /// </summary>
        /// <param name="oResult">失败时返回缺少道具</param>
        /// <returns>true，消耗品足够</returns>
        public override bool checkDeplete(out string oResult)
        {
            oResult = null;
            if (mEquip == null)
                throw new Exception("The equip is null: checkDeplete()");

            int enhanceID = mEquip.GetStrengthIndex();
            _DBC_ITEM_ENHANCE pItemEnhance = ObjectSystem.EquipEnchanceDBC.Search_Index_EQU((int)enhanceID);
            if (pItemEnhance == null)
                return true;
            if (pItemEnhance.needMoney <= CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money())
                return true;
            else
                oResult = "#{Money_Not_Enough}";

            return true;
        }

        /// <summary>
        /// 冷却时间是否结束
        /// </summary>
        /// <returns>返回当前最短冷却时间，单位毫秒</returns>
        public override int checkCoolDown()
        {
            return 0;
        }

        /// <summary>
        /// 获取成就几率
        /// </summary>
        /// <returns></returns>
        public override float getSucessRate()
        {
            if (mEquip == null)
                throw new Exception("The equip is null: getSucessRate()");

            int enhanceID = mEquip.GetStrengthIndex();
            _DBC_ITEM_ENHANCE pItemEnhance = ObjectSystem.EquipEnchanceDBC.Search_Index_EQU((int)enhanceID);
            if (pItemEnhance != null)
                return pItemEnhance.successRate;

            return 0;
        }
        public override void reset()
        {
            mEquip = null;
        }

        /// <summary>
        /// 指定操作道具
        /// </summary>
        /// <param name="item"></param>
        public override void setItem(CObject_Item item)
        {
            if (item == null)
                throw new NullReferenceException("The equip is null. in " + "EquipStrendthen:setItem()");
            mEquip = null;

            if (item is CObject_Item_Equip)
                mEquip = (CObject_Item_Equip)item;

            //todo: notify update equip
        }
        /// <summary>
        /// 向服务器发包
        /// </summary>
        public override void sendOperation()
        {
            if (mEquip == null)
                throw new NullReferenceException("The equip is null. in " + "EquipStrendthen:sendOperation()");
            if (mEquip.TypeOwner == ITEM_OWNER.IO_MYSELF_EQUIP)
                StrengthenEquip(mEquip.GetPosIndex(), 0);
            else if (mEquip.TypeOwner == ITEM_OWNER.IO_MYSELF_PACKET)
                StrengthenEquip(mEquip.GetPosIndex(), 1);
            else if (mEquip.TypeOwner == ITEM_OWNER.IO_PET_EQUIPT)
                StrengthenEquip(mEquip.GetPosIndex(), 2);
        }
        #endregion

        // temp fix for test [2/13/2012 SUN]
        public void StrengthenEquip(short index, byte inBag)
        {
            UIInfterface.Instance.Clear_XSCRIPT();
            UIInfterface.Instance.Set_XSCRIPT_Function_Name("FinishEnhance");
            UIInfterface.Instance.Set_XSCRIPT_ScriptID(809262);
            UIInfterface.Instance.Set_XSCRIPT_Parameter(0, index);
            UIInfterface.Instance.Set_XSCRIPT_Parameter(1, inBag);
            UIInfterface.Instance.Set_XSCRIPT_Parameter(2, mRoleIndex-1);
            UIInfterface.Instance.Set_XSCRIPT_ParamCount(3);
            UIInfterface.Instance.Send_XSCRIPT();

        }

        public override void setRoleIndex(int index)
        {
            mRoleIndex = index;
        }

        private CObject_Item_Equip mEquip;
        int mRoleIndex = 0;
    }
}