using System;
using System.Collections.Generic;
using UnityEngine;

using Interface;
// namespace SGWEB
// {
public class CursorMng
{
    // 事件委托，监听鼠标改变的时候触发事件
    public delegate void CursorChange(ENUM_CURSOR_TYPE cursorType);
    public event CursorChange CursotChanged;

    public delegate void UICursorChange(Texture uiCursor);
    public event UICursorChange UICursotChanged;

    protected bool m_bShow;							// 是否显示光标
    protected ENUM_CURSOR_TYPE m_nCursorState;		// 光标状态
    //HCURSOR		m_hCursor[ CURSOR_NUMBER ];		// 光标
    public Texture m_hUICursor;			// UI控制光标

    //鼠标上储存的命令
    protected SCommand_Mouse m_cmdCurrent_Left;		// 待触发指令_左键
    protected SCommand_Mouse m_cmdCurrent_Right;	// 待触发指令_右键

    //移动事件间隔的时间
    private static float uPrevMoveTime = 0;

    // 更改为单例模式
    static readonly CursorMng instance = new CursorMng();

    private CursorMng()
    {
        m_bShow = true;
        m_nCursorState = ENUM_CURSOR_TYPE.CURSOR_NORMAL;
        m_hUICursor = null;
        m_cmdCurrent_Left = new SCommand_Mouse();
        m_cmdCurrent_Right = new SCommand_Mouse();
        m_cmdCurrent_Left.CleanUp();
        m_cmdCurrent_Right.CleanUp();

        m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
        m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
    }

    public static CursorMng Instance
    {
        get
        {
            return instance;
        }
    }

    //--------------------------------------------------
    //强制设置鼠标光标
    public void SetCursor(ENUM_CURSOR_TYPE nType)
    {
        m_nCursorState = nType;
    }

    public ENUM_CURSOR_TYPE GetCursor()
    {
        if (!m_bShow)
            return ENUM_CURSOR_TYPE.CURSOR_WINBASE;

        return m_nCursorState;
    }

    //响应WM_SETCURSOR
    public void OnSetCursor()
    {
        if (m_bShow)
        {
            if (CursotChanged != null)
                CursotChanged(GetCursor());
        }
    }

    //进入UI控制模式
    public void EnterUICursor(Texture hCursor)
    {
        m_hUICursor = hCursor;
        if (UICursotChanged != null)
            UICursotChanged(m_hUICursor);
    }
    public void LeaveUICursor()
    {
        m_hUICursor = null;
        if (UICursotChanged != null)
            UICursotChanged(m_hUICursor);
    }

    //显示/隐藏鼠标
    public void ShowCursor(bool bShow)
    {
        if (bShow == m_bShow)
            return;

        m_bShow = bShow;

        //::ShowCursor(m_bShow);
    }
    //取鼠标状态
    public ENUM_CURSOR_TYPE GetCursor_State()
    {
        return m_nCursorState;
    }

    //得到命令
    public SCommand_Mouse MouseCommand_GetLeft() { return m_cmdCurrent_Left; }
    public SCommand_Mouse MouseCommand_GetRight() { return m_cmdCurrent_Right; }
    //清空
    public void MouseCommand_Clear() { m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL; }

