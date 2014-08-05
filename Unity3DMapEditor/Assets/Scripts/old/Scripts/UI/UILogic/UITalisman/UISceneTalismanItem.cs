using UnityEngine;
public class UISceneTalismanItem:UIButton
{
    public int index_;
    public override void OnInput(ref POINTER_INFO ptr)
    {
        switch (ptr.evt)
        {
            case POINTER_INFO.INPUT_EVENT.MOVE:
                {
                    GameObject bufferToolTip = UIWindowMng.Instance.GetWindowGo("BufferTooltipWindow");
                    if (bufferToolTip != null)
                    {
                        Vector2 ptMouse = GameProcedure.s_pInputSystem.GetMouseUIPos();
                        UIBufferToolTip toolTip = bufferToolTip.GetComponent<UIBufferToolTip>();
                        //toolTip.ShowTooltip(ptMouse.x, ptMouse.y, bufferID_);
                    }
                }
                break;
            case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
                {
                    GameObject bufferToolTip = UIWindowMng.Instance.GetWindowGo("BufferTooltipWindow");
                    if (bufferToolTip != null)
                    {
                        UIBufferToolTip toolTip = bufferToolTip.GetComponent<UIBufferToolTip>();
                        toolTip.Hide();
                    }
                }
                break;
            default:
                break;
        }
        base.OnInput(ref ptr);
    }
}