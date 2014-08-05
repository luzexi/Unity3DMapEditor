using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct InfoData
{
    public uint lifeTime;
    public bool isMove;
    public float moveSpeed;
    public uint circleTime;
    public string showInfo;

    internal void Reset()
    {
        lifeTime = 0;
        isMove = true;
        circleTime = 3;
        showInfo = "";
        moveSpeed = 1;
    }
}

public class UISystemBulletin : MonoBehaviour {
    public UIListItem item;
    public UIScrollList list;

    float startPos;
    float endPos;
    void Awake()
    {
        if (item == null || list == null)
            return;
        startPos = list.viewableArea.x / 2;
        endPos = -startPos;
        currData.Reset();

        item.anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
        item.transform.localPosition = new Vector3(startPos,
                                            item.transform.localPosition.y, item.transform.localPosition.z);

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_NEW_GONGGAOMESSAGE, ShowBulletin);
        //AddTestData();
        Hide();
    }

    void Hide()
    {
        gameObject.transform.parent.gameObject.SetActiveRecursively(false);
    }
    void Show()
    {
        gameObject.transform.parent.gameObject.SetActiveRecursively(true);
        EZScreenPlacement ScreenPlacement = gameObject.transform.parent.gameObject.GetComponent<EZScreenPlacement>();
        if (ScreenPlacement != null)
            ScreenPlacement.SetCamera(UISystem.Instance.UiCamrea);
    }

    private void AddTestData()
    {
        InfoData data = new InfoData();
        data.isMove = true;
        data.circleTime = 3;
        string text = "测试数据测试数据测试数据测试数据测试数据测试数据测试数据";
        for (uint i = 1; i < 4; i++)
        {
            data.showInfo = text + ",Num:" + i;
            data.moveSpeed = i*3;
            allInfos.Add(data);
        }
    }

    List<InfoData> allInfos = new List<InfoData>();
    public void AddInfos(string text)
    {
        InfoData data = new InfoData();
        data.showInfo = text;
        data.isMove = true;
        data.circleTime = 3;
        allInfos.Add(data);
    }

    public void ShowBulletin(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId == GAME_EVENT_ID.GE_NEW_GONGGAOMESSAGE)
        {
            if (vParam.Count == 0)
                return;

            if (vParam[0] == "1")
            {
                AddInfos(vParam[1]);
            }

            Show();
        }
    }

    InfoData currData;
    uint currLifeTime;
    uint currCircleTime;
    float currSpeed;
    float currWidth;

	// Update is called once per frame
    void Update()
    {
        if (IsEmpty())
            return;

        if (!CheckMoveNext())
        {
            ResetData();
            return;
        }

        UpdateMove();
	}

    void UpdateMove()
    {
        if (!currData.isMove)
            return;

        if (item.transform.localPosition.x + currWidth < endPos)
        {
            item.transform.localPosition = new Vector3(startPos,
                                            item.transform.localPosition.y, item.transform.localPosition.z);
            currCircleTime += 1;
        }
        else
        {
            item.transform.localPosition = new Vector3(item.transform.localPosition.x - currSpeed,
                                            item.transform.localPosition.y, item.transform.localPosition.z);
        }
        list.ClipItems();
    }

    bool IsEmpty()
    {
        if (currData.showInfo == "" && allInfos.Count == 0)
        {
            Hide();
            return true;
        }
        return false;
    }

    bool CheckMoveNext()
    {
        if (currData.showInfo == "" ||
            /*(currData.lifeTime != 0 && currData.lifeTime >= currLifeTime)||*/
            (currData.circleTime != 0 && currData.circleTime <= currCircleTime)
            )
        {
            return MoveNext();
        }
        return true;
    }

    bool MoveNext()
    {
        if (allInfos.Count == 0)
            return false;

        ResetData();

        currData = allInfos[0];
        allInfos.RemoveAt(0);

        if (currData.moveSpeed != 0)
            currSpeed = currData.moveSpeed;

        item.Text = currData.showInfo;
        currWidth = item.TextObj.TotalWidth;

        return true;
    }

    void ResetData()
    {
        currData.Reset();
        currCircleTime = 0;
        currLifeTime = 0;
        currSpeed = 1;
        currWidth = 0;

        item.Text = "";
        item.transform.localPosition = new Vector3(startPos,
                                            item.transform.localPosition.y, item.transform.localPosition.z);
    }
}
