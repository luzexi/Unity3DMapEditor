using System;
using System.Collections.Generic;
using System.Text;
using DBSystem;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using UnityEngine;

public class UIString
{
    // 更改为单例模式
    static readonly UIString sInstance = new UIString();


    private UIString() 
    {
    }

    static public UIString Instance { get { return sInstance; } }


    Dictionary<string, string> m_theDirectionary = new Dictionary<string, string>();
    Dictionary<string, string> orginalDirectionary = new Dictionary<string, string>();
    Dictionary<string, string> NPCMissionDictionary = new Dictionary<string, string>();


    public string GetUnicodeString(byte[] texts)
    {
        return EncodeUtility.Instance.GetUnicodeString(texts);
    }


    ////转化字符串
    void ParserString_Prebuild(string strSource, out string strOut)
    {
        const char KeyParserBegin = '{';
        const char KeyParserMiddle = '=';
        const char KeyParserEnd = '}';

        int nValidBegin = 0;
        strOut = "";

        do
        {
            if (nValidBegin >= strSource.Length) break;

            int nValidEnd = strSource.IndexOf(KeyParserBegin, nValidBegin);

            //最后一段正常值
            if (nValidEnd == -1)
            {
                strOut += strSource.Substring(nValidBegin);

                break;
            }

            //加入正常值
            if (nValidEnd != nValidBegin)
            {
                strOut += strSource.Substring(nValidBegin, nValidEnd - nValidBegin);
            }

            int nKeyStart = nValidEnd + 1;									//"{"
            int nKeyMiddle = strSource.IndexOf(KeyParserMiddle, nKeyStart);	//"="
            int nKeyEnd = strSource.IndexOf(KeyParserEnd, nKeyStart);		//"}"

            //合法的Key描述
            if (nKeyEnd != -1 && nKeyMiddle != -1 && nKeyEnd > nKeyMiddle)
            {
                string strKey_Left = strSource.Substring(nKeyStart, nKeyMiddle - nKeyStart);
                string strKey_Right = strSource.Substring(nKeyMiddle + 1, nKeyEnd - nKeyMiddle - 1);

                if (strKey_Left == "ID")
                {
                    KeyValuePair<bool, string> isValidKey = _IsValidDictionKey(strKey_Right);
                    //找到key
                    if (isValidKey.Key)
                    {
                        //加入从key转化过来的值
                        strOut += isValidKey.Value;

                        nValidBegin = nKeyEnd + 1;
                        continue;
                    }
                }
            }
			try{
            strOut += strSource.Substring(nValidEnd, nKeyEnd - nValidEnd);
			}
			catch
			{
				LogManager.LogError("error string : " + strSource);
				return;
			}
            nValidBegin = nKeyEnd;
        } while (true);

    }
    //VOID				ParserString_Runtime(const STRING& strSource, CEGUI::String& strOut);
    //STRING				ParserString_VarParam(const STRING& strID, va_list ptr);
    //VOID				ParserString_NoColor(const STRING& strSrc, STRING&strOut, BOOL bControl = FALSE);
    //STRING				ParserString_NoVarParam(const STRING& strID);

    //// 转换字符串，使其回到parserstring_runtime之前，现在只支持部分标志 [4/18/2011 Sun]
    //VOID				RollBackParseredString(const CEGUI::String& strSource, STRING& strOut);
    //// 去掉字符串中的各种特殊标记 [6/20/2011 Sun]
    //VOID				ParserString_IgnoreParam(const STRING& strSource, STRING& strOut, BOOL bIgnoreFace = FALSE);

    ////聊天模板字串的取得
    //STRING				getTalkTemplate(const STRING& strKey, UINT index);
    //STRING				ModifyChatTemplate(const STRING& strSrc, const STRING& strTalker, const STRING& strTarget);

    //STRING				getTalkRandHelpMsg();


