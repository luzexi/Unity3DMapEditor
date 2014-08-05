using System;
using System.Collections;
using System.Collections.Generic;
using Interface;
using Network.Packets;
//--------------------------------------------------------------------------------------------------------------------------
//
// 在界面上显示的模型.
//
public class CModelShowInUI
{
    //-----------------------------------------------------------------------------------------------------------------------
    //
    // 用于在ui界面上显示的信息.
    //
    public static uint m_TeamNumCount = 0;					// 模型计数
    public CObject_PlayerOther m_pAvatar;						// 用于UI显示的逻辑对象.
    public string m_szBufModel;				// 模型名字.


    // 创建新的模型
    public bool CreateNewUIModel()
    {
        return true;
    }

    // 删除ui模型
    public bool DestroyUIModel()
    {
        return true;
    }

    // 设置模型信息
    public void SetUIModelInfo(HUMAN_EQUIP point, int nID)
    {

    }

    // 得到ui模型名字
    public string GetUIModelName()
    {
        return "";
    }

    // 脸部模型
    public void SetFaceMeshId(int nID)
    {
    }

    // 头发模型
    public void SetFaceHairId(int nID)
    {
    }
    // 头发颜色
    public void SetHairColor(uint nID)
    {

    }
}
// 客户端队伍|团队数据结构
public class TeamMemberInfo
{
    public uint m_GUID;
    public int m_OjbID;						//
    public short m_SceneID;
    public string m_szNick;	// 1.昵称
    public int m_uFamily;						// 2.门派
    public int m_uDataID;						// 3.性别
    public int m_uLevel;						// 4.等级
    public int m_nPortrait;					// 5.头像
    public WORLD_POS m_WorldPos;						// 6.位置（坐标）
    public int m_nHP;							// 7.HP
    public int m_dwMaxHP;						// 8.HP上限
    public int m_nMP;							// 9.MP
    public int m_dwMaxMP;						// 10.MP上限
    public int m_nAnger;						// 11.怒气
    public int m_WeaponID;						// 12.武器
    public int m_CapID;						// 13.帽子
    public int m_ArmourID;						// 14.衣服
    public int m_CuffID;						// 15.护腕
    public int m_FootID;						// 16.靴子
    // 17.buff，暂时不考虑
    public SimpleImpactList m_SimpleImpactList;				// Buff 列表
    public bool m_bDeadLink;					// 18.断线
    public bool m_bDead;						// 19.死亡
    public int m_uFaceMeshID;					// 20.面部模型
    public int m_uHairMeshID;					// 21.头发模型
    public uint m_uHairColor;					// 22.头发颜色
    public int m_uBackID;						// 23.背饰 [8/30/2010 Sun]

    public CModelShowInUI m_UIModel = new CModelShowInUI();						// 在界面显示的模型
    public TeamMemberInfo()
    {
        m_GUID = 0;
        m_OjbID = 0;						//
        m_SceneID = 0;
        m_uFamily = 9;						// 2.五行属性
        m_uDataID = 0;						// 3.性别
        m_uLevel = 0;						// 4.等级
        m_nPortrait = -1;						// 5.头像
        m_nHP = 0;						// 7.HP
        m_dwMaxHP = 0;						// 8.HP上限
        m_nMP = 0;						// 9.MP
        m_dwMaxMP = 0;						// 10.MP上限
        m_nAnger = 0;						// 11.怒气
        m_WeaponID = 0;						// 12.武器
        m_CapID = 0;						// 13.帽子
        m_ArmourID = 0;						// 14.衣服
        m_CuffID = 0;						// 15.护腕
        m_FootID = 0;						// 16.靴子
        m_bDeadLink = false;					// 18.断线
        m_bDead = false;					// 19.死亡
        m_uFaceMeshID = 0;						// 20.面部模型
        m_uHairMeshID = 0;						// 21.头发模型
        m_uHairColor = 0;						// 22.头发颜色
        m_uBackID = 0;						// 23.背饰 [8/30/2010 Sun]
    }
}

public class TeamInfo
{
    public short m_TeamID;			// 1 or 2, 3, ...
    public List<TeamMemberInfo> m_TeamMembers = new List<TeamMemberInfo>();		// [MAX_TEAM_MEMBER]
}

public struct TeamCacheInfo // 这个东西是为了面板显示申请人或者邀请队伍的信息所设立
{
    public uint m_GUID;
    public string m_szNick;	// 1.昵称
    public uint m_uFamily;						// 2.五行属性
    public short m_Scene;						// 3.场景
    public uint m_uLevel;						// 4.等级
    public uint m_uDetailFlag;					// 以下信息是否显示
    public ushort m_uDataID;						// 5.性别
    public uint m_WeaponID;						// 7.武器
    public uint m_CapID;						// 8.帽子
    public uint m_ArmourID;						// 9.衣服
    public uint m_CuffID;						// 10.护腕
    public uint m_FootID;						// 11.靴子
    public uint m_uFaceMeshID;					// 12.面部模型
    public uint m_uHairMeshID;					// 13.头发模型
    public uint m_uHairColor;					// 14.头发颜色

