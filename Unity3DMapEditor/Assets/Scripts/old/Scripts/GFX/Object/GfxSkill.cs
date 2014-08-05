using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
namespace GFX
{
    public class GfxSkillHitGroundEffect
    {
        float       mAttachTime = 0;
        float       mShakeTime = 0;
        string      mAttachPoint = "";
        string      mEffectName  = "";
        string      mAnimateName = "";
        Vector3     mOffsetPos   = Vector3.zero;
        Quaternion  mOffsetRotation = Quaternion.identity;
        Vector3     mOffsetScale = new Vector3(1, 1, 1);
        GfxEffect   mEffect      = null;
        bool        mAttach      = false;
        public Vector3 OffsetPos
        {
            get { return mOffsetPos; }
            set { mOffsetPos = value; }
        }

        public Quaternion OffsetRotation
        {
            get { return mOffsetRotation; }
            set { mOffsetRotation = value; }
        }

        public Vector3 OffsetScale
        {
            get { return mOffsetScale; }
            set { mOffsetScale = value; }
        }

        public float AttachTime
        {
            get { return mAttachTime; }
            set { mAttachTime = value; }
        }

        public string AttachPoint
        {
            get { return mAttachPoint; }
            set { mAttachPoint = value; }
        }

        public string EffectName
        {
            get { return mEffectName; }
            set { mEffectName = value; }
        }

        public bool Attach
        {
            get { return mAttach; }
            set { mAttach = value; }
        }

        public GfxEffect Effect
        {
            get { return mEffect; }
            set { mEffect = value; }
        }

        public string AnimateName
        {
            get { return mAnimateName; }
            set { mAnimateName = value; }
        }

        public float ShakeTime
        {
            get { return mShakeTime; }
            set { mShakeTime = value; }
        }

        public string getParameter(string name)
        {
            string resultStr = "";
            if (name == "AttachTime")
            {
                resultStr = "" + mAttachTime;
            }
            else if (name == "AttachPoint")
            {
                resultStr = mAttachPoint;
            }
            else if (name == "EffectTemplateName")
            {
                resultStr = mEffectName;
            }
            else if (name == "OffsetPos")
            {
                resultStr = mOffsetPos[0] + " " + mOffsetPos[1] + " " + mOffsetPos[2];
            }
            else if (name == "OffsetRotation")
            {
                resultStr = mOffsetRotation[0] + " " + mOffsetRotation[1] + " " + mOffsetRotation[2] + " " + mOffsetRotation[3];
            }
            else if (name == "OffsetScale")
            {
                resultStr = mOffsetScale[0] + " " + mOffsetScale[1] + " " + mOffsetScale[2];
            }
            else if (name == "Attach")
            {
                resultStr = "" + mAttach;
            }
            else if (name == "Animation")
            {
                resultStr = mAnimateName;
            }
            else if (name == "ShakeTime")
            {
                resultStr = "" + mShakeTime;
            }
            return resultStr;
        }