    ////初始化
    public void Init()
    {
        //----------------------------------------------------
        //字符串字典

        DBC.COMMON_DBC<_DBC_STRING_DICT> dicDBC = CDataBaseSystem.Instance.GetDataBase<_DBC_STRING_DICT>((int)DataBaseStruct.DBC_STRING_DICT);
        DBC.COMMON_DBC<_DBC_NPC_MISSION> NPCMission = CDataBaseSystem.Instance.GetDataBase<_DBC_NPC_MISSION>((int)DataBaseStruct.DBC_NPC_MISSION);
        foreach (_DBC_STRING_DICT item in dicDBC.StructDict.Values)
        {
            orginalDirectionary[item.szKey] = item.szString;
        }

        //系统关键字
        //m_theDirectionary[UKS_PLAYERNAME] = 

        //---------------------------------
        //字符串字典id转化
        // 由于在foreach里面修改directionary会出错，所以拷贝一份 [2/20/2012 Ivan]
        foreach (KeyValuePair<string, string> kv in orginalDirectionary)
        {
            string strOut;
            ParserString_Prebuild(kv.Value, out strOut);

            m_theDirectionary[kv.Key] = strOut;
        }

        foreach (KeyValuePair<int,_DBC_NPC_MISSION> itemNPC in NPCMission.StructDict)
        {
            NPCMissionDictionary[itemNPC.Key.ToString()] = itemNPC.Value.m_DefaultDialog;
        }

        orginalDirectionary = null;
        //------------------------------------------------

        //聊天模板创建
        //GenerateTalkTemplateTable();

        // 表情映射
        //加载所有ui
        // 	const tDataBase* pDBC = pUIDBC->GetDataBase(DBC_CODE_CONVERT);
        // 	for(INT i=0; i<(INT)pDBC->GetRecordsNum(); i++)
        // 	{
        // 		const _DBC_CODE_CONVERT* pLine = (const _DBC_CODE_CONVERT*)((tDataBase*)pDBC)->Search_LineNum_EQU(i);
        // 		CEGUI::utf32 dwValue = 0;
        // 		sscanf( pLine->szCode, "%8X", &dwValue );
        // 		m_mapCodeConvertTable[ pLine->nID ] = dwValue ;
        // 	}

        //------------------------------------------------
        // 过滤词数组构造
        // 	const tDataBase* pDBCfl = pUIDBC->GetDataBase(DBC_TALK_FILTER);
        // 	for(INT i=0; i<(INT)pDBCfl->GetRecordsNum(); i++)
        // 	{
        // 		const _DBC_TALK_FILTER* pLine = (const _DBC_TALK_FILTER*)((tDataBase*)pDBCfl)->Search_LineNum_EQU(i);
        // 		m_vecFilterTable.push_back(pLine->szString);
        // 	}

        //------------------------------------------------
        // 完全匹配过滤表构造
        //GenerateFullCompareTable();
    }

    ////过滤字符串中的非法字符
    //BOOL	_CheckStringFilter(const STRING& strIn);
    ////完全匹配过滤
    //BOOL	_CheckStringFullCompare(const STRING& strIn, const STRING& strFilterType = "all", BOOL bUseAllTable = TRUE);
    ////检查字符串的合法性
    //BOOL	_CheckStringCode(const STRING& strIn, STRING& strOut);


    //////////////////////////////////////////////////////////////////////////
    //private
    KeyValuePair< bool, string>	_IsValidDictionKey(string strKey)
    {
        string value;
        if (m_theDirectionary.TryGetValue(strKey, out value))
            return new KeyValuePair<bool, string>(true, value);
        else
            return new KeyValuePair<bool, string>(false, "");
    }

    KeyValuePair<bool,string> IsNPCDictionary(string strkey)
    {
        string NPCvalue;
        if (NPCMissionDictionary.TryGetValue(strkey,out NPCvalue))
        {
            return new KeyValuePair<bool, string>(true, NPCvalue);
        }
        else
        {
            return new KeyValuePair<bool, string>(false, "");
        }
    }
    //VOID				ParserString_RuntimeNew(const STRING& strSource, CEGUI::String& strOut);
    //VOID				GenerateTalkTemplateTable();
    //VOID				GenerateFullCompareTable();

