
namespace Network
{
    public enum PACKET_DEFINE
    {
        PACKET_NONE = 0,						//0，空
        /*------------------------GC-------------------------------------------------------------------------------------------*/
        /* ID:1 */
        PACKET_GC_TEST,
        PACKET_GC_CONNECT,											//服务器收到PACKET_CG_CONNECT后发送给客户端
        PACKET_GC_LEAVESCENE,										//服务器发给其他客户端某玩家离开的消息
        PACKET_GC_ENTERSCENE,										//服务器发给客户端进入场景的确认消息
        PACKET_GC_NEWITEM,											//有新的独立物品出现在自己的视野范围
        PACKET_GC_NEWITEMBOX,
        PACKET_GC_NEWMONSTER,										//有新的怪物进入自己的视野范围
        PACKET_GC_NEWMONSTER_DEATH,									//创建死亡的OBJ
        PACKET_GC_NEWMONSTER_MOVE,									//创建移动的OBJ
        /* ID:10 */
        PACKET_GC_NEWPLAYER,									//有新的其他玩家进入自己的视野范围
        PACKET_GC_NEWPLAYER_DEATH,									//创建死亡的PLAYER
        PACKET_GC_NEWPLAYER_MOVE,									//创建移动的PLAYER
        PACKET_GC_PLAYERREALMOVE,									//废弃
        PACKET_GC_MONSTERREALMOVE,									//废弃
        PACKET_GC_CHARBASEATTRIB,									//刷新角色基本属性
        PACKET_GC_CHAREQUIPMENT,									//刷新角色简单装备表
        PACKET_GC_DIE,
        PACKET_GC_LEVELUP,
        PACKET_GC_LEVELUPRESULT,									//请求升级结果
        /* ID:20 */
        PACKET_GC_MANUALATTRRESULT,							//点数分配结果
        PACKET_GC_RESETATTRRESULT,									//洗点结果
        PACKET_GC_MYEQUIPMENT,										//废弃
        PACKET_GC_OTHEREQUIPMENT,									//废弃
        PACKET_GC_DETAILATTRIB,									//刷新当前主角属性
        PACKET_GC_DETAILEQUIPMENTLIST,								//刷新当前主角装备列表
        PACKET_GC_DETAILSKILLLIST,									//刷新当前主角技能列表
        PACKET_GC_DETAILSUBEFFECTLIST,								//刷新当前主角附加属性列表
        PACKET_GC_MYBAGLIST,										//刷新主角背包
        PACKET_GC_CHARIDLE,											//通知CLIENT进入IDLE状态
        /* ID:30 */
        PACKET_GC_CHARJUMP,									//通知CLIENT跳跃一次
        PACKET_GC_CHARMOVE,											//通知CLIENT移动
        PACKET_GC_MOVE,											//废弃
        PACKET_GC_ARRIVE,											//废弃
        PACKET_GC_CHARSKILL_CREATEBULLET,							//创建子弹
        PACKET_GC_CHARSKILL_EFFECT,									//技能作用效果(伤害)
        PACKET_GC_CHARSKILL_GATHER,									//技能聚气
        PACKET_GC_CHARSKILL_GATHER_MODIFY,							//技能聚气时间的延长等等
        PACKET_GC_CHARSKILL_LEAD,									//技能引导
        PACKET_GC_CHARSKILL_LEAD_MODIFY,							//技能引导时间的缩短等等
        /* ID:40 */
        PACKET_GC_CHARSKILL_SEND,							//技能发招
        PACKET_GC_ATTACK,											//废弃
        PACKET_GC_OTHERATTACK,										//废弃
        PACKET_GC_ERRORATTACK,										//废弃
        PACKET_GC_SKILL,											//废弃
        PACKET_GC_OTHERSKILL,										//废弃
        PACKET_GC_ERRORSKILL,										//废弃
        PACKET_GC_SKILLPREPARE,									//废弃
        PACKET_GC_USEITEMRESULT,									//使用道具结果
        PACKET_GC_BOXITEMLIST,										//Obj_ItemBox 物品列表
        /* ID:50 */
        PACKET_GC_PICKRESULT,								//捡取结果
        PACKET_GC_USEEQUIPRESULT,									//装配装备结果
        PACKET_GC_DELOBJECT,										//删除OBJ
        PACKET_GC_UNEQUIPRESULT,									//卸下装备结果
        PACKET_GC_DISCARDITEMRESULT,								//丢弃物品结果
        PACKET_GC_RETCHANGESCENE,									//服务器回应客户端的场景变换请求
        PACKET_GC_CHAT,												//说话
        PACKET_GC_NOTIFYCHANGESCENE,								//服务器通知客户端可以改变场景了
        PACKET_GC_ITEMINFO,											//服务器返回某件装备的详细信息
        PACKET_GC_DETAILEQUIPLIST,									//服务器返回自身装备详细信息
        /* ID:60 */
        PACKET_GC_NEWPORTAL,									//有一个传送点可以看见了
        PACKET_GC_DETAILXINFALIST,									//刷新当前主角心法列表
        PACKET_GC_DISCARDEQUIPRESULT,								//丢弃物品结果
        PACKET_GC_NOTIFYEQUIP,										//通知客户端包裹里添加新装备了
        PACKET_GC_NEWSKILLOBJ,										//创建SkillObj的消息
        PACKET_GC_USEGEMRESULT,										//使用宝石结果
        PACKET_GC_REMOVEGEMRESULT,									//卸载宝石结果
        PACKET_GC_CHARIMPACT,										//角色附加效果
        PACKET_GC_OPERATE_RESULT,									//操作的返回
        PACKET_GC_USEEQUIPSKILL_RESULT,
        /* ID:70 */
        PACKET_GC_TEAMERROR,									//
        PACKET_GC_TEAMASKINVITE,									//
        PACKET_GC_TEAMLEADERASKINVITE,								//
        PACKET_GC_TEAMRESULT,										//
        PACKET_GC_TEAMASKAPPLY,										//
        PACKET_GC_SCRIPTCOMMAND,									//脚本到客户端的指令
        PACKET_GC_ABILITY_RESULT,									//生活技能的操作返回结果
        PACKET_GC_DETAILABILITYINFO,								//服务器端发送玩家生活技能信息
        PACKET_GC_COOLDOWN_UPDATE,									//冷却时间更新
        PACKET_GC_ABILITY_ACTION,									//生活技能的操作动作
        /* ID:80 */
        PACKET_GC_ABILITY_SUCC,								//生活技能的成功消息
        PACKET_GC_NOTIFYTEAMINFO,									//通知客户端,队伍中信息变化
        PACKET_GC_MISSIONLIST,										//返回任务列表
        PACKET_GC_MISSIONADD,										//增加一个任务
        PACKET_GC_MISSIONREMOVE,									//删除一个任务
        PACKET_GC_MISSIONMODIFY,									//更改一个任务参数
        PACKET_GC_CHANNELERROR,									//
        PACKET_GC_CHANNELRESULT,									//
        PACKET_GC_MAIL,											//
        PACKET_GC_NOTIFYMAIL,										//
        /* ID:90 */
        PACKET_GC_NEWPLATFORM,								//创建操作台的消息
        PACKET_GC_ABILITYLEVEL,										//生活技能信息
        PACKET_GC_ABILITYEXP,										//生活技能熟练度信息
        PACKET_GC_PRESCRIPTION,										//生活技能配方信息
        PACKET_GC_ABANDON_ABILITY,									//放弃生活技能
        PACKET_GC_ITEMBOXCHANGESTAGE,								//Obj_ItemBox改变外观
        PACKET_GC_CANPICKMISSIONITEMLIST,							//可以拾取的任务物品列表
        PACKET_GC_ADDCANPICKMISSIONITEM,							//添加一个可以拾取的任务物品
        PACKET_GC_REMOVECANPICKMISSIONITEM,							//删除一个可以拾取的任务物品
        PACKET_GC_MODIFYSPEED,										//对象速度改变
        /* ID:100 */
        PACKET_GC_MONSTERSPEAK,								//怪物泡泡说话
        PACKET_GC_TEAMMEMBERINFO,									//返回队友信息
        PACKET_GC_LOCK_TARGET,										//玩家锁定目标
        PACKET_GC_SHOPMERCHANDISELIST,								//商品列表	
        PACKET_GC_SHOPSOLDLIST,										//回购商品列表
        PACKET_GC_SHOPUPDATEMERCHANDISELIST,						//更新商品列表
        PACKET_GC_SHOPBUY,											//通知客户端买是否成功
        PACKET_GC_SHOPSELL,											//通知客户端卖是否成功
        PACKET_GC_SHOPREPAIR,										//通知服务器修理物品
        PACKET_GC_MENPAIINFO,										//门派信息（ID）
        /* ID:110 */
        PACKET_GC_JOINMENPAI,								//加入门派的结果
        PACKET_GC_XINFASTUDYINFO,									//可以学习的心法（包括级别）信息
        PACKET_GC_STUDYXINFA_H,										//学习的结果
        PACKET_GC_ABILITY_STUDYINFO,								//学习生活技能的信息
        PACKET_GC_CHARALLTITLES,									//服务器通知客户端此人所有的称号
        PACKET_GC_ACTIVETIME_UPDATE,								//废弃
        PACKET_GC_OBJ_TELEPORT,										//角色瞬移
        PACKET_GC_BANKBEGIN,										//通知客户端开始打开银行
        PACKET_GC_BANKACQUIRELIST,									//银行中的物品发送给客户端
        PACKET_GC_BANKADDITEM,										//通知客户端加物品结果
        /* ID:120 */
        PACKET_GC_BANKREMOVEITEM,							//通知客户端移除物品结果
        PACKET_GC_BANKSWAPITEM,										//通知客户端从银行中与背包或装备栏中交换物品的结果
        PACKET_GC_BANKMONEY,										//通知客户端存钱取钱结果
        PACKET_GC_DETAILATTRIB_PET,									//宠物的详细属性
        PACKET_GC_EXCHANGEERROR,									//交易出错
        PACKET_GC_EXCHANGEAPPLYI,									//申请交易
        PACKET_GC_EXCHANGEESTABLISHI,								//交易确定
        PACKET_GC_EXCHANGESYNCHLOCK,								//交易同步锁
        PACKET_GC_EXCHANGESYNCHCONFIRMII,							//交易同步确定
        PACKET_GC_EXCHANGESYNCHII,									//交易同步
        /* ID:130 */
        PACKET_GC_EXCHANGESUCCESSIII,						//交易成功消息
        PACKET_GC_EXCHANGECANCEL,									//交易取消
        PACKET_GC_PLAYER_DIE,										//主角死亡
        PACKET_GC_PLAYER_RELIVE,									//主角复活
        PACKET_GC_MANIPULATEPETRET,									//请求操作宠物的返回结果
        PACKET_GC_ASK_TEAMFOLLOW,									//给队员发送进入组队跟随请求
        PACKET_GC_RETURN_TEAMFOLLOW,								//给客户端发送组队跟随状态
        PACKET_GC_TEAMFOLLOW_LIST,									//组队跟随列表
        PACKET_GC_TEAMFOLLOW_ERROR,									//组队跟随下的错误消息
        PACKET_GC_MISSIONRESULT,									//任务完成与否
        /* ID:140 */
        PACKET_GC_CHAR_DIRECT_IMPACT, 						//效果:一次性效果
        PACKET_GC_CHAR_BUFF, 										//效果: 持续性效果
        PACKET_GC_DETAIL_BUFF,										//效果: 持续性效果的详细数据.数据量大,只发给自己
        PACKET_GC_CHARSKILL_MISSED,									//技能: 技能没有命中
        PACKET_GC_CHAR_IMPACT_LIST_UPDATE,							//更新效果列表(简要信息，广播用)
        PACKET_GC_DETAIL_IMPACT_LIST_UPDATE,						//更新效果列表(详细信息，只给玩家自己发)
        PACKET_GC_DETAIL_HEALS_AND_DAMAGES, 						//HP,MP,Rage,Strike Point的变化，供客户端显示伤害和治疗用
        PACKET_GC_TARGET_LIST_AND_HIT_FLAGS,						//目标列表和击中与否，用于客户端演示子弹和击中与否
        PACKET_GC_PACKAGE_SWAPITEM,									//交换包裹里的两个物品的结果
        PACKET_GC_STALLAPPLY,										//服务器通知客户端申请在此处摆摊
        /* ID:150 */
        PACKET_GC_STALLESTABLISH,							//通知客户端在此处摆摊
        PACKET_GC_STALLOPEN,										//摊位物品列表摊位信息
        PACKET_GC_STALLADDITEM,										//告诉客户端向摊位中加物品
        PACKET_GC_STALLREMOVEITEM,									//通知客户器从摊位中拿出物品
        PACKET_GC_STALLITEMPRICE,									//通知服务器从摊位中的物品改变售价
        PACKET_GC_STALLBUY,											//通知客户端购买店铺物品
        PACKET_GC_STALLCLOSE,										//通知客户端自己关闭店铺
        PACKET_GC_STALLERROR,										//通知摆摊错误
        PACKET_GC_BBSMESSAGES,										//留言板更新
        PACKET_GC_RELATION,											//游戏服务器向客户端发送好友相关消息
        /* ID:160 */
        PACKET_GC_GUILD,									//帮会消息
        PACKET_GC_GUILDLIST,										//返回帮会列表(UCHAR, {GuildID_t,NAME,Level,UserCount,Status})
        PACKET_GC_GUILDERROR,										//返回帮会操作错误(enum)
        PACKET_GC_GUILDRETURN,										//返回帮会操作结果(enum)
        PACKET_GC_GUILDBASEINFO,									//帮会基本信息
        PACKET_GC_GUILDMEMBERS,										//帮众信息
        PACKET_GC_GUILDAUTHORITY,									//帮会权限表
        PACKET_GC_ITEMSYNCH,										//物品移动消息
        PACKET_GC_ITEMLIST,											//发送物品列表给客户端
        PACKET_GC_TEAM_LIST,										//返回队员列表
        /* ID:170 */
        PACKET_GC_NEWSPECIAL,								//创建一个SpecialObj到客户端
        PACKET_GC_UICOMMAND,										//发送客户端的某个用户接口命令
        PACKET_GC_SPLITITEMRESULT,									//拆分物品
        PACKET_GC_BANKITEMINFO,										//银行物品信息
        PACKET_GC_SPECIAL_OBJ_ACT_NOW,								//特殊对象演示特效或动作
        PACKET_GC_SPECIAL_OBJ_FADE_OUT,								//特殊对象死亡或解体
        PACKET_GC_PLAYERSHOPERROR,									//玩家商店的出错信息
        PACKET_GC_PLAYERSHOPAPPLY,									//服务器通知客户端创建玩家商店所需的判定信息
        PACKET_GC_PLAYERSHOPESTABLISH,								//玩家商店是否建立成功
        PACKET_GC_PLAYERSHOPACQUIRESHOPLIST,						//服务器通知客户端创建玩家商店所需的判定信息
        /* ID:180 */
        PACKET_GC_PACKUP_PACKET,							//返回整理背包的结果
        PACKET_GC_PLAYERSHOPONSALE,									//通知客户端商品上下架
        PACKET_GC_PLAYERSHOPMONEY,									//通知客户端金钱存取
        PACKET_GC_RETSETTING,										//向客户端返回用户设置数据
        PACKET_GC_PLAYERSHOPOPENSTALL,								//开张或关闭柜台
        PACKET_GC_REMOVEPET,										//修改用户设置数据
        PACKET_GC_PETPLACARDLIST,									//宠物公告板数据
        PACKET_GC_CITYERROR,										//客户端的城市错误信息
        PACKET_GC_CITYNOTIFY,										//客户端的城市通知
        PACKET_GC_CITYATTR,											//server把城市属性刷到客户端
        /* ID:190 */
        PACKET_GC_NEWPET,									//有新的宠物进入自己的视野范围
        PACKET_GC_NEWPET_DEATH,										//创建死亡的宠物
        PACKET_GC_NEWPET_MOVE,										//创建移动的宠物
        PACKET_GC_MINORPASSWD,										//二级密码
        PACKET_GC_CHARMOVERESULT,									//移动指令的执行返回
        PACKET_GC_PLAYERSHOPSALEOUT,								//盘出商店
        PACKET_GC_PLAYERSHOPBUYSHOP,								//盘入此店
        PACKET_GC_CHARSTOPACTION,									//停止Action逻辑
        PACKET_GC_GUILDAPPLY,										//申请建立工会
        PACKET_GC_PRIVATEINFO,										//玩家资料（刷给客户端）
        /* ID:200 */
        PACKET_GC_WORLDTIME,								//Server->Client 刷新世界时间
        PACKET_GC_CHARACTION,										//做一个动作
        PACKET_GC_CHARMODIFYACTION,									//干扰动作的执行
        PACKET_GC_PLAYER_CALLOF,									//邀请去某地
        PACKET_GC_FINGER,											//查找玩家
        PACKET_GC_PLAYERSHOPRECORDLIST,								//记录列表
        PACKET_GC_PLAYERSHOPUPDATEPARTNERS,							//更新合伙人列表
        PACKET_GC_PLAYERSHOPUPDATEFAVORITE,							//根据新的收藏家内容刷新商店列表
        PACKET_GC_PLAYERSHOPTYPE,									//通知客户端改变商店类型
        PACKET_GC_PLAYERSHOPSTALLSTATUS,							//向服务器请求改变某个柜台状态
        /* ID:210 */
        PACKET_GC_CHARDOACTION,								//让某角色做一个指定动作
        PACKET_GC_ADDLOCKOBJ,										//返回请求密码保护的结果
        PACKET_GC_NOTIFYRMB,										// 
        PACKET_GC_MISSIONDESC,										//返回任务描述
        PACKET_GC_FIRSTLOGIN,										//玩家第一次登录
        PACKET_GC_SYSTEMMSG,										//Server向Client发送系统消息
        PACKET_GC_MISSIONHAVEDONEFLAG,								//修改任务完成 [2011-6-16] by: cfp+
        PACKET_GC_EQUIPADDSLOTRESULT,		                        //打孔结果 [2011-7-14] by: cfp+
        PACKET_GC_NOTIFYCAMERAANIMATION,						    //// 通知客户端播放动画 [8/4/2011 zzh+]

