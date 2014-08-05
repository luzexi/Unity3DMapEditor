
using System;
using System.Collections.Generic;
/// <summary>
/// 装备合成管理器
/// </summary>
namespace Interface
{
    public class CompoundManager
    {

        Dictionary<string, ICompoundOperator> mOperators = new Dictionary<string, ICompoundOperator>();
        ICompoundOperator mCurrentOperator;
        int mRoleIndex;

        public ICompoundOperator CurrentOperator
        {
            get
            {

                return mCurrentOperator;
            }

        }

        public void InitializeOperator()
        {
            RegisterOperator(EquipStrengthenOp.Name, new EquipStrengthenOp());
            RegisterOperator(EquipUpLevelOp.Name, new EquipUpLevelOp());
            RegisterOperator(EquipEnchaseGemOp.Name, new EquipEnchaseGemOp());
            RegisterOperator(GemCompoundOp.Name, new GemCompoundOp());
        }

        public void RegisterOperator(string name, ICompoundOperator operatorAction)
        {
            if (mOperators.ContainsKey(name))
                throw new Exception("The operator: " + name + "was exist! ");
            if (operatorAction == null)
                throw new NullReferenceException("The operator is null. in " + "CompoundManager:RegisterOperator()");
            mOperators[name] = operatorAction;

        }
        public void setOperator(string name)
        {
            ICompoundOperator op;
            if (!mOperators.TryGetValue(name, out op))
            {
                throw new NullReferenceException("can not find " + name + "operator!");
            }
            mCurrentOperator = op;
            mCurrentOperator.reset();
        }
        public void setOperatorTarget(CObject_Item item)
        {
            if (CurrentOperator != null)
                CurrentOperator.setItem(item);
        }
       
        public void setOperatorRole(int index)
        {
            mRoleIndex = index;
        }

        public void setOperatorNull()
        {
            mCurrentOperator = null;
        }
        public void setEnchaseGemPos(short[] pos)
        {
            if (CurrentOperator != null)
            {
                if (CurrentOperator.GetName() == EquipEnchaseGemOp.Name)
                {
                    ((EquipEnchaseGemOp)CurrentOperator).setEnchaseIndex(pos);
                }
            }
        }


        //检测当前的操作条件是否满足
        public bool checkOperator(out string oResult)
        {
            oResult = "";
            if(mCurrentOperator == null)
                throw new NullReferenceException("Current Operator is null. in " + "CompoundManager:checkOperator()");
            if (!mCurrentOperator.isCanOperated(out oResult))
                return false;
            if (mCurrentOperator.checkCoolDown() > 0)
            {
                oResult = "is cooling down";
                return false;
            }
            if (mCurrentOperator.checkDeplete(out oResult))
                return false;

            return true;
        }

        public string beginOperation()
        {
            if (CurrentOperator == null)
                throw new NullReferenceException("Current Operator is null. in " + "CompoundManager:sendOperation()");
            string error = null;
            CurrentOperator.setRoleIndex(mRoleIndex);
            CurrentOperator.beginOperate(ref error);

            return error;
        }

    }
}