    public void ParserString_Runtime(string src, out string sOut)
    {
        ParserString(src, out sOut, true);
	}
	
	public void ParserString_RuntimeNoHL(string src, out string sOut)
    {
        ParserString(src, out sOut, false);
		sOut = sOut.Replace(hyperLinkColor,"");
	}

    /// <summary>
    /// 解析文字的地方
    /// #{IDSTR}  查表替换字符串
    /// #ID{001}  查找NPCEvent表
    /// ------------颜色标记--------------
    /// #R		表示后面的字体为红色(red)
    /// #G		表示后面的字体为绿色(green)
    /// #B		表示后面的字体为蓝色(blue)
    /// #K		表示后面的字体为黑色(black)
    /// #Y		表示后面的字体为黄色(yellow)
    /// #W		表示后面的字体为白色(white)
    /// ------------特殊标记--------------
    /// #r 换行
    /// #{PlayerName} 玩家名字
    /// ------------超链接-------------
    /// 采集超链接 eg:{#caiji[sid,x,y,id]描述文字}
    /// 坐标超链接 eg:{#pos[100,200]描述文字}  跨场景超链接:{#pos[1,100,200]描述文字}
    /// 物品超链接 eg:{#item[100200]描述文字}
    /// npc超链接 eg:{#npc[sid,x,y,,id]描述文字} sid标示场景id ，x和y标示坐标，id标示npc的id
    /// 打怪超链接 eg:{#attack[sid,x,y,id]描述文字}
    /// 解析剧情对白eg:#JQ{1001}  [4/24/2012 Ivan]
    /// </summary>
    /// <param name="src">输入文字</param>
    /// <param name="sOut">输出文字</param>
    /// <param name="addLink">判断超链接是否可以点击</param>
    void ParserString(string src, out string sOut,bool enableLink)
    {
        sOut = "";
        if (src == null)
            return;
        // #{IDSTR}  查表替换字符串 [2/20/2012 Ivan]
        sOut = parseStringDictionary(src);
        // #ID{001}  查找NPCEvent表 [2/22/2012 Ruanmin]
        sOut = parseNPCDefaultDialog(sOut);
        // 解析颜色 [3/30/2012 Ivan]
        sOut = parseColors(sOut);
        // 解析特殊标记 [3/30/2012 Ivan]
        sOut = ParseOthers(sOut);

        // 解析超链接
        // 一定要放在最后解析这个，因为文本字符串是要拿来生成guid的，点击的时候识别用 -ivan
        HyperLink link = new HyperLink();
        sOut = ParseHyperLink(sOut, ref link);

        if (enableLink && link.allItems.Count != 0)
            HyperLinkManager.Instance.AddHyper(link);
    }

    private string ParseOthers(string src)
    {
        src = src.Replace("#r", "\n");
        if (CObjectManager.Instance.getPlayerMySelf() != null)
            src = src.Replace("#{PlayerName}", CObjectManager.Instance.getPlayerMySelf().GetCharacterData().Get_Name());
        return src;
    }

    private string parseColors(string src)
    {
        //#R		表示后面的字体为红色(red)
        //#G		表示后面的字体为绿色(green)
        //#B		表示后面的字体为蓝色(blue)
        //#K		表示后面的字体为黑色(black)
        //#Y		表示后面的字体为黄色(yellow)
        //#W		表示后面的字体为白色(white)
        src = src.Replace("#R", UnityEngine.Color.red.ToString());
        src = src.Replace("#G", UnityEngine.Color.green.ToString());
        src = src.Replace("#B", UnityEngine.Color.blue.ToString());
        src = src.Replace("#K", UnityEngine.Color.black.ToString());
        src = src.Replace("#Y", UnityEngine.Color.yellow.ToString());
        src = src.Replace("#W", UnityEngine.Color.white.ToString());
        src = src.Replace("#cyan", UnityEngine.Color.cyan.ToString());
       

        return src;
    }

