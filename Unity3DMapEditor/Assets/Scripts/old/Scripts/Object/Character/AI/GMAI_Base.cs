public abstract class CAI_Base
{
    public abstract bool		    Tick();

    // 压入一条指令
    public abstract bool		    PushCommand( SCommand_AI cmd );
    public abstract bool		    PushCommand_MoveTo(float fDestX, float fDestZ, bool bAutoMove);
    public abstract bool		    PushCommand_AutoHit(int isAutoHit);
    public abstract bool		    PushCommand_Jump();
    public abstract bool	        PushCommand_UseSkill(int idSkill, uint guidTarget, int idTargetObj, float fTargetX, float fTargetZ, float fDir);

    // 中断当前 AI
    protected abstract  void		    OnAIStopped();

    // 中断移动 [8/23/2010 Sun]
    protected abstract  void		    OnAIStopMove();	

    // 重置
    public abstract  void		    Reset();

    protected  abstract RC_RESULT	OnCommand(SCommand_AI cmd );
    //AI指令
   

    public CAI_Base(CObject_Character CharObj)
    {
        m_pCharacterObj = CharObj;
    }

    public CObject_Character	    GetCharacter() { return m_pCharacterObj; }
    protected  CObject_Character	m_pCharacterObj;
};
