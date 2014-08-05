using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : Window, tActionReference
{
    CActionItem currActionItem = null;
    public CActionItem CurrActionItem
    {
        get { return currActionItem; }
        set { currActionItem = value; }
    }

    PackedSprite coolDownSprite;
    bool isInCooldown = false;
    public override void Initial()
    {
        coolDownSprite = gameObject.GetComponentInChildren<PackedSprite>();
        if (coolDownSprite == null)
        {
            UnityEngine.Object cooldown = UIWindowMng.Instance.GetObjFromCommon("Cooldown");
            if (cooldown != null)
            {
                GameObject cooldownGo = UnityEngine.Object.Instantiate(cooldown,
                                        Vector3.zero, new Quaternion(0, 0, 0, 0)) as GameObject;
                cooldownGo.transform.parent = gameObject.transform;
                cooldownGo.transform.localPosition = new Vector3(0, 0, -0.1f);
                coolDownSprite = cooldownGo.GetComponent<PackedSprite>();
            }
            else
            {
                LogManager.LogError("Can not found Cooldown Prefab.");
                return;
            }
        }

        coolDownSprite.SetAnimCompleteDelegate(AnimEndDelegate);
        coolDownSprite.RenderCamera = UISystem.Instance.UiCamrea;

        if (ezButton != null)
            ezButton.AddInputDelegate(DragDelegate);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_SHORTKEY, ShortKeyClick);

        ClearAction();
    }

    KeyCode regKeyCode = KeyCode.None;
    public void RegistShortKey(KeyCode key,string showName)
    {
        regKeyCode = key;
        SetCornerChar(CORNER_NUMBER_POS.ANP_TOPLEFT, showName);
    }
    public void RegistShortKey(KeyCode key)
    {
        RegistShortKey(key, key.ToString());
    }
    public void DelShortkey()
    {
        regKeyCode = KeyCode.None;
        SetCornerChar(CORNER_NUMBER_POS.ANP_TOPLEFT, "");
    }

    public void ShortKeyClick(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (vParam.Count == 0)
            return;
        KeyCode key = (KeyCode)Convert.ToInt32(vParam[0]);
        if (regKeyCode == key && currActionItem != null)
        {
            currActionItem.DoAction();
        }
    }

    public string Name()
    {
        return gameObject.name;
    }

    //是否允许执行logic
    public bool enableDoAction = true;

    public bool EnableDoAction
    {
        get { return enableDoAction; }
        set { enableDoAction = value; }
    }


    public void SetActionItem(int itemId)
    {
//         if (itemId == -1)
//         {
//             ClearAction();
//             return;
//         }
        UpdateItem(itemId);
    }

    public void SetActionItemByActionId(int actionId)
    {
        if (actionId == -1)
        {
            ClearAction();
            return;
        }
        CActionItem action = CActionSystem.Instance.GetActionByActionId(actionId);
        if (action == null)
        {
            //LogManager.LogWarning("item is null,id:" + itemId);
            ClearAction();
            return;
        }
        UpdateItemFromAction(action);
    }

    private void ClearAction()
    {
        //断开和原来的Action的联系
        if (currActionItem != null)
        {
            currActionItem.RemoveRefrence(this);
        }
        currActionItem = null;
        ClearAllChar();
        SetIcon("");
        //DisableDrag();

        // 清空的时候关闭Tooltip [3/15/2012 Ivan]
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_SUPERTOOLTIP, "0");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId">该ID对应objectSystem创建新物体NewItem时的ID</param>
    public void UpdateItem(int itemId)
    {
        CActionItem action = CActionSystem.Instance.GetActionByItemId(itemId);
//         if (action == null)
//         {
//             //LogManager.LogWarning("item is null,id:" + itemId);
//             ClearAction();
//             return;
//         }
        UpdateItemFromAction(action);
    }

    public void UpdateItemFromAction(CActionItem action)
    {
        //断开和原来的Action的联系
        if (currActionItem != null && currActionItem != action)
        {
            currActionItem.RemoveRefrence(this);
        }
		
        currActionItem = action;

        if (currActionItem == null)
        {
            ClearAction();
            return;
        }

        //EnableDrag();

        //绑定逻辑和ui
        currActionItem.AddReference(this, false);

        SetIcon(currActionItem.GetIconName());

        // 更新cooldown [3/15/2012 Ivan]
        currActionItem.UpdateCoolDown();

        if (action.GetNum() > 1)
            SetCornerChar(CORNER_NUMBER_POS.ANP_BOTRIGHT, action.GetNum().ToString());
    }

    public void AddInputDelegate(EZInputDelegate inputDelegate)
    {
        if (ezButton != null)
            ezButton.AddInputDelegate(inputDelegate);
    }

    void AnimEndDelegate(SpriteBase sprt)
    {
        isInCooldown = false;
    }

    void DragDelegate(ref POINTER_INFO ptr)
    {
        //LogManager.LogWarning(ptr.evt.ToString());
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
                break;
            case POINTER_INFO.INPUT_EVENT.PRESS:
                break;
            case POINTER_INFO.INPUT_EVENT.TAP:
                if (!isInCooldown && currActionItem != null && enableDoAction)
                {
                    //隐藏Tooltip
                    currActionItem.NotifyTooltipsHide();

                    // 如果同时处于拖拽状态，必须关闭拖拽 [2/22/2012 Ivan]
                    {
                        ActionButton atcionTar = null;
                        if (ptr.hitInfo.collider != null && ptr.hitInfo.collider.gameObject != null)
                        {
                            atcionTar = ptr.hitInfo.collider.gameObject.GetComponent<ActionButton>();
                        }
                        if (UISystem.Instance.IsDragging())
                            UISystem.Instance.OnDragEnd(atcionTar);
                    }

                    currActionItem.DoAction();
                }
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE:
                if (currActionItem != null)
                    currActionItem.NotifyTooltipsShow();
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
                if (currActionItem != null)
                    currActionItem.NotifyTooltipsHide();
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE:
            case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
                if (UISystem.Instance.IsDragging())
                {
                    ActionButton atcionTar = null;
                    if(ptr.hitInfo.collider!=null && ptr.hitInfo.collider.gameObject != null)
                    {
                        atcionTar = ptr.hitInfo.collider.gameObject.GetComponent<ActionButton>();
                    } 
                    UISystem.Instance.OnDragEnd(atcionTar);
                }
                break;
            case POINTER_INFO.INPUT_EVENT.DRAG:
                if (!isInCooldown && currActionItem != null)
				{
	                if (enableDrag)
	                {
	                    if (!UISystem.Instance.IsDragging())
	                    {
	                        UISystem.Instance.OnDragBeging(this);
	                    }
	                }
	                currActionItem.NotifyTooltipsHide();
				}
                break;
            default:
                break;
        }
    }


    #region tActionReference Members
    
    public void BeDestroyed()
    {
        ClearAction();
    }

    public void UpdateRef(int actionId)
    {
        CActionItem action = CActionSystem.Instance.GetActionByActionId(actionId);
        UpdateItemFromAction(action);
    }

    public void SetCheck(bool bCheck)
    {
    }

    public void SetDefault(bool bDefault)
    {
		//if(bDefault)
           //ezActButton.controlIsEnabled = false;
		
    }

    public void EnterCoolDown(int fTime, float fPercent)
    {
        if (coolDownSprite != null)
        {
            if (fTime <= 0)
            {
                coolDownSprite.StopAnim();
                isInCooldown = false;
            }
            else
            {
                float maxFrame = 0;
                float needFrameRate = 0;
                if (coolDownSprite.animations.Length != 0)
                {
                    maxFrame = coolDownSprite.animations[0].GetFrameCount();
                    needFrameRate = maxFrame / ((float)fTime / 1000);
                    coolDownSprite.animations[0].framerate = needFrameRate;
                }
                int startFrame = 0;
                if (fPercent != 0)
                {
                    startFrame = (int)(fPercent * maxFrame);
                }

                coolDownSprite.PlayAnim(0, startFrame);

                isInCooldown = true;
            }
        }
    }


    SpriteText txtTL;
    SpriteText txtTR;
    SpriteText txtBL;
    SpriteText txtBR;
    void ClearAllChar()
    {
        // 因为左上角是用来显示快捷键的，所以要判断下 [4/11/2012 Ivan]
        if (txtTL != null && regKeyCode == KeyCode.None)
            txtTL.Text = "";
        if (txtTR != null)
            txtTR.Text = "";
        if (txtBL != null)
            txtBL.Text = "";
        if (txtBR != null)
            txtBR.Text = "";
    }

    SpriteText GetNewText(string text)
    {
        GameObject go = new GameObject();
        go.layer = LayerManager.UILayer;
        go.transform.parent = gameObject.transform;
        go.transform.localPosition = Vector3.zero;
        SpriteText st = go.AddComponent<SpriteText>();
        st.pixelPerfect = true;
        st.Text = text;
        return st;
    }

    public void SetCornerChar(CORNER_NUMBER_POS pos, string szChar)
    {
        if (ezButton == null)
            return;
        int shiftLength = ezButton.Anchor == SpriteRoot.ANCHOR_METHOD.UPPER_LEFT ? 32 : 16;
        switch (pos)
        {
            case CORNER_NUMBER_POS.ANP_TOPLEFT:
                if (txtTL != null)
                    txtTL.Text = szChar;
                else
                {
                    txtTL = GetNewText(szChar);
                    txtTL.Anchor = SpriteText.Anchor_Pos.Upper_Left;
                    txtTL.transform.localPosition = new UnityEngine.Vector3(-shiftLength, shiftLength, -0.11f);
                }
                break;
            case CORNER_NUMBER_POS.ANP_TOPRIGHT:
                if (txtTR != null)
                    txtTR.Text = szChar;
                else
                {
                    txtTR = GetNewText(szChar);
                    txtTR.Anchor = SpriteText.Anchor_Pos.Upper_Right;
                    txtTR.transform.localPosition = new UnityEngine.Vector3(shiftLength, shiftLength, -0.11f);
                }
                break;
            case CORNER_NUMBER_POS.ANP_BOTLEFT:
                if (txtBL != null)
                    txtBL.Text = szChar;
                else
                {
                    txtBL = GetNewText(szChar);
                    txtBL.Anchor = SpriteText.Anchor_Pos.Lower_Left;
                    txtBL.transform.localPosition = new UnityEngine.Vector3(-shiftLength, -shiftLength, -0.11f);
                }
                break;
            case CORNER_NUMBER_POS.ANP_BOTRIGHT:
                if (txtBR != null)
                    txtBR.Text = szChar;
                else
                {
                    txtBR = GetNewText(szChar);
                    txtBR.Anchor = SpriteText.Anchor_Pos.Lower_Right;
                    txtBR.transform.localPosition = new UnityEngine.Vector3(shiftLength, -shiftLength, -0.11f);
                }
                break;
            default:
                break;
        }
    }

    public void Disable()
    {
        ezButton.controlIsEnabled = false;
        //DisableDrag();
    }

    public void Enable()
    {
        ezButton.controlIsEnabled = true;
        //EnableDrag();
    }

    // 修改默认可以拖动 [4/11/2012 SUN]
    bool enableDrag = true;
    public void EnableDrag()
    {
        enableDrag = true;
    }
    public void DisableDrag()
    {
        enableDrag = false;
    }

    #endregion
}
