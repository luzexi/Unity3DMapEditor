using UnityEngine;
using System.Collections;

public class MouseCursor : MonoBehaviour
{
    //variables
    public Texture2D normalCursor;
    public Texture2D attackCursor;
    public Texture2D selectCursor;
    public Texture2D speakCursor;

    Texture2D currentCursor;

    // Scale the cursor so it should look right in any aspect ratio, and turn off the OS mouse pointer
    void Start()
    {
    }

    void Awake()
    {
        Screen.showCursor = false;
        //注册鼠标改变
        CursorMng.Instance.CursotChanged += ShowMouse;
        CursorMng.Instance.UICursotChanged += ShowUI;

        ShowMouse(ENUM_CURSOR_TYPE.CURSOR_NORMAL);
    }

    void OnGUI()
    {
        // Get mouse X and Y position as a percentage of screen width and height
        DrawCursor(Event.current.mousePosition.x, Event.current.mousePosition.y );
    }

    void DrawCursor(float x, float y)
    {
        if (mouseIcon == null)
        {
            if (!currentCursor)
            {
                LogManager.LogError("鼠标没有给予图片.");
                return;
            }
            GUI.DrawTexture(new Rect(x, y, currentCursor.width, currentCursor.height), currentCursor);
        }
        else
        {
            GUI.DrawTexture(new Rect(x, y, mouseIcon.width, mouseIcon.height), mouseIcon);
        }
    }

    Texture mouseIcon = null;
    void ShowUI(Texture icon)
    {
        mouseIcon = icon;
    }

    void ShowMouse(ENUM_CURSOR_TYPE cursorType)
    {
        switch (cursorType)
        {
            case ENUM_CURSOR_TYPE.CURSOR_WINBASE:
                break;
            case ENUM_CURSOR_TYPE.CURSOR_NORMAL:
                currentCursor = normalCursor;
                break;
            case ENUM_CURSOR_TYPE.CURSOR_ATTACK:
                currentCursor = attackCursor;
                break;
            case ENUM_CURSOR_TYPE.CURSOR_AUTORUN:
                break;
            case ENUM_CURSOR_TYPE.CURSOR_UNREACHABLE:
                break;
            case ENUM_CURSOR_TYPE.CURSOR_PICKUP:
            case ENUM_CURSOR_TYPE.CURSOR_MINE:
            case ENUM_CURSOR_TYPE.CURSOR_HERBS:
            case ENUM_CURSOR_TYPE.CURSOR_INTERACT:
                currentCursor = selectCursor;
                break;
            case ENUM_CURSOR_TYPE.CURSOR_FISH:
                break;
            case ENUM_CURSOR_TYPE.CURSOR_SPEAK:
                currentCursor = speakCursor;
                break;
            case ENUM_CURSOR_TYPE.CURSOR_REPAIR:
                break;
            case ENUM_CURSOR_TYPE.CURSOR_HOVER:
                break;
            case ENUM_CURSOR_TYPE.CURSOR_IDENTIFY:
                break;
            case ENUM_CURSOR_TYPE.CURSOR_ADDFRIEND:
                break;
            case ENUM_CURSOR_TYPE.CURSOR_EXCHANGE:
                break;
            case ENUM_CURSOR_TYPE.CURSOR_CATCH_PET:
                break;
            case ENUM_CURSOR_TYPE.CURSOR_HYPERLINK_HOVER:
                currentCursor = selectCursor;
                break;
            case ENUM_CURSOR_TYPE.CURSOR_NUMBER:
                break;
            default:
                break;
        }
    }
}
