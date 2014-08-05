using UnityEngine;
using DBSystem;
public class UIBufferToolTip : MonoBehaviour
{
    public short bufferID_;
    public SpriteText bufferDesc;
    private string bufferDesc_;
    void Awake()
    {
        gameObject.SetActiveRecursively(false);
        bufferDesc.text = "无描述";
    }

    public void Hide()
    {
        gameObject.SetActiveRecursively(false);
    }

    public void ShowTooltip(float x, float y, short BuffID)
    {
        if(bufferID_ != BuffID)
		{   
	        bufferID_ = BuffID;
	        _DBC_BUFF_IMPACT pBuffImpact = CDataBaseSystem.Instance.GetDataBase<_DBC_BUFF_IMPACT>((int)DataBaseStruct.DBC_BUFF_IMPACT).Search_Index_EQU((int)bufferID_);
            
            if (pBuffImpact != null)
	        {
                bufferDesc_ = pBuffImpact.m_pszInfo;
	        }
	        else
	        {
                bufferDesc_ = "无描述";
	        }
		}
        bufferDesc.Text = bufferDesc_;
        gameObject.transform.position = new Vector3(x, y-160, UIZDepthManager.TooltipZ);
		gameObject.SetActiveRecursively(true);
    }


    // 添加一个通用描述 [3/7/2012 SUN]
    public void ShowTooltip(float x, float y, string text)
    {
        bufferDesc.Text = text;
        gameObject.transform.position = new Vector3(x, y, UIZDepthManager.TooltipZ);
        gameObject.SetActiveRecursively(true);
    }
}