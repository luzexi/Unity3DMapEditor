
using System.Collections;
using DBSystem;
using Network;
using Network.Packets;
namespace Interface
{
    
public class PlayerMySelf
{
	static readonly PlayerMySelf instance = new PlayerMySelf();
    public static PlayerMySelf Instance
    {
        get
        {
            return instance;
        }
    }
	
	public PlayerMySelf()
    {
    }
	
	public  string GetName()
	{
		return CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Name();
	}

    public string GetCurTitle()
    {
        //心情不能出现在称号的位置
        //if (CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_CurTitleType())
        //{
           // return "";
        //}

        return CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_CurTitle();
    }
	
	public string GetData(string typeName)
	{
		if(CObjectManager.Instance.getPlayerMySelf() == null)
		{
			return "";
		}
		
		CCharacterData pCharData = CObjectManager.Instance.getPlayerMySelf().GetCharacterData();
		
		if(typeName == "LEVEL")   //等级
		{
			return pCharData.Get_Level().ToString();
		}
        else if (typeName == "HP")     //生命值
        {
            return pCharData.Get_HP().ToString();
        }
        else if (typeName == "MP")     //魔法值
        {
            return pCharData.Get_MP().ToString();
        }
        else if (typeName == "MENPAI")      //门派
        {
            return pCharData.Get_MenPai().ToString();
        }
        else if (typeName == "AMBIT")   //境界名称
        {
            return pCharData.Get_Ambit().ToString();
        }
        else if (typeName == "PORTRAIT")   //头像
        {
            //得到玩家和自己的头像
            string pImageName = "";
            //打开生物定义表
            _DBC_CHAR_FACE pFaceImage = CDataBaseSystem.Instance.GetDataBase<_DBC_CHAR_FACE>((int)DataBaseStruct.DBC_CHAR_FACE).Search_Index_EQU(pCharData.Get_PortraitID());

            if (pFaceImage != null)
            {
                pImageName = pFaceImage.pImageSetName;
            }

            //else // 默认状态不显示头像图标， 头像图标显示空
            //{
            //	pFaceImage = (const _DBC_CHAR_FACE*)(s_pFaceImageDBC->Search_LineNum_EQU(0));
            //	pImageName = pFaceImage->pImageSetName;
            //}//

            return pImageName;
        }
        else if (typeName == "STR")  //力量 力量 金
        {//基础值
            return (pCharData.Get_STR() - pCharData.Get_BringSTR()).ToString();
        }
        else if (typeName == "SPR")    //灵气 灵力 木
        {
            return (pCharData.Get_SPR() - pCharData.Get_BringSPR()).ToString();
        }
        else if (typeName == "CON")   //体制 体制 水
        {
            return (pCharData.Get_CON() - pCharData.Get_BringCON()).ToString();
        }
        else if (typeName == "INT")    //定力 智力 火
        {
            return (pCharData.Get_INT() - pCharData.Get_BringINT()).ToString();
        }
        else if (typeName == "DEX")	//身法 敏捷 土
        {
            return (pCharData.Get_DEX() - pCharData.Get_BringDEX()).ToString();
        }
        else if (typeName == "BRING_STR")
        {
            return pCharData.Get_BringSTR().ToString();
        }
        else if (typeName == "BRING_SPR")
        {
            return pCharData.Get_BringSPR().ToString();
        }
        else if (typeName == "BRING_CON")
        {
            return pCharData.Get_BringCON().ToString();
        }
        else if (typeName == "BRING_INT")
        {
            return pCharData.Get_BringINT().ToString();
        }
        else if (typeName == "BRING_DEX")
        {
            return pCharData.Get_BringDEX().ToString();
        }
        else if (typeName == "ATT_PHYSICS")			//物理攻击力
        {
            return pCharData.Get_AttPhysics().ToString();
        }
        else if (typeName == "ATT_MAGIC")				// 魔法攻击力 [7/16/2011 ivan edit]
        {
            return pCharData.Get_AttMagic().ToString();
        }
        else if (typeName == "DEF_PHYSICS")			//物理防御力
        {
            return pCharData.Get_DefPhysics().ToString();
        }
        else if (typeName == "DEF_MAGIC")				//魔法防御力
        {
            return pCharData.Get_DefMagic().ToString();
        }
        else if (typeName == "HIT")					//命中率
        {
            return pCharData.Get_Hit().ToString();
        }
        else if (typeName == "MISS")					//闪避率
        {
            return pCharData.Get_Miss().ToString();
        }
        else if (typeName == "CRITRATE")				//会心率
        {
            return pCharData.Get_CritRate().ToString();
        }
        else if (typeName == "DEFCRITRATE")			// 会心防御 [12/24/2010 ivan edit]
        {
            return pCharData.Get_DefCritRate().ToString();
        }
        else if (typeName == "MAXHP")       //最大生命值
        {
            return pCharData.Get_MaxHP().ToString();
        }
        else if (typeName == "MAXMP")     //最大魔法值
        {
            return pCharData.Get_MaxMP().ToString();
        }
		
		return "0";
	}
	