        /* ID:219 */
        PACKET_GC_BUSADDPASSENGER,		                    //加入一个乘客到BUS [2011-8-12] by: cfp+
        PACKET_GC_BUSMOVE,                                          //BUS开始移动 [2011-8-12] by: cfp+
        PACKET_GC_BUSREMOVEPASSENGER,                               //BUS移除乘客 [2011-8-12] by: cfp+
        PACKET_GC_BUSSTOPMOVE,                                      //BUS停止移动 [2011-8-12] by: cfp+
        PACKET_GC_NEWBUS,                                           //新BUS [2011-8-12] by: cfp+
        PACKET_GC_NEWBUS_MOVE,                                      //新BUS移动 [2011-8-12] by: cfp+
        PACKET_GC_BUSREMOVEALLPASSENGER,                            //BUS移除所有乘客 [2011-8-12] by: cfp+
        PACKET_GC_RET_RECRUITINFO,                                  // G->C 返回活动队伍 [3/19/2012 adomi]
        PACKET_GC_USECHARMRESULT,
        /* ID:228 */
        PACKET_GC_CHARMINFOFLUSH,                                        //角色使用符印的信息
        PACKET_GC_RET_TALISMANBAG,                                  // 返回法宝包信息 [3/28/2012 Adomi]
        PACKET_GC_RESETTALISMANBAGSIZE,                             // 重置法宝相关包大小 [3/31/2012 Adomi]
        PACKET_GC_OPERATETALISMANRESULT,                            // 操作法宝返回结果 [3/31/2012 Adomi]
        PACKET_GC_DETAIL_PET_EQUIPLIST,								// 服务器返回宠物装备列表详细信息 [5/2/2012 Adomi]
        PACKET_GC_OPERATE_PET_EQUIP,                                // 操作宠物装备返回结果 [5/2/2012 Adomi]
        PACKET_GC_MAXCOUNT,											//所有GC包的个数
        /*------------------------GC-------------------------------------------------------------------------------------------*/

