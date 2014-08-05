using UnityEngine;
using System.Collections;

//struct _OWN_SKILL;
//struct _OWN_XINFA;
//struct _OWN_MISSION;
//class CTeamOrGroup;

public abstract class CDetailAttrib_Base
{

    public virtual bool Init()
    {
        return true;
    }

    public abstract void Term();

    public abstract void Tick();

    //public virtual void ChangeSkillAttrib( const _OWN_SKILL *pSkillInit ){}

    //public virtual void ChangeXinFaAttrib( const _OWN_XINFA *pXinFaInit ){}
    //生活技能数据发生改变
    //public virtual void ChangeLifeAbilityAttrib( INT idAbility, _OWN_ABILITY pAbility) {}
    //生活技能数据发生改变
    //public virtual void ChangeLifeAbilityAttrib( INT idAbility, BYTE type, UINT value) {}

    public abstract void AddMission(_OWN_MISSION pMission);

    public abstract void ModifyMission(_OWN_MISSION pMission);

    public abstract void ModifyMissionData(int[] pMissionData);

    public abstract void RemoveMission(int idMission);

    public abstract void UpdateCanPickMissionItemList(uint dwItemCount, uint[] paItemList);

    public abstract void AddCanPickMissionItem(uint dwItemDataID);

    public abstract void RemoveCanPickMissionItem(uint dwItemDataID);

    //public virtual void UpdateCoolDownList( const UINT *pdwCoolDownList, UINT dwUpdateNum ){}

    //public virtual CTeamOrGroup GetTeamOrGroup( ){ return NULL; }
}
