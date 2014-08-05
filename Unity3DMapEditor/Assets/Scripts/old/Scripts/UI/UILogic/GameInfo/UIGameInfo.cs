using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIGameInfo : MonoBehaviour
{
    public GameObject textPrefab;
    public UIScrollList list;

    void Awake()
    {
        UISystem.Instance.AddHollowWindow(list.gameObject);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_INFO_SELF, ShowInfos);
    }
    
//     void Start()
//     {
//         for (int i = 0; i < 100; i++)
//         {
//             AddNewItem("alskdjfa;lskdjfla;sk" + i.ToString());
//         }
//         //list.PositionItems();
//     }

    public void ShowInfos(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (vParam.Count != 0)
        {
            string text = vParam[0];
            string showText = UIString.Instance.ParserString_Runtime(text);
            AddNewItem(showText);
        }
    }

    void TestDialogue(string content)
    {
        int id;
        if (UIString.Instance.ParseDialogue(content,out id))
        {
            CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_PLAY_DIALOGUE, id);
        }
    }

    void AddNewItem(string text)
    {
        // Instantiates a new item from the item object and
        // sets any attached text object to "Level 1":
        if (list.Count >= 1)
        {
            IUIListObject lastItem = list.GetItem(list.Count - 1);
            if (lastItem.Text == text)
                return;
        }

        IUIListObject newItem = list.CreateItem(textPrefab, text);

        UISystem.Instance.AddHollowWindow(newItem.gameObject);

        list.ScrollToItem(newItem, 0.5f, EZAnimation.EASING_TYPE.Linear);
    }
}