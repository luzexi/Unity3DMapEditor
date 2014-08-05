using DBSystem;
namespace Interface
{
    public class Character
    {
        static readonly Character instance = new Character();
        public static Character Instance
        {
            get
            {
                return instance;
            }
        }

        public Character()
        {

        }

        public string GetName()
        {
            CObject_Character pCharObj = _GetTargetAsCharacter();
			
			if(pCharObj != null)
            	return pCharObj.GetCharacterData().Get_Name();
			else
				return "";
        }

        public float GetHPPercent()
        {
            CObject_Character pCharObj = _GetTargetAsCharacter();
            return pCharObj.GetCharacterData().Get_HPPercent();
        }

        public float GetMPPercent()
        {
            CObject_Character pCharObj = _GetTargetAsCharacter();
            return pCharObj.GetCharacterData().Get_MPPercent();
        }

        //对象是否存在 "target", "pet", ...
        public bool IsPresent()
        {
            if (CObjectManager.Instance.GetMainTarget() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //是否是敌对阵营
        public int IsEnemy()
        {
            CObject_Character pCharObj = (CObject_Character)CObjectManager.Instance.GetMainTarget();

            //选中的不是其他玩家
            //if( g_theKernel.IsKindOf( pCharObj->GetClass(), GETCLASS(CObject_PlayerOther) ) == FALSE )
            //	state->PushInteger( -2 );

            ENUM_RELATION sCamp = GameInterface.Instance.GetCampType(pCharObj, CObjectManager.Instance.getPlayerMySelf());

            if (sCamp != ENUM_RELATION.RELATION_FRIEND)
            {
                return 1;  //敌人
            }
            return 0;
        }

        public string GetData(string typeName, int objServerID)
        {
            CObject_Character pCharObj = null;

            if (objServerID != -1)
            {
                pCharObj = (CObject_Character)(CDataPool.Instance.GetTargetEquip());
            }
            else
            {
                pCharObj = _GetTargetAsCharacter();
            }

            if (pCharObj == null)
            {
                return "没有得到Obj指针";
            }

            if (typeName == "LEVEL")   //等级
            {
                string ss;
                ss = pCharObj.GetCharacterData().Get_Level().ToString();
                return ss;
            }
            else if (typeName == "NAME")   //名字
            {
                return pCharObj.GetCharacterData().Get_Name();
            }
            else if (typeName == "ISPKENEMY")
            {
                if (CObjectManager.Instance.getPlayerMySelf().GetRelationOther(pCharObj) == 0)
                {
                    return "1";
                }
                return "0";
            }

            return "1";

        }

        public CObject_Character _GetTargetAsCharacter()
        {
            CObject pObj = (CObject)CObjectManager.Instance.GetMainTarget();
            //if (!pObj || !g_theKernel.IsKindOf(pObj->GetClass(), GETCLASS(CObject_Character)))
            //{
            // TDThrow("None target or it isn't character target!");
            // }
            if (pObj == null)
            {
                LogManager.LogError("None target or it isn't character target!");
            }
            return (CObject_Character)pObj;
        }


        internal string GetHeadIcon()
        {
            CObject_Character pCharObj = _GetTargetAsCharacter();
            if (pCharObj.GetCharacterType() == CHARACTER_TYPE.CT_MONSTER)
            {
                CObject_PlayerNPC npc = pCharObj as CObject_PlayerNPC;
                if (npc != null)
                    return npc.GetPortrait();
                else
                    return "";
            }

            //得到玩家和自己的头像
            string pImageName = "";
            //打开生物定义表
            _DBC_CHAR_FACE pFaceImage = CDataBaseSystem.Instance.GetDataBase<_DBC_CHAR_FACE>(
                (int)DataBaseStruct.DBC_CHAR_FACE).Search_Index_EQU(pCharObj.GetCharacterData().Get_PortraitID());

            if (pFaceImage != null)
            {
                pImageName = pFaceImage.pImageSetName;
            }

            return pImageName;
        }
    }
}