        public bool setParameter(string name, string value)
        {
            bool result = false;
            string spaceDelim = " ";
            value = value.TrimStart(spaceDelim.ToCharArray());
            value = value.TrimEnd(spaceDelim.ToCharArray());
            if (name == "AttachTime")
            {
                mAttachTime = (float)Convert.ToDouble(value);
                result = true;
            }
            else if (name == "AttachPoint")
            {
                mAttachPoint = value;
                result = true;
            }
            else if (name == "EffectTemplateName")
            {
                mEffectName = value;
                result = true;
            }
            else if (name == "OffsetPos")
            {
                string[] offsetPosArray = value.Split(new string[1] { " " }, StringSplitOptions.None);
                if (offsetPosArray.Length == 3)
                {
                    mOffsetPos = new Vector3((float)Convert.ToDouble(offsetPosArray[0]),
                                             (float)Convert.ToDouble(offsetPosArray[1]),
                                             (float)Convert.ToDouble(offsetPosArray[2]));
                    result = true;
                }

            }
            else if (name == "OffsetRotation")
            {
                string[] offsetRotationArray = value.Split(new string[1] { " " }, StringSplitOptions.None);
                if (offsetRotationArray.Length == 4)
                {
                    mOffsetRotation = new Quaternion((float)Convert.ToDouble(offsetRotationArray[0]),
                                                  (float)Convert.ToDouble(offsetRotationArray[1]),
                                                  (float)Convert.ToDouble(offsetRotationArray[2]),
                                                  (float)Convert.ToDouble(offsetRotationArray[3]));
                    result = true;
                }

            }
            else if (name == "OffsetScale")
            {
                string[] offsetScaleArray = value.Split(new string[1] { " " }, StringSplitOptions.None);
                if (offsetScaleArray.Length == 4)
                {
                    mOffsetScale = new Vector3((float)Convert.ToDouble(offsetScaleArray[0]),
                                                  (float)Convert.ToDouble(offsetScaleArray[1]),
                                                  (float)Convert.ToDouble(offsetScaleArray[2]));
                    result = true;
                }
                result = true;
            }
            else if (name == "Attach")
            {
                mAttach = Convert.ToBoolean(value);
                result = true;
            }
            else if (name == "Animation")
            {
                mAnimateName = value;
                result = true;
            }
            else if (name == "ShakeTime")
            {
                mShakeTime = (float)Convert.ToDouble(value);
                result = true;
            }
            return result;
        }
    }

    public class GfxSkillEffect
    {
        float       mAttachTime;
        string      mAttachPoint 	= "";
        string      mEffectName 	= "";
        Vector3     mOffsetPos 		= Vector3.zero;
        Quaternion mOffsetRotation  = Quaternion.identity;
        Vector3     mOffsetScale 	= new Vector3(1,1,1);
        GfxEffect   mEffect         = null;
        bool        mAttach;
        public Vector3 OffsetPos
        {
            get { return mOffsetPos;}
            set { mOffsetPos = value; }
        }
        
        public Quaternion OffsetRotation
        {
            get { return mOffsetRotation; }
            set { mOffsetRotation = value; }
        }

        public Vector3 OffsetScale
        {
            get { return mOffsetScale; }
            set { mOffsetScale = value; }
        }

        public float AttachTime
        {
            get { return mAttachTime; }
            set { mAttachTime = value; }
        }

        public string AttachPoint
        {
            get { return mAttachPoint; }
            set { mAttachPoint = value; }
        }

        public string EffectName
        {
            get { return mEffectName; }
            set { mEffectName = value; }
        }

        public bool Attach
        {
            get { return mAttach; }
            set { mAttach = value; }
        }

        public GfxEffect Effect
        {
            get { return mEffect; }
            set { mEffect = value; }
        }

        public bool    setParameter(string name, string value)
        {
            bool result = false;
            string spaceDelim = " ";
            value = value.TrimStart(spaceDelim.ToCharArray());
            value = value.TrimEnd(spaceDelim.ToCharArray());
            if (name == "AttachTime")
            {
                mAttachTime = (float)Convert.ToDouble(value);
                result = true;
            }
            else if (name == "AttachPoint")
            {
                mAttachPoint = value;
                result = true;
            }
            else if (name == "EffectTemplateName")
            {
                mEffectName = value;
                result = true;
            }
            else if (name == "OffsetPos")
            {
                string[] offsetPosArray = value.Split(new string[1] { " " }, StringSplitOptions.None);
                if (offsetPosArray.Length == 3)
                {
                    mOffsetPos = new Vector3((float)Convert.ToDouble(offsetPosArray[0]), 
                                             (float)Convert.ToDouble(offsetPosArray[1]), 
                                             (float)Convert.ToDouble(offsetPosArray[2]));
                    result = true;
                }
               
            }
            else if (name == "OffsetRotation")
            {
                string[] offsetRotationArray = value.Split(new string[1] { " " }, StringSplitOptions.None);
                if (offsetRotationArray.Length == 4)
                {
                    mOffsetRotation = new Quaternion((float)Convert.ToDouble(offsetRotationArray[0]),
                                                  (float)Convert.ToDouble(offsetRotationArray[1]),
                                                  (float)Convert.ToDouble(offsetRotationArray[2]),
                                                  (float)Convert.ToDouble(offsetRotationArray[3]));
                    result = true;
                }
                
            }
            else if (name == "OffsetScale")
            {
                string[] offsetScaleArray = value.Split(new string[1] { " " }, StringSplitOptions.None);
                if (offsetScaleArray.Length == 4)
                {
                    mOffsetScale    = new Vector3((float)Convert.ToDouble(offsetScaleArray[0]),
                                                  (float)Convert.ToDouble(offsetScaleArray[1]),
                                                  (float)Convert.ToDouble(offsetScaleArray[2]));
                    result = true;
                }
                result = true;
            }
            else if (name == "Attach")
            {
                mAttach = Convert.ToBoolean(value);
                result = true;
            }
            return result;
        }

