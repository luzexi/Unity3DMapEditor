using System;
using System.Collections.Generic;

// namespace SGWEB
// {
    public enum ACTION_OPTYPE	//类型
    {
        AOT_EMPTY = 0,					//-空-

        AOT_SKILL,					//战斗技能
        AOT_SKILL_LIFEABILITY,		//生活技能
        AOT_ITEM,					//物品
        AOT_XINFA,					//心法
        AOT_PET_SKILL,				//宠物技能
        AOT_EQUIP,					//装备
        AOT_CHATMOOD,				//聊天动作
        AOT_MOUSECMD_REPARE,		//鼠标指令_修理
        AOT_MOUSECMD_IDENTIFY,		//鼠标指令_鉴定
        AOT_MOUSECMD_ADDFRIEND,		//鼠标指令_加为好友
        AOT_MOUSECMD_EXCHANGE,		//鼠标指令_加为好友
        AOT_MOUSECMD_CATCHPET,		//鼠标指令_捉宠
        AOT_MOUSECMD_SALE,			//鼠标指令_卖物品
        AOT_MOUSECMD_BUYMULT,		//鼠标指令_买多个物品
        AOT_HOTKEY_MODIFY,			// 修改自定义快捷键 [8/26/2011 edit by ZL]
		AOT_TALISMAN,//法宝
    };

    //显示字符, cChar所显示的字符，0不显示
    public enum CORNER_NUMBER_POS
    {
        ANP_TOPLEFT = 0,
        ANP_TOPRIGHT,
        ANP_BOTLEFT,
        ANP_BOTRIGHT,
    };

    public interface tActionReference
    {
        //逻辑Action消失
        void BeDestroyed();
        //数据更新
        void UpdateRef(int id);
        //按钮按下
        void SetCheck(bool bCheck);
        //按钮设置成default的状态
        void SetDefault(bool bDefault);
        //进入冷却
        void EnterCoolDown(int fTime, float fPercent);
        void SetCornerChar(CORNER_NUMBER_POS pos, string szChar);

        void Disable();
        void Enable();
    };

    public abstract class tActionItem
    {
        //得到id
        public abstract int GetID();
        //得到名称
        public abstract string GetName();
        //得到图标
        public abstract string GetIconName();
        //设置Check状态
        public abstract void SetCheckState(bool bCheck);
        //添加引用
        public abstract void AddReference(tActionReference pRef, bool bIsInMenuToolbar);
        //断开引用
        public abstract void RemoveRefrence(tActionReference pRef);

        //销毁引用的实例
        public abstract void DestroyImpl();

        public abstract new ACTION_OPTYPE GetType();
        //类型字符串
        public abstract ActionNameType GetType_String();
        //得到定义ID
        /*
        |	对于战斗技能, 是技能表中的ID (DBC_SKILL_DATA)
        |	对于生活技能，是生活技能表中的ID(DBC_LIFEABILITY_DEFINE)
        |	对于物品，是物品表中的ID(DBC_ITEM_*)
        |	对于心法，是心法表中的ID(DBC_SKILL_XINFA)
        */
        public abstract int GetDefineID();
        //得到数量
        public abstract int GetNum();
        //得到内部数据
        public abstract object GetImpl();
        //得到解释
        public abstract string GetDesc();
        //得到冷却状组ID
        public abstract int GetCoolDownID();
        //得到所在容器的索引
        public abstract int GetPosIndex();
        //激活动作
        public abstract void DoAction();
        //激活动作(副操作)
        public abstract void DoSubAction();
        //是否有效
        public abstract bool IsValidate();
        // Is Enabled
        public abstract bool IsEnabled();
        //Enable
        public abstract void Enable();
        //Disable
        public abstract void Disable();
        //检查冷却是否结束
        public abstract bool CoolDownIsOver();
        //拖动结束
        public abstract void NotifyDragDropDragged(bool bDestory, string szTargetName, string szSourceName);
        //显示tooltips
        public abstract void NotifyTooltipsShow();
        //隐藏tooltips
        public abstract void NotifyTooltipsHide();
        //显示tooltips2
        public abstract void NotifyTooltips2Show();
        //隐藏tooltips2
        public abstract void NotifyTooltips2Hide();
        //查询逻辑属性
        public abstract string GetAttributeValue(string szAttributeName);
        //用于聊天界面超链接的字符串
        //public abstract string GenHyperLinkString();
    };

    public abstract class tActionSystem
    {
        //根据id取得Action
        public abstract tActionItem GetAction(int id);
        //当前正在显示Tootips的按钮
        public abstract tActionItem GetTooltipsFocus();

        public abstract void SaveAction();
        //得到缺省操作
        public abstract tActionItem GetDefaultAction();
        //设置缺省操作
        public abstract void SetDefaultAction(tActionItem pDefAction);

        public abstract void UpdateLinkByItemTable(int nIdTable);
    }
/*}*/
