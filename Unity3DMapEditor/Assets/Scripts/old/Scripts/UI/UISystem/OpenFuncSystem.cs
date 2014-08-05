using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class OpenFuncSystem
{
    static readonly OpenFuncSystem instance = new OpenFuncSystem();
    public static OpenFuncSystem Instance { get { return instance; } }

    public List<_DBC_FUNC_OPEN_LIST> funcList = null;
    GameObject[] uiGoList = null;

    public void Initial()
    {
        InitialAllWindow();
    }

    void InitialAllWindow()
    {
        DBC.COMMON_DBC<_DBC_FUNC_OPEN_LIST> allList = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_FUNC_OPEN_LIST>
                                                                ((int)DataBaseStruct.DBC_FUNC_OPEN_LIST);

        funcList = new List<_DBC_FUNC_OPEN_LIST>();
        uiGoList = new GameObject[allList.StructDict.Values.Count];
        int i =0;
        foreach (_DBC_FUNC_OPEN_LIST func in allList.StructDict.Values)
        {
            GameObject go = GameObject.Find(func.uiName);
            funcList.Add(func);
            uiGoList[i++] = go;
        }

        RegistConditions();
    }

    void RegistConditions()
    {
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UI_INFOS, UpdateCondition);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_APPLICATION_INITED, UpdateCondition);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_FUNC_OPEN, UpdateCondition);
    }

    void UnRegistConditions()
    {
        CEventSystem.Instance.UnRegistEventHandle(GAME_EVENT_ID.GE_FUNC_OPEN, UpdateCondition);
    }

    public void UpdateCondition(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_UI_INFOS)
        {
            if (vParam.Count == 1 && vParam[0] == "MissionDoneTag")
            {
                UpdateFuncEnable();
            }
            
        }
        else
        {
            UpdateFuncEnable();
        }
    }

    public void UpdateFuncEnable()
    {
        if (funcList.Count == 0)
        {
            UnRegistConditions();
            return;
        }

        CObject_PlayerMySelf mySelf = CObjectManager.Instance.getPlayerMySelf();
        CDetailAttrib_Player myData = CDetailAttrib_Player.Instance;
        if (mySelf == null || myData == null)
            return;
        //////////////////////////////////////////////////////////////////////////
        // 由于界面表现要有特殊标示，所以特殊对待
        int x1Pos = 0;
        int y2Pos = 45;
        int index = -1;
        //////////////////////////////////////////////////////////////////////////
        foreach (_DBC_FUNC_OPEN_LIST enumerator in funcList)
        {
            index++;
            bool showEnable = true;
            _DBC_FUNC_OPEN_LIST func = enumerator;
            if (func.needLevel != MacroDefine.INVALID_ID && mySelf.GetCharacterData().Get_Level() < func.needLevel)
                showEnable = false;
            else if (func.receiveMission != MacroDefine.INVALID_ID)
            {
                if((myData.GetMissionIndexByID(func.receiveMission) == MacroDefine.INVALID_ID) &&
                !myData.IsMissionHaveDone((uint)func.receiveMission))
                    showEnable = false;
            }
            else if (func.finishMission != MacroDefine.INVALID_ID)
            {
                if (!myData.IsMissionHaveDone((uint)func.finishMission))
                    showEnable = false;
            }

            if (uiGoList[index] == null)
            {
                uiGoList[index] = GameObject.Find(enumerator.uiName);
            }

            if (uiGoList[index] != null)
            {
                if (showEnable)
                {
                    if (uiGoList[index].layer != LayerManager.UILayer)
                    {
                        uiGoList[index].layer = LayerManager.UILayer;
                    }

                    //////////////////////////////////////////////////////////////////////////
                    Vector3 pos = uiGoList[index].transform.localPosition;
                    switch (enumerator.specialTag)
                    {
                        case 1:
                            pos.x = x1Pos;
                            x1Pos -= 45;
                            break;
                        case 2:
                            pos.y = y2Pos;
                            y2Pos += 45;
                            break;
                        default:
                            break;
                    }
                    uiGoList[index].transform.localPosition = pos;
                    //////////////////////////////////////////////////////////////////////////

                    //                 funcList.Remove(enumerator.Current.Key);
                    //                 enumerator = funcList.GetEnumerator();
                }
                else
                {
                    if (uiGoList[index].layer == LayerManager.UILayer)
                        uiGoList[index].layer = LayerManager.DefaultLayer;
                }
            }

        }
    }
}