    public CModelShowInUI m_UIModel;						// 在界面显示的模型


};


public struct InviteTeam
{
    public uint m_InviterGUID;
    public List<TeamCacheInfo> m_InvitersInfo;
};

public class CTeamOrGroup
{ // when a team change to a group, the member of m_TeamMembers will be assigned to m_TeamInfo[0].

    public enum TEAM_OR_GROUP_TYPE
    {
        INVAILD_TYPE = 0,
        TEAM_TYPE = 1,
        GROUP_TYPE = 2,
    }

    public enum UI_ON_OPEN
    {
        UI_ALL_CLOSE = 0,
        UI_INVITE_ON_OPEN = 1,
        UI_APPLY_ON_OPEN = 2,
    }


    public const int MAX_INVITE_TEAM = 7;
    public const int MAX_PROPOSER = 18;


    public CTeamOrGroup()
    {
        CleanUp();
    }

    public void CleanUp()
    {
        m_Type = TEAM_OR_GROUP_TYPE.INVAILD_TYPE;
        m_ID = MacroDefine.INVALID_ID;
        m_MyTeamID = 0xFF;
        m_LeaderID = MacroDefine.UINT_MAX;
        m_TeamAllocation = 0;
    }

    public bool HasTeam()
    {
        return m_Type != TEAM_OR_GROUP_TYPE.INVAILD_TYPE;
    }

    // 组建队伍，设置队伍的 leader，并且加入为第一个成员
    public bool CreateTeam(TeamMemberInfo leader, short TeamID)
    {
        if (m_Type != TEAM_OR_GROUP_TYPE.INVAILD_TYPE)
        {
            throw new Exception("create team failed  type = " + m_Type.ToString());
        }

        m_Type = TEAM_OR_GROUP_TYPE.TEAM_TYPE;
        m_ID = TeamID;
        SetLeaderGUID(leader.m_GUID);

        m_TeamMembers.Add(leader);

        //通知Talk Interface，队伍建立
        //Talk..TeamCreate(m_ID);
        //设置角色层数据
        CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_HaveTeamFlag(true);

        // 更新队伍信息 [9/26/2011 Ivan edit]
        CObject_Character player = CObjectManager.Instance.FindServerObject(leader.m_OjbID) as CObject_Character;
        if (player != null)
        {
            player.GetCharacterData().RefreshName();
        }

        return true;
    }

    // 解散队伍
    public void DismissTeam()
    {
        switch (m_Type)
        {
            case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                { // 队伍的情况
                    for (int i = 0; i < m_TeamMembers.Count; i++)
                    {
                        // 更新队员头顶信息颜色 [10/9/2011 Ivan edit]
                        CObject_Character player = CObjectManager.Instance.FindServerObject((int)m_TeamMembers[i].m_OjbID) as CObject_Character;
                        if (player != null)
                        {
                            player.GetCharacterData().RefreshName();
                        }
                    }
                    m_TeamMembers.Clear();
                }
                break;
            case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                {
                    m_TeamInfo.Clear();
                }
                break;
            default:
                return;
        }

        CleanUp();

        //设置角色层数据
        CObject_PlayerMySelf self = CObjectManager.Instance.getPlayerMySelf();
        if (self != null)
        {
            self.GetCharacterData().Set_HaveTeamFlag(false);
            self.GetCharacterData().Set_TeamLeaderFlag(false);
            self.GetCharacterData().Set_TeamFullFlag(false);
            self.GetCharacterData().Set_TeamFollowFlag(false);
        }
    }

