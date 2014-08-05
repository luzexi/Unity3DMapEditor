using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Interface;

public sealed class TextRectCheck
{
    public string Text { get; set; }
    public uint CharStart { get; set; }
    public uint CharLength { get; set; }
    public bool CheckPosIn(uint charPos)
    {
        if (charPos >= CharStart && charPos <= (CharStart + CharLength))
            return true;
        else
            return false;
    }
}

public abstract class HyperItemBase
{
    public string Name { get; set; }
    public TextRectCheck textCheck = new TextRectCheck();

    public bool CheckHover(uint clickCharPos)
    {
        if (textCheck.CheckPosIn(clickCharPos))
        {
            return true;
        }
        return false;
    }

    public virtual void Click() { }
}

public class HyperPos : HyperItemBase
{
    public int SceneId { get; set; }
    public Vector3 PosTarget { get; set; }

    public override void Click()
    {
        LogManager.LogWarning("HyperPos " + PosTarget);
        GameInterface.Instance.AutoHitState = 0;
        AutoReleaseSkill.Instance.SetTargetObject(-1);
        if (SceneId >= 0)
            GameInterface.Instance.Player_MoveTo(SceneId, PosTarget);
        else
            GameInterface.Instance.Player_MoveTo(PosTarget);
    }
}

public class HyperNpc : HyperPos
{
    public int NpcId { get; set; }
    public override void Click()
    {
        //LogManager.LogWarning("HyperNpc " + NpcId);
        GameInterface.Instance.AutoHitState = 0;
        AutoReleaseSkill.Instance.SetTargetObject(-1);//取消自动释放技能
        int currentScene = GameProcedure.s_pWorldManager.GetActiveSceneID();
        //同场景寻路
        if (currentScene == SceneId)
        {
            // 当鼠标点击超链接时候，显示寻路中（临时方案，具体等策划案子） [9/1/2011 edit by ZL]
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_AUTO_RUNING_CHANGE,1);
            
            GameInterface.Instance.Player_MoveTo(PosTarget);

            GameInterface.Instance.Player_AddCMDAfterMove(CMD_AFTERMOVE_TYPE.CMD_AFMV_SPEAK, NpcId
                , new Vector2(PosTarget.x, PosTarget.z));
        }
        else// 跨场景和npc对话 [3/14/2012 Ivan]
        {
            GameInterface.Instance.Player_DoingAfterMoveScene(SceneId, PosTarget, this);
        }
    }
}
public class HyperTripper : HyperPos
{
    public int TripperID { get; set; }
    public override void Click()
    {
        int currentScene = GameProcedure.s_pWorldManager.GetActiveSceneID();
        //同场景寻路
        if (currentScene == SceneId)
        {
            // 当鼠标点击超链接时候，显示寻路中（临时方案，具体等策划案子） [9/1/2011 edit by ZL]
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_AUTO_RUNING_CHANGE, 1);

            GameInterface.Instance.Player_MoveTo(PosTarget);

            GameInterface.Instance.Player_AddCMDAfterMove(CMD_AFTERMOVE_TYPE.CMD_AFMV_Tripper_ACTIVE, TripperID
                , new Vector2(PosTarget.x, PosTarget.z));
        }
        else// 跨场景和npc对话 [3/14/2012 Ivan]
        {
            GameInterface.Instance.Player_DoingAfterMoveScene(SceneId, PosTarget, this);
        }
    }
}

public class HyperAttack : HyperPos
{
    public int TargetId { get; set; }
    public override void Click()
    {
        GameInterface.Instance.AutoHitState = 0;//取消自动打怪
        AutoReleaseSkill.Instance.SetTargetObject(-1);//取消自动释放
        int currentScene = GameProcedure.s_pWorldManager.GetActiveSceneID();
        //同场景寻路
        if (currentScene == SceneId)
        {
            // 当鼠标点击超链接时候，显示寻路中（临时方案，具体等策划案子） [9/1/2011 edit by ZL]
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_AUTO_RUNING_CHANGE, 1);

            GameInterface.Instance.Player_MoveTo(PosTarget);

            GameInterface.Instance.Player_AddCMDAfterMove(CMD_AFTERMOVE_TYPE.CMD_AFMV_AutoHit, TargetId
                , new Vector2(PosTarget.x, PosTarget.z));
        }
        else// 跨场景杀怪 [3/14/2012 Ivan]
        {
            GameInterface.Instance.Player_DoingAfterMoveScene(SceneId, PosTarget, this);
        }
    }
}

public class HyperItem : HyperItemBase
{
    public int ItemIdTable { get; set; }

    public override void Click()
    {
        // 临时打印
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "点击了物品超链接，物品ID:" + ItemIdTable);
    }
}

public class HyperLink
{
    string fullText;
    public string FullText
    {
        get
        {
            return fullText;
        }
        set
        {
            fullText = UIString.Instance.GetMd5(value);
        }
    }
    public List<HyperItemBase> allItems = new List<HyperItemBase>();

    public HyperItemBase CheckHover(int charIndex)
    {
        foreach (HyperItemBase linkItem in allItems)
        {
            if (linkItem.CheckHover((uint)charIndex))
            {
                return linkItem;
            }
        }
        return null;
    }
}

public class HyperLinkManager
{
    // 更改为单例模式
    static readonly HyperLinkManager instance = new HyperLinkManager();
    private HyperLinkManager() { }
    static public HyperLinkManager Instance { get { return instance; } }

    Dictionary<string, HyperLink> allLinks = new Dictionary<string, HyperLink>();
    
    public void AddHyper(HyperLink link)
    {
        if (link.FullText == "")
            return;
        if (!allLinks.Keys.Contains(link.FullText))
            allLinks.Add(link.FullText, link);
    }

    //public void HyperClick(string showText, int charIndex)
    //{
    //    string dicName = UIString.Instance.GetMd5(showText);

    //    HyperLink link;
    //    if (allLinks.TryGetValue(dicName,out link))
    //    {
    //        link.Click(charIndex);
    //    }
    //}

    internal HyperItemBase GetHyperHover(string showText, int charIndex)
    {
        //删除多余的特殊字符
        showText = showText.Replace("\n", "");
        string dicName = UIString.Instance.GetMd5(showText);

        HyperLink link;
        if (allLinks.TryGetValue(dicName, out link))
        {
            return link.CheckHover(charIndex);
        }
        return null;
    }
}
