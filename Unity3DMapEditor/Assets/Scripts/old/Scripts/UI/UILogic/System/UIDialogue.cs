using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Interface;

struct DialogueContent
{
    public string iconName;
    public string speakerName;
    public string speakContent;
}
public class UIDialogue : MonoBehaviour {

	// Use this for initialization
    void Awake()
    {
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_PLAY_DIALOGUE, PlayDialogue);

        EZScreenPlacement ScreenPlacement = gameObject.transform.root.gameObject.GetComponent<EZScreenPlacement>();
        if (ScreenPlacement != null)
            ScreenPlacement.SetCamera(UISystem.Instance.UiCamrea);

        gameObject.transform.root.gameObject.SetActiveRecursively(false);

        InitialAllDialogue();

        BoxCollider box = mask.GetComponent<BoxCollider>();
        if (box != null)
        {
            box.size = new UnityEngine.Vector3(1280, 720, 0);
            box.center = new UnityEngine.Vector3(0, 360, 0);
        }
        head.autoResize = false;
        head.pixelPerfect = false;
        bgDownOther.transform.localPosition = new Vector3(-0.5f, 0, 1);
        bgUpOther.transform.localPosition = new Vector3(-0.5f, 721, 1);
        bgDownMe.transform.localPosition = new Vector3(-0.5f, 0, 1);
        bgUpMe.transform.localPosition = new Vector3(-0.5f, 721, 1);
	}

    public UIButton bgUpMe;
    public UIButton bgUpOther;
    public UIButton bgDownMe;
    public UIButton bgDownOther;
    public UIButton head;
    public SpriteText name;
    public SpriteText text;
    public UIButton help;
    public UIButton mask;

    DBC.COMMON_DBC<_DBC_Dialogue> dialogues = null;
    private void InitialAllDialogue()
    {
        dialogues = DBSystem.CDataBaseSystem.Instance.GetDataBase<_DBC_Dialogue>((int)DataBaseStruct.DBC_DIALOGUE);
        if (dialogues == null)
            LogManager.LogWarning("剧情对白配置表为空，path:Private/system/JuQingDialog.txt");
    }

    List<DialogueContent> currContent = null;
    public void PlayDialogue(GAME_EVENT_ID eventId, List<string> vParam)
    {
        if (eventId!= GAME_EVENT_ID.GE_PLAY_DIALOGUE || vParam.Count == 0)
            return;

        if (vParam.Count == 1)
        {
            int id = int.Parse(vParam[0]);
            if (id < 0)
                return;
            Play(id);
        }
    }

    void Play(int id)
    {
        ParseNewDialogue(id);

        ShowDialogueWindow();
        MoveNext();
    }

    Color preColor;
    void ShowDialogueWindow()
    {
        preColor = RenderSettings.ambientLight;
        RenderSettings.ambientLight = Color.black;
        UISystem.Instance.UiCamrea.cullingMask = LayerManager.DialogueMask;
        gameObject.transform.root.gameObject.SetActiveRecursively(true);
    }

    void HideDialogueWindow()
    {
        RenderSettings.ambientLight = preColor;
        UISystem.Instance.UiCamrea.cullingMask = LayerManager.UIMask;
        gameObject.transform.root.gameObject.SetActiveRecursively(false);

        // 通知剧情播放结束 [4/24/2012 Ivan]
        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_STOP_DIALOGUE);
    }

    private void ParseNewDialogue(int id)
    {
        _DBC_Dialogue dialogue = dialogues.Search_Index_EQU(id);
        if (dialogue == null)
            return;

        currIndex = 0;
        currContent = new List<DialogueContent>();
        string[] temps = dialogue.diglogueGroup.Split(']');
        foreach (string item in temps)
        {
            if (item.Length <= 0)
                continue;

            string temp = item;
            temp = temp.Replace(",[", "");
            temp = temp.Replace("[", "");
            DialogueContent content = new DialogueContent();
            int first = temp.IndexOf('/');
            content.iconName = temp.Substring(0, first);
            int second = temp.IndexOf('/', first + 1);
            content.speakerName = temp.Substring(first + 1, second - first - 1);
            content.speakContent = temp.Substring(second + 1, temp.Length - second - 1);
            
            currContent.Add(content);
        }
        return;
    }

    string GetIconName(string text)
    {
        if (text == "MyIcon")
        {
            // 由于现在美术人手不足，所以男女暂时只有各一个 [5/10/2012 Ivan]
            if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Gender() == 0)
                return "wplayer";
            else
                return "mplayer";
            
            // return PlayerMySelf.Instance.GetData("PORTRAIT");
        }
        else
        {
            return text;
        }
    }

    int currIndex = 0;
    public bool MoveNext()
    {
        if (currIndex >= currContent.Count || currContent.Count == 0)
        {
            HideDialogueWindow();
            return false;
        }
        //1)
        if (currContent[currIndex].iconName == "")
        {
            head.gameObject.active = false;
        }
        else
        {
            head.gameObject.active = true;
            Texture2D icon = IconManager.Instance.GetIcon(GetIconName(currContent[currIndex].iconName));
            head.SetTexture(icon);
            head.width = icon.width;
            head.height = icon.height;
        }
        //2)
        if (currContent[currIndex].speakerName == "" || currContent[currIndex].speakerName == "自己")
        {
            name.Text = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Name();
        }
        else
        {
            name.Text = currContent[currIndex].speakerName;
        }
        ToggleSpeaker(currContent[currIndex].speakerName);
        //3)
        string showText;
        UIString.Instance.ParserString_RuntimeNoHL(currContent[currIndex].speakContent, out showText);
        text.Text = showText;
        //4)
        currIndex++;
        
        return true;
    }

    void ToggleSpeaker(string speakerName)
    {
        if (speakerName == "" || speakerName=="自己")
        {
            head.Anchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT;
            head.transform.localPosition = new Vector3(-640f, 0, 0.5f);
            name.transform.localPosition = new Vector3(-391.1998f, 163.9301f, 0.5f);
            text.transform.localPosition = new Vector3(-354.1712f, 118.6921f, 0.5f);
            help.transform.localPosition = new Vector3(388.0941f, 86.00439f, 0.5f);
            bgDownOther.active = false;
            bgDownOther.transform.localPosition = new Vector3(-0.5f, 0, 1);
            bgUpOther.active = false;
            bgUpOther.transform.localPosition = new Vector3(-0.5f, 721, 1);
            bgDownMe.active = true;
            bgDownMe.transform.localPosition = new Vector3(-0.5f, 0, 1);
            bgUpMe.active = true;
            bgUpMe.transform.localPosition = new Vector3(-0.5f, 721, 1);
        }
        else
        {
            head.Anchor = SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT;
            head.transform.localPosition = new Vector3(640f, 0, 0.5f);
            name.transform.localPosition = new Vector3(-244.2686f, 163.9301f, 0.5f);
            text.transform.localPosition = new Vector3(-207.2399f, 118.6921f, 0.5f);
            help.transform.localPosition = new Vector3(-469.0284f, 86.00439f, 0.5f);
            bgDownOther.active = true;
            bgUpOther.active = true;
            bgDownMe.active = false;
            bgUpMe.active = false;
        }
    }
}