    // 增加一个组员，如果是团队的话，需要传入团队所在的小组号
    public void AddMember(TeamMemberInfo member, short TeamID, byte TeamIndex)
    {
        TeamMemberInfo pTMInfo = new TeamMemberInfo();

        pTMInfo.m_GUID = member.m_GUID;
        pTMInfo.m_OjbID = member.m_OjbID;						//
        pTMInfo.m_SceneID = member.m_SceneID;
        pTMInfo.m_uFamily = member.m_uFamily;					// 2.五行属性
        pTMInfo.m_uDataID = member.m_uDataID;					// 3.性别
        pTMInfo.m_nPortrait = member.m_nPortrait;					// 5.头像
        pTMInfo.m_szNick = member.m_szNick;

        CObject_PlayerMySelf pObj = CObjectManager.Instance.getPlayerMySelf();
        switch (m_Type)
        {
            case TEAM_OR_GROUP_TYPE.INVAILD_TYPE:
                {
                    if (CreateTeam(pTMInfo, TeamID) == false)
                        pTMInfo = null;
                }
                break;
            case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                { // 队伍加人的情况
                    if (m_TeamMembers.Count >= GAMEDEFINE.MAX_TEAM_MEMBER)
                    {
                        pTMInfo = null;
                        CEventSystem.Instance.PushEvent(GAME_EVENT_ID.GE_INFO_SELF, "队伍已满，无法加人");
                        return;
                    }

                    m_TeamMembers.Add(pTMInfo);
                    // 队伍满人标记 [6/14/2011 edit by ZL]
                    if (m_TeamMembers.Count == GAMEDEFINE.MAX_TEAM_MEMBER)
                    {
                        CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_TeamFullFlag(true);

                    }
                    // 更新队伍信息 [9/26/2011 Ivan edit]
                    CObject_Character player = CObjectManager.Instance.FindServerObject((int)pTMInfo.m_OjbID) as CObject_Character;
                    if (player != null)
                    {
                        player.GetCharacterData().RefreshName();
                    }
                }
                break;
            case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                { // 团队加人的情况
                    if (TeamIndex < 0 || TeamIndex >= GAMEDEFINE.MAX_TEAMS_IN_GROUP)
                    {
                        pTMInfo = null;
                        return;
                    }

                    ++TeamIndex; // 默认传入的数字为 0 ~ MAX_TEAMS_IN_GROUP-1

                    TeamInfo pTeamInfo = GetTeam(TeamIndex);

                    if (pTeamInfo != null)
                    {
                        if (pTeamInfo.m_TeamMembers.Count >= GAMEDEFINE.MAX_TEAM_MEMBER)
                        {
                            pTMInfo = null;
                            return;
                        }
                    }
                    else
                    { // 如果该小组不存在，则创建，并添加入团队
                        pTeamInfo = new TeamInfo();
                        pTeamInfo.m_TeamID = TeamIndex;
                        m_TeamInfo.Add(pTeamInfo);

                        //队伍满
                        if (pTeamInfo.m_TeamMembers.Count >= GAMEDEFINE.MAX_TEAM_MEMBER)
                        {
                            CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_TeamFullFlag(true);
                        }
                    }

                    pTeamInfo.m_TeamMembers.Add(pTMInfo);

                    if (pTMInfo.m_GUID == pObj.GetServerGUID())
                    {
                        m_MyTeamID = TeamIndex;
                    }
                }
                break;
            default:
                return;
        }

        if (pTMInfo.m_GUID != pObj.GetServerGUID())
        { // 需要加载队友的头像窗口
            // 请求该队友的数据
            CGAskTeamMemberInfo Msg = new CGAskTeamMemberInfo();
            Msg.SceneID = pTMInfo.m_SceneID;
            Msg.ObjID = (uint)pTMInfo.m_OjbID;
            Msg.GUID = pTMInfo.m_GUID;

            NetManager.GetNetManager().SendPacket(Msg);
        }
        else
        {
            FillMyInfo(pTMInfo);
        }

        //// 创建一个界面的模型.
        //pTMInfo.m_UIModel.CreateNewUIModel();
        //// 设置ui模型
        //// 设置性别
        //pTMInfo.m_UIModel.m_pAvatar.GetCharacterData().Set_RaceID(pTMInfo.m_uDataID);
    }

    // 一个组员离开
    public void DelMember(uint guid)
    {
        switch (m_Type)
        {
            case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                {
                    // Keep ObjId [10/9/2011 Ivan edit]
                    int teamMemberObjId = MacroDefine.INVALID_ID;

                    for (int i = 0; i < m_TeamMembers.Count; i++)
                    {
                        if (m_TeamMembers[i].m_GUID == guid)
                        {
                            teamMemberObjId = m_TeamMembers[i].m_OjbID;
                            m_TeamMembers.RemoveAt(i);
                            break;
                        }
                    }

                    //队伍不再满
                    if (m_TeamMembers.Count < GAMEDEFINE.MAX_TEAM_MEMBER)
                    {
                        CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_TeamFullFlag(false);
                    }

                    // 更新队伍信息 [9/26/2011 Ivan edit]
                    CObject_Character player = CObjectManager.Instance.FindServerObject(teamMemberObjId) as CObject_Character;

                    if (player != null)
                    {
                        player.GetCharacterData().RefreshName();
                    }
                }
                break;
            case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                { // 团队情况
                    bool bFind = false;

                    for (int i = 0; i < m_TeamInfo.Count; i++)
                    {
                        TeamInfo teamInfo = m_TeamInfo[i];
                        for (int j = 0; j < teamInfo.m_TeamMembers.Count; j++)
                        {
                            if (teamInfo.m_TeamMembers[j].m_GUID == guid)
                            {
                                teamInfo.m_TeamMembers.RemoveAt(j);
                                bFind = true;
                                if (teamInfo.m_TeamMembers.Count < 1)
                                    m_TeamInfo.RemoveAt(i);

                                break;
                            }
                        }
                        if (bFind)
                            break;
                    }

                }
                break;
            default:
                return;
        }

        if (guid == m_LeaderID)
        { // choose a new leader
            switch (m_Type)
            {
                case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                    { // 队伍情况
                        if (m_TeamMembers.Count < 1)
                        { // 不在此处处理队伍解散的情况
                            return;
                        }

                        SetLeaderGUID(m_TeamMembers[0].m_GUID);
                    }
                    break;
                case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                    { // 团队情况，暂时不考虑团队选择新团长的规则
                        //Assert(FALSE);
                    }
                    break;
                default:
                    //Assert(FALSE);
                    return;
            }
        }
    }