        public string getParameter(string name)
        {
            string resultStr = "";
            if (name == "AttachTime")
            {
                resultStr =  "" + mAttachTime;
            }
            else if (name == "AttachPoint")
            {
                resultStr = mAttachPoint;
            }
            else if (name == "EffectTemplateName")
            {
                resultStr = mEffectName;
            }
            else if (name == "OffsetPos")
            {
                resultStr = mOffsetPos[0] + " " + mOffsetPos[1] + " " + mOffsetPos[2];
            }
            else if (name == "OffsetRotation")
            {
                resultStr = mOffsetRotation[0] + " " + mOffsetRotation[1] + " " + mOffsetRotation[2] + " " + mOffsetRotation[3];
            }
            else if (name == "OffsetScale")
            {
                resultStr = mOffsetScale[0] + " " + mOffsetScale[1] + " " + mOffsetScale[2];
            }
            else if (name == "Attach")
            {
                resultStr = "" + mAttach; //? "true" : "false";
            }
            return resultStr;
        }
    }

    public class GfxSkill
    {
        protected string                            mSkillName;
        protected List<float>                       mHitTimeArray;
        protected List<float>                       mBreakTimeArray;
        protected List<float>                       mShakeTimeArray;
        protected string                            mAnimateName;
        protected bool                              mRepeatEffect;
        protected bool                              mEnableRibbon = true;
        protected List<GfxSkillEffect>              mSkillEffects;
        protected GfxSkillHitGroundEffect           mHitGroundEffect = null;
        
        public bool isHitGroundEffectExist()
        {
           return mHitGroundEffect != null;
        }

        public bool RepeatEffect
        {
            get { return mRepeatEffect; }
            set { mRepeatEffect = value; }
        }

        public bool EnableRibbon
        {
            get { return mEnableRibbon; }
            set { mEnableRibbon = value; }
        }

        public GfxSkillHitGroundEffect HitGroundEffect
        {
            get { return mHitGroundEffect; }
            set { mHitGroundEffect = value; }
        }
        
        public GfxSkill(string skillName)
		{
            mSkillName      = skillName;
            mHitTimeArray   = new List<float>();
            mBreakTimeArray = new List<float>();
            mShakeTimeArray = new List<float>();
            mRepeatEffect   = false;
            mSkillEffects   = new List<GfxSkillEffect>();
		}

