using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using Network;
using Network.Packets;
using Network.Handlers;
using Network.tools;

//NetManager单例
public class NetManager : NetObjManager
{
    private static NetManager netManager = null;
    private static float LastHeartBeatTime = Time.time;

    //得到NetManager实例
    public static NetManager GetNetManager()
    {
        return netManager;
    }
    public NetManager()
    {
        netManager = this;
    }

    /// <summary>
    /// 实现log回调函数
    /// </summary>
    /// <param name="type">log的类型</param>
    /// <param name="format">信息</param>
    public override void ShowLog(LogerType type, string format)
    {
        //switch (type)
        //{
        //    case LogerType.DEBUG:
        //        LogManager.Log(format);
        //        break;
        //    case LogerType.ERROR:
        //        LogManager.LogError(format);
        //        GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_NET_CLOSE, format);
        //        break;
        //    case LogerType.INFO:
        //        LogManager.Log(format);
        //        break;
        //    case LogerType.WARN:
        //        LogManager.LogWarning(format);
        //        break;
        //    default:
        //        break;
        //}
    }

    /// <summary>
    /// 重载 RegisterFactory,
    /// 用来注册 Handler 和 Packet，纯虚函数
    /// 修改日志： 
    ///             时间：2012年2月7日 
    ///             内容：  重构注册接口，只需要一个函数既可，
    ///                     且CG 或 CL 的包可以不用注册Handler
    ///             修改人： cfp
    /// </summary>
    public override void RegisterFactory()
    {
        try
        {
            //////////////////////////////////////////////////////////////////////////
            //CG..Cient用户端发往Server伺服器的逻辑包
            //可以不注册Handler
            RegisterFactoryManager.AddFactory( new CGChatFactory(),              null );
            RegisterFactoryManager.AddFactory( new CGHeartBeatFactory(),         null );
            RegisterFactoryManager.AddFactory( new CGConnectFactory(),           null );
            RegisterFactoryManager.AddFactory( new CGEnterSceneFactory(),        null );
            RegisterFactoryManager.AddFactory( new CGAskChangeSceneFactory(),    null );
            RegisterFactoryManager.AddFactory( new CGCommandFactory(),           null );
            RegisterFactoryManager.AddFactory( new CGCharAskBaseAttribFactory(), null );
            RegisterFactoryManager.AddFactory( new CGAskDetailAttribFactory(),   null );
            RegisterFactoryManager.AddFactory( new CGAskMyBagListFactory(),      null );
            RegisterFactoryManager.AddFactory( new CGCharMoveFactory(),          null );
            RegisterFactoryManager.AddFactory( new CGCharPositionWarpFactory(),  null );
            RegisterFactoryManager.AddFactory( new CGCharUseSkillFactory(),      null );
            RegisterFactoryManager.AddFactory( new CGLockTargetFactory(),        null );
            RegisterFactoryManager.AddFactory( new CGAskDetailSkillFactory(),    null );
            RegisterFactoryManager.AddFactory( new CGAskSkillClassFactory(),     null );
            RegisterFactoryManager.AddFactory( new CGAskItemInfoFactory(),       null );
            RegisterFactoryManager.AddFactory(new CGExecuteScriptFactory(),      null);
            RegisterFactoryManager.AddFactory(new CGEventRequestFactory(), null);
            RegisterFactoryManager.AddFactory(new CGCharDefaultEventFactory(), null);
            RegisterFactoryManager.AddFactory(new CGUseEquipFactory(), null);
            RegisterFactoryManager.AddFactory(new CGAskDetailEquipListFactory(), null);
            RegisterFactoryManager.AddFactory(new CGUnEquipFactory(), null);
            RegisterFactoryManager.AddFactory(new CGUseItemFactory(), null);
            RegisterFactoryManager.AddFactory(new CGAskTalismanBagFactory(), null);
            RegisterFactoryManager.AddFactory(new CGOperateTalismanFactory(), null);
            RegisterFactoryManager.AddFactory(new CGAskDetailPetEquipListFactory(), null);

            //装备
            RegisterFactoryManager.AddFactory(new CGCharAskEquipmentFactory(), null);
            //任务
            RegisterFactoryManager.AddFactory( new CGAskMissionDescFactory(),    null );
            RegisterFactoryManager.AddFactory( new CGAskMissionListFactory(),    null );
            RegisterFactoryManager.AddFactory( new CGMissionAbandonFactory(),    null );
            RegisterFactoryManager.AddFactory( new CGMissionAcceptFactory(),     null );
            RegisterFactoryManager.AddFactory( new CGMissionCheckFactory(),      null );
            RegisterFactoryManager.AddFactory( new CGMissionContinueFactory(),   null );
            RegisterFactoryManager.AddFactory( new CGMissionRefuseFactory(),     null );
            RegisterFactoryManager.AddFactory( new CGMissionSubmitFactory(),     null );
            RegisterFactoryManager.AddFactory( new CGMissionTraceFactory(),      null );
            RegisterFactoryManager.AddFactory( new CGAskMailFactory(),           null );
            //背包
            RegisterFactoryManager.AddFactory(new CGPackage_SwapItemFactory(), null);
            //UseAbility
            RegisterFactoryManager.AddFactory(new CGUseAbilityFactory(), null);

            RegisterFactoryManager.AddFactory(new CGAskStudyXinfaFactory(), null);
            RegisterFactoryManager.AddFactory(new CGAskClearCDTimeFactory(), null);
            RegisterFactoryManager.AddFactory(new CGReqRandomAttrFactory(), null);
            RegisterFactoryManager.AddFactory(new CGManipulatePetFactory(), null);
            RegisterFactoryManager.AddFactory(new CGIssuePetPlacardFactory(), null);
            RegisterFactoryManager.AddFactory(new CGSetPetAttribFactory(), null);
            RegisterFactoryManager.AddFactory(new CGModifySettingFactory(), null);
            RegisterFactoryManager.AddFactory(new CGUseGemFactory(), null);
            RegisterFactoryManager.AddFactory(new CGRemoveGemFactory(), null);
            RegisterFactoryManager.AddFactory(new CGGemCompoundFactory(), null);
            RegisterFactoryManager.AddFactory(new CGAskSettingFactory(), null);
            RegisterFactoryManager.AddFactory(new CGShopBuyFactory(), null);
            RegisterFactoryManager.AddFactory(new CGShopSellFactory(), null);
            RegisterFactoryManager.AddFactory(new CGOpenItemBoxFactory(), null);
            RegisterFactoryManager.AddFactory(new CGBankAcquireListFactory(), null);
            RegisterFactoryManager.AddFactory(new CGBankAddItemFactory(), null);
            RegisterFactoryManager.AddFactory(new CGBankMoneyFactory(), null);
            RegisterFactoryManager.AddFactory(new CGBankRemoveItemFactory(), null);
            RegisterFactoryManager.AddFactory(new CGBankSwapItemFactory(), null);
            RegisterFactoryManager.AddFactory(new CGCharSetMaskFactory(), null);
            RegisterFactoryManager.AddFactory(new CGPickBoxItemFactory(), null);
            RegisterFactoryManager.AddFactory(new CGAskDetailAbilityInfoFactory(), null);
            RegisterFactoryManager.AddFactory(new CGTeamLeaveFactory(), null);
            RegisterFactoryManager.AddFactory(new CGTeamDismissFactory(), null);
            //复活相关包
            RegisterFactoryManager.AddFactory(new CGPlayerDieResultFactory(), null);
            RegisterFactoryManager.AddFactory(new CGDiscardItemFactory(), null);
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            //CL..Cient用户端发往Login伺服器的逻辑包
            //可以不注册Handler
            RegisterFactoryManager.AddFactory( new CLAskLoginFactory(),      null );
            RegisterFactoryManager.AddFactory( new CLConnectFactory(),       null );
            RegisterFactoryManager.AddFactory( new CLAskCharListFactory(),   null );
            RegisterFactoryManager.AddFactory( new CLAskCharLoginFactory(),  null );
            RegisterFactoryManager.AddFactory( new CLAskCreateCharFactory(), null );

            RegisterFactoryManager.AddFactory( new CLAskDeleteCharFactory(), null);
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            //GC..Server伺服器发往Cient用户端的逻辑包
            RegisterFactoryManager.AddFactory( new GCChatFactory(),              new GCChatHandler());
            RegisterFactoryManager.AddFactory( new GCConnectFactory(),           new GCConnectHandler());
            RegisterFactoryManager.AddFactory( new GCEnterSceneFactory(),        new GCEnterSceneHandler());
            RegisterFactoryManager.AddFactory( new GCNewItemBoxFactory(),        new GCNewItemBoxHandler());
            RegisterFactoryManager.AddFactory( new GCPickResultFactory(),        new GCPickResultHandler());
            RegisterFactoryManager.AddFactory( new GCBoxItemListFactory(),       new GCBoxItemListHandler());
            RegisterFactoryManager.AddFactory( new GCNewMonsterFactory(),        new GCNewMonsterHandler());
            RegisterFactoryManager.AddFactory( new GCCharBaseAttribFactory(),    new GCCharBaseAttribHandler());
            RegisterFactoryManager.AddFactory( new GCNotifyChangeSceneFactory(), new GCNotifyChangeSceneHanlder());
            RegisterFactoryManager.AddFactory( new GCRetChangeSceneFactory(),    new GCRetChangeSceneHandler());
            RegisterFactoryManager.AddFactory( new GCMyBagListFactory(),         new GCMyBagListHandler());
            RegisterFactoryManager.AddFactory( new GCCharMoveFactory(),          new GCCharMoveHandler());
            RegisterFactoryManager.AddFactory( new GCArriveFactory(),            new GCArriveHandler());
            RegisterFactoryManager.AddFactory( new GCObjTeleportFactory(),       new GCObjTeleportHandler());
            RegisterFactoryManager.AddFactory( new GCRemoveGemResultFactory(), new GCRemoveGemResultHandler());
            RegisterFactoryManager.AddFactory( new GCUseGemResultFactory(), new GCUseGemResultHandler());

            RegisterFactoryManager.AddFactory( new GCCharSkillCreateBulletFactory(), new GCCharSkillCreateBulletHandler());
            RegisterFactoryManager.AddFactory( new GCCharSkillGatherFactory(),       new GCCharSkillGatherHandler());
            RegisterFactoryManager.AddFactory( new GCCharSkillLeadFactory(),         new GCCharSkillLeadHandler());
            RegisterFactoryManager.AddFactory( new GCCharSkillMissedFactory(),       new GCCharSkillMissedHandler());
            RegisterFactoryManager.AddFactory( new GCCharSkillSendFactory(),         new GCCharSkillSendHandler());
            RegisterFactoryManager.AddFactory( new GCCharSkillGatherModifyFactory(), new GCCharSkillGatherModifyHandler());
            RegisterFactoryManager.AddFactory( new GCCharSkillLeadModifyFactory(),   new GCCharSkillLeadModifyHandler());
            RegisterFactoryManager.AddFactory( new GCDetailHealsAndDamagesFactory(), new GCDetailHealsAndDamagesHandler());
            RegisterFactoryManager.AddFactory( new GCDetailBuffFactory(),            new GCDetailBuffHandler());
            RegisterFactoryManager.AddFactory( new GCCharImpactListUpdateFactory(),  new GCCharImpactListUpdateHandler());
            RegisterFactoryManager.AddFactory( new GCCharDirectImpactFactory(),      new GCCharDirectImpactHandler());
            RegisterFactoryManager.AddFactory( new GCCharBuffFactory(),              new GCCharBuffHandler());
            RegisterFactoryManager.AddFactory( new GCDetailSkillListFactory(),       new GCDetailSkillListHandler());
            RegisterFactoryManager.AddFactory( new GCCooldownUpdateFactory(),        new GCCooldownUpdateHandler());
            RegisterFactoryManager.AddFactory( new GCDetailImpactListUpdateFactory(), new GCDetailImpactListUpdateHandler());
            RegisterFactoryManager.AddFactory(new GCTargetListAndHitFlagsFactory(), new GCTargetListAndHitFlagsHandler());
            RegisterFactoryManager.AddFactory(new GCDetailXinFaListFactory(), new GCDetailXinFaClassHandler());
            RegisterFactoryManager.AddFactory(new GCCharMoveResultFactory(), new GCCharMoveResultHandler());
            RegisterFactoryManager.AddFactory(new GCCharFirstLoginFactory(), new GCCharFirstLoginHandler());
            RegisterFactoryManager.AddFactory(new GCSpecialObjActNowFactory(), new GCSpecialObjActNowHandler());
            RegisterFactoryManager.AddFactory(new GCSpecialObjFadeOutFactory(),new GCSpecialObjFadeOutHandler());
            RegisterFactoryManager.AddFactory(new GCNewSpecialFactory(), new GCNewSpecialHandler());
            RegisterFactoryManager.AddFactory(new GCCharJumpFactory(), new GCCharJumpHandler());
            RegisterFactoryManager.AddFactory(new GCCharModifyActionFactory(), new GCCharModifyActionHandler());

            RegisterFactoryManager.AddFactory( new GCDetailAttribFactory(),      new GCDetailAttribHandler());
            RegisterFactoryManager.AddFactory( new GCNewMonster_MoveFactory(),   new GCNewMonster_MoveHandler());
            RegisterFactoryManager.AddFactory( new GCDelObjectFactory(),         new GCDelObjectHandler());
            RegisterFactoryManager.AddFactory( new GCNewPlayerFactory(),         new GCNewPlayerHandler());
            RegisterFactoryManager.AddFactory( new GCNewPlayer_MoveFactory(),    new GCNewPlayer_MoveHandler());
            RegisterFactoryManager.AddFactory( new GCOperateResultFactory(),     new GCOperateResultHandler());
            RegisterFactoryManager.AddFactory( new GCLevelUpResultFactory(),     new GCLevelUpResultHandler());
            RegisterFactoryManager.AddFactory( new GCLevelUpFactory(),           new GCLevelUpHandler());
            RegisterFactoryManager.AddFactory( new GCManualAttrResultFactory(),  new GCManualResultHandler());
            RegisterFactoryManager.AddFactory(new GCLeaveSceneFactory(), new GCLeaveSceneHandler());
            RegisterFactoryManager.AddFactory(new GCShopMerchandiseListFactory(), new GCShopMerchandiseListHandler());
            RegisterFactoryManager.AddFactory(new GCShopBuyFactory(), new GCShopBuyHandler());
            RegisterFactoryManager.AddFactory(new GCShopSoldListFactory(), new GCShopSoldListHandler());
            RegisterFactoryManager.AddFactory(new GCShopSellFactory(), new GCShopSellHandler());
            RegisterFactoryManager.AddFactory(new GCAbilityActionFactory(), new GCAbilityActionHandler());
            RegisterFactoryManager.AddFactory(new GCAbilityResultFactory(), new GCAbilityResultHandler());
            RegisterFactoryManager.AddFactory(new GCAbilitySuccFactory(), new GCAbilitySuccHandler());
            RegisterFactoryManager.AddFactory(new GCBankAcquireListFactory(), new GCBankAcquireListHandler());
            RegisterFactoryManager.AddFactory(new GCBankBeginFactory(), new GCBankBeginHandler());
            RegisterFactoryManager.AddFactory(new GCBankAddItemFactory(), new GCBankAddItemHandler());
            RegisterFactoryManager.AddFactory(new GCBankItemInfoFactory(), new GCBankItemInfoHandler());
            RegisterFactoryManager.AddFactory(new GCBankMoneyFactory(), new GCBankMoneyHandler());
            RegisterFactoryManager.AddFactory(new GCBankRemoveItemFactory(), new GCBankRemoveItemHandler());
            RegisterFactoryManager.AddFactory(new GCBankSwapItemFactory(), new GCBankSwapItemHandler());
            RegisterFactoryManager.AddFactory(new GCRetTeamRecruitInfoFactory(), new GCRetTeamRecruitInfoHandler());
            RegisterFactoryManager.AddFactory(new GCTeamResultFactory(), new GCTeamResultHandler());
            RegisterFactoryManager.AddFactory(new GCAbilityExpFactory(), new GCAbilityExpHandler());
            RegisterFactoryManager.AddFactory(new GCPrescriptionFactory(), new GCPrescriptionHandler());
            RegisterFactoryManager.AddFactory(new GCNotifyTeamInfoFactory(), new GCNotifyTeamInfoHandler());
            RegisterFactoryManager.AddFactory(new GCTeamMemberInfoFactory(), new GCTeamMemberInfoHandler());
            //装备相关包
            RegisterFactoryManager.AddFactory(new GCCharEquipmentFactory(), new GCCharEquipmentHandler());
            RegisterFactoryManager.AddFactory(new GCItemInfoFactory(),      new GCItemInfoHandler());
            RegisterFactoryManager.AddFactory(new GCNotifyEquipFactory(), new GCNotifyEquipHanlder());
            RegisterFactoryManager.AddFactory(new GCDetailEquipListFactory(), new GCDetailEquipListHandler());
            RegisterFactoryManager.AddFactory(new GCUseEquipResultFactory(), new GCUseEquipResultHandler());
            RegisterFactoryManager.AddFactory(new GCUnEquipResultFactory(), new GCUnEquipResultHandler());
            RegisterFactoryManager.AddFactory(new GCStudyXinfaFactory(), new GCStudyXinfaHandler());
            RegisterFactoryManager.AddFactory(new GCRetSettingFactory(), new GCRetSettingHandler());
            RegisterFactoryManager.AddFactory(new GCCharStopActionFactory(), new GCCharStopActionHandler());
            RegisterFactoryManager.AddFactory(new GCDetailPetEquipListFactory(), new GCDetailPetEquipListHandler());
            RegisterFactoryManager.AddFactory(new GCOperatePetEquipResultFactory(), new GCOperatePetEquipResultHandler());
            RegisterFactoryManager.AddFactory(new GCDiscardItemResultFactory(), new GCDiscardItemResultHandler());

            //任务相关包
            RegisterFactoryManager.AddFactory( new GCScriptCommandFactory(),                new GCScriptCommandHandler());
            RegisterFactoryManager.AddFactory( new GCMissionModifyFactory(),                new GCMissionModifyHandler());
            RegisterFactoryManager.AddFactory( new GCRetMissionDescFactory(),               new GCRetMissionDescHandler());
            RegisterFactoryManager.AddFactory( new GCMissionListFactory(),                  new GCMissionListHandler());
            RegisterFactoryManager.AddFactory( new GCMissionAddFactory(),                   new GCMissionAddHandler());
            RegisterFactoryManager.AddFactory( new GCMissionRemoveFactory(),                new GCMissionRemoveHandler());
            RegisterFactoryManager.AddFactory( new GCMissionResultFactory(),                new GCMissionResultHandler());
            RegisterFactoryManager.AddFactory( new GCMissionHaveDoneFlagFactory(),          new GCMissionHaveDoneFlagHandler());
            RegisterFactoryManager.AddFactory( new GCCanPickMissionItemListFactory(),       new GCCanPickMissionItemListHandler());
            RegisterFactoryManager.AddFactory( new GCAddCanPickMissionItemFactory(),        new GCAddCanPickMissionItemHandler());
            RegisterFactoryManager.AddFactory( new GCRemoveCanPickMissionItemFactory(),     new GCRemoveCanPickMissionItemHandler());

            

            //背包
            RegisterFactoryManager.AddFactory(new GCPackage_SwapItemFactory(), new GCPackage_SwapItemHandler());
            RegisterFactoryManager.AddFactory(new GCUseItemResultFactory(), new GCUseItemResultHandler());
           
            //宠物相关包
            RegisterFactoryManager.AddFactory(new GCNewPetFactory(), new GCNewPetHandler());
            RegisterFactoryManager.AddFactory(new GCNewPet_MoveFactory(), new GCNewPet_MoveHandler());
            RegisterFactoryManager.AddFactory(new GCNewPet_DeathFactory(), new GCNewPet_DeathHandler());
            RegisterFactoryManager.AddFactory(new GCDetailAbilityInfoFactory(), new GCDetailAbilityInfoHandler());
            RegisterFactoryManager.AddFactory(new GCDetailAttrib_PetFactory(), new GCDetailAttrib_PetHandler());
            RegisterFactoryManager.AddFactory(new GCWorldTimeFactory(), new GCWorldTimeHandler());
            RegisterFactoryManager.AddFactory(new GCManipulatePetRetFactory(), new GCManipulatePetRetHandler());
            RegisterFactoryManager.AddFactory(new GCRemovePetFactory(), new GCRemovePetHandler());
            RegisterFactoryManager.AddFactory(new GCPetPlacardListFactory(), new GCPetPlacardListHandler());

            RegisterFactoryManager.AddFactory(new GCNewMonster_DeathFactory(), new GCNewMonster_DeathHandler());
            RegisterFactoryManager.AddFactory(new GCNewPlayer_DeathFactory(), new GCNewPlayer_DeathHandler());

            //复活相关包
            RegisterFactoryManager.AddFactory(new GCPlayerDieFactory(), new GCPlayerDieHandler());
            RegisterFactoryManager.AddFactory(new GCPlayerReliveFactory(), new GCPlayerReliveHandler());
            //////////////////////////////////////////////////////////////////////////

            //////////////////////////////////////////////////////////////////////////
            //LC..Login伺服器发往Cient用户端的逻辑包
            RegisterFactoryManager.AddFactory( new LCRetConnectFactory(),    new LCRetConnectHandler());
            RegisterFactoryManager.AddFactory( new LCRetLoginFactory(),      new LCRetLoginHandler());
            RegisterFactoryManager.AddFactory( new LCStatusFactory(),        new LCStatusHandler());
            RegisterFactoryManager.AddFactory( new LCRetCharListFactory(),   new LCRetCharListHandler());
            RegisterFactoryManager.AddFactory( new LCRetCharLoginFactory(),  new LCRetCharLoginHandler());
            RegisterFactoryManager.AddFactory( new LCRetCreateCharFactory(), new LCRetCreateCharHandler());
            RegisterFactoryManager.AddFactory(new LCRetDeleteCharFactory(), new LCRetDeleteCharHandler());

            //符印相关的包  [3/22/2012 ZZY]
            RegisterFactoryManager.AddFactory(new CGUseSymbolFactory(), null);
            RegisterFactoryManager.AddFactory(new GCUseSymbolResultFactory(), new GCUseSymbolResultHandler());
            RegisterFactoryManager.AddFactory(new GCCharmInfoFlushFactory(), new GCCharmInfoFlushHandler());
            RegisterFactoryManager.AddFactory(new CGAskFlushCharmInfoFactory(), null);

            //法宝相关包
            RegisterFactoryManager.AddFactory(new GCRetTalismanBagFactory(), new GCRetTalismanBagHandler());
            RegisterFactoryManager.AddFactory(new GCOperateTalismanResultFactory(), new GCRetTalismanBagHandler());
            RegisterFactoryManager.AddFactory(new GCResetTalismanBagSizeFactory(), new GCResetTalismanBagSizeHandler());
            RegisterFactoryManager.AddFactory(new GCOperateTalismanResultFactory(), new GCOperateTalismanResultHandler());

            RegisterFactoryManager.AddFactory(new GCRelationFactory(), new GCRelationHandler());

            // 系统公告 [4/16/2012 Ivan]
            RegisterFactoryManager.AddFactory(new GCSystemMsgFactory(), new GCSystemMsgHandler());
            //////////////////////////////////////////////////////////////////////////
        }
        catch (System.Exception ex)
        {
            LogManager.LogError(" RegisterFactory: " + ex.ToString());
        }

    }