    // 更换队员位置
    public void ExchangeMemberPosition(uint guid1, uint guid2, byte TeamIndex1, byte TeamIndex2)
    {
        TeamMemberInfo pTMInfo1, pTMInfo2;

        pTMInfo1 = GetMember(guid1);
        if (pTMInfo1 == null)
        {
            throw new NullReferenceException("Team member1 is null : " + guid1);
        }

        pTMInfo2 = GetMember(guid2);
        if (pTMInfo2 == null)
        {
            throw new NullReferenceException("Team member2 is null : " + guid2);
        }

        switch (m_Type)
        {
            case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                { // 队伍情况
                    for (int i = 0; i < m_TeamMembers.Count; ++i)
                    {
                        if (m_TeamMembers[i].m_GUID == pTMInfo1.m_GUID)
                        {
                            m_TeamMembers[i] = pTMInfo2;
                            continue;
                        }

                        if (m_TeamMembers[i].m_GUID == pTMInfo2.m_GUID)
                        {
                            m_TeamMembers[i] = pTMInfo1;
                            continue;
                        }
                    }
                }
                break;
            case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                { // 团队情况（暂时不考虑）
                }
                break;
            default:
                return;
        }
    }

    // 更新队员信息（暂时没有用到）
    public void UpdateMemberInfo(TeamMemberInfo member, uint guid)
    {

    }

    // 队长
    public uint GetLeaderGUID() { return m_LeaderID; }

    // 设置队长 GUID
    public void SetLeaderGUID(uint guid)
    {
        if (guid == CObjectManager.Instance.getPlayerMySelf().GetServerGUID())
        {
            CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_TeamLeaderFlag(true);
        }
        else
        {
            CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_TeamLeaderFlag(false);
        }

        m_LeaderID = guid;
    }

    // 任命新队长
    public void Appoint(uint guid)
    {
        switch (m_Type)
        {
            case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                { // 队伍情况
                    TeamMemberInfo pTMInfo;

                    for (int i = 0; i < m_TeamMembers.Count; i++)
                    {
                        if (m_TeamMembers[i].m_GUID == guid)
                        {
                            pTMInfo = m_TeamMembers[i]; // 释放队员信息
                            m_TeamMembers.RemoveAt(i);
                            m_TeamMembers.Insert(0, pTMInfo);
                            SetLeaderGUID(guid);
                            break;
                        }
                    }
                }
                break;
            case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                { // 团队情况
                }
                break;
            default:
                return;
        }
    }
    // 得到队员的数量
    public int GetTeamMemberCount(byte TeamIndex /*= -1*/)
    {
        switch (m_Type)
        {
            case TEAM_OR_GROUP_TYPE.INVAILD_TYPE:
                return 0;
            case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                { // 队伍的情况
                    return m_TeamMembers.Count;
                }
                break;
            case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                { // 团队的情况
                    if (TeamIndex < 1 || TeamIndex > GAMEDEFINE.MAX_TEAMS_IN_GROUP)
                    {
                        return 0;
                    }

                    return GetTeam(TeamIndex).m_TeamMembers.Count;
                }
                break;
            default:
                return 0;
        }
    }
    public int GetTeamMemberCount()
    {
        return GetTeamMemberCount(0);
    }

    // 得到某个队友显示在队友界面里面的具体位置 1,2,3...
    public int GetMemberUIIndex(uint guid, byte TeamIndex/* = -1*/)
    {
        // 这里的 TeamIndex 不需要自增
        List<TeamMemberInfo> TeamMembers;
        uint myGUID = (uint)CObjectManager.Instance.getPlayerMySelf().GetServerGUID();

        if (guid == myGUID)
        { // 自己没有序号
            return MacroDefine.INVALID_ID;
        }

        switch (m_Type)
        {
            case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                { // 队伍的情况
                    TeamMembers = m_TeamMembers;
                }
                break;
            case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                { // 团队的情况
                    if (TeamIndex < 1 || TeamIndex > GAMEDEFINE.MAX_TEAMS_IN_GROUP)
                    {
                        return MacroDefine.INVALID_ID;
                    }

                    if (m_MyTeamID != TeamIndex)
                    { // 暂时不考虑显示团队成员信息
                        return MacroDefine.INVALID_ID;
                    }

                    TeamMembers = GetTeam(TeamIndex).m_TeamMembers;
                }
                break;
            default:
                return MacroDefine.INVALID_ID;
        }

        int i=0;
        foreach (TeamMemberInfo info in TeamMembers)
        {
            if (info.m_GUID != myGUID)
                i++;
            if (info.m_GUID == guid)
                return i;

        }
        return MacroDefine.INVALID_ID;
    }
    public int GetMemberUIIndex(uint guid)
    {
        return GetMemberUIIndex(guid, 0);
    }