        public GfxSkill(GfxSkill skill)
        {
            mSkillName      = skill.mSkillName;
            mRepeatEffect   = skill.mRepeatEffect;
            mAnimateName    = skill.mAnimateName;
            mHitTimeArray   = new List<float>();
            mBreakTimeArray = new List<float>();
            mShakeTimeArray = new List<float>();
            for (int i = 0; i < skill.mHitTimeArray.Count; i++)
            {
                mHitTimeArray.Add(skill.mHitTimeArray[i]);
            }

            for (int i = 0; i < skill.mBreakTimeArray.Count; i++)
            {
                mBreakTimeArray.Add(skill.mBreakTimeArray[i]);
            }

            for (int i = 0; i < skill.mShakeTimeArray.Count; i++)
            {
                mShakeTimeArray.Add(skill.mShakeTimeArray[i]);
            }

            mSkillEffects = new List<GfxSkillEffect>();
            for (int i = 0; i < skill.mSkillEffects.Count; i++)
            {
                GfxSkillEffect newSkillEffect = new GfxSkillEffect();
                newSkillEffect.Attach      = skill.mSkillEffects[i].Attach;
                newSkillEffect.AttachPoint = skill.mSkillEffects[i].AttachPoint;
                newSkillEffect.AttachTime  = skill.mSkillEffects[i].AttachTime;
                newSkillEffect.EffectName  = skill.mSkillEffects[i].EffectName;
                newSkillEffect.OffsetPos   = skill.mSkillEffects[i].OffsetPos;
                newSkillEffect.OffsetRotation = skill.mSkillEffects[i].OffsetRotation;
                newSkillEffect.OffsetScale = skill.mSkillEffects[i].OffsetScale;
                mSkillEffects.Add(newSkillEffect);
            }
            if (skill.mHitGroundEffect != null)
            {
                mHitGroundEffect = new GfxSkillHitGroundEffect();
                mHitGroundEffect.OffsetPos = skill.mHitGroundEffect.OffsetPos;
                mHitGroundEffect.OffsetRotation = skill.mHitGroundEffect.OffsetRotation;
                mHitGroundEffect.AttachTime = skill.mHitGroundEffect.AttachTime;
                mHitGroundEffect.AttachPoint = skill.mHitGroundEffect.AttachPoint;
                mHitGroundEffect.EffectName = skill.mHitGroundEffect.EffectName;
                mHitGroundEffect.Attach = skill.mHitGroundEffect.Attach;
                mHitGroundEffect.AnimateName = skill.mHitGroundEffect.AnimateName;
                mHitGroundEffect.ShakeTime = skill.mHitGroundEffect.ShakeTime;
            }
        }
		
		public void shutDown()
		{
            for (int i = 0; i < mSkillEffects.Count; i++)
            {
                if (mSkillEffects[i].Effect != null)
                {
                    GFXObjectManager.Instance.DestroyObject(mSkillEffects[i].Effect);
                }
            }
		}

        public GfxSkillEffect  addSkillEffect()
        {
            GfxSkillEffect newSkillEffect = new GfxSkillEffect();
            mSkillEffects.Add(newSkillEffect);
            return newSkillEffect;
        }

        public bool removeSkillEffect(int index)
        {
            if (index < 0 || index >= mSkillEffects.Count)
                return false;
            mSkillEffects.RemoveAt(index);
            return true;
        }

        public string getSkillName()
        {
            return mSkillName;
        }

        public GfxSkillEffect getSkillEffect(int skillIndex)
        {
            return mSkillEffects[skillIndex];
        }

        public int getSkillEffectCount(){return mSkillEffects.Count;}

        public string getAnimateName()
        {
            return mAnimateName;
        }

        public int getHitTimeCount()
        {
            return mHitTimeArray.Count;
        }

        public int getBreakTimeCount()
        {
            return mBreakTimeArray.Count;
        }

        public int getShakeTimeCount()
        {
            return mShakeTimeArray.Count;
        }

        public float getBreakTime(int index)
        {
            return mBreakTimeArray[index];
        }

        public float getHitTime(int index)
        {
            return mHitTimeArray[index];
        }

        public float getShakeTime(int index)
        {
            return mShakeTimeArray[index];
        }

        public void setAnimateName(string val)
        {
            mAnimateName = val;
        }

        public void setBreakTime(string val)
        {
            mBreakTimeArray.Clear();
            string[] splitedTime = val.Split(new string[1] { " " }, StringSplitOptions.None);
            foreach(string curTime in splitedTime)
            {
                if (curTime == "")
                {

                }
                else
                {
                    double time = Convert.ToDouble(curTime);
                    mBreakTimeArray.Add((float)time);
                }
            }
        }

