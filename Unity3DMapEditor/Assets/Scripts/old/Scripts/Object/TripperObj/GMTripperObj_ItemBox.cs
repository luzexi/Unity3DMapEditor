
/**
	放在地上的宝箱
*/

public class CTripperObject_ItemBox :	  CTripperObject
{
	//Tripper 类型
	public override TRIPPER_OBJECT_TYPE		Tripper_GetType() 	{ return TRIPPER_OBJECT_TYPE.TOT_ITEMBOX; }
	//能否鼠标操作
    public override bool Tripper_CanOperation() 
    {
        if(CObjectManager.Instance.getPlayerMySelf().GetServerGUID() == (int)m_idOwner)
 		    return true;
 	    else
 		    return false;
    }
	//鼠标类型
    public override ENUM_CURSOR_TYPE Tripper_GetCursor() { return ENUM_CURSOR_TYPE.CURSOR_PICKUP; }
	//进入激活
    public override void Tripper_Active()
    {
        Network.Packets.CGOpenItemBox msg = new Network.Packets.CGOpenItemBox();
 	    msg.setObjID((uint)ServerID);
 	    NetManager.GetNetManager().SendPacket(msg);
    }

	//-----------------------------------------------------
	///根据初始化物体，并同步到渲染层
    public override void Initial(object pInit)
    { 
        if ( mRenderInterface == null )
	    {
		    string itemBoxAsset = "ItemBox";//掉落包的资源名
            //创建渲染层实体
            mRenderInterface = GFX.GFXObjectManager.Instance.createObject(itemBoxAsset,  GFX.GFXObjectType.ACTOR);
		    //设置选择优先级
		    mRenderInterface.RayQuery_SetLevel(RAYQUERY_LEVEL.RL_TRIPPEROBJ);
		    mRenderInterface.SetData(ID);
	    }

        m_uBirthTime = GameProcedure.s_pTimeSystem.GetTimeNow();

	    //渐入  [7/4/2011 zzy]
	    SetTransparent(1,0);
	    SetTransparent(0,1);
        base.Initial(pInit);
        base.CreateRenderInterface();
    }
    ///逻辑函数
    public override void Tick()
    {
        //自动拾取
        if (!m_bPickUp)
        {
            if (m_uBirthTime + 2000 < GameProcedure.s_pTimeSystem.GetTimeNow())
            {
                if (Tripper_CanOperation())
                    Tripper_Active();

                m_bPickUp = true;
            }
        }
        SetMapPosition(mPosition.x, mPosition.z);
        base.Tick();
    }
	public override void Release()
    {
        base.Release();
    }
	//设置掉落箱的归属
	public void						SetOwnerGUID(uint nID) { m_idOwner = nID; }

	protected uint			m_idOwner;		//物品主人的ObjID,或者组队的ID

    bool m_bPickUp = false;

    uint m_uBirthTime = 0;

};
