
/**
	传送点
*/
using UnityEngine;
public class CTripperObject_Transport :	 CTripperObject
{
	//Tripper 类型
	 public override TRIPPER_OBJECT_TYPE	Tripper_GetType() 	{ return TRIPPER_OBJECT_TYPE.TOT_TRANSPORT; }
	//能否鼠标操作
	public override bool					Tripper_CanOperation()  { return false; }
	//鼠标类型
	public override ENUM_CURSOR_TYPE		Tripper_GetCursor()  { return  ENUM_CURSOR_TYPE.CURSOR_NORMAL; }
	//进入激活
	public virtual void					Tripper_Active() {}

	//-----------------------------------------------------
	///根据初始化物体，并同步到渲染层
	public	override	void				Initial(object pInit)
    {
        if ( mRenderInterface == null )
	    {
		    //创建渲染层实体
		    mRenderInterface = GFX.GFXObjectManager.Instance.createObject("Cast1",  GFX.GFXObjectType.ACTOR);
		    mRenderInterface.SetData(ID);

            mRenderInterface2 = GFX.GFXObjectManager.Instance.createObject("pilon", GFX.GFXObjectType.ACTOR);
            mRenderInterface2.SetData(ID);
	    }
        base.Initial(pInit);
        base.CreateRenderInterface();
    }
    public override void Tick()
    {
        SetMapPosition(mPosition.x, mPosition.z);
        base.Tick();
    }
	public override void Release()
    {
        GFX.GFXObjectManager.Instance.DestroyObject(mRenderInterface2);
        base.Release();
    }
	public override void SetPosition(Vector3 pos)
    {
        base.SetPosition(pos);

    }
	GFX.GfxObject			mRenderInterface2;

};
