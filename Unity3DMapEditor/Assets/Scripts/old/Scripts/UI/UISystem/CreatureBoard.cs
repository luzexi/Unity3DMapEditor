using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBoard
{
    SpriteText spritText;
    GameObject goPos;
    GameObject logicNode = new GameObject();
    GameObject phyNode = new GameObject();

    const float OrgTextSize = 7;
    public CreatureBoard(int id)
    {
        goPos = UnityEngine.Object.Instantiate(UIWindowMng.Instance.CaptionTextPrefab,
            Vector3.zero, new Quaternion(0, 0, 0, 0)) as GameObject;
        if (goPos == null)
        {
            LogManager.Log("Create UI failed: " + "UI_Name_" + id);
            return;
        }
        goPos.name = "UI_Name_" + id;

        logicNode.name = "LogicNode_" + id;
        logicNode.transform.parent = goPos.transform;
        logicNode.transform.localPosition = Vector3.zero;
        phyNode.name = "PhyNode_" + id;
        phyNode.transform.parent = goPos.transform;
        phyNode.transform.localPosition = Vector3.zero;


        spritText = goPos.GetComponent<SpriteText>();

        InitialStateWins();

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PLAY_DIALOGUE, PlayDialogue);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_STOP_DIALOGUE, PlayDialogue);
    }

    public void PlayDialogue(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_PLAY_DIALOGUE)
        {
            goPos.layer = LayerManager.DialogueLayer;
        }
        else
        {
            goPos.layer = LayerManager.UILayer;
        }
    }

    internal void Reset()
    {
        m_nDistance = 0.0f;
        serverId = -1;

        showAllWindows = true;

        roleName = "";
        showRoleName = false;
        spritText.Text = "";

        currentIndex = 0;
        preXPos = 0;

        HideAllState();
    }

    //删除自身
    public void Destroy()
    {
        UICreatureBoardSystem.Instance.DestroyCreatureBoard(this);
    }

    Vector3 screenPos = Vector3.zero;
    //设置在屏幕上的坐标(像素)
    public void SetPosition(Vector3 pos)
    {
        if (m_nDistance > UICreatureBoardSystem.Instance.MaxDispearDistance)
        {
            //showAllWindows = false;
            Show(false);
            return;
        }
        else
            Show(true);
        
        screenPos = pos;

        pos.x -= nameHalfWidth;
        goPos.transform.position = pos;
    }

    public void SetPosition(Vector2 pos2)
    {
        Vector3 pos3 = new Vector3(pos2.x, pos2.y, UIZDepthManager.NameZ);
        SetPosition(pos3);
    }

    public Vector3 GetPosition()
    {
        return screenPos;
    }

    //显示窗口(用于自身管理是否显示，比如超出界限就自动隐藏)
    bool showAllWindows = true;
    //显示/隐藏(用于逻辑控制是否显示)
    bool m_bShow = false;
    public bool isShow()
    {
        return m_bShow ? true : false;
    }
    public void Show(bool bShow)
    {
        if (bShow == m_bShow)
            return;

        if (bShow /*&& roleName.Length != 0*/)
        {
            showRoleName = true;
            m_bShow = true;
            goPos.active =(true);
            logicNode.SetActiveRecursively(true);
        }
        else
        {
            showRoleName = false;
            m_bShow = false;
            goPos.active = (false);
            logicNode.transform.root.gameObject.SetActiveRecursively(false);
            //HideAllState();
        }

        ClearUnusedFightInfo();

        UpdateExtraEffect();
        ShowCurrentState();
    }

    GameObject extraEffect = null;
    /// <summary>
    /// 检查是否有额外的特效需要显示
    /// </summary>
    private void UpdateExtraEffect()
    {
        if (m_bShow == false || serverId == -1)
        {
            HideEffect();
        }
        else
        {
            CObject_PlayerNPC npc = CObjectManager.Instance.FindServerObject(serverId) as CObject_PlayerNPC;
            if (npc != null )
            {
                if (npc.GetExtraEffectName() == "")
                    HideEffect();
                else
                    ShowEffect(npc.GetExtraEffectName(), npc.GetEffectHeight());
                
            }
        }
    }

    float effectPosY;
    private void ShowEffect(string effectName,float height)
    {
        if (extraEffect != null)
        {
            if (extraEffect.name == effectName)
            {
                extraEffect.SetActiveRecursively(true);
                return;
            }
            else
            {
                GameObject.DestroyImmediate(extraEffect);
            }
        }

        extraEffect = UnityEngine.Object.Instantiate(UIWindowMng.Instance.GetObjFromCommon(effectName),
               Vector3.zero, Quaternion.identity) as GameObject;
        extraEffect.name = effectName;
        extraEffect.active = true;
        extraEffect.transform.parent = phyNode.transform;
        extraEffect.transform.localPosition = new Vector3(nameHalfWidth, nameHeight, 0.1f);
        effectPosY = nameHeight + height;
    }

    void HideEffect()
    {
        if (extraEffect != null)
        {
            extraEffect.active = false;
        }
    }

    bool IsEffectShowing()
    {
        if (extraEffect != null && extraEffect.active == true)
            return true;
        
        return false;
    }

    //设置名字
    bool showRoleName;
    public bool ShowRoleName
    {
        get { return showRoleName; }
        set { showRoleName = value; }
    }

    float nameHalfWidth = 0;
    float nameHeight = 0;
    string roleName = "";
    public void SetElement_Name(string szName)
    {
        if (roleName != szName)
        {
            roleName = szName;

            if (roleName.Length != 0)
                ShowRoleName = true;

            if (spritText != null)
            {
                spritText.Text = roleName;

                nameHalfWidth = (goPos.renderer.bounds.max.x - goPos.renderer.bounds.min.x) / 2;
                nameHeight = (goPos.renderer.bounds.max.y - goPos.renderer.bounds.min.y);
            }
        }
    }

    List<UIButton> infos = new List<UIButton>();
    static int currentIndex = 0;
    static float preXPos = 0;
    UIButton GetNextInfoWin()
    {
        if (currentIndex > 0)
            preXPos += infos[currentIndex - 1].renderer.material.mainTexture.width;

        if (currentIndex < infos.Count)
        {
            infos[currentIndex].gameObject.transform.localPosition = new UnityEngine.Vector3(preXPos, 0, 0);
            return infos[currentIndex++];
        }

        GameObject go = new GameObject(goPos.name);
        go.layer = LayerManager.UILayer;
        go.transform.parent = logicNode.transform;
        go.transform.localPosition = new Vector3(preXPos, 0, 0);
        UIButton infoWin = go.AddComponent<UIButton>();
        infoWin.pixelPerfect = true;
        infoWin.autoResize = true;
        infoWin.SetUVs(new Rect(0, 0, 1, 1));
        infoWin.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
        go.renderer.material.shader = Shader.Find("Sprite/Vertex Colored, Fast");

        infos.Add(infoWin);
        return infos[currentIndex++];
    }

    void ClearUnusedFightInfo()
    {
        for (int i = currentIndex; i < infos.Count; i++)
        {
            infos[i].active = false;
        }
    }

    public void SetFightInfo(ENUM_DMG_MOVE_TYPE moveType, ENUM_DAMAGE_TYPE dmgType, string dmgText)
    {
        if (dmgType == ENUM_DAMAGE_TYPE.DAMAGE_NORMAL || dmgType == ENUM_DAMAGE_TYPE.DAMAGE_CRITICAL)
        {
            string exName = "hui-";
            switch (moveType)
            {
                case ENUM_DMG_MOVE_TYPE.MOVE_STATUS:
                    break;
                case ENUM_DMG_MOVE_TYPE.MOVE_DAMAGE_OTHER:
                    exName = "hui-";
                    break;
                case ENUM_DMG_MOVE_TYPE.MOVE_DAMAGE_ME:
                    exName = "hong-";
                    break;
                case ENUM_DMG_MOVE_TYPE.MOVE_HEAL_HP:
                    exName = "lv-";
                    break;
                case ENUM_DMG_MOVE_TYPE.MOVE_HEAL_MP:
                    exName = "zi-";
                    break;
                case ENUM_DMG_MOVE_TYPE.MOVE_SCENE_NAME:
                    break;
                default:
                    break;
            }

            if (dmgType == ENUM_DAMAGE_TYPE.DAMAGE_CRITICAL)
            {
                UIButton win = GetNextInfoWin();
                win.SetTexture(IconManager.Instance.GetIcon("baoji"));
                exName = "huang-";
            }

            char[] nums = dmgText.ToCharArray();
            foreach (char num in nums)
            {
                UIButton win = GetNextInfoWin();
                win.SetTexture(IconManager.Instance.GetIcon(exName + num));
            }
        }
        else if (dmgType == ENUM_DAMAGE_TYPE.DAMAGE_MISS)
        {
            UIButton win = GetNextInfoWin();
            win.SetTexture(IconManager.Instance.GetIcon("weimingzhong"));
        }
        else if (dmgType == ENUM_DAMAGE_TYPE.DAMAGE_Exp)
        {
            UIButton win = GetNextInfoWin();
            win.SetTexture(IconManager.Instance.GetIcon("jingyian"));

            dmgText = "+" + dmgText;
            char[] nums = dmgText.ToCharArray();
            foreach (char num in nums)
            {
                win = GetNextInfoWin();
                win.SetTexture(IconManager.Instance.GetIcon("lan-" + num));
            }
        }
    }

    //设置逻辑对象ID
    int serverId;
    public void SetElement_ObjId(int nObjId)
    {
        serverId = nObjId;
    }

    // 设置离主角的位置，超过一定范围的位置会进行文字变淡
    float m_nDistance = 0;
    public void SetInfoDistance(float nDistance) { m_nDistance = nDistance; }

    //     int m_InfoState;
    //     public void SetInfoState(int nState)
    //     {
    //         m_InfoState = nState;
    //     }

    //CEGUI::Window* GetMainWindow(void) { return m_pWindow; }

    //设置境界
    //public void SetElement_Ambit(string szName);	

    //设置称号
    //public void SetElement_Title(string szTitle, INT nType = 0);

    //设置队长标记
    //public void SetElement_LeaderFlag(bool bShow);

    //设置摆摊上的文字
    //public void SetElement_SaleText(string szSaleText);
    //设置是否显示摆摊信息
    //public void SetElement_SaleSign(bool bShow);

    //public void SetElement_PaoPaoText( string szPaoPao );

    //public void SetElement_PaoPaoText( string szPaoPao, float paopaoLife );

    //public	void ShowPaoPao(void);

    //public void SetTitleType(INT nType) { m_nTitleType = nType; }
    //public void SetBangpaiFlag(INT nBangpaiFlag) { m_nBangpaiFlag = nBangpaiFlag; }

    internal void SetScale(float m_fScale)
    {
        if (goPos == null)
            return;
        Vector3 newScale = new Vector3(m_fScale, m_fScale, 1);
        goPos.transform.localScale = newScale;
    }

    internal void SetTextColor(Color m_ColorType)
    {
        spritText.Color = m_ColorType;
    }

    internal void SetAlpha(float m_fAlpha)
    {
        // TODO
    }

    GameObject acceptWin;
    GameObject continueWin;
    GameObject finishWin;
    void InitialStateWins()
    {
        Vector3 missPos = new Vector3(0, 0, 0.1f);
        _DBC_EFFECT stateName = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_EFFECT>(
            (int)DataBaseStruct.DBC_EFFECT).Search_Index_EQU((int)NpcMissState.AcceptNormal);
        if (stateName != null)
        {
            acceptWin = UnityEngine.Object.Instantiate(UIWindowMng.Instance.GetObjFromCommon(stateName.effectName),
               Vector3.zero, Quaternion.identity) as GameObject;
            acceptWin.name = "UI_Miss_Name_" + stateName.effectName;

            acceptWin.transform.parent = phyNode.transform;
            acceptWin.transform.localPosition = missPos;
        }
        else
        {
            LogManager.LogError("没有在Effect.txt配置可接任务特效名字");
        }

        stateName = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_EFFECT>(
            (int)DataBaseStruct.DBC_EFFECT).Search_Index_EQU((int)NpcMissState.ContinueNormal);
        if (stateName != null)
        {
            continueWin = UnityEngine.Object.Instantiate(UIWindowMng.Instance.GetObjFromCommon(stateName.effectName),
                Vector3.zero, Quaternion.identity) as GameObject;
            continueWin.name = "UI_Miss_Name_" + stateName.effectName;

            continueWin.transform.parent = phyNode.transform;
            continueWin.transform.localPosition = missPos;
        }
        else
        {
            LogManager.LogError("没有在Effect.txt配置执行中任务特效名字");
        }

        stateName = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_EFFECT>(
            (int)DataBaseStruct.DBC_EFFECT).Search_Index_EQU((int)NpcMissState.FinishNormal);
        if (stateName != null)
        {
            finishWin = UnityEngine.Object.Instantiate(UIWindowMng.Instance.GetObjFromCommon(stateName.effectName),
                Vector3.zero, Quaternion.identity) as GameObject;
            finishWin.name = "UI_Miss_Name_" + stateName.effectName;

            finishWin.transform.parent = phyNode.transform;
            finishWin.transform.localPosition = missPos;
        }
        else
        {
            LogManager.LogError("没有在Effect.txt配置可交任务特效名字");
        }

        HideAllState();
    }
    void HideAllState()
    {
        acceptWin.SetActiveRecursively(false);
        continueWin.SetActiveRecursively(false);
        finishWin.SetActiveRecursively(false);

        currentMissTagWin = null;
    }

    GameObject currentMissTagWin;
    /// <summary>
    /// 显示NPC的任务信息
    /// </summary>
    /// <param name="currMissState"></param>
    internal void ShowMissState(NpcMissState currMissState)
    {
        HideAllState();
        if (currMissState != NpcMissState.None)
        {
            switch (currMissState)
            {
                case NpcMissState.AcceptNormal:
                    currentMissTagWin = acceptWin;
                    break;
                case NpcMissState.FinishNormal:
                    currentMissTagWin = finishWin;
                    break;
                case NpcMissState.ContinueNormal:
                    currentMissTagWin = continueWin;
                    break;
                default:
                    break;
            }
            //currentMissTagWin.SetActiveRecursively(true);
            ShowCurrentState();
        }
    }

    private void ShowCurrentState()
    {
        if (currentMissTagWin != null && logicNode.active != false)
        {
            currentMissTagWin.transform.localPosition = new Vector3(nameHalfWidth, IsEffectShowing() ? effectPosY : nameHeight, 0.1f);
            currentMissTagWin.SetActiveRecursively(true);
        }
    }
}