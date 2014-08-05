using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISelectRole : MonoBehaviour {

    void Awake()
    {
        GameObject parent = UISystem.Instance.UiCamrea.gameObject;
        Vector3 vec = parent.transform.position;
        gameObject.transform.position = parent.transform.position + new Vector3(0, 0, 10);
        // 隐藏地形
        GameObject terrain = GameObject.Find("chuangjian");
        if (terrain != null)
            terrain.SetActiveRecursively(false);

        gameObject.SetActiveRecursively(false);
        //CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_GAMELOGIN_OPEN_SELECT_CHARACTOR, SelectRole);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_GAMELOGIN_REFRESH_ROLE_SELECT_CHARACTOR, SelectRole);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (alreaHaveRole)
                EnterGame();
            else
                CreateOrDelRole();
        }
    }

    public void SelectRole(GAME_EVENT_ID eventID, List<string> vParam)
    {
        gameObject.SetActiveRecursively(true);
        UpdateRoleList();
    }

    public UIButton createRoleBtn;
    public SpriteText lblName;
    bool alreaHaveRole = false;
    int roleCount = 0;
    void UpdateRoleList()
    {
        roleCount = GameProcedure.s_ProcCharSel.GetCurRoleCount();

        if (roleCount == 0)
        {
            alreaHaveRole = false;
            createRoleBtn.Text = "创建角色";
            lblName.Text = "";
        }
        else
        {
            alreaHaveRole = true;
            createRoleBtn.Text = "删除角色";
            // 暂时只允许玩家创建一个角色
            DB_CHAR_BASE_INFO roleInfo = GameProcedure.s_ProcCharSel.GetRoleInfo(0);
            string roleName = EncodeUtility.Instance.GetUnicodeString(roleInfo.m_Name);
            lblName.Text = roleName + " " + roleInfo.m_Level + "级";
            GameProcedure.s_ProcCharSel.SetCurSel(0);
        }
    }

    public void Return2Login()
    {
        GameProcedure.s_ProcCharSel.ChangeToAccountInput();
    }

    public void EnterGame()
    {
        if (roleCount == 0)
        {
            LogManager.LogWarning("没有创建角色.");
        }
        else
        {
            GameProcedure.s_ProcCharSel.SendEnterGameMsg(0);

            gameObject.SetActiveRecursively(false);
        }
    }

    public void CreateOrDelRole()
    {
        if (alreaHaveRole)
            UIWindowMng.messageBoxSelf.Show("", "是否删除当前角色？", DeleteRole, null);
        else
        {
            GameProcedure.s_ProcCharSel.ChangeToCreateRole();
            gameObject.SetActiveRecursively(false);
        }
    }
    void DeleteRole()
    {
        GameProcedure.s_ProcCharSel.DelSelRole();
    }

}
