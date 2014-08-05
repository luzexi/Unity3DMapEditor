using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ComplexListWindow : MonoBehaviour {

 List<CmConfig> allWindows = new List<CmConfig>();
	
    GameObject childGo;
    Vector3 orgPos;
    //public GameObject bgGo;
    //UIButton bg = null;

    public GameObject ActionPrefab;
    public GameObject OptionPrefab;
    public GameObject TextPrefab;
	//public GameObject RewardTip;


    public float maxTextLength = 230;
    void Initial() 
    {
		orgPos = Vector3.zero;
		childGo = new GameObject("cmpRoot");
		GameObject parentGo = GameObject.Find("ComplexListWindow");
		if(parentGo == null)
		{
			LogManager.LogError("can't find ComplexListWindow in QuestListWindow");
			return;
		}
		else
		{
			GFX.GfxUtility.attachGameObject(childGo, parentGo);
		}
    }
	
    float OrgTextSize = 4.0f;
    public bool AddText(string content)
    {
        return AddText(content, OrgTextSize);
    }

    public bool AddText(string content , float size)
    {
        TxtConfig sText = new TxtConfig();
        sText.Size = size;
        // 加入文字转义 [2/20/2012 Ivan]
        string parseRes;
        UIString.Instance.ParserString_Runtime(content,out parseRes);
        sText.Content = parseRes;
        allWindows.Add(sText);
        return true;
    }

    public bool AddOption(string winName,string opText)
    {
        OptionConfig op = new OptionConfig();
        // 加入文字转义 [2/20/2012 Ivan]
        string parseRes;
        UIString.Instance.ParserString_Runtime(opText, out parseRes);
        op.Content = parseRes;
        op.WinName = winName;
        allWindows.Add(op);
        return true;
    }
	
	 private void AddAction(ref Vector3 pos, ActionConfig action)
     {
         GameObject option = Instantiate(ActionPrefab) as GameObject;
         option.name = "RewardItem id:" + action.id;
         option.transform.position = pos;
         GFX.GfxUtility.attachGameObject(option, childGo);
         option.layer = LayerManager.UILayer;

         //ActionButton button = option.GetComponent<ActionButton>();
		 ActionButton button = option.GetComponentInChildren<ActionButton>();
		 if(button == null)
		 {
			LogManager.LogError("任务物品奖励框没有找到!");
			return;
		 }
		 
         button.UpdateItem(action.id);
         float buttonHeight = (option.renderer.bounds.max.y - option.renderer.bounds.min.y);
         pos.y -= buttonHeight;
     }
	
     private bool AddOption(ref Vector3 pos, OptionConfig op)
     {
        //控制选项上部空5个像素 [3/23/2012 Ivan]
         pos.y -= 12;

         GameObject option = Instantiate(OptionPrefab) as GameObject;
         option.name = op.WinName;
         option.transform.position = pos;
         GFX.GfxUtility.attachGameObject(option, childGo);
         option.layer = LayerManager.UILayer;

         UIButton button = option.GetComponent<UIButton>();
         button.Anchor = SpriteRoot.ANCHOR_METHOD.UPPER_LEFT;
         button.spriteText.Anchor = SpriteText.Anchor_Pos.Upper_Left;
         button.spriteText.lineSpacing = 1;
         button.Text = op.Content;

         float buttonHeight = (button.spriteText.renderer.bounds.max.y - button.spriteText.renderer.bounds.min.y);
         pos.y -= buttonHeight;

         button.AddInputDelegate(OptionClick);

         return true;
     }

     static int textCount = 0;
     bool AddTextWind(ref Vector3 pos, TxtConfig config)
     {
         GameObject textGo = Instantiate(TextPrefab) as GameObject;
         textGo.name = "CmpWin_Text_" + textCount++;
         textGo.transform.position = pos;
         GFX.GfxUtility.attachGameObject(textGo, childGo);
         textGo.layer = LayerManager.UILayer;

         UIButton textBtn = textGo.GetComponent<UIButton>();
         textBtn.spriteText.Text = config.Content;

         float buttonHeight = (textBtn.spriteText.renderer.bounds.max.y - textBtn.spriteText.renderer.bounds.min.y);
         pos.y -= buttonHeight;
		
         return true;
     }

     public void ReLayout()
     {
         Vector3 tempPos = orgPos;
			
         for (int i = 0; i < allWindows.Count; i++)
         {
             CmConfig item = allWindows[i];
             if (item is TxtConfig)
             {
                 TxtConfig txtConfig = item as TxtConfig;
                 AddTextWind(ref tempPos, txtConfig);
             }
             else if (item is OptionConfig)
             {
                 OptionConfig op = item as OptionConfig;
				 AddOption(ref tempPos, op);
             }
             else if (item is ActionConfig)
             {
                 ActionConfig action = item as ActionConfig;
				 AddAction(ref tempPos, action);
             }
             else if (item is SpaceConfig)
             {
                 SpaceConfig space = item as SpaceConfig;
                 orgPos.y -= space.LineCount * space.LineHeight;
             }
         }
     }

	

    public delegate void CmpListOpClick(string opName);
    protected CmpListOpClick opInputDelegate;
    public ComplexListWindow.CmpListOpClick OpInputDelegate
    {
        get { return opInputDelegate; }
        set { opInputDelegate = value; }
    }

    void OptionClick(ref POINTER_INFO ptr)
    {
        if (ptr.hitInfo.collider == null || ptr.hitInfo.collider.gameObject == null)
            return;
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
                break;
            case POINTER_INFO.INPUT_EVENT.PRESS:
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE:
                break;
            case POINTER_INFO.INPUT_EVENT.TAP:
                if (opInputDelegate != null)
                {
                    opInputDelegate(ptr.hitInfo.collider.gameObject.name);
                }
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE:
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
                break;
            case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
                break;
            case POINTER_INFO.INPUT_EVENT.DRAG:
                break;
            default:
                break;
        }
    }

    public void CleanAll()
    {
        GameObject.Destroy(childGo);
        allWindows.Clear();
        this.Initial();
    }

    //temp for test
//     void Awake()
//     {
//         Initial();
//         AddText("test1test1test1test1test1test1test1test1test1test1test1test1");
//         AddText("test1test1test1test1test1test1test1test1test1test1test1test1");
//         AddOption("test11@12","测试按钮1");
//         AddOption("test11@11", "测试按钮2");
//         AddText("test1test1test1test1test1test1test1test1test1test1test1test1");
// 		ReLayout();
// 
//         OpInputDelegate += Click;
//     }
// 
//     void Click(string name)
//     {
//         LogManager.LogWarning(name);
//     }

    internal void AddMoney(uint money)
    {
        //temp
        AddText("奖励金钱:" + money);
    }

    internal void AddExp(uint exp)
    {
        AddText("奖励经验:" + exp);
    }

    internal void AddItem(uint itemId, int itemNum)
    {
        ActionConfig action = new ActionConfig();
        action.id = (int)itemId;
        action.num = itemNum;

        allWindows.Add(action);
    }

    // 添加空行
    internal void AddSpaceLine()
    {
        AddSpaceLine(1);
    }

    internal void AddSpaceLine(int count)
    {
        SpaceConfig space = new SpaceConfig();
        space.LineCount = count;

        allWindows.Add(space);
    }

    internal string GetFirstOptionName()
    {
        for (int i = 0; i < allWindows.Count; i++ )
        {
            if (allWindows[i] is OptionConfig)
            {
                OptionConfig item = allWindows[i] as OptionConfig;
                return item.WinName;
            }
        }
        return null;
    }
}
