using System.Collections.Generic;
public class UIBuffContainer
{
    private List<UIBuffer> bufferList_ = new List<UIBuffer>();
    public  void Add(UIBuffer  buffer)
    {
        bufferList_.Add(buffer);
    }
    //id -1 表示主角
    public void OnUpdate()
    {
        foreach(UIBuffer buffer in bufferList_)
        {
            buffer.UpdateTime();
        }
    }

    public void OnTick()
    {
        foreach (UIBuffer buffer in bufferList_)
        {
            buffer.TickTime();
        }
    }
}

//待加入组队的buff更新信息
public class UIBufferManager
{
    private UIBuffContainer avatarBuff_ = new UIBuffContainer();
    private UIBuffContainer targetBuff_ = new UIBuffContainer(); //其他玩家和npc的buff，队友
    private List<UIBuffContainer> teamBuff_   = new List<UIBuffContainer>();
    static readonly UIBufferManager sInstance = new UIBufferManager();
    static public UIBufferManager Instance
    {
        get
        {
            return sInstance;
        }
    }
   
    public UIBufferManager()
    {
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_IMPACT_SELF_UPDATE, OnUpdateBuffer);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_IMPACT_SELF_UPDATE_TIME, OnUpdateBuffer);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_BUFF_UPDATE, OnUpdateBuffer);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_MAINTARGET_CHANGED, OnUpdateBuffer);
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_MAINTARGET_OPEND, OnUpdateBuffer);
        for (int i = 0; i < GAMEDEFINE.MAX_TEAM_MEMBER; i++)
        {
            teamBuff_.Add(new UIBuffContainer());
        }
    }

    public void RegisterTargetBuffer(UIBuffer buffer)
    {
        targetBuff_.Add(buffer);
    }

    public void RegisterTeamBuffer(int teamIndex, UIBuffer buffer)
    {
        if (teamIndex >= GAMEDEFINE.MAX_TEAM_MEMBER)
        {
            LogManager.LogError("RegisterTeamBuffer teamIndex " + teamIndex + " greater than MAX_TEAM_MEMBER " + GAMEDEFINE.MAX_TEAM_MEMBER);
            return;
        }
        if (teamIndex < 0)
        {
            LogManager.LogError("RegisterTeamBuffer teamIndex " + teamIndex + " less than 0 ");
            return;
        }
        teamBuff_[teamIndex].Add(buffer);
    }

    public void RegisterAvatarBuffer(UIBuffer buffer)
    {
        avatarBuff_.Add(buffer);
    }

    public void OnUpdateBuffer(GAME_EVENT_ID id,List<string> vParam)
    {
        switch(id)
        {
            case GAME_EVENT_ID.GE_IMPACT_SELF_UPDATE:
            case GAME_EVENT_ID.GE_IMPACT_SELF_UPDATE_TIME:
            {
                avatarBuff_.OnUpdate();
            }
            break;
            case GAME_EVENT_ID.GE_BUFF_UPDATE:
            case GAME_EVENT_ID.GE_MAINTARGET_CHANGED:
            case GAME_EVENT_ID.GE_MAINTARGET_OPEND:
            {
                targetBuff_.OnUpdate();
            }
            break;
        }
    }

    public void Tick()
    {
        avatarBuff_.OnTick();
    }
}