        public void setHitTime(string val)
        {
            mHitTimeArray.Clear();
            string[] HittedTime = val.Split(new string[1] { " " }, StringSplitOptions.None);
            foreach (string curTime in HittedTime)
            {
                if (curTime == "")
                {

                }
                else
                {
                    double time = Convert.ToDouble(curTime);
                    mHitTimeArray.Add((float)time);
                }
            }
        }

        public void setShakeTime(string val)
        {
            mShakeTimeArray.Clear();
            string[] ShakedTime = val.Split(new string[1] { " " }, StringSplitOptions.None);
            foreach (string curTime in ShakedTime)
            {
                if (curTime == "")
                {

                }
                else
                {
                    double time = Convert.ToDouble(curTime);
                    mShakeTimeArray.Add((float)time);
                }
            }
        }

        public void setRepeatEffect(string val)
        {
            mRepeatEffect = Convert.ToBoolean(val);
        }

        public bool getRepeatEffect()
        {
            return mRepeatEffect;
        }

        public bool setParameter(string name, string val)
        {
            bool result = false;
            string spaceDelim = " ";
            val = val.TrimStart(spaceDelim.ToCharArray());
            val = val.TrimEnd(spaceDelim.ToCharArray());
            if(name == "Animation")
            {
                setAnimateName(val);
                result = true;
            }
            else if (name == "BreakTime")
            {
                setBreakTime(val);
                result = true;
            }
            else if (name == "HitTime")
            {
                setHitTime(val);
                result = true;
            }
            else if(name == "ShakeTime")
            {
                setShakeTime(val);
                result = true;
            }
            else if(name == "RepeatEffect")
            {
                setRepeatEffect(val);
                result = true;
            }
            else if (name == "EnableRibbon")
            {
                mEnableRibbon = Convert.ToBoolean(val);
                result = true;
            }
            return result;
        }

        public string getParameter(string name)
        {
            string resultStr = "";
            if (name == "Animation")
            {
                resultStr = getAnimateName();
            }
            else if (name == "BreakTime")
            {
                for (int i = 0; i < mBreakTimeArray.Count;i++)
                {
                    resultStr += mBreakTimeArray[i];
                    if(i + 1 < mBreakTimeArray.Count)
                    {
                        resultStr += " ";
                    }
                }
            }
            else if (name == "HitTime")
            {
                for (int i = 0; i < mHitTimeArray.Count;i++)
                {
                    resultStr += mHitTimeArray[i];
                    if (i + 1 < mHitTimeArray.Count)
                    {
                        resultStr += " ";
                    }
                }
            }
            else if (name == "ShakeTime")
            {
                for (int i = 0; i < mShakeTimeArray.Count; i++)
                {
                    resultStr += mShakeTimeArray[i];
                    if (i + 1 < mShakeTimeArray.Count)
                    {
                        resultStr += " ";
                    }
                }
            }
            else if (name == "RepeatEffect")
            {
                resultStr = "" +  mRepeatEffect;
            }
            else if (name == "EnableRibbon")
            {
                resultStr = "" + mEnableRibbon;
            }
            return resultStr;
        }
    }

    public class GfxSkillManager
    {
        static readonly GfxSkillManager sInstance = new GfxSkillManager();
		static string   SkillPath = "Skill/";
        static public GfxSkillManager Instance
        {
            get
            {
                return sInstance;
            }
        }
        static AssetBundle allSkillAsset = null;
        public static UnityEngine.AssetBundle AllSkillAsset
        {
            get { return allSkillAsset; }
            set
            {
                allSkillAsset = value;
            }
        }

        private Dictionary<string,GfxSkill>  mSkillMap = new Dictionary<string,GfxSkill>();
        public GfxSkill createSkill(string skillName)
        {
            GfxSkill skill = null;
            if (!mSkillMap.ContainsKey(skillName))
            {
                parseSkillFile(skillName);
            }
            if (!mSkillMap.TryGetValue(skillName, out skill))
            {
                return null;
            }
            return new GfxSkill(skill);
        }

