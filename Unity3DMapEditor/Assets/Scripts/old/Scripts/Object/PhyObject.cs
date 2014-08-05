using UnityEngine;
public class CObject_Phy :  CObject
{
    public enum PHY_EVENT_ID
    {
        PE_NONE = -1,									//空事件
        PE_COLLISION_WITH_GROUND,						//物体落地或与地面发生接触
        PE_OBJECT_BEGIN_MOVE,							//开始移动
        PE_OBJECT_TURN_AROUND,							//旋转
    };

    private const int       MAX_REGISTER_EVENTS_NUM = 20;
    private PHY_EVENT_ID[]  m_aEventList;
    private uint            m_nEventListNum;
    private bool            m_bIsInAir;
    private bool            m_bIsEnable;
    private Vector3         m_fvLinearSpeed;
    private Vector3         m_fvRotSpeed;
    private uint            m_nLastTickTime;
    public CObject_Phy()
    {
	   m_aEventList     = new PHY_EVENT_ID[MAX_REGISTER_EVENTS_NUM];
       m_bIsInAir       = false;
       m_bIsEnable      = false;
       m_fvLinearSpeed  = Vector3.zero;
       m_fvRotSpeed     = Vector3.zero;
       m_nLastTickTime  = 0;
    }

    public override void SetMapPosition(float x, float z)//根据地面的高度设置物体的位置
    {
        float y = 0;
        y = GFX.GfxUtility.getSceneHeight(x, z);

        Vector3 fvCurObjPos = GetPosition();
        float fInAirHeight  = fvCurObjPos.y;

        if (m_bIsEnable)
        {
            if (m_fvLinearSpeed.y < 0)
            {//下降过程
                //---------------------------------------------------
                if (y > fInAirHeight)
                {//已经落地了
                    SetPosition(new Vector3(x, y, z));		//在行走面上
                    DispatchPhyEvent(PHY_EVENT_ID.PE_COLLISION_WITH_GROUND,null);
                    m_bIsInAir = false;
                }
                else
                {//在空中设置空中的高度
                    SetPosition(new Vector3(x, fInAirHeight, z));	//在空中
                    m_bIsInAir = true;
                }
            }
            else
            {//上升过程，一定在空中
                if (y > fInAirHeight)
                {
                    SetPosition(new Vector3(x, y, z));	//在空中
                }
                else
                {
                    SetPosition(new Vector3(x, fInAirHeight,z));	//在空中
                }
                m_bIsInAir = true;
            }
        }
        else
        {
            SetPosition(new Vector3(x, y, z));		//在行走面上
        }
        SetFootPosition(new Vector3(x, y, z));
    }

    public override void Initial(object pInit)
    {
        base.Initial(pInit);
	    for(uint i =0; i< MAX_REGISTER_EVENTS_NUM; i++)
	    {
		    m_aEventList[i] = PHY_EVENT_ID.PE_NONE;
	    }
	    m_nEventListNum = 0;
	    //  [8/19/2010 Sun]
	    RegisterPhyEvent(PHY_EVENT_ID.PE_OBJECT_BEGIN_MOVE);
        
	    return;
    }

    public virtual void NotifyPhyEvent(PHY_EVENT_ID eventid, object pParam)
    {
        return;
    }
    public void RegisterPhyEvent(PHY_EVENT_ID eventid)
    {
	    if( m_nEventListNum == MAX_REGISTER_EVENTS_NUM )
	    {
		    return;
	    }
	    for(uint i = 0; i < m_nEventListNum; i++)
	    {
		    if(m_aEventList[i] == eventid)
			    return;
	    }
	    m_aEventList[m_nEventListNum++] = eventid;
    }

    //注销一个物理事件
    public void UnRegisterPhyEvent(PHY_EVENT_ID eventid)
    {
	    if( m_nEventListNum == 0)
	    {
		    return;
	    }

	    for(uint i = 0; i < m_nEventListNum; i++)
	    {
		    if(m_aEventList[i] == eventid)
		    {
			    m_aEventList[i] = m_aEventList[m_nEventListNum-1];
			    m_nEventListNum--;
			    break;
		    }
	    }
    }

    public void AddLinearSpeed(Vector3 vSpeed)
    {
	    if(m_bIsInAir == true)
		    return;
	    m_fvLinearSpeed = m_fvLinearSpeed+vSpeed;
    }

    public void PhyEnable(bool bFlag)
    {
	    if(bFlag == false)
	    {
		    m_fvLinearSpeed = Vector3.zero;
            m_fvRotSpeed    = Vector3.zero;
	    }
	    m_nLastTickTime	=	0;
	    m_bIsEnable		=	bFlag;
    }
    
    public override void Tick()
    {
        //调用物理系统对物体位置进行矫正
	    if(m_bIsEnable == false)
	    {
		    return;
	    }

	    //当前位置
	    Vector3	fvTempPos	= GetPosition();
	    Vector3	fvCurPos	= fvTempPos;
	    //物理每桢 50毫秒
	    uint	nCurTime	= GameProcedure.s_pTimeSystem.GetTimeNow();

	    //第一次不跑tick
	    if(m_nLastTickTime == 0)
	    {
		    //记录上次时间点
		    m_nLastTickTime	=	nCurTime;
		    return;
	    }
        const uint  PHY_MACRO_MSECONDS_PER_FRAME = 10;
        const float PHY_MACRO_GRAVITY = 9.8f;
        const float	SF_Factor = (float)((float)PHY_MACRO_MSECONDS_PER_FRAME/(float)1000);
	    uint	nDeltaTime	=	GameProcedure.s_pTimeSystem.CalSubTime(m_nLastTickTime, nCurTime);
	    if(nDeltaTime < PHY_MACRO_MSECONDS_PER_FRAME)
	    {
		    return;
	    }

	    uint	nStridTimes	    = (uint)(nDeltaTime/PHY_MACRO_MSECONDS_PER_FRAME);
	    float	fUsedGravity	=	PHY_MACRO_GRAVITY;

	    while(nStridTimes != 0)
	    {
		    //下落速度要有个增量（真实物理显得人物太重了）
		    if(m_fvLinearSpeed.y > 0)
		    {
			    fUsedGravity = PHY_MACRO_GRAVITY + 25.0f;
		    }
		    else
		    {
			    fUsedGravity = PHY_MACRO_GRAVITY + 70.0f;
		    }

		    //计算垂直的速度变化
		    m_fvLinearSpeed.y -= fUsedGravity*SF_Factor;

		    //更新位置
		    fvCurPos = fvCurPos+m_fvLinearSpeed*SF_Factor;

		    //
		    nStridTimes--;
	    }
    	
	    //记录上次时间点
	    m_nLastTickTime	=	nCurTime;

	    //更新位置
        SetPosition(fvCurPos);

	    //更新位置
	    SetMapPosition(fvCurPos.x, fvCurPos.z);
	    base.Tick();
    }

    void DispatchPhyEvent(PHY_EVENT_ID eventid, object pParam)
    {
	    for(uint i = 0; i<m_nEventListNum; i++)
	    {
		    if(m_aEventList[i] == eventid)
		    {//已被注册
			    NotifyPhyEvent(eventid, pParam);
			    return;
		    }
	    }
    }
}