    public string ParserString_Runtime(string src)
    {
        string sOut;
        ParserString_Runtime(src, out sOut);
        return sOut;
    }

    public string parseStringDictionary(string src)
    {
        Regex rDic = new Regex("#{.*?}");
        MatchCollection mc = rDic.Matches(src);
        for (int i = 0; i < mc.Count; i++) 
        {
            // Add the match string to the string array.   
            string value = mc[i].Value;
            string key = value.Substring(2, value.Length - 3);
            KeyValuePair<bool, string> dicValue = _IsValidDictionKey(key);
            if (!dicValue.Key)
                LogManager.LogError(value + ":在字典表找不到对应的项.");
            
            src = src.Replace(value, dicValue.Value);
        }

        return src;
    }
	
	public string paresFuBenContentDictionary(string src)
	{
		int index = src.IndexOf("#FB{");
		if( index == -1)
		{
			LogManager.LogError("副本格式填写错误" + src);
			return src;
		}
		else
		{
		   int firstKeyIndex = src.IndexOf('{') + 1;
		   int lastKeyIndex = src.IndexOf('}');
		   string[] poss = src.Substring(firstKeyIndex, lastKeyIndex - firstKeyIndex).Split(',');
		   if(poss.Length != 3)
		   {
				LogManager.LogError("副本格式填写错误" + src);
				return src;
		   }
		
		   for(int i = 0; i < 2; i++)
		     {
			    KeyValuePair<bool, string> dicValue = _IsValidDictionKey(poss[i]);
			    if(!dicValue.Key)
				   LogManager.LogError(poss[i] + ":在字典表找不到对应的项.");
			
			    src = src.Replace(poss[i], dicValue.Value);
		     }
			
		  return src;
		}
	}

    // 添加读取NPCEvent表 [2/22/2012 Ruanmin]
    string parseNPCDefaultDialog(string src)
    {
        MatchCollection matches = Regex.Matches(src, @"(?<=#ID{\b)[^{}]*");
        
        for (int i = 0; i < matches.Count; i++)
        {
			KeyValuePair<bool, string> NPCValue = IsNPCDictionary(matches[i].Value);
            if (!NPCValue.Key)
            {
                LogManager.LogError("无法找到 " + matches[i].Value + " 在NPCEvent表中对应的项。");
            }
            src = src.Replace("#ID{" + matches[i].Value + "}", NPCValue.Value);
        }

        return src;
    }

    string hyperLinkColor = "RGBA(0.44, 0.63, 0.85, 1.000)";
    // 解析文本超链接
    // 一定要放在最后解析这个，因为文本字符串是要拿来生成guid的，点击的时候识别用 -ivan
    public string ParseHyperLink(string src,ref HyperLink link)
    {
        if (src == null || src == "")
            return src;

        ParsePosLink(ref link, ref src);

        ParseItemLink(ref link, ref src);

        ParseNpcTalkLink(ref link, ref src);

        ParseAttackLink(ref link, ref src);

        ParseCaiJikLink(ref link, ref src);

        DealWithColor(ref link, src);

        return src;
    }

    private void ParseCaiJikLink(ref HyperLink link, ref string src)
    {
        //采集超链接 eg:{#caiji[sid,x,y,id]描述文字}
        Regex rDic = new Regex(@"{#caiji\[\d+,\d+,\d+,\d+\]\w*}");
        MatchCollection mc = rDic.Matches(src);
        for (int i = 0; i < mc.Count; i++)
        {
            // Add the match string to the string array.   
            string fullText = mc[i].Value;
            int firstKeyIndex = fullText.IndexOf('[') + 1;
            int lastKeyIndex = fullText.IndexOf(']');
            string[] poss = fullText.Substring(firstKeyIndex, lastKeyIndex - firstKeyIndex).Split(',');
            if (poss.Length != 4)
            {
                LogManager.LogError("超链接格式填写错误:" + fullText);
                continue;
            }

            int descLength = fullText.IndexOf('}') - lastKeyIndex - 1;
            string desc = fullText.Substring(lastKeyIndex + 1, descLength);
            // 将特定标示转换为显示文字
            src = src.Replace(fullText, hyperLinkColor + desc + Color.white);

            HyperTripper caiji = new HyperTripper();
            caiji.SceneId = int.Parse(poss[0]);
            caiji.PosTarget = new UnityEngine.Vector3(float.Parse(poss[1]), 0, float.Parse(poss[2]));
            caiji.TripperID = int.Parse(poss[3]);
            caiji.textCheck.Text = desc;
            link.allItems.Add(caiji);
        }
    }