        public GfxSkill createSkillTemplate(string skillName)
        {
            GfxSkill ret = null;
            if (!mSkillMap.ContainsKey(skillName))
            {
                ret = new GfxSkill(skillName);
                mSkillMap[skillName] = ret;
            }
            else
            {
                Debug.LogWarning("skill template with name already exists! ,EffectManager::createSkillTemplate");
            }
		    return ret;
        }
		
		public GfxSkill createAndParseSkillTemplate(string skillName)
        {
            GfxSkill skill = null;
            if (!mSkillMap.ContainsKey(skillName))
            {
                parseSkillFile(skillName);
            }
            if (!mSkillMap.TryGetValue(skillName, out skill))
            {
                return null;
            }
            return skill;
        }

        public void destroySkillTemplate(string skillName)
        {
            if (mSkillMap.ContainsKey(skillName))
            {
                mSkillMap.Remove(skillName);
            }
        }
        
        protected bool parseSkillFile(string skillName)
        {
            if (skillName == null || skillName.Length == 0) return false;
            StreamReader skillDataReader = null;
            MemoryStream skillData = null;
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                FileInfo fileInfo = new FileInfo("Assets/Resources/Skill/" + skillName + ".txt");
                if (fileInfo.Exists)
                {
                    skillDataReader = new StreamReader("Assets/Resources/Skill/" + skillName + ".txt");
                }
                else
                {
                    return false;
                }
            }
            else
            {
                TextAsset skillFile = null;
                if (GfxSkillManager.AllSkillAsset != null)
                {
                    if (GfxSkillManager.AllSkillAsset.Contains(skillName))
                    {
                        skillFile = GfxSkillManager.AllSkillAsset.Load(skillName) as TextAsset;
                        if (skillFile == null)
                        {
                            LogManager.LogError("skillName " + "does not exists");
                            return false;
                        }
                    }
                }
                else
                {
                    LogManager.LogError("Skill AssetBundle does not bind");
                    return false;
                }
                if (skillFile == null) return false;
                skillData = new MemoryStream(skillFile.bytes);
                skillDataReader = new StreamReader(skillData);
            }
            string line;
            GfxSkill skill = null;
            int lineNO = 0;
            while ((line = skillDataReader.ReadLine()) != null)
            {
                lineNO++;
                //Debug.LogWarning("LineNO:" + lineNO);
                if (skill == null)
                {
                    if (!(line.Length == 0 || line.Substring(0, 2) == "//"))
                    {
                        string[] szTemp = line.Split(new string[1] { " " }, StringSplitOptions.None);
                        if (szTemp[0] != "skill" || szTemp.Length != 2)
                        {
                            Debug.LogError("Wrong skill name line parseSkillFile");
                            continue;
                        }
                        skill = createSkillTemplate(szTemp[1]);
                        skipToNextOpenBrace(skillDataReader);
                    }
                }
                else
                {
                    string delim = "\t";
		            string spaceDelim = " ";
		            line = line.TrimStart(spaceDelim.ToCharArray());
		            line = line.TrimEnd(spaceDelim.ToCharArray());
		            string strline = line.Trim(delim.ToCharArray());
					if(strline == "}")
                    {
                        skill = null;
                    }
                    else if(strline == "AnimEffect")
                    {
                        skipToNextOpenBrace(skillDataReader);
                        parseAnimEffectInfo(skillDataReader,skill);
                    }
                    else if (strline == "Sound")
                    {
                        //skipToNextOpenBrace(skillDataReader);
                        //parseAnimSound(skillDataReader,skill);
                        parseInvalidSection(skillDataReader);
                    }
                    else if (strline == "Ribbon")
                    {
                        parseInvalidSection(skillDataReader);
                    }
                    else if ( strline == "SceneLight" )
                    {
                        parseInvalidSection(skillDataReader);
                    }
                    else if (strline == "HitGroundEffect")
                    {
                        skipToNextOpenBrace(skillDataReader);
                        parseHitGroundEffectInfo(skillDataReader, skill);
                    }
                    else
                    {
                        parseSkillAttrib(strline, skill);
                    }
                }
                
            };
            skillDataReader.Close();
            if (skillData != null)
                skillData.Close();
            return true;

        }