	public float GetFloatData(string typeName)
	{
		if(CObjectManager.Instance.getPlayerMySelf() == null)
		{
			return -1;
		}
		
		CCharacterData pCharData = CObjectManager.Instance.getPlayerMySelf().GetCharacterData();
       if (typeName == "EXP")     //经验
        {
            if (pCharData.Get_Exp() < 0)
            {
                return 0;
            }
            else
            {
                return pCharData.Get_Exp();
            }
        }
        else if (typeName == "NEEDEXP")    //升级经验
        {
            return pCharData.Get_MaxExp();
        }
		return 0;
	}
	
	public float GetHPPercent()
	{
		return CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_HPPercent();
	}
	
	public float GetMPPercent()
	{
		return CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_MPPercent();
	}
		
    //随机加点
    public void SendAskRandomAttri(byte index,sbyte nRank)
    {
       // CGReqRandomAttr msg = (CGReqRandomAttr)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_REQRANDOMATTR);
       // msg.Level = nRank;
		//msg.Index = index;
        //NetManager.GetNetManager().SendPacket(msg);
        LogManager.LogWarning("index " + index + "Rank " + nRank);
        UIInfterface.Instance.Clear_XSCRIPT();
        UIInfterface.Instance.Set_XSCRIPT_Function_Name("FinishPeiyang");
        UIInfterface.Instance.Set_XSCRIPT_ScriptID(809301);
        UIInfterface.Instance.Set_XSCRIPT_Parameter(0, index);
        UIInfterface.Instance.Set_XSCRIPT_Parameter(1, nRank);
        UIInfterface.Instance.Set_XSCRIPT_ParamCount(2);
        UIInfterface.Instance.Send_XSCRIPT();
    }
    public void SendAskBringResult(byte index)
    {
        if (index == 0)
        {
            CGAskDetailAttrib msg = (CGAskDetailAttrib)NetManager.GetNetManager().CreatePacket((int)PACKET_DEFINE.PACKET_CG_ASKDETAILATTRIB);
            msg.ObjID = (uint)CObjectManager.Instance.getPlayerMySelf().ServerID;
            NetManager.GetNetManager().SendPacket(msg);
        }
        else
        {
            SDATA_PET curPet = CDataPool.Instance.Pet_GetPet(index - 1);
            if (curPet == null)
            {
                LogManager.LogError("SendAskBringResult index is wrong " + index);
                return;
            }
            CGManipulatePet manipulatePet = new CGManipulatePet();
            manipulatePet.ObjectID = (uint)CObjectManager.Instance.getPlayerMySelf().ServerID;
            manipulatePet.PetGUID = curPet.GUID;
            manipulatePet.Type     = (int)ENUM_MANIPULATE_TYPE.MANIPULATE_ASKOWNPETINFO;
            NetManager.GetNetManager().SendPacket(manipulatePet);
		    LogManager.LogWarning("SendAskBringResult ObjID " + curPet.idServer);
        }
        
    }
    public _ATTR_LEVEL1 GetRandomAttr()
    {
        return CDataPool.Instance.RandomAttrs[0];
    }

    // 请求复活 [3/14/2012 Ivan]
    public void AskReliveYuanDi()
    {
        CGPlayerDieResult msg = new CGPlayerDieResult();
        msg.DieResultCode = ENUM_DIE_RESULT_CODE.DIE_RESULT_CODE_RELIVE;
        NetManager.GetNetManager().SendPacket(msg);
    }

    internal void AskReliveZhuCheng()
    {
//         CGPlayerDieResult msg = new CGPlayerDieResult();
//         msg.DieResultCode = ENUM_DIE_RESULT_CODE.DIE_RESULT_CODE_OUT_GHOST;
//         NetManager.GetNetManager().SendPacket(msg);
    }

    internal void AskReliveChangJing()
    {
        CGPlayerDieResult msg = new CGPlayerDieResult();
        msg.DieResultCode = ENUM_DIE_RESULT_CODE.DIE_RESULT_CODE_OUT_GHOST;
        NetManager.GetNetManager().SendPacket(msg);
    }
    public void LeaveTeam()
    {
        if (CUIDataPool.Instance.GetTeamOrGroup() != null)
        {
            if (CUIDataPool.Instance.GetTeamOrGroup().GetTeamMemberCount(0) > 0)
            {
                CUIDataPool.Instance.LeaveTeam();
            }
        }
    }
    public void DisMissTeam()
    {
        if (CUIDataPool.Instance.GetTeamOrGroup() != null)
        {
            if (CUIDataPool.Instance.GetTeamOrGroup().GetTeamMemberCount(0) > 0)
            {
                CUIDataPool.Instance.DismisstTeam();
            }
        }
    }
    //新手引导
    public uint GetTutorialMask(int nBit)
	{
        if (CObjectManager.Instance.getPlayerMySelf() == null)
            return 0;
		//low - high 0~31
		int bit = nBit >= 32 ? 31 : nBit;
		uint nMask = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_TutorialMask();

		return (nMask >> bit) & 0x1;
	}
	public void SetTutorialMask(int nBit)
	{
		//low - high 0~31
		int bit = nBit >=32 ? 31 : nBit;
		uint nMask = CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_TutorialMask();

		nMask |= (uint)(0x1 << bit);

		CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Set_TutorialMask(nMask);

        CGCharSetMask _msg = new CGCharSetMask();
        _msg.HelpMask = (nMask);
        NetManager.GetNetManager().SendPacket(_msg);
	}
}
}