    private void DealWithColor(ref HyperLink link, string src)
    {
        if (link.allItems.Count != 0)
        {
            // 消除颜色
            string noColorText = src;
            Regex rDic = new Regex(@"RGBA\(.*?\)");
            MatchCollection mc = rDic.Matches(noColorText);
            for (int i = 0; i < mc.Count; i++)
            {
                // Add the match string to the string array.   
                string fullText = mc[i].Value;
                noColorText = noColorText.Replace(fullText, "");
            }
            // 由于最终点击时的字符偏移量是不包含颜色的，所以必须得到消除颜色后的坐标偏移
            foreach (HyperItemBase item in link.allItems)
            {
                item.textCheck.CharStart = (uint)noColorText.IndexOf(item.textCheck.Text);
                item.textCheck.CharLength = (uint)item.textCheck.Text.Length;
            }

            link.FullText = noColorText;
        }
    }

    // 解析所有的坐标超链接，包含跨场景和当前场景 [3/13/2012 Ivan]
    void ParsePosLink(ref HyperLink link, ref string src)
    {
        //坐标超链接 eg:{#pos[100,200]描述文字}
        Regex rDic = new Regex(@"{#pos\[\d+,\d+\]\w*}");
        MatchCollection mc = rDic.Matches(src);
        for (int i = 0; i < mc.Count; i++)
        {
            // Add the match string to the string array.   
            string fullText = mc[i].Value;
            int firstKeyIndex = fullText.IndexOf('[') + 1;
            int lastKeyIndex = fullText.IndexOf(']');
            string[] poss = fullText.Substring(firstKeyIndex, lastKeyIndex - firstKeyIndex).Split(',');
            if (poss.Length != 2)
            {
                LogManager.LogError("超链接格式填写错误:" + fullText);
                continue;
            }

            int descLength = fullText.IndexOf('}') - lastKeyIndex - 1;
            string desc = fullText.Substring(lastKeyIndex + 1, descLength);
            // 将特定标示转换为显示文字
            src = src.Replace(fullText, hyperLinkColor + desc + Color.white);

            HyperPos hyperPos = new HyperPos();
            hyperPos.SceneId = -1;
            hyperPos.PosTarget = new UnityEngine.Vector3(float.Parse(poss[0]), 0, float.Parse(poss[1]));
            hyperPos.textCheck.Text = desc;
            link.allItems.Add(hyperPos);
        }

        // 解析跨场景超链接 [3/13/2012 Ivan]
        // eg:{#pos[1,100,200]描述文字}
        rDic = new Regex(@"{#pos\[\d+,\d+,\d+\]\w*}");
        mc = rDic.Matches(src);
        for (int i = 0; i < mc.Count; i++)
        {
            // Add the match string to the string array.   
            string fullText = mc[i].Value;
            int firstKeyIndex = fullText.IndexOf('[') + 1;
            int lastKeyIndex = fullText.IndexOf(']');
            string[] poss = fullText.Substring(firstKeyIndex, lastKeyIndex - firstKeyIndex).Split(',');
            if (poss.Length != 3)
            {
                LogManager.LogError("超链接格式填写错误:" + fullText);
                continue;
            }

            int descLength = fullText.IndexOf('}') - lastKeyIndex - 1;
            string desc = fullText.Substring(lastKeyIndex + 1, descLength);
            // 将特定标示转换为显示文字
            src = src.Replace(fullText, hyperLinkColor + desc + Color.white);

            HyperPos hyperPos = new HyperPos();
            hyperPos.SceneId = int.Parse(poss[0]);
            hyperPos.PosTarget = new UnityEngine.Vector3(float.Parse(poss[1]), 0, float.Parse(poss[2]));
            hyperPos.textCheck.Text = desc;
            link.allItems.Add(hyperPos);
        }

    }