    // 将界面的索引转换成队伍数组中的索引
    public TeamMemberInfo GetMemberByUIIndex(int UIIndex, byte TeamIndex /*= -1*/)
    {
        List<TeamMemberInfo> pTeamMembers;
        uint myGUID = (uint)CObjectManager.Instance.getPlayerMySelf().GetServerGUID();

        switch (m_Type)
        {
            case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                { // 队伍的情况
                    pTeamMembers = m_TeamMembers;
                }
                break;
            case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                { // 团队的情况
                    if (TeamIndex < 1 || TeamIndex > GAMEDEFINE.MAX_TEAMS_IN_GROUP)
                    {
                        return null;
                    }

                    if (m_MyTeamID != TeamIndex)
                    { // 暂时不考虑显示团队成员信息
                        return null;
                    }

                    pTeamMembers = (GetTeam(TeamIndex).m_TeamMembers);
                }
                break;
            default:
                return null;
        }

        int i=0;
        foreach (TeamMemberInfo info in pTeamMembers)
        {

            if (i == UIIndex)
                return info;
            i++;
        }

        return null;
    }

    // 通过界面索引, 得到server id
    public int GetMemberServerIdByUIIndex(int UIIndex, byte TeamIndex /*= -1*/)
    {

        List<TeamMemberInfo> pTeamMembers;
        uint myGUID = (uint)CObjectManager.Instance.getPlayerMySelf().GetServerGUID();

        switch (m_Type)
        {
            case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                { // 队伍的情况
                    pTeamMembers = m_TeamMembers;
                }
                break;
            case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                { // 团队的情况
                    if (TeamIndex < 1 || TeamIndex > GAMEDEFINE.MAX_TEAMS_IN_GROUP)
                    {
                        return -1;
                    }

                    if (m_MyTeamID != TeamIndex)
                    { // 暂时不考虑显示团队成员信息
                        return -1;
                    }

                    pTeamMembers = (GetTeam(TeamIndex).m_TeamMembers);
                }
                break;
            default:
                return -1;
        }

        int i=0;
        foreach (TeamMemberInfo info in pTeamMembers)
        {
            if (info.m_GUID != myGUID)
                i++;
            if (i == UIIndex)
                return info.m_OjbID;
        }

        return -1;
    }

    // 通过界面索引得到选中队员的guid
    public uint GetMemberGUIDByUIIndex(int UIIndex, byte TeamIndex /*= -1*/)
    {
        TeamMemberInfo pInfo = GetMemberByUIIndex(UIIndex, TeamIndex);
        if (pInfo != null)
        {
            return pInfo.m_GUID;
        }
        return MacroDefine.UINT_MAX;
    }

    // 得到自己的索引
    public int GetSelfIndex(byte TeamIndex /*= -1*/)
    {
        return 0;
    }

    // 根据某个 guid 找到具体队友
    public TeamMemberInfo GetMember(uint guid)
    {
        switch (m_Type)
        {
            case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                { // 队伍情况

                    foreach (TeamMemberInfo info in m_TeamMembers)
                    {
                        if (info.m_GUID == guid)
                            return info;
                    }
                }
                break;
            case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                { // 团队情况
                    foreach (TeamInfo team in m_TeamInfo)
                    {
                        foreach (TeamMemberInfo info in team.m_TeamMembers)
                        {
                            if (info.m_GUID == guid)
                                return info;
                        }
                    }
                }
                break;
            default:
                //assert(FALSE);
                return null;
        }

        return null;
    }

    // 根据server id 找到具体队友
    public TeamMemberInfo GetMemberByServerId(int iServerId)
    {
        switch(m_Type)
	{
	case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
		{ // 队伍情况

            foreach (TeamMemberInfo info in m_TeamMembers)
            {
                if(info.m_OjbID == iServerId)
                    return info;
            }
		}
		break;
	case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
		{ // 团队情况
            foreach (TeamInfo team in m_TeamInfo)
            {
                foreach (TeamMemberInfo info in team.m_TeamMembers)
                {
                    if (info.m_OjbID == iServerId)
                        return info;
                }
            }
		}
		break;
	default:
		//assert(FALSE);
		return null;
	}

	return null;
    }

    // 得到队伍中第 idx 个队员
    public TeamMemberInfo GetMemberByIndex(int idx, byte TeamIndex /*= -1*/)
    {
        switch (m_Type)
        {
            case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
                { // 队伍的情况
                    if (m_TeamMembers.Count > idx)
                    {
                        return m_TeamMembers[idx];
                    }
                }
                break;
            case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
                { // 团队的情况
                    if (TeamIndex < 1 || TeamIndex > GAMEDEFINE.MAX_TEAMS_IN_GROUP)
                    {
                        return null;
                    }

                    if (GetTeam(TeamIndex).m_TeamMembers.Count > idx)
                    {
                        return GetTeam(TeamIndex).m_TeamMembers[idx];
                    }
                }
                break;
            default:
                return null;
        }

        return null;
    }
    public TeamMemberInfo GetMemberByIndex(int idx)
    {
        return GetMemberByIndex(idx, 0);
    }

