using DBSystem;
public class CDirectlyImpactMgr
{
    static readonly CDirectlyImpactMgr sInstance = new CDirectlyImpactMgr();//唯一的实例
    static public CDirectlyImpactMgr Instance
    {
        get
        {
            return sInstance;
        }
    }

    public CDirectlyImpactMgr()
    {
    }
    public _DBC_DIRECT_IMPACT GetDirectlyImpact(uint impactID)
    {
        return CDataBaseSystem.Instance.GetDataBase<_DBC_DIRECT_IMPACT>((int)DataBaseStruct.DBC_DIRECTLY_IMPACT).Search_Index_EQU((int)impactID);
    }
};

