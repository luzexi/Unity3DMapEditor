public class _IMPACT_DIRECT
{
	public int				m_nID;
	public int				m_nCreaterID;
	public int				m_nLogicCountOfCreater;
	public int[] 			m_aParams = new int[GAMEDEFINE.DEF_IMPACT_DIRECT_PARAM_NUMBERS];

	public _IMPACT_DIRECT()
	{
		m_nID					= MacroDefine.INVALID_ID;
		m_nCreaterID			= MacroDefine.INVALID_ID;
		m_nLogicCountOfCreater	= -1;
	}

	public void Reset()
	{
		m_nID					= MacroDefine.INVALID_ID;
		m_nCreaterID			= MacroDefine.INVALID_ID;
		m_nLogicCountOfCreater	= -1;
	}
};
public class _OWN_IMPACT //struct like class, can use memcpy to copy it 
{
		private	enum _OWN_IMPACT_TYPE
		{
            //消散
			IDX_FADE_OUT = 0,
            //会心
			IDX_CRITICAL = 1,
            //产生效果的技能是人类使用的
			IDX_CREATE_BY_PLAYER,
			IDX_MAX,
		};
		public	enum _OWN_IMPACT_MAX_FLAG_NUMBER
		{
			MAX_FLAG_NUMBER = _OWN_IMPACT_TYPE.IDX_MAX,
		};
		public	enum _OWN_IMPACT_MAX_PARAM_NUMBER
		{
            //最大的impact层数,实际允许的最大层数是MAX_IMPACT_PARAM_NUMBER-1
            //最后一个成员为相应的算法保留
			MAX_IMPACT_PARAM_NUMBER = 8
		};

		public	_OWN_IMPACT()
		{
			CleanUp();
		}

		void CleanUp()
		{
			m_nSN = MacroDefine.INVALID_ID;
			m_nDataIndex = MacroDefine.INVALID_ID;
			m_nImpactID = MacroDefine.INVALID_ID;
			m_nSkillID = MacroDefine.INVALID_ID;
			m_nCasterObjID = MacroDefine.UINT_MAX; //  [2010-12-15] by: cfp+
			m_nCasterUniqueID = MacroDefine.INVALID_ID;
			m_nCasterLogicCount = 0;
			m_nContinuance = 0;
			m_nContinuanceElapsed = 0;
			m_nIntervalElapsed = 0;
			m_Flags.ClearAllFlags();
			//MarkFadeOutFlag();// 缺省是无效的
			
		}
        public int SN
        {
            get 
            {
                return this.m_nSN;
            }
            set
            {
                this.m_nSN = value;
            }
        }
		
        
		public short DataIndex
        {   
            get
            {
                return this.m_nDataIndex;
            }
            set
            {
                this.m_nDataIndex = value;
            }
        }
		

		public short ImpactID
        {
            get
            {
                return this.m_nImpactID;
            }
            set
            {
                this.m_nImpactID = value; 
            }
        }


		public short SkillID
        {
            get
            {
                return this.m_nSkillID;
            }
            set
            {
                this.m_nSkillID = value;
            }
        }
		

		public uint CasterObjID
        {   
            get{return this.m_nCasterObjID;}
            set{this.m_nCasterObjID = value;}
        }

	

		public int	CasterUniqueID
        {   
            get{return this.m_nCasterUniqueID;}
            set{this.m_nCasterUniqueID = value;}
        }


		public int	CasterLogicCount
        {   get{return this.m_nCasterLogicCount;}
            set{this.m_nCasterLogicCount = value;}        
        }

		

		public int Continuance
        {
            get{return this.m_nContinuance;}
            set{this.m_nContinuance = value;}
        }
		

		public int ContinuanceElapsed
        {   
            get{return this.m_nContinuanceElapsed;}
            set{this.m_nContinuanceElapsed = value;}
        }

		public int IntervalElapsed
        {   
            get{return m_nIntervalElapsed;}
            set{m_nIntervalElapsed = value;}
        }

		bool	IsCreateByPlayer(){return m_Flags.GetFlagByIndex((int)_OWN_IMPACT_TYPE.IDX_CREATE_BY_PLAYER);}
		void	MarkCreateByPlayerFlag() {m_Flags.MarkFlagByIndex((int)_OWN_IMPACT_TYPE.IDX_CREATE_BY_PLAYER);}
		void	ClearCreateByPlayerFlag() {m_Flags.ClearFlagByIndex((int)_OWN_IMPACT_TYPE.IDX_CREATE_BY_PLAYER);}

		bool	IsFadeOut() {return m_Flags.GetFlagByIndex((int)_OWN_IMPACT_TYPE.IDX_FADE_OUT);}
		void	MarkFadeOutFlag() {m_Flags.MarkFlagByIndex((int)_OWN_IMPACT_TYPE.IDX_FADE_OUT);}
		void	ClearFadeOutFlag() {m_Flags.ClearFlagByIndex((int)_OWN_IMPACT_TYPE.IDX_FADE_OUT);}

		bool	IsCriticalHit() {return m_Flags.GetFlagByIndex((int)_OWN_IMPACT_TYPE.IDX_CRITICAL);}
		void	MarkCriticalFlag() {m_Flags.MarkFlagByIndex((int)_OWN_IMPACT_TYPE.IDX_CRITICAL);}
		void	ClearCriticalFlag() {m_Flags.ClearFlagByIndex((int)_OWN_IMPACT_TYPE.IDX_CRITICAL);}

		int		GetParamByIndex(int nIdx)
		{
			if(0>nIdx || (int)_OWN_IMPACT_MAX_PARAM_NUMBER.MAX_IMPACT_PARAM_NUMBER<=nIdx)
			{
				//Assert(FALSE && "[OWN_IMPACT::GetParamByIndex]:Illegal index found!!");
				return 0;
			}
			return m_aParams[nIdx];
		}
		void	SetParamByIndex(int nIdx, int nValue)
		{
			if(0>nIdx || (int)_OWN_IMPACT_MAX_PARAM_NUMBER.MAX_IMPACT_PARAM_NUMBER<=nIdx)
			{
				//Assert(FALSE && "[OWN_IMPACT::SetParamByIndex]:Illegal index found!!");
				return;
			}
			m_aParams[nIdx] = nValue;
		}
		private int 	m_nSN;
        //在impact表格中的索引
		private short 	m_nDataIndex;
		private short	m_nImpactID;
        //产生此效果的技能ID
		private short 	m_nSkillID;
        //技能使用者的ID
		private uint m_nCasterObjID;
		private int		m_nCasterUniqueID;
		private int		m_nCasterLogicCount;
        //效果持续时间
		private int	m_nContinuance;
        //效果持续时间的逝去值
		private int 	m_nContinuanceElapsed;
        //内部逝去时间
		private int 	m_nIntervalElapsed;
        //效果状态的位标记
		GameUtil.BitFlagSet_T	m_Flags = new GameUtil.BitFlagSet_T((uint)_OWN_IMPACT_MAX_FLAG_NUMBER.MAX_FLAG_NUMBER);
        //效果参数的数组
		private int[] 	m_aParams = new int[(uint)_OWN_IMPACT_MAX_PARAM_NUMBER.MAX_IMPACT_PARAM_NUMBER];
} ;