    /// <summary>
    /// 网络状态改变回调函数
    /// </summary>
    /// <param name="netStatus">改变的网络状态</param>
    public override void NetStatusChange(NETMANAGER_STATUS netStatus)
    {
        if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
            SetNetStatus(netStatus);
    }
    //--------------------------------------------------------------------------------------------------------------
    //
    // Tick不同的游戏流程
    //

    // Tick 游戏登录流程.
    public void TickGameLoginProcedure()
    {

        //LogManager.Log("NetManager tick: login=" + GameProcedure.s_ProcLogIn.GetStatus());
        switch (GameProcedure.s_ProcLogIn.GetStatus())
        {
            case GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_DEBUG_SETTING:
                {

                    break;
                }

            case GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_SELECT_SERVER:
                {

                    break;
                }

            //尚未登录,准备状态
            case GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_DISCONNECT:
                {
                    break;
                }

            //SOCKET连接中...
            case GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_CONNECTING:
                {
                    WaitConnecting();
                    break;
                }

            //连接失败
            case GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_CONNECT_FAILED:
                {
                    break;
                }

            case GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_ACCOUNT_BEGIN_REQUESTING:
                {
                    break;
                }
            //连接成功后的其他状态,开始正常处理消息包
            case GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_CONNECTED_OK:
                {
                    WaitPacket();
                    //CGameProcedure::s_pProcLogIn->OpenCountInputDlg();
                    //CGameProcedure::s_pProcLogIn->SetStatus(CGamePro_Login::LOGIN_ACCOUNT_REQUESTING);
                    break;
                }
            case GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_ACCOUNT_REQUESTING:
                {
                    WaitPacket();
                    break;
                }
            default:
                {
                    WaitPacket();
                    break;
                }
        }
    }

