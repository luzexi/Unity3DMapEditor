/**
	Tripper Object
*/

public class CTripperObject :	 CObject_Static
{
	// 左键指令的分析
	public override void FillMouseCommand_Left(ref SCommand_Mouse pOutCmd, tActionItem pActiveSkill)
    {
        if(Tripper_CanOperation())
	    {
            pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_HIT_TRIPPEROBJ;
		    pOutCmd.SetValue<int>(0,ServerID);
	    }
	    else
	    {
            pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
	    }
    }
	// 右键指令的分析
    public override void FillMouseCommand_Right(ref SCommand_Mouse pOutCmd, tActionItem pActiveSkill)
    {
        if(Tripper_CanOperation())
	    {
            pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_CONTEXMENU;
		    pOutCmd.SetValue<int>(0,ServerID);
	    }
	    else
	    {
            pOutCmd.m_typeMouse = MOUSE_CMD_TYPE.MCT_NULL;
	    }
    }
	//用于为掉落包、可采集物品设置淡入  [7/4/2011 zzy]
	public void					SetTransparent(float fTransparency, float fTime)
    {
        //todo
    }
};