        /*------------------------LC-------------------------------------------------------------------------------------------*/
        PACKET_LC_RETLOGIN = 400,									//Login 返回客户端的请求结果
        /* ID:401*/
        PACKET_LC_RETCHARLIST,								//返回角色列表
        PACKET_LC_STATUS,											//返回客户端当排队状态
        PACKET_LC_RETCHARLOGIN,										//请求登陆指定角色
        PACKET_LC_RETCREATECHAR,									//返回创建角色结果
        PACKET_LC_RETDELETECHAR,									//返回删除角色结果
        PACKET_LC_RETCONNECT,										//Login返回Client连接请求结果
        PACKET_LC_MAXCOUNT,											//所有LC包的个数
        /*------------------------LC-------------------------------------------------------------------------------------------*/

        /*------------------------CL-------------------------------------------------------------------------------------------*/
        PACKET_CL_ASKLOGIN = 450,									//客户端请求Login登录
        /* ID:451*/
        PACKET_CL_ASKCHARLIST,								//请求角色列表
        PACKET_CL_ASKCHARLOGIN,										//请求登陆指定角色
        PACKET_CL_ASKCREATECHAR,									//请求创建角色
        PACKET_CL_ASKDELETECHAR,									//请求删除角色
        PACKET_CL_CONNECT,											//Client请求Login连接
        /*------------------------CL-------------------------------------------------------------------------------------------*/

