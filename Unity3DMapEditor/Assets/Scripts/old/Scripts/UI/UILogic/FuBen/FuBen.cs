using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class FuBenConfig
{
	public FuBenConfig()
	{
	}
	
	public string IconName { get; set; }
    public string FuBenName { get; set; }
	public int StarNum { get; set; }
	public string WinName { get; set; }

}

public class FuBen : MonoBehaviour {
	
 
    const int MAX_EQUIPGRID = 4;
    int mCurrentPage = 0;//用于翻页
	
	int mMaxPage = 0;
	
	List<FuBenConfig> allWindows = new List<FuBenConfig>();
	
	public List<UIButton> mFuBenBgs;
	public List<ActionButton> mFuBenIcons;
	public List<SpriteText> mFuBenNames;
	public List<GameObject> mItems;
	public UIButton mPrePage;
    public UIButton mNextPage;
	int mCurrentSelectedEquip = 0;
	
	Dictionary<string, int> mFuBenIndexs = new Dictionary<string, int>();
	
	void Awake()
	{
		gameObject.transform.root.gameObject.SetActiveRecursively(true);
		
		registerWidgets();
		RegistAllEvents();
		UpdateFuBen();
        //Hide();
	}
	
	void RegistAllEvents()
	{
		CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_OPEN_FUBEN, OpenFuBen);
	}
	
	public void OpenFuBen(GAME_EVENT_ID eventId, List<string> vParam)
	{
		if(eventId == GAME_EVENT_ID.GE_OPEN_FUBEN)
		{
			gameObject.transform.root.gameObject.SetActiveRecursively(true);
			UpdateFuBen();
		}
	}
	
	void registerWidgets()
	{
		for (int i = 0; i < mFuBenBgs.Count; i++ )
        {
			mFuBenIndexs.Add(mFuBenBgs[i].transform.parent.gameObject.name,i);
            
        }
		
		foreach (ActionButton action in mFuBenIcons)
        {
            action.AddInputDelegate(OnEquipActionButtonClicked);
        }
		
		FbInputDelegate += EventSelect;
		
	}
	
	public delegate void FbOpClick(string opName);
	protected FbOpClick fbInputDelegate;
	public FuBen.FbOpClick FbInputDelegate
	{
		get { return fbInputDelegate; }
		set { fbInputDelegate = value; }
	}
	
	void OnEquipActionButtonClicked(ref POINTER_INFO ptr)
    {
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            Transform parent = ptr.targetObj.gameObject.GetComponent<Transform>().parent;
            string parentName = parent.gameObject.name;
            if (mFuBenIndexs.ContainsKey(parentName))
            {
                int nIndex = mFuBenIndexs[parentName];
				//mCurrentSelectedEquip = nIndex + mCurrentPage * MAX_EQUIPGRID;
				for(int i = 0; i < mFuBenIcons.Count; i++)
				{
						if(i == nIndex)
							mFuBenBgs[i].Hide(false);
						else
							mFuBenBgs[i].Hide(true);
				}

            }
			
			if(fbInputDelegate != null)
			{
				fbInputDelegate(ptr.hitInfo.collider.gameObject.name);
			}

        }
    }
	

	
	void UpdateFuBen()
	{
		
		allWindows.Clear();
		
		int listCount = CUIDataPool.Instance.m_pEventList.m_yItemCount;
		
		for(int i = 0; i < listCount; i++)
		{
			ScriptEventItem item = CUIDataPool.Instance.m_pEventList.GetItem((byte)i);
			if(item != null)
			{
				switch(item.m_nType)
					{
					    case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_INVALID:
                            break;
                        case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SECTION:
                            break;
                        case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_SCRIPT_ID:
                            string content = item.m_strString.m_szString;
					        string opName = item.m_idScript + "|" + item.m_index + "|" + i;
					        AddOption(opName, content);
                            break;
                        case ENUM_EVENT_ITEM_TYPE.EVENT_ITEM_TYPE_TEXT:
                            break;
                       default:
                            break;
					}
			}
		}

		ReLayout();
	}
	
	bool AddOption(string winName, string opText)
    {
		string icon;
		string name;
		int num;
		FuBenConfig op = new FuBenConfig();
        // 加入文字转义
        string parseRes;
        parseRes = UIString.Instance.ParserString_Runtime(opText);
		UIString.Instance.ParseFuBen(parseRes, out icon, out name, out num);
		op.IconName = icon;
		op.FuBenName = name;
		op.StarNum = num;
		op.WinName = winName;
        allWindows.Add(op);
        return true;
    }
	
	 void ReLayout()
     {
		 int nCount = 0;
		 bool bResetSelectIndex = true;
		 int nBeginIndex = mCurrentPage * MAX_EQUIPGRID;	
         for (int i = 0; i < allWindows.Count; i++)
         {
             FuBenConfig item = allWindows[i];
			 if(item != null)
			 {
				if (nCount < nBeginIndex || nCount - nBeginIndex >= MAX_EQUIPGRID)
                  {
                     nCount++;
                     continue;
                  }
				
				 //不重置当前选择的索引
                 //if (nCount - nBeginIndex >= mCurrentSelectedEquip)
				 //if(mCurrentPage == mCurrentSelectedEquip/MAX_EQUIPGRID)
                  //    bResetSelectIndex = false;
				 
				mFuBenIcons[nCount-nBeginIndex].SetIcon(item.IconName);
				mFuBenNames[nCount-nBeginIndex].Text = item.FuBenName;
				mFuBenIcons[nCount-nBeginIndex].gameObject.name = item.WinName;
				nCount++;
			 }
         }
		
		
       
        //if (bResetSelectIndex)
        //{
		//	for(int i = 0; i < MAX_EQUIPGRID; i++)
		//		mFuBenBgs[i].Hide(true);
        //}
		 mMaxPage = nCount / MAX_EQUIPGRID;
         if (nCount % MAX_EQUIPGRID != 0)
               mMaxPage++;
         UpatePageNum();
		 HideItem();
     }
	
	void HideItem()
	{
		int j = mCurrentPage * MAX_EQUIPGRID;
		for(int i = 0; i < MAX_EQUIPGRID; i++)
		{
			if(i + j >= allWindows.Count)
			{
				mItems[i].SetActiveRecursively(false);
			}
			else
				mItems[i].SetActiveRecursively(true);
		}
		
	}
	
	void OnChangePageNext()
    {
        mCurrentPage++;
        UpdateFuBen();
    }
    void OnChangePagePre()
    {
        mCurrentPage--;
        UpdateFuBen();
    }
	void UpatePageNum()
    {
        if (mMaxPage > mCurrentPage + 1)
            mNextPage.controlIsEnabled = true;
        else
            mNextPage.controlIsEnabled = false;
        if (mCurrentPage > 0)
            mPrePage.controlIsEnabled = true;
        else
            mPrePage.controlIsEnabled = false;
        int npage = mCurrentPage + 1;
    }
	
	void Hide()
    {
        gameObject.transform.root.gameObject.SetActiveRecursively(false);
    }
	
	
    void EventSelect(string opName)
    {
        string[] ops = opName.Split('|');
        if (ops.Length != 3)
        {
            LogManager.LogWarning("event name's count is wrong.");
            return;
        }
        int script = int.Parse(ops[0]);
        int m_index = int.Parse(ops[1]);
        int localIndex = int.Parse(ops[2]);
        CUIDataPool.Instance.SendSelectEvent(localIndex, m_index, script);
		
		Hide();
    }
}