    // 改变申请、邀请界面的打开状态
    public void SetUIFlag(UI_ON_OPEN flag)
    {
        if (flag == 0)
        {
            // 刷新数据结构，关闭申请（邀请）界面的时候将一些过期数据清除
            switch (m_UIFlag)
            {
                case UI_ON_OPEN.UI_INVITE_ON_OPEN:
                    {
                        while (m_InviteTeams.Count > MAX_INVITE_TEAM)
                        {
                            EraseInviteTeam(0);
                        }
                    }
                    break;
                case UI_ON_OPEN.UI_APPLY_ON_OPEN:
                    {
                        while (m_Proposers.Count > MAX_PROPOSER)
                        {
                            EraseProposer(0);
                        }
                    }
                    break;
                default:
                    return;
            }
        }

        m_UIFlag = flag;
    }

    // 取得申请、邀请界面的打开状态
    public UI_ON_OPEN GetUIFlag() { return m_UIFlag; }

    // 增加一个邀请队伍，TRUE 成功，反之失败
    public bool AddInviteTeam(InviteTeam pTeam)
    {
        for (int i = 0; i < m_InviteTeams.Count; ++i)
        {
            if (m_InviteTeams[i].m_InviterGUID == pTeam.m_InviterGUID)
            { // 同样的申请人就不接受了
                return false;
            }
        }

        m_InviteTeams.Add(pTeam);

        if (m_InviteTeams.Count > MAX_INVITE_TEAM)
        { // 只有在邀请界面关闭时才做即时操作
            if (m_UIFlag == UI_ON_OPEN.UI_ALL_CLOSE)
            {
                EraseInviteTeam(0);
            }
        }

        return true;
    }

    // 得到当前邀请队伍的数量
    public int GetInviteTeamCount() { return m_InviteTeams.Count; }

    // 根据索引得到某个队伍
    public InviteTeam GetInviteTeamByIndex(int idx)
    {
        if (idx < 0 || idx >= m_InviteTeams.Count)
        {
            
            return new InviteTeam();
        }

        return m_InviteTeams[idx];
    }
    // 根据名字得到某个队伍 [6/15/2011 edit by ZL] 
    public InviteTeam GetInviteTeamByName(string nickName)
    {
        foreach (InviteTeam inviteTeam in m_InviteTeams)
        {
            foreach (TeamCacheInfo cacheInfo in inviteTeam.m_InvitersInfo)
            {
                if (cacheInfo.m_szNick == nickName)
                    return inviteTeam;
            }
        }
 
        return new InviteTeam();
    }


    // 清除某个邀请队伍
    public void EraseInviteTeam(int idx)
    {
        if (idx < 0 || idx >= m_InviteTeams.Count)
        {
            return;
        }
        m_InviteTeams.RemoveAt(idx);
    }

    // 通过名字清楚某个邀请队伍 [6/16/2011 edit by ZL]
    public void EraseInviteTeamByName(string nickName)
    {
        int idx = 0;
        foreach (InviteTeam inviteTeam in m_InviteTeams)
        {
            
            foreach (TeamCacheInfo cacheInfo in inviteTeam.m_InvitersInfo)
            {
                if (cacheInfo.m_szNick == nickName)
                    break;
            }
            idx++;
        }
        if (idx == m_InviteTeams.Count) return;

        EraseInviteTeam(idx);
    }

    // 清除邀请队列
    public void ClearInviteTeam()
    {
        m_InviteTeams.Clear();
    }

    // 增加一个申请者
    public bool AddProposer(TeamCacheInfo pProposer)
    {
        foreach (TeamCacheInfo info in  m_Proposers)
        {
            if (info.m_GUID == pProposer.m_GUID)
                return false;
        }


        m_Proposers.Add(pProposer);

        if (m_Proposers.Count > MAX_PROPOSER)
        { // 只有在申请界面关闭时才做即时操作
            if (m_UIFlag == UI_ON_OPEN.UI_ALL_CLOSE)
            {
                EraseProposer(0);
            }
        }

        return true;
    }

    // 清除一个申请者
    public void EraseProposer(int idx)
    {
        if (idx < 0 || idx >= m_Proposers.Count)
        {
            return;
        }
        m_Proposers.RemoveAt(idx);
    }

    // 通过名字清除一个申请者 [6/15/2011 edit by ZL]
    public void EraseProposerByName(string nickName)
    {
        foreach (TeamCacheInfo info in m_Proposers)
        {
            if (info.m_szNick == nickName)
            {
                m_Proposers.Remove(info);
                return;
            }
        }
    }