        void skipToNextOpenBrace(StreamReader dataStream)
        {
            string line = "";
			string delim = "\t";
			string spaceDelim = " ";
            while (!dataStream.EndOfStream)
            {
	            line = line.TrimStart(spaceDelim.ToCharArray());
	            line = line.TrimEnd(spaceDelim.ToCharArray());
	            string strline = line.Trim(delim.ToCharArray());
				if(strline == "{")break;
				
				line = dataStream.ReadLine();
            }
        }

        void parseInvalidSection(StreamReader dataStream)
        {
            skipToNextOpenBrace(dataStream);
            string line = "";
			string delim = "\t";
	        string spaceDelim = " ";
            while (!dataStream.EndOfStream)
            {
	            line = line.TrimStart(spaceDelim.ToCharArray());
	            line = line.TrimEnd(spaceDelim.ToCharArray());
	            string strline = line.Trim(delim.ToCharArray());
				if(strline == "}")break;
				
				line = dataStream.ReadLine();
            }
        }

        void parseAnimSound(StreamReader dataStream, GfxSkill skill)
        {
            //AnimationSound* sound = skill->addAnimationSound();

            //assert(sound);

            //// Parse emitter details
            //String line;

            //while (!stream->eof())
            //{
            //    line = stream->getLine();
            //    ++mWrongLineNum;

            //    // Ignore comments & blanks
            //    if (!(line.length() == 0 || line.substr(0, 2) == "//"))
            //    {
            //        if (line == "}")
            //        {
            //            // Finished emitter
            //            break;
            //        }
            //        else
            //        {
            //            // Attribute
            //            //	Ogre::StringUtil::toLowerCase(line);
            //            parseAnimSoundAttrib(line, sound);
            //        }
            //    }
            //}
        }

        void parseHitGroundEffectInfo(StreamReader dataStream, GfxSkill skill)
        {
            GfxSkillHitGroundEffect HitGroundEffect = new GfxSkillHitGroundEffect();
            skill.HitGroundEffect = HitGroundEffect;
            string line;
            while (!dataStream.EndOfStream)
            {
                line = dataStream.ReadLine();
                // Ignore comments & blanks
                if (!(line.Length == 0 || line.Substring(0, 2) == "//"))
                {
                    string delim = "\t";
                    string spaceDelim = " ";
                    line = line.TrimStart(spaceDelim.ToCharArray());
                    line = line.TrimEnd(spaceDelim.ToCharArray());
                    string strline = line.Trim(delim.ToCharArray());
                    if (strline == "}")
                    {
                        // Finished emitter
                        break;
                    }
                    else
                    {
                        // Attribute
                        parseHitGroundEffectInfoAttrib(strline, HitGroundEffect);
                    }
                }
            }
        }

        void parseHitGroundEffectInfoAttrib(string line, GfxSkillHitGroundEffect HitGroundEffect)
        {
            string[] szTemp = line.Split(new string[1] { " " }, StringSplitOptions.None);

            if (szTemp.Length <= 0)
            {
                Debug.LogError("the number of parameters must be >0! parseHitGroundEffectInfoAttrib " + line);
                return;
            }

            if (szTemp.Length == 2)
            {
                if (false == HitGroundEffect.setParameter(szTemp[0], szTemp[1]))
                {
                    Debug.LogError("Bad HitGroundEffect Info attribute line parseHitGroundEffectInfoAttrib " + line);
                }
            }
            else
            {
                string valueString = "";
                for (int i = 1; i < szTemp.Length; i++)
                {
                    valueString += szTemp[i];
                    if (i + 1 < szTemp.Length)
                    {
                        valueString += " ";
                    }
                }
                if (false == HitGroundEffect.setParameter(szTemp[0], valueString))
                {
                    Debug.LogError("Bad HitGroundEffect Info Complex attribute line parseHitGroundEffectInfoAttrib " + line);
                }
            }
        }