    // 解析所有物品超链接 [3/13/2012 Ivan]
    void ParseItemLink(ref HyperLink link, ref string src)
    {
        //物品超链接 eg:{#item[100200]描述文字}
        Regex rDic = new Regex(@"{#item\[\d+\]\w*}");
        MatchCollection mc = rDic.Matches(src);
        for (int i = 0; i < mc.Count; i++)
        {
            // Add the match string to the string array.   
            try
            {
                string fullText = mc[i].Value;
                int firstKeyIndex = fullText.IndexOf('[') + 1;
                int lastKeyIndex = fullText.IndexOf(']');
                string itemId = fullText.Substring(firstKeyIndex, lastKeyIndex - firstKeyIndex);

                int descLength = fullText.IndexOf('}') - lastKeyIndex - 1;
                string desc = fullText.Substring(lastKeyIndex + 1, descLength);
                // 将特定标示转换为显示文字
                src = src.Replace(fullText, hyperLinkColor + desc + Color.white);

                HyperItem hyperItem = new HyperItem();
                hyperItem.ItemIdTable = int.Parse(itemId);
                hyperItem.textCheck.Text = desc;
                link.allItems.Add(hyperItem);
            }
            catch (System.Exception ex)
            {
                LogManager.LogWarning("物品超链接出错：" + ex.ToString());
                continue;
            }
        }
    }

    // 解析NPC对话超链接 [3/13/2012 Ivan]
    void ParseNpcTalkLink(ref HyperLink link, ref string src)
    {
        //npc超链接 eg:{#npc[sid,x,y,,id]描述文字} sid标示场景id ，x和y标示坐标，id标示npc的id
        Regex rDic = new Regex(@"{#npc\[\d+,\d+,\d+,\d+\]\w*}");
        MatchCollection mc = rDic.Matches(src);
        for (int i = 0; i < mc.Count; i++)
        {
            // Add the match string to the string array.   
            string fullText = mc[i].Value;
            int firstKeyIndex = fullText.IndexOf('[') + 1;
            int lastKeyIndex = fullText.IndexOf(']');
            string[] poss = fullText.Substring(firstKeyIndex, lastKeyIndex - firstKeyIndex).Split(',');
            if (poss.Length != 4)
            {
                LogManager.LogError("超链接格式填写错误:" + fullText);
                continue;
            }

            int descLength = fullText.IndexOf('}') - lastKeyIndex - 1;
            string desc = fullText.Substring(lastKeyIndex + 1, descLength);
            // 将特定标示转换为显示文字
            src = src.Replace(fullText, hyperLinkColor + desc + Color.white);

            HyperNpc npc = new HyperNpc();
            npc.SceneId = int.Parse(poss[0]);
            npc.PosTarget = new UnityEngine.Vector3(float.Parse(poss[1]), 0, float.Parse(poss[2]));
            npc.NpcId = int.Parse(poss[3]);
            npc.textCheck.Text = desc;
            link.allItems.Add(npc);
        }
    }

