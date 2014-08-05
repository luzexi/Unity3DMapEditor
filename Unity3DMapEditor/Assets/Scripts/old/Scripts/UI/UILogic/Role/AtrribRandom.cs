using UnityEngine;
using System;
using System.Collections.Generic;
using Interface;

public class AtrribRandom : MonoBehaviour
{

    #region MonoBehaviour
    int activeAtrribIndex_ = 0;//0 为主角，其余为宠物
    UISelfEquip selfEquipt_ = null;
    bool[] needShowRandomValue_ = new bool[7];

    void Start()
    {
        //gameObject.SetActiveRecursively(false);
        //GameObject roleWinGo = UIWindowMng.Instance.GetWindowGo("RoleTipWindow");
        //if (roleWinGo != null)
        //{
        //    gameObject.transform.parent = roleWinGo.transform;
        //    Vector3 pos = Vector3.zero;
        //    pos.x = 84;
        //    gameObject.transform.localPosition = pos;
		//	LogManager.LogError(gameObject.transform.localPosition.ToString());
        //}
        gameObject.SetActiveRecursively(true);
        UpdateAttribute();
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGGLE_ATTRIRANDOM, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_ATTRIRANDOM, OnEvent);
		if(gameObject.active)
		{
			CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_ATTRIRANDOMSHOWN);
		}
    }

    #endregion

    public SpriteText textStr;//力量
    public SpriteText textCon;//体质
    public SpriteText textInt;//韧性
    public SpriteText textDex;//敏捷
    public SpriteText textSpr;//智慧

    public SpriteText textStrRandom;//力量
    public SpriteText textConRandom;//体质
    public SpriteText textIntRandom;//韧性
    public SpriteText textDexRandom;//敏捷
    public SpriteText textSprRandom;//智慧

    public UIButton StrLevelUp;
    public UIButton ConLevelUp;
    public UIButton IntLevelUp;
    public UIButton DexLevelUp;
    public UIButton SprLevelUp;

    public UIButton StrLevelDown;
    public UIButton ConLevelDown;
    public UIButton IntLevelDown;
    public UIButton DexLevelDown;
    public UIButton SprLevelDown;

    public void OnEvent(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        if (gAME_EVENT_ID == GAME_EVENT_ID.GE_TOGGLE_ATTRIRANDOM)
        {
			if (gameObject.active)
            {
                //UIWindowMng.Instance.ShowWindow("PeiYangWindow");
                
                UpdateAttribute();
            }
        }
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UPDATE_ATTRIRANDOM)
        {
            if (gameObject.active)
                UpdateAttribute();
        }
    }

    void SetButtonState(UIButton btnUp, UIButton btnDown, int value1, int value2)
    {
        if (value1 > value2)
        {
            btnUp.Hide(true);
            btnDown.Hide(false);
        }
        else if (value1 < value2)
        {
            btnUp.Hide(false);
            btnDown.Hide(true);
        }
        else
        {
            btnUp.Hide(true);
            btnDown.Hide(true);
        }
    }

    void UpdateAttribute()
    {
        if (getCurActiveIndex() == 0)
        {
            UpdateAvaterAttribute();
        }
        else
        {
            UpdatePetAttribute();
        }
    }

    void UpdateAvaterAttribute()
    {
        _ATTR_LEVEL1 randomValue = CDataPool.Instance.RandomAttrs[0];
        if (randomValue.m_pAttr == null)
            randomValue.m_pAttr = new int[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_NUMBER];

        textStr.Text = PlayerMySelf.Instance.GetData("STR") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_STR");
        UnityEngine.Color fontColor = UnityEngine.Color.green;
        if (isActiveRoleRandomShowValue())
        {
            if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_BringSTR() > randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_STR])
            {
                fontColor = UnityEngine.Color.red;
            }
            else
                fontColor = UnityEngine.Color.green;

            textStrRandom.Hide(false);
            textStrRandom.Text = fontColor + randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_STR].ToString();
            SetButtonState(StrLevelUp,
                       StrLevelDown,
                       CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_BringSTR(),
                       randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_STR]);
        }
        else
        {
            textStrRandom.Hide(true);
            StrLevelUp.Hide(true);
            StrLevelDown.Hide(true);
        }

        textCon.Text = PlayerMySelf.Instance.GetData("CON") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_CON");
        if (isActiveRoleRandomShowValue())
        {
            if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_BringCON() > randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_CON])
                fontColor = UnityEngine.Color.red;
            else
                fontColor = UnityEngine.Color.green;
            textConRandom.Hide(false);
            textConRandom.Text = fontColor + randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_CON].ToString();
            SetButtonState(ConLevelUp,
                           ConLevelDown,
                           CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_BringCON(),
                           randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_CON]);
        }
        else
        {
            textConRandom.Hide(true);
            ConLevelUp.Hide(true);
            ConLevelDown.Hide(true);
        }

        textInt.Text = PlayerMySelf.Instance.GetData("INT") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_INT");
        if (isActiveRoleRandomShowValue())
        {
            if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_BringINT() > randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_INT])
                fontColor = UnityEngine.Color.red;
            else
                fontColor = UnityEngine.Color.green;
            textIntRandom.Hide(false);
            textIntRandom.Text = fontColor + randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_INT].ToString();
            SetButtonState(IntLevelUp,
                           IntLevelDown,
                           CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_BringINT(),
                           randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_INT]);
        }
        else
        {
            textIntRandom.Hide(true);
            IntLevelUp.Hide(true);
            IntLevelDown.Hide(true);
        }

        textSpr.Text = PlayerMySelf.Instance.GetData("SPR") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_SPR");
        if (isActiveRoleRandomShowValue())
        {
            if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_BringSPR() > randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_SPR])
                fontColor = UnityEngine.Color.red;
            else
                fontColor = UnityEngine.Color.green;
            textSprRandom.Hide(false);
            textSprRandom.Text = fontColor + randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_SPR].ToString();
            SetButtonState(SprLevelUp,
                           SprLevelDown,
                           CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_BringSPR(),
                           randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_SPR]);
        }
        else
        {
            textSprRandom.Hide(true);
            SprLevelUp.Hide(true);
            SprLevelDown.Hide(true);
        }

        textDex.Text = PlayerMySelf.Instance.GetData("DEX") + "+" + UnityEngine.Color.green + PlayerMySelf.Instance.GetData("BRING_DEX");
        if (isActiveRoleRandomShowValue())
        {
            if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_BringDEX() > randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_DEX])
                fontColor = UnityEngine.Color.red;
            else
                fontColor = UnityEngine.Color.green;
            textDexRandom.Hide(false);
            textDexRandom.Text = fontColor + randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_DEX].ToString();
            SetButtonState(DexLevelUp,
                           DexLevelDown,
                           CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_BringDEX(),
                           randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_DEX]);
        }
        else
        {
            textDexRandom.Hide(true);
            DexLevelUp.Hide(true);
            DexLevelDown.Hide(true);
        }
    }

    void UpdatePetAttribute()
    {
        _ATTR_LEVEL1 randomValue = CDataPool.Instance.RandomAttrs[getCurActiveIndex()];
        if (randomValue.m_pAttr == null)
            randomValue.m_pAttr = new int[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_NUMBER];
        UnityEngine.Color fontColor = UnityEngine.Color.green;
        SDATA_PET curPet = CDataPool.Instance.Pet_GetValidPet(getCurActiveIndex() - 1);//CDataPool.Instance.Pet_GetPet(getCurActiveIndex() - 1);
        if (curPet == null) return;

        textStr.Text = curPet.AttrStr + " + " + UnityEngine.Color.green + curPet.AttrStrBring;
        if (isActiveRoleRandomShowValue())
        {
            if (curPet.AttrStrBring > randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_STR])
                fontColor = UnityEngine.Color.red;
            else
                fontColor = UnityEngine.Color.green;
            textStrRandom.Hide(false);
            textStrRandom.Text = fontColor + randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_STR].ToString();
            SetButtonState( StrLevelUp,
                            StrLevelDown,
                            curPet.AttrStrBring,
                            randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_STR]);
        }
        else
        {
            textStrRandom.Hide(true);
            StrLevelUp.Hide(true);
            StrLevelDown.Hide(true);
        }

        textCon.Text = curPet.AttrCon + " + " + UnityEngine.Color.green + curPet.AttrConBring;
        if (isActiveRoleRandomShowValue())
        {
            if (curPet.AttrConBring > randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_CON])
                fontColor = UnityEngine.Color.red;
            else
                fontColor = UnityEngine.Color.green;
            textConRandom.Hide(false);
            textConRandom.Text = fontColor + randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_CON].ToString();
            SetButtonState( ConLevelUp,
                            ConLevelDown,
                            curPet.AttrConBring,
                            randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_CON]);
        }
        else
        {
            textConRandom.Hide(true);
            ConLevelUp.Hide(true);
            ConLevelDown.Hide(true);
        }

        textDex.Text =  curPet.AttrDex + " + " + UnityEngine.Color.green + curPet.AttrDexBring;
        if (isActiveRoleRandomShowValue())
        {
            if (curPet.AttrDexBring > randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_DEX])
                fontColor = UnityEngine.Color.red;
            else
                fontColor = UnityEngine.Color.green;
            textDexRandom.Hide(false);
            textDexRandom.Text = fontColor + randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_DEX].ToString();
            SetButtonState( DexLevelUp,
                            DexLevelDown,
                            curPet.AttrDexBring,
                            randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_DEX]);
        }
        else
        {
            textDexRandom.Hide(true);
            DexLevelUp.Hide(true);
            DexLevelDown.Hide(true);
        }


        textSpr.Text = curPet.AttrSpr + " + " + UnityEngine.Color.green + curPet.AttrSprBring;
        if (isActiveRoleRandomShowValue())
        {
            if (curPet.AttrSprBring > randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_SPR])
                fontColor = UnityEngine.Color.red;
            else
                fontColor = UnityEngine.Color.green;
            textSprRandom.Hide(false);
            textSprRandom.Text = fontColor + randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_SPR].ToString();
            SetButtonState(SprLevelUp,
                            SprLevelDown,
                            curPet.AttrSprBring,
                            randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_SPR]);
        }
        else
        {
            textSprRandom.Hide(true);
            SprLevelUp.Hide(true);
            SprLevelDown.Hide(true);
        }

        textInt.Text = curPet.AttrInt + " + " + UnityEngine.Color.green + curPet.AttrIntBring;
        if (isActiveRoleRandomShowValue())
        {
            if (curPet.AttrIntBring > randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_INT])
                fontColor = UnityEngine.Color.red;
            else
                fontColor = UnityEngine.Color.green;
            textIntRandom.Hide(false);
            textIntRandom.Text = fontColor + randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_INT].ToString();
            SetButtonState( IntLevelUp,
                            IntLevelDown,
                            curPet.AttrIntBring,
                            randomValue[(int)CHAR_ATTR_LEVEL1.CATTR_LEVEL1_INT]);
        }
        else
        {
            textIntRandom.Hide(true);
            IntLevelUp.Hide(true);
            IntLevelDown.Hide(true);
        }
    }

    public void OnSaveAttri()
    {
        //LogManager.Log("AtrribRandom: Save Attri");
        setActiveRoleRandomShowValue(false);
        ReqPeiyang(-1);
        PlayerMySelf.Instance.SendAskBringResult((byte)getCurActiveIndex());
 
    }
    public void OnVip0()
    {
        setActiveRoleRandomShowValue(true);
        ReqPeiyang(0);
        //LogManager.Log("AtrribRandom: OnVip0 Attri");
    }
    public void OnVip1()
    {
        setActiveRoleRandomShowValue(true);
        ReqPeiyang(1);
        //LogManager.Log("AtrribRandom: OnVip1 Attri");
    }
    public void OnVip2()
    {
        setActiveRoleRandomShowValue(true);
        ReqPeiyang(2);
        //LogManager.Log("AtrribRandom: OnVip2 Attri");
    }
    public void OnVip3()
    {
        setActiveRoleRandomShowValue(true);
        ReqPeiyang(3);
        //LogManager.Log("AtrribRandom: OnVip3 Attri");
    }
    void ReqPeiyang(sbyte nRank)
    {
        PlayerMySelf.Instance.SendAskRandomAttri((byte)getCurActiveIndex(), nRank);
    }

    int getCurActiveIndex()
    {
        if (selfEquipt_ == null)
        {
            GameObject roletip = UIWindowMng.Instance.GetWindowGo("RoleTipWindow");
            selfEquipt_ = roletip.GetComponent<UISelfEquip>();
        }
        return selfEquipt_.ActivePage;
    }

    bool isActiveRoleRandomShowValue()
    {
        int index = getCurActiveIndex();
        if (index < 0 || index >= needShowRandomValue_.Length)
        {
            return false;
        }
        return needShowRandomValue_[index];
    }

    void setActiveRoleRandomShowValue(bool isShow)
    {
        int index = getCurActiveIndex();
        if (index < 0 || index >= needShowRandomValue_.Length)
        {
            return;
        }
        needShowRandomValue_[index] = isShow;
    }
    void ResetText()
    {
        textStr.Text = "";
        textCon.Text = "";
        textInt.Text = "";
        textDex.Text = "";
        textSpr.Text = "";
    }
    void Close()
    {
        gameObject.SetActiveRecursively(false);
    }
}