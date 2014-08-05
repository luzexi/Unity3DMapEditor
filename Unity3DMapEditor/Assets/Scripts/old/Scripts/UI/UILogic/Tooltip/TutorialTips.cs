using System.Collections.Generic;
using UnityEngine;
using Interface;

public class TutorialTips : MonoBehaviour
{
    class TutorialInfo
    {
        public int index;
        public GAME_EVENT_ID eventID;
        public string target;
        public string tipText;
        public bool isDone = false;
        public int nData;
		public int nMark;
		public int SelectedBg;
		//public Vector3 nPos;

        public TutorialInfo(int idx, GAME_EVENT_ID eventId, string window, string tip, int data, int mark, int Bg)
        {
            index = idx;
            eventID = eventId;
            target = window;
            tipText = tip;
            nData = data;
			nMark = mark;
			SelectedBg = Bg;
        }
    };
    class Tutorial
    {
        public List<TutorialInfo> tutorials = new List<TutorialInfo>();
        public int nId;

        public TutorialInfo GetCurTutorial()
        {
            foreach (TutorialInfo info in tutorials)
            {
                if (!info.isDone)
                    return info;
            }
            return null;
        }
        public bool isHit(GAME_EVENT_ID eventID)
        {
            foreach (TutorialInfo info in tutorials)
            {
                if (!info.isDone)
                {
                    if (info.eventID == eventID)
                        return true;

                    return false;
                }
            }
            return false;
        }
        public void Done()
        {
            int i = -1;
            for (i=0; i < tutorials.Count; i++)
            {
                if (!tutorials[i].isDone)
                {
                    tutorials[i].isDone = true;
                    break;
                }
            }
        }
        public bool isEnd()
        {
            if (tutorials.Count == 0)
                return true;
            if (tutorials[tutorials.Count - 1].isDone)
                return true;
            return false;

        }

    };

    public SpriteText tutorialText;
    GameObject curParent;
    AutoSpriteControlBase ctrlTarget;
    Tutorial curTutorial = null;

    List<Tutorial> mTutorials = new List<Tutorial>();


