
/**
	生活技能
	操作平台
*/
using UnityEngine;
public class CTripperObject_Platform :	 CTripperObject
{
	//Tripper 类型
	public override TRIPPER_OBJECT_TYPE		Tripper_GetType() 	{ return TRIPPER_OBJECT_TYPE.TOT_PLATFORM; }
	//能否鼠标操作
	public override bool Tripper_CanOperation() 
    {
        return false;
    }
	//鼠标类型
	public override ENUM_CURSOR_TYPE Tripper_GetCursor() { return ENUM_CURSOR_TYPE.CURSOR_SPEAK; }
	//进入激活
	public override void Tripper_Active()
    {
        //todo
//         Packets::CGCharDefaultEvent Msg; 
// 	    Msg.setObjID( GetServerID() );
// 
// 	    CNetManager::GetMe()->SendPacket(&Msg);
    }
	//设置平台的类型
	public bool							SetPlatformID(int nID)
    {
        return true;
    }
	//取得平台的类型
	public int								GetPlatformID()
    {
        return MacroDefine.INVALID_ID;
    }

	//-----------------------------------------------------
	///根据初始化物体，并同步到渲染层
	public override void Initial(object pInit)
    {

    }
	public override void Release()
    {
        base.Release();
    }
	public override void SetPosition(Vector3 pos)
    {

    }
	GFX.GfxObject			mRenderInterface2;
};
