using System;
using System.Collections.Generic;
using DBSystem;

public class CampStand
{
    private static readonly CampStand instance = new CampStand();

    // 禁止在外面创建实例 [1/30/2012 Ivan]
    private CampStand() { }

    public static CampStand Instance
    {
        get { return instance; }
    }

    internal ENUM_RELATION CalcRelationType(short aCamp, short bCamp)
    {
        _DBC_CAMP_AND_STAND pCampAndStand = CDataBaseSystem.Instance.GetDataBase<_DBC_CAMP_AND_STAND>((int)DataBaseStruct.DBC_CAMP_AND_STAND).Search_Index_EQU((int)aCamp);
	    if( pCampAndStand == null )
	    {
		    if (aCamp == bCamp)//temp fix
			    return ENUM_RELATION.RELATION_FRIEND;
		    else
			    return ENUM_RELATION.RELATION_ENEMY;
	    }

	    if( bCamp < 0 || bCamp > pCampAndStand.campData.Length-1 )
            return ENUM_RELATION.RELATION_FRIEND;

        if (pCampAndStand.campData[bCamp] == 1) ////zzh: 1，表示敌对可攻击；0，不可以攻击
            return ENUM_RELATION.RELATION_ENEMY;
	    else
            return ENUM_RELATION.RELATION_FRIEND;
    }
}