    void loadTutorial()
    {
        Tutorial t = new Tutorial();
        t.nId = 0; ;
        //t.tutorials.Add(new TutorialInfo(0, GAME_EVENT_ID.GE_QUEST_EVENTLIST, "QuestDialog/Container/ComplexWindow", "tutorial_1",-1));
		t.tutorials.Add(new TutorialInfo(0, GAME_EVENT_ID.GE_QUEST_INFO, "QuestDialog/Container/ButtonAccept", "点击接受任务1",-1,-1,0));
		t.tutorials.Add(new TutorialInfo(1, GAME_EVENT_ID.GE_ADD_MISSION, "TaskTraceWindow/TreePrefab", "点击自动寻路2",1,0,0));
		t.tutorials.Add(new TutorialInfo(2, GAME_EVENT_ID.GE_QUEST_AFTER_CONTINUE, "QuestDialog/Container/ButtonContinue", "点击完成任务3",-1,-1,0));
		t.tutorials.Add(new TutorialInfo(3, GAME_EVENT_ID.GE_QUEST_INFO, "QuestDialog/Container/ButtonAccept", "点击接受任务4",-1,1,0));
		mTutorials.Add(t);
		
		Tutorial t1 = new Tutorial();
		t1.nId = 1;
		//t1.tutorials.Add(new TutorialInfo(0, GAME_EVENT_ID.GE_QUEST_EVENTLIST, "QuestDialog/Container/ComplexWindow", "tutorial_4",-1));
		//t1.tutorials.Add(new TutorialInfo(1, GAME_EVENT_ID.GE_QUEST_CONTINUE_DONE, "QuestDialog/Container/ButtonContinue", "tutorial_5",-1));
		//t1.tutorials.Add(new TutorialInfo(2, GAME_EVENT_ID.GE_UPDATE_MISSION, "TaskTraceWindow/TreePrefab", "tutorial_6",-1));
		t1.tutorials.Add(new TutorialInfo(0, GAME_EVENT_ID.GE_QUEST_AFTER_CONTINUE, "QuestDialog/Container/ButtonContinue", "点击完成任务5",-1,-1,0));
		t1.tutorials.Add(new TutorialInfo(1, GAME_EVENT_ID.GE_ADD_MISSION, "TaskTraceWindow/TreePrefab", "点击自动寻路6",3,3,0));
        mTutorials.Add(t1);
			
		Tutorial t2 = new Tutorial();
		t2.nId = 2;
		t2.tutorials.Add(new TutorialInfo(0, GAME_EVENT_ID.GE_FUNC_OPEN, "MainMenuWindow/PosFix/equip","点击装备按钮7",6,1,1));
		t2.tutorials.Add(new TutorialInfo(1, GAME_EVENT_ID.GE_TOGGLE_EQUIP_ENHANCE, "EquipWindow/Equip_Enchance/QiangHuaButton","点击完成强化8",-1,1,0));
		t2.tutorials.Add(new TutorialInfo(2, GAME_EVENT_ID.GE_UPDATE_EQUIP, "EquipWindow/CloseBtn", "点击关闭窗口9",-1,0,0));
		//t2.tutorials.Add(new TutorialInfo(3, GAME_EVENT_ID.GE_QUEST_AFTER_CONTINUE, "QuestDialog/Container/ButtonContinue", "点击完成任务10",-1,1,0));
		mTutorials.Add(t2);	
		
		Tutorial t3 = new Tutorial();
		t3.nId = 3;
		t3.tutorials.Add(new TutorialInfo(0, GAME_EVENT_ID.GE_ADD_MISSION, "TaskTraceWindow/TreePrefab", "点击自动寻路10",5,5,0));
		t3.tutorials.Add(new TutorialInfo(1, GAME_EVENT_ID.GE_ADD_MISSION, "TaskTraceWindow/TreePrefab", "点击自动寻路11",6,6,0));
		t3.tutorials.Add(new TutorialInfo(2, GAME_EVENT_ID.GE_ADD_MISSION, "TaskTraceWindow/TreePrefab", "点击自动寻路12",7,7,0));
		t3.tutorials.Add(new TutorialInfo(3, GAME_EVENT_ID.GE_ADD_MISSION, "MainMenuWindow/PosFix/Role", "点击任务培养13", 9,1,1));
        t3.tutorials.Add(new TutorialInfo(4, GAME_EVENT_ID.GE_ROLE_TIPWINDOWSHOWN, "RoleTipWindow/RoleWindow/RoleInfoWindow/PeiYangBtn", "点击打开培养窗口14", -1,1,0));
        t3.tutorials.Add(new TutorialInfo(5, GAME_EVENT_ID.GE_ATTRIRANDOMSHOWN, "PeiYangWindow/Money", "点击进行培养15", -1,1,0));
        t3.tutorials.Add(new TutorialInfo(6, GAME_EVENT_ID.GE_UPDATE_ATTRIRANDOM, "PeiYangWindow/BaoCun", "点击进行保存16", -1,1,0));
        t3.tutorials.Add(new TutorialInfo(7, GAME_EVENT_ID.GE_UPDATE_ATTRIRANDOM, "RoleTipWindow/CloseBtn", "点击关闭窗口17", -1,0,0));
		t3.tutorials.Add(new TutorialInfo(8, GAME_EVENT_ID.GE_ADD_MISSION,"TaskTraceWindow/TreePrefab","点击自动寻路18",10,10,0));
		mTutorials.Add(t3);
		
		Tutorial t4 = new Tutorial();
		t4.nId = 4;
		t4.tutorials.Add(new TutorialInfo(0, GAME_EVENT_ID.GE_FUNC_OPEN, "MainMenuWindow/PosFix/FengWu", "点击心法按钮19",3,1,1));
		t4.tutorials.Add(new TutorialInfo(1, GAME_EVENT_ID.GE_TOGGLE_XINFASHOW, "XinfaWindow/Container/Mover/XinfaListItem0/Item0/LevelUp", "点击升级按钮20",-1,1,0));
		t4.tutorials.Add(new TutorialInfo(2, GAME_EVENT_ID.GE_SKILL_UPDATE, "XinfaWindow/CloseBtn", "点击关闭心法窗口",-1,0,0));
		//t4.tutorials.Add(new TutorialInfo(2, GAME_EVENT_ID.GE_ADD_MISSION, "MainMenuWindow/Container/ShowMenu", "请点击",20,1));
		//t4.tutorials.Add(new TutorialInfo(3, GAME_EVENT_ID.GE_TOGGLE_EQUIP_SHENGDANG, "EquipWindow/Equip_ShengDang/ShengDangBtn", "请点击",-1,1));
		//t4.tutorials.Add(new TutorialInfo(4, GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, "EquipWindow/CloseBtn", "请点击",-1));
		//t4.tutorials.Add(new TutorialInfo(5, GAME_EVENT_ID.GE_ADD_MISSION, "TaskTraceWindow/TreePrefab", "请点击",30));
		//t4.tutorials.Add(new TutorialInfo(6, GAME_EVENT_ID.GE_QUEST_EVENTLIST, "QuestDialog/Container/ComplexWindow", "请点击",-1));
		
		mTutorials.Add(t4);

    }
	//UIButton planeRowButton; //水平引导图标
	//UIButton downRowButton; //竖直引导图标
	bool autoClosed = false;
	TutorialInfo CurrentTutorial = null;
	int TimeNow;
	int TimePre = 0;
    void Awake()
    {
        loadTutorial();
		//planeRowButton = GetComponentInChildren<UIButton>();
        gameObject.SetActiveRecursively(false);
        for (int i = 0; i < mTutorials.Count; i++)
        {
            Tutorial tutorial = mTutorials[i];
            for (int j = 0; j < tutorial.tutorials.Count; j++ )
            {
                CEventSystem.Instance.RegisterEventHandle(tutorial.tutorials[j].eventID, OnEvent);
            }
            
        }
		
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_ROLE_TIPWINDOW,HideEvent);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGGLE_EQUIPWINDOW,HideEvent);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_TOGLE_XINFASKILL_PAGE,HideEvent);
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_MISSION_TUTORIALTIPHIDE,HideEvent);
    }
	
	public void HideEvent(GAME_EVENT_ID eventId, List<string> vParam)
	{
		if(eventId == GAME_EVENT_ID.GE_MISSION_TUTORIALTIPHIDE)
		{
			int id = int.Parse(vParam[0]);
			if(id == CurrentTutorial.nMark)
			{
				//if(gameObject.active)
				//{
					if(!CurrentTutorial.isDone)
						TutorialDone();
					HideTooltip();
					
					//LogManager.LogError("隐藏自动寻路的引导： " + CurrentTutorial.tipText);
				//}
			}
		}
		else
		{
			if(CurrentTutorial != null)
			{
				if(CurrentTutorial.isDone)
					HideTooltip();
			}
		}
	}
    public void OnEvent(GAME_EVENT_ID eventId, List<string> vParam)
    {
        //特殊判断
        if (eventId == GAME_EVENT_ID.GE_QUEST_EVENTLIST)
        {
            if (!CUIDataPool.Instance.IsHadScriptEvent())
                return;
			
        }
        Tutorial tutorial = GetTutorialList(eventId);
        if (tutorial == null)
        {
            if (IsTutorialEnd())
                RemoveAllEventHandle();
            return;
        }
		CurrentTutorial = tutorial.GetCurTutorial();
        if (CurrentTutorial.nData != -1 && vParam.Count != 0)
        {
            int data = int.Parse(vParam[0]);
            //if (info.nData != data)
			if(CurrentTutorial.nData != data)
                return;
        }
		
		//LogManager.LogError("npc对话后显示的引导： " + CurrentTutorial.tipText);

        //GameObject go = GameObject.Find(info.target);
		GameObject go = GameObject.Find(CurrentTutorial.target);
		
		if(go == null)
		{
			LogManager.LogError("找不到引导指向的目标" + CurrentTutorial.target + CurrentTutorial.tipText);
		}

        curTutorial = tutorial;
        //SetTutorialText(info.tipText);
		SetTutorialText(CurrentTutorial.tipText);
        SetParent(go);
        
    }
	
    private void HideTooltip()
    {
		int count = 0;
        if (ctrlTarget != null)
            ctrlTarget.RemoveInputDelegate(TTInputDelegate);
        ctrlTarget = null;
        
		if(gameObject.active)
           gameObject.SetActiveRecursively(false);
        gameObject.transform.parent = null;
		
    }
	
	public UIButton planeBg;
	public UIButton downBg;
	Vector3 StartPosition;
	Vector3 EndPosition;
	bool Mark = false;
	
	private void ShowTooltip(GameObject obj)
	{
		gameObject.transform.parent = obj.transform;
		TimePre = 0;
		Vector3 offset;
		Vector3 btnPos = obj.transform.position;  //指向的按钮所在位置
		
		//if(planeRowButton == null)
		if(planeBg == null)
		{
			LogManager.LogError("Row is null"+gameObject.name);
			return;
		}
			
		if(obj.gameObject.name == "ComplexWindow")
		{
			offset = new Vector3(70,80,0);

			gameObject.transform.position = btnPos - offset;
			
			StartPosition = gameObject.transform.position;
			EndPosition = new Vector3(StartPosition.x + 20,StartPosition.y,StartPosition.z);
			
		}
		else if(obj.gameObject.name == "TreePrefab")
		{
			offset = new Vector3(110,40,0);
			gameObject.transform.position = btnPos - offset;
			
			StartPosition = gameObject.transform.position;
			EndPosition = new Vector3(StartPosition.x + 20,StartPosition.y,StartPosition.z);
		}
		else
		{
			UIButton btn = obj.GetComponent<UIButton>();
			
			if(btn.width == 0)
			{
				//float buttonHeight = (btn.spriteText.renderer.bounds.max.y - btn.spriteText.renderer.bounds.min.y);
			    //float buttonWidth = (btn.spriteText.renderer.bounds.max.x - btn.spriteText.renderer.bounds.min.x);
				if(CurrentTutorial.SelectedBg == 0)
					//gameObject.transform.localPosition = new Vector3(-planeRowButton.width/2 -60,- buttonHeight*3/2,-2);
					//gameObject.transform.localPosition = new Vector3(-planeBg.width/2 -60,- buttonHeight*3/2,-2);
					gameObject.transform.localPosition = new Vector3(-planeBg.width/2 - 60, -20, -2);
				else if(CurrentTutorial.SelectedBg == 1)
					//gameObject.transform.localPosition = new Vector3(-60,- buttonHeight*3/2,-2);
					gameObject.transform.localPosition = new Vector3(-55, -40, -2);
				
			}
			else
			{
				if(CurrentTutorial.SelectedBg == 0)
				{
					gameObject.transform.localPosition = new Vector3(-planeBg.width/2 - btn.width/2-30,- btn.height/2,-2);
				}
				else if(CurrentTutorial.SelectedBg == 1)
				{
					gameObject.transform.localPosition = new Vector3(btn.width/2, btn.height-10,-2);
				}
			}
			
			StartPosition = gameObject.transform.position;
			
			if(CurrentTutorial.SelectedBg == 0)
				EndPosition = new Vector3(StartPosition.x + 20,StartPosition.y,StartPosition.z);
			else if(CurrentTutorial.SelectedBg == 1)
				EndPosition = new Vector3(StartPosition.x,StartPosition.y + 20,StartPosition.z);
				
				
			
			//gameObject.transform.localPosition = new Vector3(-RowButton.width/2 - btn.width/2-30,- btn.height/2,-2);
			//EndPosition = new Vector3(StartPosition.x + 20,StartPosition.y,StartPosition.z);
			//gameObject.transform.localPosition = new Vector3(btnPos.x-RowButton.width/2 - btn.width/2-30,btnPos.y - btn.height/2,0 );

			//LogManager.LogError("当前z值：" + gameObject.transform.position);
			
		}
		
		Mark = false;
		gameObject.SetActiveRecursively(true);

		
		if(CurrentTutorial.SelectedBg == 0)
		{
			planeBg.Hide(false);
			downBg.Hide(true);
		}
		else if(CurrentTutorial.SelectedBg == 1)
		{
			planeBg.Hide(true);
			downBg.Hide(false);
		}
	}
	
	void Update()
	{
		float smooth = 0.6f;
		TimeNow++;
		
		//if(autoClosed)
		//{
		//	if(TimeNow > TimePre +150)
		//	{
		//		TutorialDone();
		//	    HideTooltip();
		//		autoClosed = false;
		//		LogManager.LogError("当前引导时间： " + TimeNow);
		//	}
		//}
		
		if(!Mark)
		{
			if(CurrentTutorial.SelectedBg == 0)
			{
				gameObject.transform.position += new Vector3(smooth,0,0);
				if(gameObject.transform.position.x > EndPosition.x -2)
				{
					Mark = true;
				}
			}
			else if(CurrentTutorial.SelectedBg == 1)
			{
				gameObject.transform.position += new Vector3(0,smooth,0);
				if(gameObject.transform.position.y > EndPosition.y -2)
				{
					Mark = true;
				}
			}
		}
		else
		{
			if(CurrentTutorial.SelectedBg == 0)
			{
				gameObject.transform.position -= new Vector3(smooth,0,0);
				if(gameObject.transform.position.x < StartPosition.x -2)
				{
					Mark = false;
				}
			}
			else if(CurrentTutorial.SelectedBg == 1)
			{
				gameObject.transform.position -= new Vector3(0,smooth,0);
				if(gameObject.transform.position.y < StartPosition.y -2)
				{
					Mark = false;
				}
			}
		}
	}
	
    public void SetParent(GameObject parent)
    {
        if (!parent)
        {
            HideTooltip();
            return;
        }

        if (parent.name == "ComplexWindow")
        {
            ComplexWindow cw = parent.GetComponent<ComplexWindow>();
            if (cw != null)
            {
                cw.OpInputDelegate += ComplexWindowInputDelegate;
            }
        }
		else if(parent.name == "TreePrefab")
		{
			TreeControl tc = parent.GetComponent<TreeControl>();
			if(tc != null)
			{
				tc.OpItemInputDelegate += TreeControlInputDelegate;
			}
		}
        else
        {
            ctrlTarget = parent.GetComponent<AutoSpriteControlBase>();
            if (ctrlTarget != null)
                ctrlTarget.AddInputDelegate(TTInputDelegate);
        }
        curParent = parent;
		
		ShowTooltip(parent);
		
		if(CurrentTutorial.nMark == 0)
		{
			if(!CurrentTutorial.isDone)
				TutorialDone();
		}
        
    }
    public void SetTutorialText(string text)
    {
        tutorialText.Text = text;
    }
    public void TTInputDelegate(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP
            || ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
        {
			
			if(!CurrentTutorial.isDone)
				TutorialDone();
			HideTooltip();
        }
    }
    public void ComplexWindowInputDelegate(string name)
    {
        TutorialDone();
        HideTooltip();
        ComplexWindow cw = curParent.GetComponent<ComplexWindow>();
        if (cw != null)
        {
            cw.OpInputDelegate -= ComplexWindowInputDelegate;
        }
    }
	
	public void TreeControlInputDelegate(string name)
	{

		//if(CurrentTutorial.nMark != 0)
		if(!CurrentTutorial.isDone)
			TutorialDone();
		HideTooltip();

		//HideTooltip();

		TreeControl tc = curParent.GetComponent<TreeControl>();
		if(tc != null)
		{
			tc.OpItemInputDelegate -= TreeControlInputDelegate;
		}
	}

    Tutorial GetTutorialList(GAME_EVENT_ID eventId)
    {

        foreach (Tutorial t in mTutorials )
        {
            if(IsTutorialDone(t.nId))
                continue;
            if(t.isHit(eventId))
                return t;
        }
        
        return null;
    }

    bool IsTutorialDone(int nIndex)
    {
        if (PlayerMySelf.Instance.GetTutorialMask(nIndex) != 0)
            return true;
        return false;
    }
    bool IsTutorialEnd()
    {
        bool bEnd = true;
        for (int i = 0; i < mTutorials.Count; i++ )
        {
            if (!IsTutorialDone(mTutorials[i].nId))
            {
                bEnd = false;
                break;
            }
        }
        return bEnd;
    }
    void TutorialDone()
    {
        curTutorial.Done();
        if(curTutorial.isEnd())
        {
             PlayerMySelf.Instance.SetTutorialMask(curTutorial.nId);
             RemoveEventHandle(curTutorial);
        }

        if (IsTutorialEnd())
            RemoveAllEventHandle();
    }
    void RemoveEventHandle(Tutorial tutorial)
    {
        foreach ( TutorialInfo info in tutorial.tutorials)
        {
            Tutorial t = GetTutorialList(info.eventID);
            if (t == null)
                CEventSystem.Instance.UnRegistEventHandle(info.eventID, OnEvent);
        }
        

    }
    void RemoveAllEventHandle()
    {
        for (int i = 0; i < mTutorials.Count; i++ )
        {
            RemoveEventHandle(mTutorials[i]);
        }

    }
}