    // 解析杀怪超链接 [3/13/2012 Ivan]
    void ParseAttackLink(ref HyperLink link, ref string src)
    {
        //打怪超链接 eg:{#attack[sid,x,y,id]描述文字}
        Regex rDic = new Regex(@"{#attack\[\d+,\d+,\d+,\d+\]\w*}");
        MatchCollection mc = rDic.Matches(src);
        for (int i = 0; i < mc.Count; i++)
        {
            // Add the match string to the string array.   
            string fullText = mc[i].Value;
            int firstKeyIndex = fullText.IndexOf('[') + 1;
            int lastKeyIndex = fullText.IndexOf(']');
            string[] poss = fullText.Substring(firstKeyIndex, lastKeyIndex - firstKeyIndex).Split(',');
            if (poss.Length != 4)
            {
                LogManager.LogError("超链接格式填写错误:" + fullText);
                continue;
            }

            int descLength = fullText.IndexOf('}') - lastKeyIndex - 1;
            string desc = fullText.Substring(lastKeyIndex + 1, descLength);
            // 将特定标示转换为显示文字
            src = src.Replace(fullText, hyperLinkColor + desc + Color.white);

            HyperAttack attack = new HyperAttack();
            attack.SceneId = int.Parse(poss[0]);
            attack.PosTarget = new UnityEngine.Vector3(float.Parse(poss[1]), 0, float.Parse(poss[2]));
            attack.TargetId = int.Parse(poss[3]);
            attack.textCheck.Text = desc;
            link.allItems.Add(attack);
        }
    }

    // 解析剧情对白 [4/24/2012 Ivan]
    public bool ParseDialogue(string content, out int id)
    {
        id = -1;
        if (content.Length < 6)
            return false;
        // eg:#JQ{1001}  [4/24/2012 Ivan]
        Regex rDic = new Regex("#JQ{.*?}");
        MatchCollection mc = rDic.Matches(content);
        for (int i = 0; i < mc.Count; i++)
        {
            // Add the match string to the string array.   
            string value = mc[i].Value;
            string key = value.Substring(4, value.Length - 5);
            id = int.Parse(key);
            return true;
        }
        return false;
    }
	
	// 解析任务目标[5/3/2012 shen]
	public bool ParseAcceptQuest(string content,out int nIndex)
	{
		// eg:#{M_MUBIAO} [5/3/2012 shen]
		int index = content.IndexOf("#{M_MUBIAO}");
		if( index == -1)
		{
			nIndex = -1;
			return false;
		}
		else
		{
			nIndex = index;
			return true;
		}
	}
	
	//解析完成任务目标[5/8/2012 shen]
	public bool ParseFinishQuest(string content, out int nIndex)
	{
		// eg:"#{M_FINISH}"
		int index = content.IndexOf("#{M_FINISH}");
		if(index == -1)
		{
			nIndex = -1;
			return false;
		}
		else
		{
			nIndex = index;
			return true;
		}
	}
	
	public bool ParseIsFuBen(string content)
	{
		int index = content.IndexOf("#{FB}");
		if( index == -1)
		{
			return false;
		}
		else
			return true;
	}
	
	// 解析副本内容[5/4/2012 shen]
	public bool ParseFuBen(string content, out string icon, out string name, out int num)
	{
		icon = "";
		name = "";
		num = -1;
		// eg:#FB{IMAGE_INDEX_IN_DIRECTORY,STRING_INDEX_IN_DIRECTORY,1} [5/3/2012 shen]
		if(content.Length < 6)
			return false;
		int index = content.IndexOf("#FB{");
		if( index == -1)
		{
			return false;
		}
		else
		{
			int firstKeyIndex = content.IndexOf('{') + 1;
			int lastKeyIndex = content.IndexOf('}');
			string[] poss = content.Substring(firstKeyIndex, lastKeyIndex - firstKeyIndex).Split(',');
			if(poss.Length != 3)
			{
				LogManager.LogError("副本格式填写错误" + content);
				return false;
			}
			
			icon = poss[0];
			name = poss[1];
			num = int.Parse(poss[2]);		
			return true;
		}
	}
		
    public string GetMd5(string text)
    {
        MD5CryptoServiceProvider aMD5CSP = new MD5CryptoServiceProvider();
        //注意:这里选择的编码不同 所计算出来的MD5值也不同
        Byte[] aHashTable = aMD5CSP.ComputeHash(Encoding.UTF8.GetBytes(text));
        string mD5Code = BitConverter.ToString(aHashTable).Replace("-", "");
        //aMD5Code=033BD94B1168D7E4F0D644C3C95E35BF
        return mD5Code;
    }
}
