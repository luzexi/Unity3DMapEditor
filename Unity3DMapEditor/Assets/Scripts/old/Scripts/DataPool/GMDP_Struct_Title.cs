/****************************************\
*										*
* 			   数据池数据结构			*
*					-称号				*
*										*
\****************************************/

public struct _TITLE_
{
// 	enum
// 	{
// 		INVALID_TITLE = 0,
// 		ID_TITLE,
// 		STRING_TITLE,
// 	}
// 	BOOL		bFlag;							//表明此title是否是id
// 	BYTE		bType;							//表明此title的类型
// 	union
// 	{
// 		INT		ID;								//如果是id,这个就是表中id
// 		CHAR	szTitleData[MAX_CHARACTER_TITLE];	//如果不是id，这个就是title内容
// 	};
// 	_TITLE_()
// 	{
// 		bFlag		=	INVALID_TITLE;
// 		bType		=	_TITLE::NO_TITLE;	
// 		memset(szTitleData, 0, MAX_CHARACTER_TITLE);
// 	}
    public string mTitle;
}