        void parseAnimEffectInfo(StreamReader dataStream, GfxSkill skill)
        {
            GfxSkillEffect newSkillEffect = skill.addSkillEffect();
            string line;
            while (!dataStream.EndOfStream)
            {
                line = dataStream.ReadLine();
                // Ignore comments & blanks
                if (!(line.Length == 0 || line.Substring(0, 2) == "//"))
                {
                    string delim = "\t";
                    string spaceDelim = " ";
                    line = line.TrimStart(spaceDelim.ToCharArray());
                    line = line.TrimEnd(spaceDelim.ToCharArray());
                    string strline = line.Trim(delim.ToCharArray());
                    if (strline == "}")
                    {
                        // Finished emitter
                        break;
                    }
                    else
                    {
                        // Attribute
                        parseAnimEffectInfoAttrib(strline, newSkillEffect);
                    }
                }
            }
        }

        void parseAnimEffectInfoAttrib(string line, GfxSkillEffect skillEffect)
	    {
		    // 设置element的属性
            string[] szTemp = line.Split(new string[1] { " " }, StringSplitOptions.None);

            if (szTemp.Length <= 0)
            {
                Debug.LogError("the number of parameters must be >0! parseAnimEffectInfoAttrib " + line);
                return;
            }

            if(szTemp.Length == 2)
            {
                if (false == skillEffect.setParameter(szTemp[0], szTemp[1]))
                {
                    Debug.LogError("Bad Anim Effect Info attribute line parseAnimEffectInfoAttrib " + line);
                }
            }
            else
            {
                string valueString = "";
                for(int i = 1; i < szTemp.Length;i++)
                {
                    valueString += szTemp[i];
                    if(i +1 < szTemp.Length)
                    {
                        valueString += " ";
                    }
                }
                if (false == skillEffect.setParameter(szTemp[0], valueString))
                {
                    Debug.LogError("Bad Anim Effect Info Complex attribute line parseAnimEffectInfoAttrib " + line);
                }
            }
	    }

        //void parseAnimSoundAttrib(string line, AnimationSound sound)
        //{
        //    // 设置element的属性
        //    string[] vecparams = line.Split(new string[1] { "\t" }, StringSplitOptions.None);

        //    if (vecparams.Length != 2 || false == sound.setParameter(vecparams[0], vecparams[1]))
        //    {
        //        Debug.LogError("Bad Anim sound attribute line EffectManager::parseAnimSoundAttrib");
        //    }
        //}	

        void parseSkillAttrib(string line, GfxSkill skill)
	    {
		    // 设置element的属性
            
            string[] szTemp = line.Split(new string[1] { " " }, StringSplitOptions.None);
            if (szTemp.Length <= 0)
            {
                Debug.LogError("the number of parameters must be >0! parseSkillAttrib " + line);
                return;
            }

            if (szTemp.Length == 2)
            {
                if (false == skill.setParameter(szTemp[0], szTemp[1]))
                {
                    Debug.LogError("Bad Anim Effect Info attribute line parseSkillAttrib " + line);
                }
            }
            else
            {
                string valueString = "";
                for (int i = 1; i < szTemp.Length; i++)
                {
                    valueString += szTemp[i];
                    if (i + 1 < szTemp.Length)
                    {
                        valueString += " ";
                    }
                }
                if (false == skill.setParameter(szTemp[0], valueString))
                {
                    Debug.LogError("Bad Anim Effect Info Complex attribute line parseSkillAttrib " + line);
                }
            }
	    }
		
		public void removeSkill(GfxSkill skill)
		{
			if(skill != null)
				skill.shutDown();
		}
    }
}