using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DBSystem;

public class UICreateRole : MonoBehaviour {

    public UIButton turnLeft;
    public UIButton turnRight;
	public SpriteText ZhiYeName;
	public SpriteText ZhiYeShuoMing;
	ZhiYeType m_Type = ZhiYeType.YOUXIA;
	
	enum ZhiYeType
	{
		YOUXIA,
		JIANXIAN,
		FANGSHI,
		WUSHENG,
	}

    void Awake()
    {
        GameObject parent = UISystem.Instance.UiCamrea.gameObject;
        gameObject.transform.position = parent.transform.position + new Vector3(0, 0, 10);
        //gameObject.transform.parent = parent.transform;
        if (turnLeft == null)
            turnLeft = GameObject.Find("TurnLeft").GetComponent<UIButton>();
        turnLeft.AddInputDelegate(RoteRole);
        if (turnRight == null)
            turnRight = GameObject.Find("TurnRight").GetComponent<UIButton>();
        turnRight.AddInputDelegate(RoteRole);

        gameObject.SetActiveRecursively(false);

        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_UI_INFOS, "LoginMapDownload");

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_GAMELOGIN_OPEN_CREATE_CHARACTOR, OnCreateRole);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_GAMELOGIN_REFRESH_ROLE_SELECT_CHARACTOR, OnCreateRole);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (inputName.Text.Length <=0)
            {
                UIManager.instance.FocusObject = inputName;
            }
            else
            {
                CreateRole();
            }
        }
    }

    void RoteRole(ref POINTER_INFO ptr)
    {
        if (ptr.hitInfo.collider != null && ptr.hitInfo.collider.gameObject != null)
        {
            bool isTurnLef = ptr.hitInfo.collider.gameObject == turnLeft.gameObject;
            if (isTurnLef)
                GameProcedure.s_roteSpeed = 2;
            else
                GameProcedure.s_roteSpeed = -2;
        }

        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.PRESS:
                GameProcedure.s_isRote = true;
                break;
            case POINTER_INFO.INPUT_EVENT.TAP:
            case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
            case POINTER_INFO.INPUT_EVENT.RELEASE:
            case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
                GameProcedure.s_isRote = false;
                break;
            default:
                break;
        }
    }

    public void OnCreateRole(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        if (gAME_EVENT_ID == GAME_EVENT_ID.GE_GAMELOGIN_OPEN_CREATE_CHARACTOR)
        {
            gameObject.SetActiveRecursively(true);

            EZScreenPlacement[] screenPlacements = gameObject.GetComponentsInChildren<EZScreenPlacement>();
            if (screenPlacements != null)
            {
                foreach (EZScreenPlacement screen in screenPlacements)
                {
                    screen.SetCamera(UISystem.Instance.UiCamrea);
                }
            }

            CleanOldSet();

            UIManager.instance.FocusObject = inputName;
        }
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_GAMELOGIN_REFRESH_ROLE_SELECT_CHARACTOR)
        {
            int roleCount = GameProcedure.s_ProcCharSel.GetCurRoleCount();

            if (roleCount != 0)
            {
                Return2SelectRole();
            }
        }
        
        
    }

    public List<UIButton> faceBtns;
    private void UpdateFaces()
    {
        DBC.COMMON_DBC<_DBC_CHAR_FACE> dbs = CDataBaseSystem.Instance.GetDataBase<_DBC_CHAR_FACE>(
                            (int)DataBaseStruct.DBC_CHAR_FACE);
        if (dbs != null)
        {
            int addNum = (1 - GameProcedure.s_ProcCharCreate.Sex) * GamePro_CharCreate.MaxMaleCount;
            for (int i = 0; i < 9; i++)
            {
                _DBC_CHAR_FACE item = dbs.Search_Index_EQU(i+ 1 + addNum);// 配置表从1开始计数 [3/30/2012 Ivan]
                if (item != null)
                {
                    faceBtns[i].SetTexture(IconManager.Instance.GetIcon(item.pImageSetName));
                }
            }
        }
    }
	
	void ShowZhiYeShuoMingTip(ZhiYeType type)
	{
		if(type == ZhiYeType.YOUXIA)
		{
			ZhiYeName.Text = "游 侠：";
			ZhiYeShuoMing.Text = "   擅使弓箭，喜好皮甲，物理攻击，拥有最高的暴击率。";
		}
		else if(type == ZhiYeType.JIANXIAN)
		{
			ZhiYeName.Text = "剑 仙：";
			ZhiYeShuoMing.Text = "   擅长御剑，喜好皮衣，法术攻击，攻击常伴随伤害放大效果。";
		}
		else if(type == ZhiYeType.WUSHENG)
		{
			ZhiYeName.Text = "武 圣：";
			ZhiYeShuoMing.Text = "   擅使双斧，喜好重甲，擅长群攻，攻击常伴随眩晕控制效果。";
		}
		else if(type == ZhiYeType.FANGSHI)
		{
			ZhiYeName.Text = "方 士：";
			ZhiYeShuoMing.Text = "   擅使法杖，喜好布衣，法术攻击，擅长群攻，攻击常伴随减速、失明效果。";
		}
	}

    private void CleanOldSet()
    {
        SelectGong();
        SelectWoman();
        SelectFace1();

        SetRandomName();
    }

    public void SelectGong()
    {
        GameProcedure.s_ProcCharCreate.Menpai = 0;
        GameProcedure.s_ProcCharCreate.OnRoleChanged();
		ShowZhiYeShuoMingTip(ZhiYeType.YOUXIA);
    }
    public void SelectJian()
    {
        GameProcedure.s_ProcCharCreate.Menpai = 1;
        GameProcedure.s_ProcCharCreate.OnRoleChanged();
		ShowZhiYeShuoMingTip(ZhiYeType.JIANXIAN);
    }
    public void SelectZhang()
    {
        GameProcedure.s_ProcCharCreate.Menpai = 2;
        GameProcedure.s_ProcCharCreate.OnRoleChanged();
		ShowZhiYeShuoMingTip(ZhiYeType.FANGSHI);
    }
    public void SelectFu()
    {
        GameProcedure.s_ProcCharCreate.Menpai = 3;
        GameProcedure.s_ProcCharCreate.OnRoleChanged();
		ShowZhiYeShuoMingTip(ZhiYeType.WUSHENG);
    }

    public void SelectMan()
    {
        GameProcedure.s_ProcCharCreate.Sex = 1;
        GameProcedure.s_ProcCharCreate.OnRoleChanged();
        UpdateFaces();
    }

    public void SelectWoman()
    {
        GameProcedure.s_ProcCharCreate.Sex = 0;
        GameProcedure.s_ProcCharCreate.OnRoleChanged();
        UpdateFaces();
    }

    public UITextField inputName;
    public void CreateRole()
    {
        GameProcedure.s_ProcCharCreate.CreateRoleName = inputName.Text;

        GameProcedure.s_ProcCharCreate.CreateRole();

        //Return2SelectRole();
    }

    public void SetRandomName()
    {
        inputName.Text = RandomName.Instance.GetName(GameProcedure.s_ProcCharCreate.Sex == 1);
    }

    public void Return2SelectRole()
    {
        gameObject.SetActiveRecursively(false);

        GameProcedure.s_ProcCharCreate.ChangeToRoleSel();
    }

    public void SelectFace1()
    {
        SelectFace(1);
    }
    public void SelectFace2()
    {
        SelectFace(2);
    }
    public void SelectFace3()
    {
        SelectFace(3);
    }
    public void SelectFace4()
    {
        SelectFace(4);
    }
    public void SelectFace5()
    {
        SelectFace(5);
    }
    public void SelectFace6()
    {
        SelectFace(6);
    }
    public void SelectFace7()
    {
        SelectFace(7);
    }
    public void SelectFace8()
    {
        SelectFace(8);
    }
    public void SelectFace9()
    {
        SelectFace(9);
    }

    public void SelectFace(int index)
    {
        GameProcedure.s_ProcCharCreate.SetFace(index);
        GameProcedure.s_ProcCharCreate.SetHairModel(index);
        GameProcedure.s_ProcCharCreate.OnRoleChanged();
    }
}
