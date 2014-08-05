using System.Collections.Generic;
using UnityEngine;
using Network;
using Network.Packets;
using DBSystem;
using System;
using System.Globalization;
public class UIBuffer : UIButton
{
    public enum ENUM_BUFFER_TYPE
    {
        AVATAR,
        TARGET,
        TEAM,
        NONE
    };
    public ENUM_BUFFER_TYPE  type_      = ENUM_BUFFER_TYPE.NONE;
    public  int              index_     = 0;//界面中第几个buff
    public int               teamIndex_ = 0;//组队中第几个队友
    private short            bufferID_  = 0;//当前buffid
    private uint timeStamp_ = 0;
    protected override void Awake()
    {
        base.Awake();
        Init();
        switch (type_)
        {
            case ENUM_BUFFER_TYPE.AVATAR:
                UIBufferManager.Instance.RegisterAvatarBuffer(this);
                UIBufferManager.Instance.OnUpdateBuffer(GAME_EVENT_ID.GE_IMPACT_SELF_UPDATE, new List<string>());
                break;
            case ENUM_BUFFER_TYPE.TARGET:
                {
                    UIBufferManager.Instance.RegisterTargetBuffer(this);
                    UIBufferManager.Instance.OnUpdateBuffer(GAME_EVENT_ID.GE_MAINTARGET_CHANGED, new List<string>());
                }
                break;
            case ENUM_BUFFER_TYPE.TEAM:
                UIBufferManager.Instance.RegisterTeamBuffer(teamIndex_,this);
                break;
        }
        Hide(true);
    }

    public void UpdateTime()
    {
        //CObject_Character pChar = null;
        switch (type_)
        {
            case ENUM_BUFFER_TYPE.AVATAR:
                {
                    _BUFF_IMPACT_INFO buff =  CDataPool.Instance.BuffImpact_GetByIndex(index_);
                    if(buff != null)
                    {
                        if (buff.m_nBuffID != bufferID_)
                        {
                            _DBC_BUFF_IMPACT pBuffImpact = CDataBaseSystem.Instance.GetDataBase<_DBC_BUFF_IMPACT>((int)DataBaseStruct.DBC_BUFF_IMPACT).Search_Index_EQU(buff.m_nBuffID);
                            string iconName ="";
                            if (pBuffImpact != null)
                            {
                               iconName = pBuffImpact.m_szIconName;
                            }
                            bufferID_ = buff.m_nBuffID;
                            Texture2D  tex = IconManager.Instance.GetIcon(iconName);
                            SetTexture(tex);
                        }
						timeStamp_ = GameProcedure.s_pTimeSystem.GetTimeNow();
						Hide(false);
                        this.Text = string.Format("{0:F1}", buff.m_nTimer / 1000.0f) + "s";
                    }
                    else
                    {
                        Hide(true);
						bufferID_ = 0;
                    }
                    
                }
                break;
            case ENUM_BUFFER_TYPE.TARGET:
            {
                bool needHide = true;
                if (CObjectManager.Instance.GetMainTarget() != null)
                {
                    CObject_Character obj = CObjectManager.Instance.GetMainTarget() as CObject_Character;
                    if (obj != null)
                    {
                        SImpactEffect impactEffect = obj.Impact.BuffImpact_GetByIndex(index_);
                        if (impactEffect != null)
                        {
                            if ((short)impactEffect.GetImpactID() != bufferID_)
                            {
                                _DBC_BUFF_IMPACT pBuffImpact = CDataBaseSystem.Instance.GetDataBase<_DBC_BUFF_IMPACT>((int)DataBaseStruct.DBC_BUFF_IMPACT).Search_Index_EQU((int)impactEffect.GetImpactID());
                                string iconName = "";
                                if (pBuffImpact != null)
                                {
                                    iconName = pBuffImpact.m_szIconName;
                                }
                                bufferID_ = (short)impactEffect.GetImpactID();
                                Texture2D tex = IconManager.Instance.GetIcon(iconName);
                                SetTexture(tex);   
                            }
							needHide = false;
                        }
                    }
                }
                if (needHide)
                {
                    bufferID_ = 0;
                }
                Hide(needHide);
                this.Text = "";
            }
            break;
            case ENUM_BUFFER_TYPE.TEAM:
                break;
        }
    }

    public void TickTime()
    {
        if (type_ == ENUM_BUFFER_TYPE.AVATAR && bufferID_ != 0)
        {
            uint currentTime = GameProcedure.s_pTimeSystem.GetTimeNow();
            uint deltaTime =  currentTime - timeStamp_;
            if (deltaTime > 500)
            {
                timeStamp_ = currentTime;
                int time = CDataPool.Instance.BuffImpact_GetTimeByIndex(index_);
                this.Text = string.Format("{0:F1}", time / 1000.0f) + "s";
            }
        }
    }

    public void OnClick()
    {
        if (type_ == ENUM_BUFFER_TYPE.AVATAR)
        {
            //先不做是否能取消buff的判定
            _BUFF_IMPACT_INFO pBuffImpactInfo = CDataPool.Instance.BuffImpact_GetByIndex(index_);
            if (pBuffImpactInfo != null)
            {
                CGDispelBuffReq msgDispelBuffReq = new CGDispelBuffReq();
                msgDispelBuffReq.SN = (int)pBuffImpactInfo.m_nSN;
                NetManager.GetNetManager().SendPacket(msgDispelBuffReq);
            }
        }
    }

    public override void OnInput(ref POINTER_INFO ptr)
    {
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.MOVE:
                {
                    GameObject bufferToolTip = UIWindowMng.Instance.GetWindowGo("BufferTooltipWindow");
                    if (bufferToolTip != null)
                    {
                        Vector2 ptMouse = GameProcedure.s_pInputSystem.GetMouseUIPos();
                        UIBufferToolTip toolTip = bufferToolTip.GetComponent<UIBufferToolTip>();
                        toolTip.ShowTooltip(ptMouse.x, ptMouse.y, bufferID_);
                    }
                }
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
                {
                    GameObject bufferToolTip = UIWindowMng.Instance.GetWindowGo("BufferTooltipWindow");
                    if (bufferToolTip != null)
                    {
                        UIBufferToolTip toolTip = bufferToolTip.GetComponent<UIBufferToolTip>();
                        toolTip.Hide();
                    }
                }
                break;
            default:
                break;
        }
        base.OnInput(ref ptr);
    }
}