        /*------------------------CG、GW、WG、SS、LB、BL、LW、WL---------------------------------------------------------------*/
        PACKET_CG_TEST = 500,
        /* ID:501 */
        PACKET_CG_CONNECT,									//客户端连接到游戏服务器端后发送的第一个验证消息
        PACKET_CG_HEARTBEAT,										//心跳消息
        PACKET_CG_ENTERSCENE,										//客户端试图进入某个场景时发给服务器的请求消息
        PACKET_CG_LEAVESCENE,										//客户端试图离开某个场景时发给服务器的请求消息
        PACKET_CG_ENVREQUEST,										//客户端请求服务器发送周围环境信息
        PACKET_CG_CHARASKBASEATTRIB,								//获取角色简单属性
        PACKET_CG_CHARASKEQUIPMENT,									//获取简单装备表
        PACKET_CG_REQLEVELUP,										//请求升级
        PACKET_CG_REQMANUALATTR,									//请求点数分配
        PACKET_CG_REQRESETATTR,										//洗点请求
        /* ID:511 */
        PACKET_CG_MYEQUIPMENT,								//废弃
        PACKET_CG_OTHEREQUIPMENT,									//废弃
        PACKET_CG_ASKDETAILATTRIB,									//请求更新当前主角属性
        PACKET_CG_ASKDETAILEQUIPMENTLIST,							//请求更新当前主角装备列表
        PACKET_CG_ASKDETAILSKILLLIST,								//请求更新当前主角技能列表
        PACKET_CG_ASKDETAILSUBEFFECTLIST,							//请求更新当前主角附加属性列表
        PACKET_CG_ASKMYBAGLIST,										//请求刷新当前背包
        PACKET_CG_CHARIDLE,											//通知SERVER进入IDLE状态
        PACKET_CG_CHARJUMP,											//通知SERVER跳跃一次
        PACKET_CG_CHARMOVE,											//通知SERVER移动
        /* ID:521 */
        PACKET_CG_MOVE,									//废弃
        PACKET_CG_CHARUSESKILL,										//通知SERVER使用技能
        PACKET_CG_ATTACK,											//废弃
        PACKET_CG_SKILL,											//废弃
        PACKET_CG_OPENITEMBOX,										//打开Obj_ItemBox
        PACKET_CG_PICKBOXITEM,										//捡取Obj_ItemBox里的一件物品
        PACKET_CG_USEEQUIP,											//装配包裹里的装备
        PACKET_CG_UNEQUIP,											//卸下装备
        PACKET_CG_USEITEM,											//使用道具
        PACKET_CG_DISCARDITEM,										//丢弃包裹里的物品
        /* ID:531 */
        PACKET_CG_DISCARDEQUIP,								//丢弃身上装备
        PACKET_SS_CONNECT,											//服务器之间内网连接时候的第一个通知消息
        PACKET_GS_ASKDATA,											//向源服务器端程序要求得到Player数据
        PACKET_SG_RETDATA,											//向GameServer端程序发送用户数据
        PACKET_GW_ASKUSERDATA,										//向世界数据服务器要求用户资料
        PACKET_WG_RETUSERDATA,										//返回用户资料信息
        PACKET_GW_ASKCHANGESCENE,									//向世界数据服务器发送用户资料，要求转移场景
        PACKET_WG_RETCHANGESCENE,									//返回转移场景结果信息
        PACKET_GW_NOTIFYUSER,										//游戏服务器通知世界服务器用户状态需要改变
        PACKET_WG_NOTIFYUSER,										//世界服务器通知游戏服务器用户状态变化
        /* ID:541 */
        PACKET_CG_COMMAND,									//客户端向服务器发送游戏修改指令
        PACKET_CG_ASKCHANGESCENE,									//客户端请求改变场景
        PACKET_CG_ASKITEMINFO,										//请求某件物品的详细信息
        PACKET_CG_ASKDETAILEQUIPLIST,								//请求自身装备的详细信息
        PACKET_CG_CHAT,												//说话
        PACKET_CG_ASKDETAILXINFALIST,								//请求更新当前主角心法列表
        PACKET_CG_USEGEM,											//使用宝石
        PACKET_CG_REMOVEGEM,										//卸载宝石
        PACKET_CG_CHARDEFAULTEVENT,									//缺省事件
        PACKET_CG_USEEQUIPSKILL,
        /* ID:551 */
        PACKET_CG_USEABILITY,								//有配方的生活技能指令
        PACKET_CG_TEAMINVITE,										//邀请组队
        PACKET_GW_TEAMINVITE,										//
        PACKET_WG_TEAMERROR,										//
        PACKET_WG_TEAMRESULT,										//
        PACKET_WG_NOTIFYTEAMINFO,									//
        PACKET_WG_TEAMASKINVITE,									//
        PACKET_CG_TEAMRETINVITE,									//组队回应
        PACKET_GW_TEAMRETINVITE,									//
        PACKET_WG_TEAMLEADERASKINVITE,								//
        /* ID:561 */
        PACKET_CG_TEAMLEADERRETINVITE,						//队长回应
        PACKET_GW_TEAMLEADERRETINVITE,								//
        PACKET_CG_TEAMLEAVE,										//离队
        PACKET_GW_TEAMLEAVE,										//
        PACKET_CG_TEAMDISMISS,										//解散队伍
        PACKET_GW_TEAMDISMISS,										//
        PACKET_CG_TEAMKICK,											//踢除队员
        PACKET_GW_TEAMKICK,											//
        PACKET_CG_TEAMAPPLY,										//加入队伍申请
        PACKET_GW_TEAMAPPLY,										//
        /* ID:571 */
        PACKET_WG_TEAMASKAPPLY,								//
        PACKET_GW_TEAMRETAPPLY,										//
        PACKET_CG_TEAMRETAPPLY,										//
        PACKET_CG_ASKDETAILABILITYINFO, 							//客户端玩家的生活技能信息
        PACKET_CG_TEAMAPPOINT,										//任命新队长
        PACKET_GW_TEAMAPPOINT,										//
        PACKET_CG_ABILITYGAMBLE,									//生活技能无配方合成指令
        PACKET_CG_ASK_COOLDOWN_UPDATE,								//客户端请求更新冷却时间信息 --现在不用了.
        PACKET_CG_DOEVENT,											//客户端选择事件列表中的某项执行
        PACKET_CG_MISSIONACCEPT,									//接受任务
        /* ID:581 */
        PACKET_CG_MISSIONREFUSE,							//拒绝任务
        PACKET_CG_MISSIONABANDON,									//放弃任务
        PACKET_CG_MISSIONCONTINUE,									//继续任务
        PACKET_CG_MISSIONSUBMIT,									//递交任务
        PACKET_CG_EVENTREQUEST,									//查询一个脚本事件
        PACKET_CG_ASKMISSIONLIST,									//任务列表查询
        PACKET_CG_ASKMAIL,											//
        PACKET_CG_CHANNELCREATE,									//
        PACKET_CG_CHANNELDISMISS,									//
        PACKET_CG_CHANNELINVITE,									//
        /* ID:591 */
        PACKET_CG_MISSIONTRACE,                         // 追踪任务
        PACKET_CG_CHANNELKICK,										//
        PACKET_CG_MAIL,											//
        PACKET_GW_CHANNELCREATE,									//
        PACKET_GW_CHANNELDISMISS,									//
        PACKET_GW_CHANNELINVITE,									//
        PACKET_GW_CHANNELKICK,										//
        PACKET_GW_CHAT,											//
        PACKET_GW_MAIL,											//
        PACKET_WG_CHANNELERROR,									//
        /* ID:601 */
        PACKET_WG_CHANNELRESULT,						//
        PACKET_WG_CHAT,											//
        PACKET_WG_MAIL,											//
        PACKET_CG_GEM_COMPOUND,										//宝石合成消息
        PACKET_GW_ASKMAIL,											// 
        PACKET_WG_NOTIFYMAIL,										//
        PACKET_CG_QUERYITEMBOXSTAGE,								//请求Itembox外观
        PACKET_CG_ASKTEAMINFO,										//请求队伍信息
        PACKET_GW_ASKTEAMINFO,										//请求队伍信息
        PACKET_CG_ASKTEAMMEMBERINFO,								//请求队友信息
        /* ID:611 */
        PACKET_CG_LOCK_TARGET,							//玩家锁定目标
        PACKET_GW_ASKTEAMMEMBERINFO,								//请求队伍信息
        PACKET_WG_TEAMMEMBERINFO,									//返回队友信息
        PACKET_CG_SHOPBUY,											//玩家从商品列表选择购买的物品
        PACKET_CG_SHOPSELL,											//玩家从商品列表选择要卖的物品
        PACKET_CG_SHOPREPAIR,										//通知服务器修理物品
        PACKET_CG_SHOPCLOSE,										//通知客户端交易窗口关闭关闭
        PACKET_CG_ASKJOINMENPAI,									//加入门派
        PACKET_CG_ASKSTUDYXINFA,									//请求学习某个某级心法
        PACKET_CG_ASKSTUDYABILITY,									//向服务器提交请求学习生活技能的消息
        /* ID:621 */
        PACKET_CG_CHARALLTITLES,						//客户端向服务器请求此人所有的称号
        PACKET_CG_CHARUPDATECURTITLE,								//服务器通知客户端此人所有的称号
        PACKET_CG_ASK_ACTIVETIME_UPDATE,							//客户端请求更新技能有效时间信息
        PACKET_GW_TEAMMEMBERENTERSCENE,								//队员进入新场景
        PACKET_CG_BANKACQUIRELIST,									//向服务器获取存储物列表
        PACKET_CG_BANKADDITEM,										//通知服务器往银行中加物品
        PACKET_CG_BANKREMOVEITEM,									//通知服务器从银行中移除物品
        PACKET_CG_BANKSWAPITEM,										//通知服务器从银行中与背包或装备栏中交换物品
        PACKET_CG_BANKMONEY,										//通知服务器存钱取钱
        PACKET_CG_BANKCLOSE,										//通知服务器关闭此人银行
        /* ID:631 */
        PACKET_CG_EXCHANGEAPPLYI,						//交易答复
        PACKET_CG_EXCHANGEREPLYI,									//交易答复请求
        PACKET_CG_EXCHANGESYNCHLOCK,								//交易同步锁
        PACKET_CG_EXCHANGESYNCHITEMII,								//交易同步物品
        PACKET_CG_EXCHANGESYNCHMONEYII,								//交易同步金钱
        PACKET_CG_EXCHANGEOKIII,									//交易同意消息
        PACKET_CG_EXCHANGECANCEL,									//交易取消
        PACKET_GW_LEVEUP,											//队员升级，新级别刷新到 World 备考
        PACKET_CG_PLAYER_DIE_RESULT,								//主角死亡的操作结果
        PACKET_GW_ASKSCENEDATA,										//游戏场景向世界数据服务器请求场景附加信息
        /* ID:641 */
        PACKET_WG_RETSCENEDATA,							//世界数据服务器向场景发送场景附加信息
        PACKET_GG_SCENENOTIFY,										//场景通知消息
        PACKET_CG_SET_PETATTRIB,									//通知服务器，更改宠物属性
        PACKET_CG_MANIPULATEPET,									//操作宠物请求,如召唤,收回,放生等操作
        PACKET_CG_ASK_TEAMFOLLOW,									//队长请求进入组队跟随
        PACKET_CG_STOP_TEAMFOLLOW,									//某队员（长）停止队伍跟随状态
        PACKET_CG_RETURN_TEAMFOLLOW,								//队员回复是否进入组队跟随状态
        PACKET_GW_ENTER_TEAMFOLLOW,									//某队员进入队伍跟随状态
        PACKET_GW_STOP_TEAMFOLLOW,									//某队员停止队伍跟随状态
        PACKET_WG_TEAMFOLLOW_LIST,									//发送组队跟随列表
        /* ID:651 */
        PACKET_CG_MISSIONCHECK,							//验证任务是否已经完成
        PACKET_CG_DISPEL_BUFF,										//客户端请求取消持续性效果
        PACKET_CG_CHAR_STOP_LOGIC,									//客户端请求中止当前逻辑
        PACKET_CG_PACKAGE_SWAPITEM,									//要求交换包裹里的两个物品
        PACKET_CG_STALLAPPLY,										//向服务器申请在此处摆摊
        PACKET_CG_STALLESTABLISH,									//客户端接受摊位费与税费
        PACKET_CG_STALLOPEN,										//向服务器请求摊位物品列表
        PACKET_CG_STALLADDITEM,										//通知服务器向摊位中加物品
        PACKET_CG_STALLREMOVEITEM,									//通知服务器从摊位中拿出物品
        PACKET_CG_STALLITEMPRICE,									//通知服务器从摊位中的物品改变售价
        /* ID:661 */
        PACKET_CG_STALLSHOPNAME,						//通知服务器店铺改名
        PACKET_CG_STALLBUY,											//通知服务器购买店铺物品
        PACKET_CG_STALLCLOSE,										//通知服务器自己关闭店铺
        PACKET_CG_BBSSYCHMESSAGES,									//留言板更新
        PACKET_CG_BBSAPPLY,											//留言板更新
        PACKET_CG_RELATION,											//客户端向游戏服务器发送好友相关消息
        PACKET_GW_RELATION,											//游戏服务器向世界数据服务器发送好友相关消息
        PACKET_WG_RELATION,											//世界数据服务器向游戏服务器发送好友相关消息
        PACKET_CG_GUILD,											//帮会消息
        PACKET_GW_GUILD,											//帮会消息
        /* ID:671 */
        PACKET_WG_GUILD,								//帮会消息
        PACKET_WG_GUILDLIST,										//返回帮会列表(PlayerID_t, UCHAR, {GuildID_t,NAME,Level,UserCount,Status})
        PACKET_WG_GUILDERROR,										//返回帮会操作错误(PlayerID_t, enum)
        PACKET_WG_GUILDRETURN,										//返回帮会操作结果(PlayerID_t, enum)
        PACKET_WG_GUILDBASEINFO,									//帮会基本信息
        PACKET_WG_GUILDMEMBERS,										//帮众信息
        PACKET_WG_GUILDAUTHORITY,									//帮会权限表
        PACKET_CG_ITEMSYNCH,										//物品移动消息
        PACKET_LB_ASKAUTH,											//Login 请求BillingSystem验证
        PACKET_BL_RETAUTH,											//BillingSystem 返回验证结果
        /* ID:681 */
        PACKET_LW_ASKCHARLOGIN,							//请求角色登录
        PACKET_WL_RETCHARLOGIN,										//World允许角色登录
        PACKET_WG_TEAM_LIST,										//返回队员列表
        PACKET_CG_EXECUTESCRIPT,									//请求执行服务器端的某个脚本函数
        PACKET_SS_SCENEPLAYERCOUNT,									//世界数据服务器通知游戏服务器和登陆服务器场景中的人数信息
        PACKET_CG_SPLITITEM,										//拆分物品
        PACKET_CG_PLAYERSHOPAPPLY,									//向服务器申请创建玩家商店
        PACKET_CG_PLAYERSHOPESTABLISH,								//客户确定判定信息，递交最后申请
        PACKET_CG_PLAYERSHOPACQUIRESHOPLIST,						//向服务器申请创建玩家商店
        PACKET_CG_PLAYERSHOPACQUIREITEMLIST,						//客户确定判定信息，递交最后申请
        /* ID:691 */
        PACKET_GW_COMMAND,								//需要世界数据服务器处理的GM指令
        PACKET_CG_PACKUP_PACKET,									//请求整理背包
        PACKET_CG_PLAYERSHOPONSALE,									//通知服务器商品上下架
        PACKET_CG_PLAYERSHOPBUYITEM,								//通知服务器购买商品
        PACKET_CG_PLAYERSHOPMONEY,									//通知服务器金钱存取
        PACKET_CG_ASKSETTING,										//请求得到用户设置数据
        PACKET_CG_MODIFYSETTING,									//修改用户设置数据
        PACKET_CG_PLAYERSHOPNAME,									//修改玩家商店店名
        PACKET_CG_PLAYERSHOPDESC,									//修改玩家商店的描述
        PACKET_CG_PLAYERSHOPOPENSTALL,								//开张或关闭柜台
        /* ID:701 */
        PACKET_GW_BATCHMAIL,							//发送批量消息
        PACKET_GW_CITYAPPLYNEWCITY,									//向World请求申请建立新的城市
        PACKET_WG_CITYAPPLYNEWCITY,									//回复Server请求申请建立新的城市
        PACKET_WG_CITYERROR,										//回复Server城市错误信息
        PACKET_WG_CITYATTR,											//world刷新城市属性
        PACKET_CG_CITYASKATTR,										//向服务器请求城市属性
        PACKET_GW_CITYASKINITINFO,									//启动时向服务器申请城市初始化信息
        PACKET_GW_CITYCLOSE,										//关闭城市
        PACKET_GW_CITYCLOSESUCCESS,									//关闭城市成功
        PACKET_WG_CITYCLOSE,										//关闭城市成功
        /* ID:711 */
        PACKET_WG_CITYCLOSESUCCESS,						//关闭城市成功
        PACKET_WG_CITYINITINFO,										//请求城市信息
        PACKET_GW_CITYOPT,											//城市操作
        PACKET_CG_ISSUEPETPLACARD,									//发布宠物公告
        PACKET_CG_STALLFIRSTPAGE,									//摆摊的初始页面
        PACKET_CG_MINORPASSWD,										//二级密码
        PACKET_CG_CHARPOSITIONWARP,									//位置偏差发往服务器
        PACKET_CG_PLAYERSHOPSALEOUT,								//盘出商店
        PACKET_CG_PLAYERSHOPBUYSHOP,								//盘入此店
        PACKET_CG_GUILDAPPLY,										//申请建立工会
        /* ID:721 */
        PACKET_CG_GUILDJOIN,							//申请加入工会
        PACKET_CG_GUILDASKMEMBERINFO,								//察看工会成员
        PACKET_GW_CHANGEMENPAI,										//改变门派
        PACKET_GW_UPDATETITLE,										//更新称号
        PACKET_CG_ASKPRIVATEINFO,									//玩家资料（请求）
        PACKET_CG_APPLYPRIVATEINFO,									//玩家修改玩家资料
        PACKET_WG_WORLDTIME,										//World->Server 刷新世界时间
        PACKET_CG_CHARSIT,											//玩家坐下
        PACKET_CG_PLAYER_CALLOF_RESULT,								//邀请去某地的操作结果
        PACKET_GW_CALLOFHUMAN,										//拉人的消息
        /* ID:731 */
        PACKET_WG_CALLOFHUMAN,							//拉人的消息
        PACKET_CG_STALL_DEFAULT_PAGE,								//摆摊缺省页面提交
        PACKET_CG_SETMOODTOHEAD,									//设置心情显示
        PACKET_CG_FINGER,											//查找玩家
        PACKET_GW_FINGER,											//查找玩家的结果
        PACKET_WG_FINGER,											//查找玩家的结果
        PACKET_CG_PLAYERSHOPASKFORRECORD,							//请求玩家商店交易记录，或管理记录
        PACKET_CG_PLAYERSHOPPARTNER,								//合作伙伴
        PACKET_CG_PLAYERSHOPFAVORITE,								//加入或删除收藏夹
        PACKET_CG_PLAYERSHOPTYPE,									//请求服务器改变商店类型
        /* ID:741 */
        PACKET_CG_PLAYERSHOPSIZE,						//向服务器请求改变商店大小
        PACKET_CG_CGASKLOCKOBJ,										//请求锁定一个Obj
        PACKET_CG_CHARMOODSTATE,									// 表情事件
        PACKET_GW_HEARTBEAT,										//发送玩家的心跳消息给World
        PACKET_CG_MISSIONDESC,										//得到任务描述
        PACKET_CG_FIRSTLOGIN,
        PACKET_LW_ASKDELETECHAR,									//请求删除角色
        PACKET_WL_RETDELETECHAR,									//World允许删除角色
        PACKET_BW_CONNECT,											//Web服务器与Billing服务器第一次验证KEY的消息
        PACKET_BW_VALIDATEUSER,										//Billing服务器请求Web验证用户名与密码
        /* ID:751 */
        PACKET_WB_RET_VALIDATEUSER,						//Billing服务器请求Web验证用户名与密码
        PACKET_WG_SYSTEMMSG,										//World向Server发送系统消息
        PACKET_GW_SYSTEMMSG,										//Server向World发送系统消息
        PACKET_CG_EQUIPADDSLOT,				                        //装备打孔 [2011-7-14] by: cfp+
        PACKET_CG_NOTIFYCAMERAANIMATIONOVER,						//// 客户端通知播放动画完毕 [8/4/2011 zzh+]
        PACKET_CG_CHARSETMASK,                                      //设置新手引导掩码 [2011-8-10] by: cfp+
        PACKET_CG_CHARUPDATECURPKMODE,								//// 客户端通知服务器 SET PK MODE [8/4/2011 zzh+]
        PACKET_GW_UPDATEPKMODE,										//// 更新PK MODE [8/4/2011 zzh+]
        PACKET_CG_TEAMALLOCATIONRULER,								//// 设置分配规则[8/23/2011 zzh+]
        PACKET_GW_TEAMALLOCATIONRULER,								//// 设置分配规则[8/23/2011 zzh+]
        /* ID:761 */
        PACKET_CG_TEAMRETALLOCATIONRULER,				//// 设置分配规则[8/23/2011 zzh+]
        PACKET_GW_TEAMRETALLOCATIONRULER,							//// 设置分配规则[8/23/2011 zzh+]
        PACKET_CG_ENTEROREXITBUS,                                   //乘客上下车 [2011-8-26] by: cfp+
        PACKET_GW_AUTOTEAM,                                         //G->W报名自动组队 [2011-11-15] by: cfp+
        PACKET_WG_RET_AUTO_TEAM,                                    //W->G返回自动组队的信息 [2011-11-15] by: cfp+
        PACKET_CG_ASKCLEARCDTIME,									//// CLEAR CD TIME[8/23/2011 zzh+]
        PACKET_CG_REQRANDOMATTR,									//// 随机加点 [02/13/2012 zzh+]
        PACKET_GW_ASK_RECRUITINFO,                                  // 参加活动，退出活动，自建队伍，自动加入队伍的处理 [3/19/2012 adomi]
        PACKET_WG_RET_RECRUITINFO,                                  // W->G 返回活动队伍征兵的处理 [3/19/2012 adomi]
        PACKET_WG_NOTIFYCHANGESCENE,                                // 由world发起的，通知某用户开始切换场景 [3/21/2012 Adomi]

        /* ID:771 */
        PAKCKET_CG_USESYMBOL,
        PACKET_CG_ASKTALISMANBAG,                                   // 请求法宝包信息 [3/28/2012 Adomi]
        PACKET_CG_OPERATETALISMAN,
        PACKET_CG_ASK_FLUSH_CHARM_INFO,                             // 请求符印包信息 [4/1/2012 Ranger]
        PACKET_WG_TEAMASKRTS,									////// RTS风格，进入战场副本 [3/30/2011 zzh+]
        PACKET_CG_ASKDETAIL_PET_EQUIPLIST,                          // 请求宠物装备列表 [5/2/2012 Adomi]
        PACKET_CG_OPERATE_PET_EQUIP,                                // 宠物装备的操作：穿，脱 [5/2/2012 Adomi]
        /*------------------------CG、GW、WG、SS、LB、BL、LW、WL---------------------------------------------------------------*/
        PACKET_MAX													//消息类型的最大值
    };
}