    //--------------------------------------------------
    //在鼠标上存储命令
    public void MouseCommand_Set(bool bHoverInUI, CObject pSelectObj, Vector3 fvPos, tActionItem pActiveSkill)
    {
        m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
        m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
        //bool bCtrlDown = CInputSystem.GetMe().IsKeyDown(KeyCode.KC_LCONTROL) || CInputSystem.GetMe().IsKeyDown(KeyCode.KC_RCONTROL);

        //在UI上空
        if (bHoverInUI)
        {
            if (UISystem.Instance.CurrHyperlink != null)
            {
                m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_HyperLink;
                m_cmdCurrent_Left.SetValue(0, UISystem.Instance.CurrHyperlink);
            }
            else if (pActiveSkill != null)
            {
                switch (pActiveSkill.GetType())
                {
                    //修理装备
                    case ACTION_OPTYPE.AOT_MOUSECMD_REPARE:
                        {
                            //左键挂锤子
                            m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_REPAIR;
                            //右键取消
                            m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_CANCLE_REPAIR;
                        }
                        break;

                    //鉴定装备
                    case ACTION_OPTYPE.AOT_MOUSECMD_IDENTIFY:
                        {
                            //左键鉴定
                            m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_IDENTIFY;
                            //右键取消
                            m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_CANCEL_IDENTIFY;
                        }
                        break;

                    case ACTION_OPTYPE.AOT_MOUSECMD_ADDFRIEND:
                        {
                            //左键鉴定
                            m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_ADDFRIEND;
                            //右键取消
                            m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_CANCEL_ADDFRIEND;
                        }
                        break;
                    case ACTION_OPTYPE.AOT_MOUSECMD_EXCHANGE:
                        {
                            //左键鉴定
                            m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_EXCHANGE;
                            //右键取消
                            m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_CANCEL_EXCHANGE;
                        }
                        break;
                    case ACTION_OPTYPE.AOT_MOUSECMD_SALE:
                        {
                            //左键鉴定
                            m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_SALE;
                            //右键取消
                            m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_CANCEL_SALE;
                        }
                        break;
                    case ACTION_OPTYPE.AOT_MOUSECMD_BUYMULT:
                        {
                            //左键鉴定
                            m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_BUYMULT;
                            //右键取消
                            m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_CANCEL_BUYMULT;
                        }
                        break;

                    default:
                        break;
                }
            }
            else
                return;
        }
        else
        {
            //计算相应obj鼠标指令
            if (pSelectObj != null)
            {
                pSelectObj.FillMouseCommand_Left(ref m_cmdCurrent_Left, pActiveSkill);
                pSelectObj.FillMouseCommand_Right(ref m_cmdCurrent_Right, pActiveSkill);
            }

            if (m_cmdCurrent_Left.m_typeMouse == MOUSE_CMD_TYPE.MCT_NULL)
            {
                if (pActiveSkill != null)
                {
                    switch (pActiveSkill.GetType())
                    {
                        case ACTION_OPTYPE.AOT_SKILL:
                        case ACTION_OPTYPE.AOT_PET_SKILL:
                            {
                                SCLIENT_SKILL skillImpl = (SCLIENT_SKILL)pActiveSkill.GetImpl();

                                if (skillImpl == null)
                                {
                                    {
                                        m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_PLAYER_MOVETO;
                                        m_cmdCurrent_Left.SetValue(0, fvPos.x);
                                        m_cmdCurrent_Left.SetValue(1, fvPos.z);
                                    }
                                    return;
                                }

                                //选择类型
                                // 						        ENUM_SELECT_TYPE typeSel = (ENUM_SELECT_TYPE)( pActiveSkill.type()== ACTION_OPTYPE.AOT_SKILL ? 
                                // 							        ((SCLIENT_SKILL)pSkillImpl)->m_pDefine->m_nSelectType : 
                                // 							        ((PET_SKILL)pSkillImpl)->m_pDefine->m_nSelectType);

                                ENUM_SELECT_TYPE typeSel = (ENUM_SELECT_TYPE)skillImpl.m_pDefine.m_nSelectType;
                                switch (typeSel)
                                {
                                    //点选角色
                                    case ENUM_SELECT_TYPE.SELECT_TYPE_CHARACTER:
                                        {
                                            //进入这里说明逻辑出错了，不然会在character里面填充
                                            //如果FillMouseCommand_Left填充失败，说明目标是不可攻击对象，执行移动操作
                                            if (pSelectObj == null)
                                            {
                                                m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_PLAYER_MOVETO;
                                                m_cmdCurrent_Left.SetValue(0, fvPos.x);
                                                m_cmdCurrent_Left.SetValue(1, fvPos.z);
                                            }
                                        }
                                        break;

                                    //点选范围
                                    case ENUM_SELECT_TYPE.SELECT_TYPE_POS:
                                        {
                                            //储存技能
                                            m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_SKILL_AREA;
                                            m_cmdCurrent_Left.SetValue(0, pActiveSkill);
                                            m_cmdCurrent_Left.SetValue(1, fvPos.x);
                                            m_cmdCurrent_Left.SetValue(2, fvPos.z);
                                            // 								        FLOAT fRingRange = (FLOAT)( pActiveSkill.type()==AOT_SKILL ? 
                                            // 									        ((SCLIENT_SKILL*)pSkillImpl)->m_pDefine->m_fDamageRange : 
                                            // 								        ((PET_SKILL*)pSkillImpl)->m_pDefine->m_fDamageRange);
                                            m_cmdCurrent_Left.SetValue(5, skillImpl.m_pDefine.m_fDamageRange);

                                            //取消技能
                                            m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_CANCEL_SKILL;
                                        }
                                        break;
                                    //方向
                                    case ENUM_SELECT_TYPE.SELECT_TYPE_DIR:
                                        {
                                            //储存技能
                                            m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_SKILL_DIR;
                                            m_cmdCurrent_Left.SetValue(0, pActiveSkill);

                                            //计算方向

                                            //鼠标位置
                                            Vector3 avatarPos = CObjectManager.Instance.getPlayerMySelf().GetPosition();

                                            Vector3 avatarPosOnPlane = new Vector3(avatarPos.x, 0, avatarPos.z);
                                            Vector3 PosOnPlane = new Vector3(fvPos.x, 0, fvPos.z);
                                            float angle = Vector3.Angle(avatarPosOnPlane, PosOnPlane);
                                            m_cmdCurrent_Left.SetValue(1, angle);
                                            //取消技能
                                            m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_CANCEL_SKILL;
                                        }
                                        break;
                                        //add by ss 增加对无目标技能和只对自己的处理流程
                                    case ENUM_SELECT_TYPE.SELECT_TYPE_SELF:
										{	
											m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_SKILL_SELF;
	                                        m_cmdCurrent_Left.SetValue(0, pActiveSkill);
	                                        m_cmdCurrent_Left.SetValue(1, CObjectManager.Instance.getPlayerMySelf().ServerID);
										}
                                        break;
                                    case ENUM_SELECT_TYPE.SELECT_TYPE_NONE:
										{
											m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_SKILL_NONE;
	                                        m_cmdCurrent_Left.SetValue(0, pActiveSkill);
										}
                                        break;
                                }
                            }
                            break;

                        case ACTION_OPTYPE.AOT_ITEM:
                            {
                                if (pActiveSkill == null || pActiveSkill.GetType() != ACTION_OPTYPE.AOT_ITEM)
                                    break;

                                CObject_Item pItem = (CObject_Item)(((CActionItem_Item)pActiveSkill).GetImpl());
                                if (pItem == null)
                                    break;

                                //必须是能够使用的物品,必须支持任务物品 [1/24/2011 ivan edit]
                                if (pItem.GetItemClass() != ITEM_CLASS.ICLASS_COMITEM && pItem.GetItemClass() != ITEM_CLASS.ICLASS_TASKITEM)
                                    break;

                                bool bAreaItem = ((CObject_Item_Medicine)pItem).IsAreaTargetType();

                                m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_USE_ITEM;
                                m_cmdCurrent_Left.SetValue(0, pActiveSkill);
                                m_cmdCurrent_Left.SetValue(1, 0);
                                m_cmdCurrent_Left.SetValue(2, fvPos.x);
                                m_cmdCurrent_Left.SetValue(3, fvPos.z);
                                m_cmdCurrent_Left.SetValue(4, (int)(bAreaItem ? 1 : 0));

                                m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_CANCEL_USE_ITEM;
                            }
                            break;

                        //修理单独处理
                        case ACTION_OPTYPE.AOT_MOUSECMD_REPARE:
                            {
                                //左键挂锤子
                                m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_REPAIR;
                                //右键取消
                                m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_CANCLE_REPAIR;
                            }
                            break;

                        //鉴定装备
                        case ACTION_OPTYPE.AOT_MOUSECMD_IDENTIFY:
                            {
                                //左键鉴定
                                m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_IDENTIFY;
                                //右键取消
                                m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_CANCEL_IDENTIFY;
                            }
                            break;
                        case ACTION_OPTYPE.AOT_MOUSECMD_ADDFRIEND:
                            {
                                //左键鉴定
                                m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_ADDFRIEND;
                                //右键取消
                                m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_CANCEL_ADDFRIEND;
                            }
                            break;
                        case ACTION_OPTYPE.AOT_MOUSECMD_EXCHANGE:
                            {
                                //左键鉴定
                                m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_EXCHANGE;
                                //右键取消
                                m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_UI_USE_CANCEL_EXCHANGE;
                            }
                            break;
                        case ACTION_OPTYPE.AOT_MOUSECMD_CATCHPET:
                            {
                                m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_CATCH_PET;
                                m_cmdCurrent_Left.SetValue(0, fvPos.x);
                                m_cmdCurrent_Left.SetValue(1, fvPos.z);
                            }
                            break;
                        case ACTION_OPTYPE.AOT_MOUSECMD_SALE:
                            {
                                //左键鉴定
                                m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_SALE;
                                //右键取消
                                m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_CANCEL_SALE;
                            }
                            break;
                        case ACTION_OPTYPE.AOT_MOUSECMD_BUYMULT:
                            {
                                //左键
                                m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_BUYMULT;
                                //右键
                                m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_CANCEL_BUYMULT;
                            }
                            break;
                        default:
                            //移动
                            {
                                //if (pSelectObj != CObjectManager.Instance.getPlayerMySelf())
                                {
                                    m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_PLAYER_MOVETO;
                                    m_cmdCurrent_Left.SetValue(0, fvPos.x);
                                    m_cmdCurrent_Left.SetValue(1, fvPos.z);
                                }
                            }
                            break;
                    }
                }
                else //temp fix 添加默认左键事件, TODO,find其他地方是否有默认左键事件
                {
                    //移动
                    {
                        if (pSelectObj == null)
                        {
                            m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_PLAYER_MOVETO;
                            m_cmdCurrent_Left.SetValue(0, fvPos.x);
                            m_cmdCurrent_Left.SetValue(1, fvPos.z);
                        }
                    }
                }
            }
            //选择玩家
            if (m_cmdCurrent_Right.m_typeMouse == MOUSE_CMD_TYPE.MCT_NULL)
            {
                m_cmdCurrent_Right.m_typeMouse = MOUSE_CMD_TYPE.MCT_PLAYER_SELECT;
                m_cmdCurrent_Right.SetValue(0, -1);
            }
        }
    }