    // 清除申请队列
    public void ClearProposer()
    {
        m_Proposers.Clear();
    }

    // 得到当前申请队列的数量
    public int GetProposerCount() { return (int)m_Proposers.Count; }

    // 根据索引得到某个申请人
    public TeamCacheInfo GetProposerByIndex(int idx)
    {
        if (idx < 0 || idx >= m_Proposers.Count)
        {
            return new TeamCacheInfo();
        }

        return m_Proposers[idx];
    }

    // 根据索引得到某个申请人 [6/15/2011 edit by ZL]
    public TeamCacheInfo GetProposerByName(string nickName)
    {
        for (int idx = 0; idx < m_Proposers.Count; ++idx)
        {
            if (m_Proposers[idx].m_szNick == nickName)
            {
                return m_Proposers[idx];
            }
        }

        return new TeamCacheInfo();
    }

    // 填充队伍中当前玩家的详细信息
    public void FillMyInfo()
    {
        if (!HasTeam())
        { // 没有队伍不进行操作
            return;
        }

        TeamMemberInfo pMyTMInfo = null;

        uint guid = (uint)CObjectManager.Instance.getPlayerMySelf().GetServerGUID();
        pMyTMInfo = GetMember(guid);

        if (pMyTMInfo == null)
        {
            return;
        }

        FillMyInfo(pMyTMInfo);
    }

    // 设置模型信息
    public void SetModelLook()
    {
        if ( !HasTeam() )
	{ // 没有队伍不进行操作
		return;
	}

	TeamMemberInfo pMyTMInfo = null;

	uint guid = (uint)CObjectManager.Instance.getPlayerMySelf().GetServerGUID();
	pMyTMInfo = GetMember(guid);
	if ( pMyTMInfo == null )
	{
		return;
	}

	// 设置ui模型
	pMyTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_WEAPON, pMyTMInfo.m_WeaponID);
	
