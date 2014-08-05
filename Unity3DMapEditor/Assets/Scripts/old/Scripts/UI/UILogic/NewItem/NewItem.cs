using System.Collections.Generic;
using UnityEngine;
using Interface;

public class NewItem : MonoBehaviour
{
    public ActionButton actionButton;
    public SpriteText titleText;
    public SpriteText descText;

    int actionId = -1;
	

	
	void Start()
	{
		EZScreenPlacement ScreenPlacement = gameObject.GetComponent<EZScreenPlacement>();
        if (ScreenPlacement != null)
        {
			ScreenPlacement.SetCamera(UISystem.Instance.UiCamrea);
        }

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_NEW_ITEM, OnEvent);
        //CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, OnEvent);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_HP,OnEvent);

        actionButton.AddInputDelegate(OnActionButtonClicked);

        gameObject.SetActiveRecursively(false);
	}
    //void Awake()
    //{
        //gameObject.SetActiveRecursively(true);
		
		
	//	EZScreenPlacement ScreenPlacement = gameObject.GetComponent<EZScreenPlacement>();
    //    if (ScreenPlacement != null)
            //ScreenPlacement.SetCamera(UISystem.Instance.UiCamrea);
	//		ScreenPlacement.RenderCamera = UISystem.Instance.UiCamrea;

   //     CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_NEW_ITEM, OnEvent);
        //CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED, OnEvent);
   //     CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UNIT_HP,OnEvent);

   //     actionButton.AddInputDelegate(OnActionButtonClicked);

   //     gameObject.SetActiveRecursively(false);
  //  }

    void OnEvent(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        if (gAME_EVENT_ID == GAME_EVENT_ID.GE_NEW_ITEM)
        {
            int id = int.Parse(vParam[0]);
            CActionItem actionItem = CActionSystem.Instance.GetAction_ItemID(id,false);
            if (actionItem == null) return;

            actionId = actionItem.GetID();
            actionButton.SetActionItemByActionId(actionItem.GetID());

            CObject_Item item = actionItem.GetImpl() as CObject_Item;
            if (item != null)
            {
                titleText.Text = UIString.Instance.ParserString_Runtime("#{GET_NEW_ITEM}");
                descText.Text = UIString.Instance.ParserString_Runtime( "#{NEW_ITEM_DESC}");
            }
            UIWindowMng.Instance.ShowWindow("NewItemWindow");
        }
        //else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_PACKAGE_ITEM_CHANGED)
        //{
            //CActionItem actionItem = CActionSystem.Instance.GetActionByActionId(actionId);
            //if (actionItem == null)
               // OnClose();
       // }
        else if (gAME_EVENT_ID == GAME_EVENT_ID.GE_UNIT_HP)
        {
			float hp = PlayerMySelf.Instance.GetHPPercent();
			if(hp <= 0.4)
			{
				CObject_Item item = CDataPool.Instance.GetMedicial();
				if(item != null)
				{
					actionButton.UpdateItem(item.GetID());
					titleText.Text = "血量剩余40%";
					descText.Text = UIString.Instance.ParserString_Runtime("#{NEW_ITEM_DESC}");
                    UIWindowMng.Instance.ShowWindow("NewItemWindow");
				}
			}
			else
				OnClose();
        }
    }

    public void OnClose()
    {
        UIWindowMng.Instance.HideWindow("NewItemWindow");
        actionId = -1;
    }
	
	public void OnActionButtonClicked(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
        {
            OnClose();
        }
	}
}