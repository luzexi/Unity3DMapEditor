//////////////////////////////////////////////////////////////////////////
//帮派管理器

public class GuildManager
{
    ////客户端显示的帮派列表
    //GuildInfo_t							m_GuildList[MAX_GUILD_SIZE];
    //INT									m_GuildNum;

    ////帮众列表
    //GuildMemberInfo_t					m_GuildMemList[USER_ARRAY_SIZE];
    //INT									m_GuildMaxMemNum;
    //INT									m_GuildMemNum;
    //STRING								m_GuildDesc;
    //STRING								m_GuildName;
    //BYTE								m_uPosition;
    //BYTE								m_uAccess;

    ////帮派的详细信息
    //GuildDetailInfo_t					m_GuildDetailInfo;

    ////可任命帮派的职位信息
    //GuildAppointPos_t					m_GuildPosAvail[GUILD_POSITION_SIZE];

    ////---------------------------------------------
    ////帮派成员列表
    ////---------------------------------------------
    //VOID Guild_ClearMemInfo();//清空所有帮众信息
    //GuildMemberInfo_t* Guild_GetMemInfoByIndex(INT nIndex);//通过索引获得帮众信息
    //VOID Guild_SetMemInfoByIndex(INT nIndex, GuildMemberInfo_t* pMemberInfo);//通过索引设置帮众信息

    //INT Guild_GetMemInfoNum() { return m_GuildMemNum; }//获得帮众信息
    //VOID Guild_SetMemInfoNum(INT iMemNum) { m_GuildMemNum = iMemNum; }//设置帮众信息

    //INT Guild_GetMaxMemNum() { return m_GuildMaxMemNum; }//获得帮众的最大数量
    //VOID Guild_SetMaxMemNum(INT iMaxMemNum) { m_GuildMaxMemNum = iMaxMemNum; }//设置帮众的最大数量

    //LPCTSTR Guild_GetDesc() { return m_GuildDesc.c_str(); }//获得帮派宗旨
    //VOID Guild_SetDesc(LPCTSTR pGuildDesc) { m_GuildDesc = pGuildDesc; }//设置帮派宗旨

    //LPCTSTR Guild_GetName() { return m_GuildName.c_str(); }//获得帮派宗旨
    //VOID Guild_SetName(LPCTSTR pGuildName) { m_GuildName = pGuildName; }//设置帮派宗旨

    //BYTE Guild_GetCurAccess() { return m_uAccess; }//获得当前人物权限
    //VOID Guild_SetCurAccess(BYTE uAccess) { m_uAccess = uAccess; }//设置当前人物权限

    //BYTE Guild_GetCurPosition() { return m_uPosition; }//获得当前人物职位
    //VOID Guild_SetCurPosition(BYTE uPosition) { m_uPosition = uPosition; }//设置当前人物职位


    ////---------------------------------------------
    ////帮派的详细信息
    ////---------------------------------------------
    //VOID Guild_ClearDetailInfo();//清空帮派的详细信息
    //GuildDetailInfo_t* Guild_GetDetailInfo();//获得帮派的详细信息
    //VOID Guild_SetDetailInfo(GuildDetailInfo_t* pDetailInfo);//设置帮派的详细信息


    ////---------------------------------------------
    ////帮派可任命职位
    ////---------------------------------------------
    //VOID Guild_ClearAppointInfo();//清空帮派可任命职位
    //GuildAppointPos_t* Guild_GetAppointInfoByIndex(INT nIndex);//获得帮派可任命职位
    //VOID Guild_SetAppointInfoByIndex(INT nIndex, GuildAppointPos_t* pDetailInfo);//设置帮派可任命职位
}