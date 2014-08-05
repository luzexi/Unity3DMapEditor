using System;
using System.Collections.Generic;

namespace Interface
{
    public class EquipUpLevelOp : ICompoundOperator
    {
        //暂定
        public const int MAX_LEVEL = 10;

        #region ICompoundOperator

        public static string Name = "UpLevelOp";
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
            oResult = "";
            //所有装备均可升档
            if (mEquip == null)
                return false;

            SCLIENT_PRESCR myPrescr = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Prescr(mPrescrId);
            if (myPrescr == null)
            {
                oResult = "#{NotLearnPrescr}";
                return false;
            }
            //穿戴等级
            _DBC_ITEM_EQUIP_BASE equipInfo = ObjectSystem.EquipDBC.Search_Index_EQU(myPrescr.m_pDefine.nResultID);
            if (equipInfo == null)
            {
                oResult = "NoResultID";
                return false;
            }
           // if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Level() < equipInfo.nLevelRequire)
            {
           //     oResult = "LowLevel";
           //     return false;
            }
            return true;
        }

        public override bool isMaxLevel()
        {
            if(mEquip == null)
                return true;

            if(mEquip.GetCurrentDangCi() >= MAX_LEVEL)
                return true;
            return false;
        }

        public override void setRoleIndex(int index)
        {
            mRoleIndex = index;
        }

        public override bool checkDeplete(out string oResult)
        {
            oResult = null;
            if(mEquip == null)
                throw new Exception("The equip is null: checkDeplete()");

            SCLIENT_PRESCR myPrescr = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Prescr(mPrescrId);
            if (myPrescr == null)
                return true;
            Stuff stuff = new Stuff();
            int nCount = 0;
            for (int i = 1; i < myPrescr.m_pDefine.mStuffs.Length; i++ )
            {
                stuff = myPrescr.m_pDefine.mStuffs[i];
                if (stuff.nID != -1)
                {
                    nCount = CDataPool.Instance.UserBag_CountItemByIDTable(stuff.nID);
                    if (nCount < stuff.nNum)
                    {
                        oResult = "#{NoEnoughStuff}";
                        return false;
                    }
                }
            }

            //金钱
            if (myPrescr.m_pDefine.nBindLingShi <= CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Money())
                return true;
            else
                oResult = "#{Money_Not_Enough}";

            return false;
        }

        public override int checkCoolDown()
        {
            //不会产生冷却时间
            return 0;
        }

        public override float getSucessRate()
        {
            return 100;
        }
        public override void reset()
        {

            mEquip = null;
            mPrescrId = -1;
        }
        public override void setItem(CObject_Item item)
        {
            mEquip = item as CObject_Item_Equip;
            if (mEquip == null)
                throw new Exception("The item must be equip!");

            mPrescrId = getPrescrID(mEquip.GetIdTable());
        }
        public static int getPrescrID(int nTableId)
        {
            Dictionary<int, SCLIENT_PRESCR> myPrescrs = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Prescr();
            if (myPrescrs != null)
            {
                Dictionary<int, SCLIENT_PRESCR>.Enumerator et = myPrescrs.GetEnumerator();
                while (et.MoveNext())
                {
                    SCLIENT_PRESCR prescr = et.Current.Value;
                    if (prescr.m_pDefine.mStuffs[0].nID == nTableId)
                        return prescr.m_pDefine.nID;
                }
            }
            return -1;
        }

        public override void sendOperation()
        {
            if (mEquip == null && mPrescrId != -1)
                return;
            uint nFlag = 0;
            if (mEquip.TypeOwner == ITEM_OWNER.IO_MYSELF_PACKET)
            {
                nFlag = (uint)mEquip.GetPosIndex();
                nFlag = (nFlag << 16) + 1;
            }
            else if(mEquip.TypeOwner == ITEM_OWNER.IO_MYSELF_EQUIP)
            {
                nFlag = (uint)mEquip.GetPosIndex();
                nFlag = (nFlag << 16) + 0;
            }
            else if (mEquip.TypeOwner == ITEM_OWNER.IO_PET_EQUIPT)
            {
                nFlag = 2;
                nFlag = nFlag | (0x0000ff00 & (uint)(mRoleIndex-1) << 8);
                nFlag = nFlag | (0xffff0000 & ((uint)mEquip.GetPosIndex() << 16));
            }
            GameProcedure.s_pGameInterface.Player_UseLifeAbility(mPrescrId, 1, nFlag);
        }

        #endregion


        CObject_Item_Equip mEquip;
        int mRoleIndex = 0;
        private int mPrescrId = -1; // 需求的配方iD [4/9/2012 SUN]
    }
}