using UnityEngine;
using System.Collections;
using DBSystem;
using System.Text;

public enum DataBaseStruct
{
    DBC_CHAR_RACE = 0, //种族
    DBC_CHARACTER_MODEL, //人物
    DBC_ITEM_EQUIP_BASE, //装备
    DBC_CHARACTER_MOUNT,//坐骑表
    DBC_CREATURE_ATT, //怪物
    DBC_CHAR_HAIR_GEO, //人物发型GEO
    DBC_CHAR_HEAD_GEO, //人物头部GEO
    DBC_ITEM_VISUAL_CHAR,
    DBC_ITEM_VISUAL_LOCATOR,//装备武器表
	DBC_EFFECT,
    DBC_SKILL_DATA,//技能表
    DBC_DIRECTLY_IMPACT,//伤害表
    DBC_SKILL_DEPLETE,//技能消耗表
    DBC_BULLET_DATA,//子弹表
    DBC_ITEM_MEDIC,//药品
    DBC_ITEM_SYMBOL,//符印
    DBC_SKILLDATA_V1_DEPLETE,//技能说明
    DBC_SPECIAL_OBJ_DATA,//技能特殊物品表
    DBC_SKILL_XINFA,//技能心法表
    DBC_XINFA_REQUIREMENTS, //技能心法需求表
    DBC_BUFF_IMPACT,//BUFF描述
    DBC_SCENE_DEFINE,       //场景定义
	DBC_LIFEABILITY_GROWPOINT, //生长点表
    DBC_LIFEABILITY_DEFINE, //生活技能定义表
    DBC_LIFEABILITY_ITEMCOMPOSE,//生活技能配方表
    DBC_CAMP_AND_STAND,     //阵营表
    DBC_TOTAL_NUMBER,
	DBC_CHAR_FACE,   //主角头像
    DBC_CODE_GB2UNI,//gb2312转Unicode码表
    DBC_UI_LAYOUTDEFINE, //UI组定义
    DBC_ITEM_ENHANCE,  //装备强化
    DBC_STRING_DICT,   //字典
    DBC_XINFA_STUDYSPEND, //心法学习消耗
    DBC_XINFA_RESETCD_SPEND, //心法cd清零消耗
    DBC_MISSION_LIST,  //任务列表
    DBC_MISSION_DATA,  //任务数据
    DBC_MISSION_REWARD,  //任务奖励
    DBC_MISSION_DIALOG,  //任务对话
	DBC_CHARACTER_EXPLEVEL,//主角升级经验
    DBC_NPC_MISSION,   //NPC任务列表
    DBC_ITEM_UPLEVEL,  //装备升档
    DBC_ITEM_GEM,       //宝石
    DBC_SCENE_POS_DEFINE, //场景传送点
    DBC_ACTIVITY_INFO,  //活动定义
    DBC_PET_EX_ATT,//宠物附加属性表
    DBC_PET_LEVELUP,//宠物经验表
    DBC_ITEM_TALISMAN,//宠物附加属性表   
    DBC_MISSION_DEMAND_LIST,// 任务列表 [4/6/2012 Ivan]
    DBC_ITEM_ENCHANCE_RATE, //强化系数
    DBC_DIALOGUE, // 剧情对白 [4/24/2012 Ivan]
    DBC_RANDOM_NAME,// 随机名字配置表 [5/2/2012 Ivan],
    DBC_ITEM_RULE, // 物品规则 [5/4/2012 SUN]
    DBC_XINFA_SKILL_DATA, // 心法数据 [5/9/2012 SUN]
    DBC_FUNC_OPEN_LIST,   // 功能开放配置表 [5/15/2012 Ivan]
    DBC_BUS_INFO,               // Bus配置表 [5/18/2012 Ivan]
};

public class DBStruct
{
    public static _DATABASE_DEFINE[] s_dbToLoad =
	{
		new _DATABASE_DEFINE((int)DataBaseStruct.DBC_CHAR_RACE, typeof(_DBC_CHAR_RACE), "Private/Char/CharRace"),
		new _DATABASE_DEFINE((int)DataBaseStruct.DBC_CHARACTER_MODEL, typeof(_DBC_CHARACTER_MODEL), "Private/Char/CharModelEx"),
		new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ITEM_EQUIP_BASE, typeof(_DBC_ITEM_EQUIP_BASE), "Public/Equip/EquipBase"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_CHARACTER_MOUNT, typeof(_DBC_CHARACTER_MOUNT), "Private/Char/CharMount"),
        