	// 设置ui模型
    pMyTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_CAP, pMyTMInfo.m_CapID);
		
	// 设置ui模型
    pMyTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_ARMOR, pMyTMInfo.m_ArmourID);
		
	// 设置ui模型
    pMyTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_CUFF, pMyTMInfo.m_CuffID);
	
	// 设置ui模型
    pMyTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_BOOT, pMyTMInfo.m_FootID);

    pMyTMInfo.m_UIModel.SetUIModelInfo(HUMAN_EQUIP.HEQUIP_BACK, pMyTMInfo.m_uBackID);


	if(pMyTMInfo.m_uFaceMeshID < 255)
	{
		// 设置脸形
		pMyTMInfo.m_UIModel.SetFaceMeshId(pMyTMInfo.m_uFaceMeshID);
	}
		
	if(pMyTMInfo.m_uHairMeshID < 255)
	{
		// 设置发型
		pMyTMInfo.m_UIModel.SetFaceHairId(pMyTMInfo.m_uHairMeshID);
	}
	
	//if(pMyTMInfo->m_uHairColor < 255)
	//{
	//	// 设置颜色
	//	pMyTMInfo->m_UIModel.SetHairColor(pMyTMInfo->m_uHairColor);
	//}
	//else
	//{
	//	// 设置颜色
	//	pMyTMInfo->m_UIModel.SetHairColor(0);
	//}//

	// 设置颜色
	pMyTMInfo.m_UIModel.SetHairColor(pMyTMInfo.m_uHairColor);
    }

    // 是否在同一个场景中.
    public bool IsInScene(int iIndex)
    {
        switch(m_Type)
	{
	case TEAM_OR_GROUP_TYPE.TEAM_TYPE:
		{ // 队伍的情况
			if( m_TeamMembers.Count > iIndex )
			{
				TeamMemberInfo pInfo = m_TeamMembers[iIndex];
				if(pInfo != null)
				{
					if(pInfo.m_SceneID == WorldManager.Instance.GetActiveSceneID())
					{
                        return true;
					}
				}
				else
				{
					return false;
				}
					
			}
			else
			{
				return false;
			}
		}
		break;
	case TEAM_OR_GROUP_TYPE.GROUP_TYPE:
		{ // 团队的情况
			
			return false;
		}
		break;
	default:
		{
			return false;
		}
	}

	return false;
    }

    // 更新 Buff 列表
    public void UpdateImpactsList(int ObjID, SimpleImpactList pSimpleImpactList)
    {
        TeamMemberInfo pTMInfo = GetMemberByServerId(ObjID);
        if (pTMInfo == null)
        {
            return;
        }

        pTMInfo.m_SimpleImpactList.SetImpactList(pSimpleImpactList);

        int idx = GetMemberUIIndex(pTMInfo.m_GUID, 0);
        if (idx != MacroDefine.INVALID_ID)
        {
            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_UPTEDATA_MEMBER_INFO, idx);
        }
    }

    // 增加 Buff
    public void AddImpact(int ObjID, short ImpactID)
    {
        TeamMemberInfo pTMInfo = GetMemberByServerId(ObjID);
        if (pTMInfo == null)
        {
            return;
        }

        pTMInfo.m_SimpleImpactList.AddImpact(ImpactID);

        int idx = GetMemberUIIndex(pTMInfo.m_GUID, 0);
        if (idx != MacroDefine.INVALID_ID)
        {
            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_UPTEDATA_MEMBER_INFO, idx);
        }
    }

    // 减少 Buff
    public void RemoveImpact(int ObjID, short ImpactID)
    {
        TeamMemberInfo pTMInfo = GetMemberByServerId(ObjID);
        if (pTMInfo == null)
        {
            return;
        }

        pTMInfo.m_SimpleImpactList.RemoveImpact(ImpactID);

        int idx = GetMemberUIIndex(pTMInfo.m_GUID, 0);
        if (idx != MacroDefine.INVALID_ID)
        {
            GameProcedure.s_pEventSystem.PushEvent(GAME_EVENT_ID.GE_TEAM_UPTEDATA_MEMBER_INFO, idx);
        }
    }

    // 修改组队分配方式 [8/24/2011 edit by ZL]
    public void SetTeamAllocation(byte ruler) { m_TeamAllocation = ruler; }

    // 得到组队分配方式 [8/24/2011 edit by ZL]
    public int GetTeamAllocation() { return (int)m_TeamAllocation; }

    // 得到自己的队伍信息（从自己的数据池获取）
    void FillMyInfo(TeamMemberInfo member)
    {
        if(member == null)
            throw new NullReferenceException("team member in FillMyInfo(member)");

	CObject_PlayerMySelf pMe = CObjectManager.Instance.getPlayerMySelf();

	CCharacterData pMyData = pMe.GetCharacterData();

	if(null == pMyData)
	{
		return ;
	}
	member.m_szNick= pMyData.Get_Name();
	member.m_uFamily = pMyData.Get_MenPai();
	member.m_uDataID = pMyData.Get_RaceID();
	member.m_uLevel = pMyData.Get_Level();
	member.m_nPortrait = pMyData.Get_PortraitID();
	member.m_WorldPos.m_fX = pMe.GetPosition().x;
	member.m_WorldPos.m_fZ = pMe.GetPosition().z;
	member.m_nHP = pMyData.Get_HP();
	member.m_dwMaxHP = pMyData.Get_MaxHP();
	member.m_nMP = pMyData.Get_MP();
	member.m_dwMaxMP = pMyData.Get_MaxMP();
	member.m_nAnger = 100; // 客户端没有
	member.m_WeaponID = pMyData.Get_Equip(HUMAN_EQUIP.HEQUIP_WEAPON);
    member.m_CapID = pMyData.Get_Equip(HUMAN_EQUIP.HEQUIP_CAP);
    member.m_ArmourID = pMyData.Get_Equip(HUMAN_EQUIP.HEQUIP_ARMOR);
    member.m_CuffID = pMyData.Get_Equip(HUMAN_EQUIP.HEQUIP_CUFF);
    member.m_FootID = pMyData.Get_Equip(HUMAN_EQUIP.HEQUIP_BOOT);
    member.m_uBackID = pMyData.Get_Equip(HUMAN_EQUIP.HEQUIP_BACK);//  [8/30/2010 Sun]
    member.m_bDead = (pMe.CharacterLogic_Get() == ENUM_CHARACTER_LOGIC.CHARACTER_LOGIC_DEAD);

	member.m_uFaceMeshID = pMyData.Get_FaceMesh();
	member.m_uHairMeshID = pMyData.Get_HairMesh();
	member.m_uHairColor  = pMyData.Get_HairColor();
    }

    // 根据小组号 N 得到第 N 个小组
    TeamInfo GetTeam(byte TeamIndex)
    {
        foreach(TeamInfo info in m_TeamInfo)
	{
		if( info.m_TeamID == TeamIndex )
		{
			return info;
		}
	}

	return null;
    }

    TEAM_OR_GROUP_TYPE m_Type;				// team or group
    short m_ID;				// the serial number of the team or group in the game world
    // it can be used to identify the empty team or group
    byte m_MyTeamID;			// the team ID of mine in my group
    uint m_LeaderID;			// guid of the team leader
    List<TeamMemberInfo> m_TeamMembers = new List<TeamMemberInfo>();		// [MAX_TEAM_MEMBER]
    List<TeamInfo> m_TeamInfo = new List<TeamInfo>();			// [MAX_TEAMS_IN_GROUP]

    byte m_TeamAllocation;	// 队伍分配模式 [8/24/2011 edit by ZL]

    // 申请邀请信息
    UI_ON_OPEN m_UIFlag;			// 用于判断邀请人界面或者申请人界面是否已经打开
    List<TeamCacheInfo> m_Proposers = new List<TeamCacheInfo>();		// 申请人列表，队长可见
    List<InviteTeam> m_InviteTeams = new List<InviteTeam>();		// 邀请队伍信息列表，被邀请人可见
};