    // Tick 主游戏流程
    public void TickGameMainProcedure()
    {
        //正常处理消息包
        WaitPacket();

        //定时发送心跳消息
        SendHeartBeat();
    }

    // Tick 人物选择流程
    public void TickGameSelCharcterProcedure()
    {
        //正常处理消息包
        WaitPacket();
    }
    // Tick 人物创建流程
    public void TickGameCreateCharcterProcedure()
    {
        //正常处理消息包
        WaitPacket();
    }

    // Tick 连接到服务器流程
    public void TickChangeGameServerProcedure()
    {
        switch (GameProcedure.s_ProcChangeScene.GetStatus())
        {
            case GamePro_ChangeScene.PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_DISCONNECT:
                {

                    break;
                }
            //SOCKET连接中...
            case GamePro_ChangeScene.PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_CONNECTING:
                {
                    WaitConnecting();
                    break;
                }

            //连接失败
            case GamePro_ChangeScene.PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_CONNECT_SUCCESS:
                {
                    break;
                }

            //连接成功后的其他状态,开始正常处理消息包
            case GamePro_ChangeScene.PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_SENDING_CGCONNECT:
                {
                    WaitPacket();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    // Tick 进入流程
    public void TickGameEnterProcedure()
    {
        switch (GameProcedure.s_ProcEnter.GetStatus())
        {
            case GamePro_Enter.ENTER_STATUS.ENTERSCENE_READY:
                {

                    break;
                }
            //SOCKET连接中...
            case GamePro_Enter.ENTER_STATUS.ENTERSCENE_REQUESTING:
                {
                    //LogManager.Log("Waitting for packet");
                    WaitPacket();
                    break;
                }

            //连接失败
            case GamePro_Enter.ENTER_STATUS.ENTERSCENE_OK:
                {
                    break;
                }

            //连接成功后的其他状态,开始正常处理消息包
            case GamePro_Enter.ENTER_STATUS.ENTERSCENE_FAILED:
                {

                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    //
    // Tick不同的游戏流程
    //
    //--------------------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------------------
    //
    // 设置网络状态
    // 

    // 设置网络状态
    //--------------------------------------------------------------------------------------------------------------
    //
    // 设置网络状态
    // 

    // 设置网络状态
    public void SetNetStatus(NETMANAGER_STATUS netStatus)
    {
        //当前流程是登录流程
        if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcLogIn)
        {

            // Tick 游戏登录流程.
            SetGameLoginStatus(netStatus);
        }
        //处于主游戏循环中
        else if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
        {
            // Tick 主游戏流程
            SetGameMainStatus(netStatus);
        }
        else if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcChangeScene)
        {
            GameProcedure.s_ProcChangeScene.SetStatus(GamePro_ChangeScene.PLAYER_CHANGE_SERVER_STATUS.CHANGESCENE_CONNECT_SUCCESS);
        }
    }

    // Tick 游戏登录流程.
    public void SetGameLoginStatus(NETMANAGER_STATUS netStatus)
    {
        switch (netStatus)
        {
            case NETMANAGER_STATUS.CONNECT_SUCESS:
                {
                    //LogManager.Log("Connect Sucess");
                    GameProcedure.s_ProcLogIn.SetStatus(GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_CONNECTED_OK);//
                    break;
                }
            case NETMANAGER_STATUS.CONNECT_FAILED_CONNECT_ERROR:
                {

                    GameProcedure.s_ProcLogIn.SetStatus(GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_CONNECT_FAILED);
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_NET_CLOSE, "网络连接失败！");
                    break;
                }
            case NETMANAGER_STATUS.CONNECT_FAILED_TIME_OUT:
                {
                    //LogManager.Log("Connect failed time out");
                    GameProcedure.s_ProcLogIn.SetStatus(GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_CONNECT_FAILED);
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_NET_CLOSE, "连接超时！");
                    break;
                }
            case NETMANAGER_STATUS.CONNECT_EXIT:
                {
                    //LogManager.Log("Connect exit");
                    GameProcedure.s_ProcLogIn.SetStatus(GamePro_Login.PLAYER_LOGIN_STATUS.LOGIN_CONNECT_FAILED);
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_NET_CLOSE, "连接退出！");
                }
                break;
            case NETMANAGER_STATUS.NO_CONNECT:
                GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_NET_CLOSE, "网络连接失败！");
                break;
            default:
                {
                    break;
                }
        }
    }