		new _DATABASE_DEFINE((int)DataBaseStruct.DBC_CREATURE_ATT, typeof(_DBC_CREATURE_ATT), "Public/Monster/MonsterAttrExTable"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_NPC_MISSION, typeof(_DBC_NPC_MISSION), "Public/Monster/NpcEvent"),
		new _DATABASE_DEFINE((int)DataBaseStruct.DBC_CHAR_HAIR_GEO, typeof(_DBC_CHAR_HAIR_GEO), "Public/Char/CharHairGeo"),
		new _DATABASE_DEFINE((int)DataBaseStruct.DBC_CHAR_HEAD_GEO, typeof(_DBC_CHAR_HEAD_GEO), "Private/Char/CharFaceGeo"),
		new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ITEM_VISUAL_CHAR,typeof(_DBC_ITEM_VISUAL_CHAR),"Private/Equip/ItemVisualChar"),
		new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ITEM_VISUAL_LOCATOR,typeof(_DBC_ITEM_VISUAL_LOCATOR),"Private/Equip/ItemVisualLocator"),
		new _DATABASE_DEFINE((int)DataBaseStruct.DBC_SKILL_DATA,typeof(_DBC_SKILL_DATA),"Public/Skill/SkillTemplate_V1"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_DIRECTLY_IMPACT,typeof(_DBC_DIRECT_IMPACT),"Private/Skill/ImpactDirectly"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_SKILL_DEPLETE,typeof(_DBC_SKILL_DEPLETE),"Public/Skill/SkillDeplete"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_SPECIAL_OBJ_DATA,typeof(_DBC_SPECIAL_OBJ_DATA),"public/Skill/SpecialObjData"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_CAMP_AND_STAND,typeof(_DBC_CAMP_AND_STAND),"Public/Char/CampAndStand"),
		new _DATABASE_DEFINE((int)DataBaseStruct.DBC_CHAR_FACE,typeof(_DBC_CHAR_FACE),"Private/Char/CharacterFace"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_CHARACTER_EXPLEVEL,typeof(_DBC_CHARACTER_EXPLEVEL),"Public/Char/PlayerExpLevel"),
        //待实现读取结构体的功能
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_SKILLDATA_V1_DEPLETE,typeof(_DBC_SKILLDATA_V1_DEPLETE),"Public/Skill/SkillData_V1"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_SKILL_XINFA,typeof(_DBC_SKILL_XINFA),"Public/Skill/XinFa_V1"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_BULLET_DATA,typeof(_DBC_BULLET_DATA),"Private/Skill/Bullet"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_XINFA_REQUIREMENTS,typeof(_DBC_XINFA_REQUIREMENTS),"Public/Skill/XinFaStudyRequirement"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_BUFF_IMPACT,typeof(_DBC_BUFF_IMPACT),"Public/Skill/ImpactSEData_V1"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ITEM_MEDIC,typeof(_DBC_ITEM_MEDIC),"Public/Item/CommonItem"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ITEM_SYMBOL,typeof(_DBC_ITEM_SYMBOL),"Public/Item/charminfo"),
        
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_SCENE_DEFINE, typeof(_DBC_SCENE_DEFINE), "Private/Scene/SceneDefineEx"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_CODE_GB2UNI, typeof(EncodeBase), "public/I18N/gb2Unicode"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_UI_LAYOUTDEFINE, typeof(_DBC_UI_LAYOUTDEFINE), "Private/system/InterfaceEx"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_EFFECT, typeof(_DBC_EFFECT), "Private/system/Effect"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ITEM_ENHANCE, typeof(_DBC_ITEM_ENHANCE), "Public/Equip/EquipEnhance"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_LIFEABILITY_GROWPOINT, typeof(_DBC_LIFEABILITY_GROWPOINT), "Public/Item/GrowPoint"),/*生长点表*/
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_LIFEABILITY_DEFINE, typeof(_DBC_LIFEABILITY_DEFINE), "Public/Ability/Ability"),/*生活技能表*/
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_LIFEABILITY_ITEMCOMPOSE, typeof(_DBC_LIFEABILITY_ITEMCOMPOSE), "Public/Item/ItemCompound"),/*生活技能配方表*/
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_STRING_DICT, typeof(_DBC_STRING_DICT), "Private/system/StrDictionary"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_XINFA_STUDYSPEND, typeof(_DBC_XINFA_STUDYSPEND), "Public/Skill/XinFaStudySpend_V1"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_XINFA_RESETCD_SPEND, typeof(_DBC_XINFA_RESETCD_SPEND), "Public/Skill/CDClearSpend_V1"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_MISSION_LIST, typeof(_DBC_MISSION_LIST), "Public/Mission/MissionList"),/*生长点表*/
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_MISSION_DATA, typeof(_DBC_MISSION_DATA), "Public/Mission/MissionData"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_MISSION_REWARD, typeof(_DBC_MISSION_REWARD), "Public/Mission/MissionReward"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_MISSION_DIALOG, typeof(_DBC_MISSION_DIALOG), "Public/Mission/MissionDialog"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ITEM_UPLEVEL, typeof(_DBC_ITEM_UPLEVEL), "Private/Equip/EquipLevelUp"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ITEM_GEM, typeof(_DBC_ITEM_GEM), "Public/Item/GemInfo"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_SCENE_POS_DEFINE, typeof(_DBC_SCENE_POS_DEFINE), "Private/Scene/ScenePosDefine"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ACTIVITY_INFO, typeof(_DBC_ACTIVITY_INFO), "Private/system/ActivityList"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_PET_EX_ATT,typeof(_DBC_PET_EX_ATT),"public/Pet/PetAttrTable"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_PET_LEVELUP,typeof(_DBC_PET_LEVELUP),"public/Pet/PetLevelUpTable"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ITEM_TALISMAN,typeof(_DBC_ITEM_TALISMAN),"public/item/TalismanInfo"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_MISSION_DEMAND_LIST,typeof(_DBC_MISSION_DEMAND_LIST),"Private/Mission/AllMission"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ITEM_ENCHANCE_RATE, typeof(_DBC_ITEM_ENCHANCE_RATE), "Public/Equip/EquipEnhanceRate"),
        
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_DIALOGUE, typeof(_DBC_Dialogue), "Private/Mission/JuQingDialog"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_RANDOM_NAME, typeof(_DBC_RANDOM_NAME), "Private/system/RandomName"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_ITEM_RULE, typeof(_DBC_ITEM_RULE), "Public/Item/ItemRule"),
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_XINFA_SKILL_DATA, typeof(_DBC_XINFA_SKILL_DATA), "Private/skill/Xinfa_Data"),
        
        new _DATABASE_DEFINE((int)DataBaseStruct.DBC_FUNC_OPEN_LIST, typeof(_DBC_FUNC_OPEN_LIST), "Private/system/FuncOpenList"),
        //new _DATABASE_DEFINE((int)DataBaseStruct.DBC_BUS_INFO, typeof(_BUS_INFO), "Public/Bus/BusInfo"),
	};


    // 注意，路径不要带后缀名 [3/17/2012 Ivan]
    public static string GetResources(string fileName)
    {
        fileName = fileName.Replace("\\","/");
        if (GameProcedure.Version == CurrentVersion.Debug)
        {
            Object obj = Resources.Load(fileName);
            TextAsset asset = obj as TextAsset;
            if (asset != null)
                return asset.text;
            else
                return "";
        }
        else
        {
            return LoadConfig(fileName);
        }
    }

    static string LoadConfig(string name)
    {
        if (allConfigAsset != null)
        {
            // 不允许带后缀名 [3/17/2012 Ivan]
            //name = name.Replace(".txt", "");
            string[] names = name.Split('/');
            object configFile = allConfigAsset.Load(names[names.Length -1]);
            TextAsset text = configFile as TextAsset;
            if (text != null)
                return text.text;
        }
        return "";
    }

    // 保存所有的配置文件 [3/16/2012 Ivan]
    static AssetBundle allConfigAsset = null;
    public static UnityEngine.AssetBundle AllConfigAsset
    {
        get { return allConfigAsset; }
        set 
        {
            allConfigAsset = value;
        }
    }
}