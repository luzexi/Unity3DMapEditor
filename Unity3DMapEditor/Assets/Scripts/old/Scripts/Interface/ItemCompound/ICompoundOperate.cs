//////////////////////////////////////////////////////////////////////////
//提供用于装备进化的接口
namespace Interface
{
    public abstract class ICompoundOperator
    {

        /// <summary>
        /// 是否可以被强化，一般都是道具的硬条件，例如品质等
        /// </summary>
        /// <param name="oResult">错误信息，如果成功为空</param>
        /// <returns>true 可以升级， false 不可升级</returns>
        public abstract bool isCanOperated(out string oResult);

        /// <summary>
        /// 是否已经到达最高等级
        /// </summary>
        /// <returns>true, 已达最高等级</returns>
        public abstract bool isMaxLevel();

        /// <summary>
        /// 消耗道具是否足够
        /// </summary>
        /// <param name="oResult">失败时返回缺少道具</param>
        /// <returns>true，消耗品足够</returns>
        public abstract bool checkDeplete(out string oResult);

        /// <summary>
        /// 冷却时间是否结束
        /// </summary>
        /// <returns>返回当前最短冷却时间，单位毫秒</returns>
        public abstract int checkCoolDown();

        /// <summary>
        /// 获取成就几率
        /// </summary>
        /// <returns></returns>
        public abstract float getSucessRate();

        /// <summary>
        /// 指定操作道具
        /// </summary>
        /// <param name="item"></param>
        public abstract void setItem(CObject_Item item);

        /// <summary>
        /// 开始操作
        /// </summary>
        public virtual void beginOperate(ref string Error)
        {
            //检测前提条件
            //string error = null;
            if (!isCanOperated(out Error))
            {
                return;
            }
            if (isMaxLevel())
            {
                Error = "#{MaxLevel}";
                return;
            }
            if (!checkDeplete(out Error))
            {
                return;
            }
            if (checkCoolDown() > 0)
            {
                Error = "#{CoolDown}";
                return;
            }

            //发送消息
            sendOperation();
        }
        //设置操作人物，还是宠物，人物 index 为0 ，其余为宠物
        public virtual void setRoleIndex(int index)
        {

        }

        /// <summary>
        /// 向服务器发包
        /// </summary>
        public abstract void sendOperation();

        /// <summary>
        /// 名字
        /// </summary>
        /// <returns></returns>
        public abstract string GetName();

        public abstract void reset();
    }
}