    //激活鼠标命令
    public void MouseCommand_Active(SCommand_Mouse cmd)
    {
        CObject_PlayerMySelf mySelf = CObjectManager.Instance.getPlayerMySelf();
        if (mySelf == null)
            return;

        switch (cmd.m_typeMouse)
        {
            case MOUSE_CMD_TYPE.MCT_PLAYER_MOVETO:
                {
                    if (uPrevMoveTime == 0 || GameProcedure.s_pTimeSystem.GetTimeNow() > uPrevMoveTime + 100)
                    {
                        //CEventSystem::GetMe()->PushEvent(GE_AUTO_RUNING_CHANGE, 0);
                        uPrevMoveTime = GameProcedure.s_pTimeSystem.GetTimeNow();

                        float x = cmd.GetValue<float>(0);
                        float z = cmd.GetValue<float>(1);
                        Vector3 targeMove = new Vector3(x, 0, z);
                        GameInterface.Instance.Player_MoveTo(targeMove);
                    }
                }
                break;

            case MOUSE_CMD_TYPE.MCT_PLAYER_SELECT:
                {
                    GameProcedure.s_pGameInterface.Object_SelectAsMainTarget(cmd.GetValue<int>(0)
                        , CObjectManager.DESTROY_MAIN_TARGET_TYPE.DEL_RIGHT_CLICK);

                    if (m_cmdCurrent_Left.m_typeMouse == MOUSE_CMD_TYPE.MCT_SKILL_DIR ||
                        m_cmdCurrent_Left.m_typeMouse == MOUSE_CMD_TYPE.MCT_SKILL_AREA)
                    {
                        m_cmdCurrent_Left.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
                        m_cmdCurrent_Left.SetValue(0, 0);
                        m_cmdCurrent_Left.SetValue(1, 0);
                        //恢复激活技能
                        CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                    }
                    //			AxTrace(0, 0, "Active: SelectPlayer(ID=%d)", (int)cmd.m_afParam[0]);
                }
                break;

            case MOUSE_CMD_TYPE.MCT_SKILL_OBJ:
                {
                    //首先选择
                    GameProcedure.s_pGameInterface.Object_SelectAsMainTarget(cmd.GetValue<int>(1)
                        , CObjectManager.DESTROY_MAIN_TARGET_TYPE.DEL_RIGHT_CLICK);

                    tActionItem pItem = cmd.GetValue<tActionItem>(0);
                    if (pItem == null)
                        break;

                    //进入攻击
                    if (pItem.GetType() == ACTION_OPTYPE.AOT_PET_SKILL)
                    {
                        // 先不实现宠物技能
                        // 				        int nPetSkillID = ((PET_SKILL*)(((CActionItem_PetSkill*)pItem)->GetImpl()))->m_pDefine->m_nID;
                        // 				        CDataPool::GetMe()->Pet_SendUseSkillMessage(nPetSkillID, 
                        // 					        INVALID_ID, cmd.m_afParam[1], cmd.m_afParam[2]);
                    }
                    else
                    {
                        tActionItem skill = cmd.GetValue<tActionItem>(0);
                        GameProcedure.s_pGameInterface.Player_UseSkill((CActionItem_Skill)skill, cmd.GetValue<int>(1));
                    }
                }
                break;

            case MOUSE_CMD_TYPE.MCT_SKILL_AREA:
                {
                    tActionItem pItem = cmd.GetValue<tActionItem>(0);
                    if (pItem == null)
                        break;

                    if (pItem.GetType() == ACTION_OPTYPE.AOT_PET_SKILL)
                    {
                        // 				        int nPetSkillID = ((PET_SKILL*)(((CActionItem_PetSkill*)pItem)->GetImpl()))->m_pDefine->m_nID;
                        // 				        CDataPool::GetMe()->Pet_SendUseSkillMessage(nPetSkillID, 
                        // 					        INVALID_ID, cmd.m_afParam[1], cmd.m_afParam[2]);
                    }
                    else
                    {
                        tActionItem skill = cmd.GetValue<tActionItem>(0);
                        Vector3 pos = new Vector3(cmd.GetValue<float>(1), 0, cmd.GetValue<float>(2));
                        GameProcedure.s_pGameInterface.Player_UseSkill((CActionItem_Skill)skill, pos);
                    }

                    //恢复激活Action
                    CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                }
                break;

            case MOUSE_CMD_TYPE.MCT_CANCEL_SKILL:
                {
                    //恢复激活Action
                    CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                }
                break;

            case MOUSE_CMD_TYPE.MCT_SKILL_DIR:
                {
                    // 支持方向攻击 [1/25/2011 ivan edit]
                    tActionItem pItem = cmd.GetValue<tActionItem>(0);
                    if (pItem == null)
                        break;

                    if (pItem.GetType() == ACTION_OPTYPE.AOT_PET_SKILL)
                    {
                        //TODO
                    }
                    else
                    {
                        tActionItem skill = cmd.GetValue<tActionItem>(0);
                        float dir = cmd.GetValue<float>(1);
                        GameProcedure.s_pGameInterface.Player_UseSkill((CActionItem_Skill)skill, dir);
                    }

                    //恢复激活Action
                    CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                }
                break;

            case MOUSE_CMD_TYPE.MCT_HIT_TRIPPEROBJ:
                {
                    GameProcedure.s_pGameInterface.TripperObj_Active(cmd.GetValue<int>(0));
                    //			AxTrace(0, 0, "Active: TripObj(ID=%d)", (int)cmd.m_adwParam[0]);
                }
                break;

            case MOUSE_CMD_TYPE.MCT_SPEAK:
                {
                    // 当鼠标点击其他的时候自动寻路结束（临时方案，具体等策划案子） [9/1/2011 edit by ZL]
                    CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_AUTO_RUNING_CHANGE);

                    int npcId = cmd.GetValue<int>(0);
                    //首先选择
                    GameInterface.Instance.Object_SelectAsMainTarget(npcId);

                    //谈话
                    //CGameProcedure::s_pGameInterface->Player_Speak(cmd.m_adwParam[0]);
                    GameInterface.Instance.Player_Speak(npcId);
                }
                break;

            case MOUSE_CMD_TYPE.MCT_CONTEXMENU:
                {
                    //首先选择
                    //CGameProcedure::s_pGameInterface->Object_SelectAsMainTarget((INT)cmd.m_adwParam[0], 1);

                    //显示右键菜单
                    //CGameProcedure::s_pGameInterface->Object_ShowContexMenu(cmd.m_adwParam[0]);
                }
                break;

            case MOUSE_CMD_TYPE.MCT_REPAIR:
                {
                    //do nothing...
                }
                break;

            case MOUSE_CMD_TYPE.MCT_CANCLE_REPAIR:
                {
                    //取消修理
                    CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                }
                break;


            case MOUSE_CMD_TYPE.MCT_UI_USE_IDENTIFY:
                {
                    // 使用鉴定卷轴

                    break;
                }
            case MOUSE_CMD_TYPE.MCT_UI_USE_CANCEL_IDENTIFY:
                {
                    // 取消使用鉴定卷轴
                    //CDataPool::GetMe()->Identify_UnLock();
                    break;
                }
            case MOUSE_CMD_TYPE.MCT_UI_USE_EXCHANGE:
                {
                    // 			        if(TRUE == CObjectManager::GetMe()->GetMySelf()->GetCharacterData()->Get_IsInStall())
                    // 			        {
                    // 				        CEventSystem::GetMe()->PushEvent(GE_INFO_INTERCOURSE,"你正在摆摊……");
                    // 				        return ;
                    // 			        }
                    // 			        CGameProcedure::s_pGameInterface->Object_SelectAsMainTarget((INT)cmd.m_adwParam[0]);
                    // 			        CObject* pChar = (CObject*) CObjectManager::GetMe()->GetMainTarget();
                    // 			        if(pChar && g_theKernel.IsKindOf(pChar->GetClass(), GETCLASS(CObject_PlayerOther)))
                    // 			        {
                    // 				        //先判定距离是不是合法
                    // 				        if(pChar)
                    // 				        {
                    // 					        const fVector3 pMePos = CObjectManager::GetMe()->GetMySelf()->GetPosition();
                    // 					        const fVector3 pOtherPos = ((CObject*)pChar)->GetPosition();
                    // 
                    // 					        FLOAT fDistance = 
                    // 						        TDU_GetDist(fVector2(pMePos.x, pMePos.z),
                    // 						        fVector2(pOtherPos.x, pOtherPos.z));
                    // 
                    // 					        if( EXCHANGE_MAX_DISTANCE < fDistance )
                    // 					        {
                    // 						        CEventSystem::GetMe()->PushEvent(GE_INFO_INTERCOURSE,"距离太远，启动交易失败");
                    // 						        return;
                    // 					        }
                    // 				        }
                    // 
                    // 				        //不能向已经死亡的玩家发送交易申请
                    // 				        if ( ((CObject_Character*)pChar)->CharacterLogic_Get() == CObject_Character::CHARACTER_LOGIC_DEAD )
                    // 				        {
                    // 					        CEventSystem::GetMe()->PushEvent(GE_INFO_INTERCOURSE,"不能向已经死亡的玩家申请交易");
                    // 					        return ;
                    // 				        }
                    // 
                    // 				        CGExchangeApplyI msg;
                    // 				        msg.SetObjID(pChar->GetServerID());
                    // 				        CNetManager::GetMe()->SendPacket(&msg);
                    // 				        CEventSystem::GetMe()->PushEvent(GE_INFO_INTERCOURSE,"交易请求已经发送");
                    // 				        CGameProcedure::s_pGameInterface->Skill_CancelAction();
                    // 				        return ;
                    // 			        }
                }
                break;
            case MOUSE_CMD_TYPE.MCT_UI_USE_CANCEL_EXCHANGE:
                {
                    CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                    break;
                }
            case MOUSE_CMD_TYPE.MCT_SALE:
                break;
            case MOUSE_CMD_TYPE.MCT_CANCEL_SALE:
                {
                    CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                    break;
                }
            case MOUSE_CMD_TYPE.MCT_BUYMULT:
                break;
            case MOUSE_CMD_TYPE.MCT_CANCEL_BUYMULT:
                {
                    CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                    break;
                }
            case MOUSE_CMD_TYPE.MCT_UI_USE_ADDFRIEND:
                {
                    //首先选择
                    // 			        CGRelation Msg;
                    // 			        Msg.GetRelation()->CleanUp();
                    // 			        Msg.GetRelation()->m_Type = REQ_ADDFRIEND;
                    // 			        REQUEST_ADD_RELATION_WITH_GROUP* pFriend = &(Msg.GetRelation()->m_AddRelationWithGroup);
                    // 			        pFriend->CleanUp();
                    // 			        CGameProcedure::s_pGameInterface->Object_SelectAsMainTarget((INT)cmd.m_adwParam[0]);
                    // 			        CObject_Character* pCharObj = (CObject_Character*)CObjectManager::GetMe()->GetMainTarget();
                    // 			        if( pCharObj == NULL )
                    // 				        break;
                    // 			        if( pCharObj == CObjectManager::GetMe()->GetMySelf() ) // 如果是自己，就不加
                    // 				        break;
                    // 			        if( g_theKernel.IsKindOf( pCharObj->GetClass(), GETCLASS(CObject_PlayerOther) ) ) // 如果是玩家
                    // 			        {
                    // 				        ENUM_RELATION sCamp = CGameInterface::GetMe()->GetCampType( 
                    // 					        pCharObj, 
                    // 					        (CObject*)CObjectManager::GetMe()->GetMySelf() );
                    // 
                    // 				        // 通过PK模式判断是否为敌人 [8/19/2011 edit by ZL]
                    // 				        if (sCamp != RELATION_ENEMY) {
                    // 					        INT tempRelation = CObjectManager::GetMe()->GetMySelf()->GetRelationOther(pCharObj);
                    // 					        if ( tempRelation != -1 ) 
                    // 						        sCamp = (ENUM_RELATION)tempRelation;
                    // 				        }
                    // 
                    // 				        if( sCamp != RELATION_FRIEND ) // 如果是同一阵营的
                    // 				        {
                    // 					        CGameProcedure::s_pEventSystem->PushEvent( GE_INFO_INTERCOURSE, "无效目标");
                    // 					        break;
                    // 				        }
                    // 			        }
                    // 			        else
                    // 			        {
                    // 				        CGameProcedure::s_pEventSystem->PushEvent( GE_INFO_INTERCOURSE, "无效目标");
                    // 				        break;
                    // 			        }
                    // 			        pFriend->SetTargetName( pCharObj->GetCharacterData()->Get_Name() );
                    // 			        //需要重写--Ivan
                    // 			        //pFriend->SetGroup( SCRIPT_SANDBOX::Friend::m_nCurTeam );
                    // 			        pFriend->SetRelationType( RELATION_TYPE_FRIEND );
                    // 			        CNetManager::GetMe()->SendPacket( &Msg );
                }
                break;
            case MOUSE_CMD_TYPE.MCT_UI_USE_CANCEL_ADDFRIEND:
                CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                break;
            case MOUSE_CMD_TYPE.MCT_USE_ITEM:
                {
                    //使用物品
                    // 			        CGameProcedure::s_pGameInterface->PacketItem_UserItem((
                    // 				        tActionItem*)cmd.m_apParam[0], 
                    // 				        cmd.m_adwParam[1],
                    // 				        fVector2(cmd.m_afParam[2], cmd.m_afParam[3]));

                    //恢复激活Action
                    CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                }
                break;

            case MOUSE_CMD_TYPE.MCT_CANCEL_USE_ITEM:
                {
                    //恢复激活Action
                    CActionSystem.Instance.SetDefaultAction(CObjectManager.Instance.getPlayerMySelf().GetActiveSkill());
                }
                break;

            case MOUSE_CMD_TYPE.MCT_LEAP:
                {
                    //轻功...
                    //SkillID_t id = CObjectManager::GetMe()->GetMySelf()->GetCharacterData()->GetDefaultFlySkillID();
                    //CObjectManager::GetMe()->GetMySelf()->CharacterLogic_GetAI()->PushCommand_Fly(id, cmd.m_afParam[0], cmd.m_afParam[1]);
                }
                break;
            case MOUSE_CMD_TYPE.MCT_CATCH_PET:
                {
                    //CGameProcedure::s_pGameInterface->Player_UseSkill( cmd.m_adwParam[0], int( cmd.m_adwParam[1] ) );
                }
                break;
            case MOUSE_CMD_TYPE.MCT_ENTER_BUS:
                {
                    // 进入载具 [8/26/2011 ivan edit]
                    //CGameProcedure::s_pGameInterface->Player_EnterSpecialBus(cmd.m_adwParam[0]);
                }
                break;
            case MOUSE_CMD_TYPE.MCT_EXIT_BUS:
                {
                    // 离开载具 [8/26/2011 ivan edit]
                    //CGameProcedure::s_pGameInterface->Player_ExitSpecialBus(cmd.m_adwParam[0]);
                }
                break;
            case MOUSE_CMD_TYPE.MCT_HyperLink:
                {
                    HyperItemBase link = cmd.GetValue<HyperItemBase>(0);
                    link.Click();
                }
                break;
            case MOUSE_CMD_TYPE.MCT_SKILL_NONE:
                {
					tActionItem skill = cmd.GetValue<tActionItem>(0);//无目标技能
                    CActionItem_Skill curSkill = (CActionItem_Skill)skill;
                    if (curSkill != null)
                    {
                        CObjectManager.Instance.getPlayerMySelf().Player_UseSkill(curSkill.GetDefineID());
                    }
                }
                break;
			case MOUSE_CMD_TYPE.MCT_SKILL_SELF:
                {
					tActionItem skill = cmd.GetValue<tActionItem>(0);//无目标技能
                    CActionItem_Skill curSkill = (CActionItem_Skill)skill;
                    int ServerID      = cmd.GetValue<int>(1);//针对自己的技能;
                    if (curSkill != null)
                    {
                       CObjectManager.Instance.getPlayerMySelf().Player_UseSkill(curSkill.GetDefineID(), ServerID);
                    }
				}
				break;
            default:
                //AxTrace(0, 0, "Active: ERROR!(%d)", cmd.m_typeMouse);
                break;
        }
    }

    //WX_DECLARE_DYNAMIC( CCursorMng );
}
/*}*/
