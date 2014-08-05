using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeControl : MonoBehaviour
{
    public GameObject parentPrefab;
    public GameObject childPrefab;
    public UIScrollList list;

    //void Awake()
    //{
    //    for (int i = 0; i < 5; i++)
    //    {
    //        for (int j = 0; j < 10; j++)
    //        {
    //            AddItem(i.ToString() + "_" + j.ToString(),
    //            i.ToString() + "_" + j.ToString(),
    //            i.ToString());
    //        }
    //    }
    //}

    public delegate void TreeItemClickDelegate(string itemName);
    TreeItemClickDelegate itemInputDelegate;
	public TreeControl.TreeItemClickDelegate OpItemInputDelegate
	{
		get{ return itemInputDelegate; }
		set{ itemInputDelegate = value; }
	}
    public void AddItemInputDelegate(TreeItemClickDelegate inputDelegate)
    {
        if (itemInputDelegate == null)
            itemInputDelegate = inputDelegate;
        else
            itemInputDelegate += inputDelegate;
    }

    public void ItemsInputDelegate(ref POINTER_INFO ptr)
    {
        if (itemInputDelegate != null)
        {
            switch (ptr.evt)
            {
                case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
                    break;
                case POINTER_INFO.INPUT_EVENT.PRESS:
                    break;
                case POINTER_INFO.INPUT_EVENT.RELEASE:
                    break;
                case POINTER_INFO.INPUT_EVENT.TAP:
                    {
                        if (ptr.hitInfo.collider.gameObject != null)
                        {
                            itemInputDelegate(ptr.hitInfo.collider.gameObject.name);
                        }
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
    }

    public void ParentValueChanged(IUIObject parent)
    {
        ToggleParend(parent.gameObject);
    }

    public void Clear()
    {
        list.ClearList(true);
        allParents.Clear();
    }

    public void AddItem(string childText,string childId, string parentText)
    {
        childText = UIString.Instance.ParserString_Runtime(childText);
        parentText = UIString.Instance.ParserString_Runtime(parentText); 

        List<IUIListObject> parent = GetParent(parentText);
        IUIListObject child = NewChild(childText, childId);
        parent.Add(child);

        RefreshParent(parentText);
    }


    public void AddItem(string childText, string childId)
    {
        childText = UIString.Instance.ParserString_Runtime(childText);

        IUIListObject child = NewChild(childText, childId);
    }

    IUIListObject NewChild(string name,string goName)
    {
        IUIListObject newItem = list.CreateItem(childPrefab, name);
        newItem.gameObject.name = goName;
        UIButton btn = newItem.gameObject.GetComponent<UIButton>();
        if (btn != null)
            btn.AddInputDelegate(ItemsInputDelegate);
        return newItem;
    }

    Dictionary<string, List<IUIListObject>> allParents = new Dictionary<string, List<IUIListObject>>();
    List<UIListItemContainer> allParContainers = new List<UIListItemContainer>();
    List<IUIListObject> GetParent(string parName)
    {
        List<IUIListObject> parent;
        if (!allParents.TryGetValue(parName, out parent))
        {
            IUIListObject newItem = list.CreateItem(parentPrefab, parName);
            newItem.gameObject.name = parName;
            UIStateToggleBtn radioBtn = newItem.gameObject.GetComponent<UIStateToggleBtn>();
            if (radioBtn != null)
                radioBtn.AddValueChangedDelegate(ParentValueChanged);

            UIListItemContainer container = newItem.gameObject.GetComponent<UIListItemContainer>();
            if (container != null)
                allParContainers.Add(container);

            parent = new List<IUIListObject>();
            allParents.Add(parName, parent);
        }

        return parent;
    }

    void ToggleParend(GameObject parent)
    {
        if (parent == null)
            return;

        UIStateToggleBtn radioBtn = parent.GetComponent<UIStateToggleBtn>();
        if (radioBtn == null)
            return;
        else if (radioBtn.Data == null)
        {
            radioBtn.Data = true;
            return;
        }

        string parName = parent.name;
        List<IUIListObject> parentLists;
        if (allParents.TryGetValue(parName, out parentLists))
        {
            if (radioBtn.StateNum == 0)
            {
                foreach (IUIListObject item in parentLists)
                {
                    list.RemoveItem(item, false);
                }
            }
            else
            {
                UIListItemContainer parContainer = parent.GetComponent<UIListItemContainer>();
                for (int i = 1; i <= parentLists.Count; i++)
                {
                    IUIListObject item = parentLists[i-1];
                    list.InsertItem(item, parContainer.Index + i);
                }
            }
        }

    }

    void RefreshParent(string parentName)
    {
        List<IUIListObject> parentLists;
        if (allParents.TryGetValue(parentName, out parentLists))
        {
            foreach (IUIListObject item in parentLists)
                list.RemoveItem(item, false);

            UIListItemContainer parent = null;
            foreach (UIListItemContainer item in allParContainers)
            {
                if (item != null && item.gameObject.name == parentName)
                {
                    parent = item;
                    break;
                }
            }
            if (parent != null)
            {
                UIListItemContainer parContainer = parent.GetComponent<UIListItemContainer>();
                for (int i = 1; i <= parentLists.Count; i++)
                {
                    IUIListObject item = parentLists[i - 1];
                    list.InsertItem(item, parContainer.Index + i);
                }
            }
        }
    }
}
