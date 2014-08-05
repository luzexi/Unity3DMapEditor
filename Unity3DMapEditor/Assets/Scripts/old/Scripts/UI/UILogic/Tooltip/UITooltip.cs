using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Interface;

public class UITooltip : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActiveRecursively(false);
		
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_SUPERTOOLTIP, TooltipChanged);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_SUPERTOOLTIP, TooltipChanged);
    }

    public void TooltipChanged(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_SUPERTOOLTIP)
        {
            if (vParam.Count != 0)
            {
                if (vParam[0] == "1")
                {
                    float x = float.Parse(vParam[2]);
                    float y = float.Parse(vParam[3]);
                    ShowTooltip(x, y);
                    
                }
                else if (vParam[0] == "0")
                    HideTooltip();
                else
                {
                    showText = vParam[0];
                    Vector2 ptMouse = GameProcedure.s_pInputSystem.GetMouseUIPos();
                    ShowTooltip(ptMouse.x, ptMouse.y);
                }
            }
            
        }
        else if (eventId == GAME_EVENT_ID.GE_UPDATE_SUPERTOOLTIP)
        {
            UpdateInfo();
        }
    }

    private void UpdateInfo()
    {
        if (SuperTooltips.Instance.GetActionItem() == null)
        {
            if (showText == "")
                return;
            else
                UpdateTextInfo();
        }
        else if (SuperTooltips.Instance.GetActionItem().GetType() == ACTION_OPTYPE.AOT_ITEM)
        {
            if (SuperTooltips.Instance.GetActionItem().GetImpl() is CObject_Item_Equip)
                UpdateEquipInfo();
            else
                UpdateItemInfo();
        }
        else if (SuperTooltips.Instance.GetActionItem().GetType() == ACTION_OPTYPE.AOT_SKILL )
        {
            // 心法培养和技能区别显示 [5/9/2012 SUN]
            CActionItem_Skill skill = SuperTooltips.Instance.GetActionItem() as CActionItem_Skill;
            if (skill.IsJingJieSkill())
                UpdateJingJieSkillnfo();
            else
                UpdateSkillInfo();
        }
        else if (SuperTooltips.Instance.GetActionItem().GetType() == ACTION_OPTYPE.AOT_TALISMAN)
        {
            UpdateTalismanItemInfo();
        }
		else if(SuperTooltips.Instance.GetActionItem().GetType() == ACTION_OPTYPE.AOT_PET_SKILL )
		{
			UpdatePetSkillInfo();
		}

    }

    private void HideTooltip()
    {
        showText = "";
        gameObject.SetActiveRecursively(false);
    }
	
	
	
    private void ShowTooltip(float x, float y)
    {
        gameObject.transform.position = new Vector3(x, y, UIZDepthManager.TooltipZ);
        SuperTooltips.Instance.SendAskItemInfoMsg();
        gameObject.active = true;
        ClearAll();
        UpdateInfo();
    }

    public GameObject equipGo;
    public GameObject skillGo;
    public GameObject itemGo;
    public GameObject textGo;
    public GameObject talismanGo;
    public GameObject petSkillGo;
    public GameObject xinfaSkill;
    private void ClearAll()
    {
        if (equipGo != null)
            equipGo.SetActiveRecursively(false);
        if (skillGo != null)
            skillGo.SetActiveRecursively(false);
        if (itemGo != null)
            itemGo.SetActiveRecursively(false);
        if (textGo != null)
            textGo.SetActiveRecursively(false);
        if(talismanGo != null)
            talismanGo.SetActiveRecursively(false);
        if(petSkillGo != null)
            petSkillGo.SetActiveRecursively(false);
        if (xinfaSkill != null)
            xinfaSkill.SetActiveRecursively(false);
    }

    public UIButton equipAction;
    public SpriteText equipName;
    public SpriteText equipStrongLv;
    public SpriteText equipReqLv;
    public SpriteText equipType;
    public SpriteText equipReqJob;
    public SpriteText equipAttInfo;
    public SpriteText[] equipGemInfos =new SpriteText[4];
    public SpriteText equipSelPrice;

    string[] jobNames = {"游侠","剑仙","方士" ,"武圣"};
    void UpdateEquipInfo()
    {
        CActionItem_Item equip = SuperTooltips.Instance.GetActionItem() as CActionItem_Item;
        if (equip != null)
        {
            equipAction.SetTexture(IconManager.Instance.GetIcon(equip.GetIconName()));

            equipName.Text = equip.GetName();
            equipStrongLv.Text = "强化等级：" + equip.GetStrengthLevel();
            equipReqLv.Text = equip.GetItemLevel().ToString();
            if (equip.GetNeedJob() >= 0 && equip.GetNeedJob() <= 3)
                equipReqJob.Text = jobNames[equip.GetNeedJob()];
            else
                equipReqJob.Text = "无限制";
            equipType.Text = equip.GetEquipType();
            equipAttInfo.Text = UIString.Instance.ParserString_Runtime(equip.GetAttributeValue(ITEMATTRIBUTE.ITEMATTRIBUTE_WHITE_ATT));

            // 获得宝石属性 [3/29/2012 Ivan]
            CObject_Item_Equip itemEquip = equip.GetImpl() as CObject_Item_Equip;
            if (itemEquip != null)
            {
                int i = 0;
                for (; i < itemEquip.GetGemCount(); i++)
                {
                    string info = "";
                    itemEquip.GetGemExtAttr(i, ref info);
                    if(string.IsNullOrEmpty(info))
                        equipGemInfos[i].Text = "RGBA(0.56, 0.56, 0.56, 1.000)未镶嵌";
                    else
                        equipGemInfos[i].Text = "RGBA(0.48, 0.917, 0.34, 1.000)" + UIString.Instance.ParserString_Runtime(info);
                }

            }

            equipSelPrice.Text = equip.GetAttributeValue(ITEMATTRIBUTE.ITEMATTRIBUTE_PRICE);

            equipGo.SetActiveRecursively(true);
        }
    }


    public UIButton itemAction;
    public SpriteText itemName;
    public SpriteText itemReqLevel;
    public SpriteText itemDesc;
    void UpdateItemInfo()
    {
        CActionItem_Item item = SuperTooltips.Instance.GetActionItem() as CActionItem_Item;
        if (item != null)
        {
            itemAction.SetTexture(IconManager.Instance.GetIcon(item.GetIconName()));
            itemName.Text = item.GetName();
            itemReqLevel.Text = item.GetItemLevel().ToString();
            itemDesc.Text = UIString.Instance.ParserString_Runtime(item.GetDesc());

            itemGo.SetActiveRecursively(true);
        }
    }

    public UIButton skillAction;
    public SpriteText skillName;
    public SpriteText skillLevel;
    public SpriteText skillReqLevel;
    public SpriteText skillDesc;

    void UpdateSkillInfo()
    {
        CActionItem_Skill skill = SuperTooltips.Instance.GetActionItem() as CActionItem_Skill;
        if (skill != null)
        {
            skillAction.SetTexture(IconManager.Instance.GetIcon(skill.GetIconName()));
            skillName.Text = skill.GetName();
            skillLevel.Text = skill.GetSkillLevel().ToString() + "级";
            skillReqLevel.Text = skill.GetNextRoleLevel().ToString();
            skillDesc.Text = UIString.Instance.ParserString_Runtime(skill.GetDesc());

            if (gameObject.transform.position.y<-160)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, -160, gameObject.transform.position.z);
            }
            skillGo.SetActiveRecursively(true);
        }
    }

    public UIButton     petSkillAction;
    public SpriteText   petSkillName;
    public SpriteText   petSkillDesc;
    public SpriteText   petSkillType;
    string getPetSkillTypeDesc(int type)
    {
        switch (type)
        {
            case 0:
                return "物功";
            case 1:
                return "法功";
            case 2:
                return "护主";
            case 3:
                return "防御";
            case 4:
                return "复仇";
        }
        return "无描述";
    }

	void UpdatePetSkillInfo()
	{
		CActionItem_PetSkill skill = SuperTooltips.Instance.GetActionItem() as CActionItem_PetSkill;
        if (skill != null)
        {
            petSkillAction.SetTexture(IconManager.Instance.GetIcon(skill.GetIconName()));
            petSkillName.Text = skill.GetName();
            petSkillDesc.Text = UIString.Instance.ParserString_Runtime(skill.GetDesc());
            petSkillType.Text = "技能类型：" + getPetSkillTypeDesc(skill.GetPetSkillType()); 
            if (gameObject.transform.position.y<-160)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, -160, gameObject.transform.position.z);
            }
            petSkillGo.SetActiveRecursively(true);
        }
	}

    public UIButton   talismanAction;
    public SpriteText talismanName;
    public SpriteText talismanNameLevel;
    public SpriteText talismanNameExp;
    public SpriteText talismanNameDesc;
    public SpriteText talismanPrice;
    private void UpdateTalismanItemInfo()
    {
        CActionItem_Talisman item = SuperTooltips.Instance.GetActionItem() as CActionItem_Talisman;
        if (item != null)
        {
            talismanAction.SetTexture(IconManager.Instance.GetIcon(item.GetIconName()));
            talismanName.Text = "法宝名称：" + item.GetFontColor() + item.GetName();
            talismanNameLevel.Text = "等级：" +item.GetLevel().ToString() + " 级";
            talismanNameExp.Text = "经验：" + item.GetCurExp().ToString() + "/" + item.GetNextLVExp().ToString();
            talismanNameDesc.Text = "法宝效果：RGBA(0.91, 0.67, 0.34, 1.000)" + item.GetDesc();
            talismanPrice.Text = "出售价格：" + item.GetSellPrice().ToString(); 
            if (gameObject.transform.position.y < -160)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, -160, gameObject.transform.position.z);
            }
            talismanGo.SetActiveRecursively(true);
        }
    }

    string showText = "";
    public SpriteText textDesc;
    void UpdateTextInfo()
    {
        if (showText != "")
        {
            textDesc.Text = UIString.Instance.ParserString_Runtime(showText);
            textGo.SetActiveRecursively(true);
        }
    }
    //public UIButton xinfaAction;
    public SpriteText xinfaName;
    public SpriteText xinfaDesc;
    public SpriteText xinfaLevel;
    public SpriteText xinfaRequire;
    void UpdateJingJieSkillnfo()
    {
        CActionItem_Skill xinfa = SuperTooltips.Instance.GetActionItem() as CActionItem_Skill;
        if (xinfa != null)
        {
            //xinfaAction.SetTexture(IconManager.Instance.GetIcon(xinfa.GetIconName()));
            xinfaName.Text = xinfa.GetName();
            xinfaDesc.Text = UIString.Instance.ParserString_Runtime(xinfa.GetDesc1()) ;
            xinfaLevel.Text = xinfa.GetSkillLevel().ToString() + "级";
            xinfaRequire.Text = UIString.Instance.ParserString_Runtime(xinfa.GetUpLevelDesc());

            xinfaSkill.SetActiveRecursively(true);
        }
    }
}