    // Tick 主游戏流程
    public void SetGameMainStatus(NETMANAGER_STATUS netStatus)
    {
        switch (netStatus)
        {
            case NETMANAGER_STATUS.CONNECT_SUCESS:
                {
                    break;
                }
            case NETMANAGER_STATUS.CONNECT_FAILED_CONNECT_ERROR:
                {
                    break;
                }
            case NETMANAGER_STATUS.CONNECT_FAILED_TIME_OUT:
                {
                    break;
                }
            case NETMANAGER_STATUS.CONNECT_EXIT:
                {
                    GetNetwork().Disconnect();
                    GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_NET_CLOSE, "服务器关闭连接！");
                    break;
                }

            default:
                {
                    break;
                }
        }

    }


    //
    //  设置网络状态
    //
    //--------------------------------------------------------------------------------------------------------------


    // 重载Tick()
    public override bool Tick()
    {
        //LogManager.Log("NetManager Tick()");
        //当前流程是登录流程
        if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcLogIn)
        {

            // Tick 游戏登录流程.
            TickGameLoginProcedure();
            //SendHeartBeat();

        }
        //处于主游戏循环中
        else if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
        {
            // Tick 主游戏流程
            TickGameMainProcedure();

        }
        // 处于人物选择流程
        else if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcCharSel)
        {
            // Tick 人物选择
            TickGameSelCharcterProcedure();
            //SendHeartBeat();
        }
        // 处于人物创建流程
        else if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcCharCreate)
        {
            // Tick 人物创建流程
            TickGameCreateCharcterProcedure();
            //SendHeartBeat();
        }
        // tick game server 连接流程
        else if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcChangeScene)
        {

            TickChangeGameServerProcedure();
            //SendHeartBeat();
        }
        else if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcEnter)
        {

            // Tick 进入流程
            TickGameEnterProcedure();
            //SendHeartBeat();
        }
        return true;
    }


    void WaitConnecting()
    {

        if (NetStatus != NETMANAGER_STATUS.CONNECT_SUCESS)
        {
            SetNetStatus(NetStatus);
            return;
        }
        else
        {
            if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcLogIn)
            {
                GameProcedure.s_ProcLogIn.SendClConnectMsg();
                SetNetStatus(NetStatus);//
            }
            else if (GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcChangeScene)
            {
                SetNetStatus(NetStatus);//
            }

        }
        //LogManager.Log("NetManager NetStatus:" + NetStatus);
    }
    void WaitPacket()
    {

        if (!GetNetwork().Tick())
        {
            GetNetwork().Disconnect();
            //net close
            //LogManager.Log("NetWork Failed!");
            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_NET_CLOSE, "网络连接失败：" + ErrorString);
            return;
        }
        //        if(GameProcedure.GetActiveProcedure() == GameProcedure.s_ProcMain)
        // 	        SetNetStatus(NetStatus);
    }
    void SendHeartBeat()
    {

        if (Time.time - LastHeartBeatTime >= 60)
        {
            LastHeartBeatTime = Time.time;

            CGHeartBeat msg = (CGHeartBeat)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_HEARTBEAT);

            // 发送网络连接消息
            NetManager.GetNetManager().SendPacket(msg);
        }
    }
    public void Release()
    {
        GetNetwork().Disconnect();
        netManager = null;
    }

}

