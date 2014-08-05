using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
public class ActorViewer:EditorWindow
{
	List<string> AllActors = new List<string>();
	List<string> BodyEquips= new List<string>(); 
	List<string> HairEquips= new List<string>();
	List<string> FaceEquips= new List<string>();
	List<string> ShoulderEquips= new List<string>();
	List<string> AllWeapons = new List<string>();
	List<string> AllEffects = new List<string>();
	List<string> AnimNames = new List<string>();
    

	int actorIndex = 0;
	int BodyEquipIndex = 0;
	int BodyEffectIndex = 0;
	
	int HairEquipIndex = 0;
	int FaceEquipIndex = 0;

	int ShoulderEquipIndex = 0;
	int ShoulderEffectIndex = 0;
		
	int AnimNameIndex = 0;
	int rightWeaponIndex = 0;
	int rightWeaponEffectIndex = 0;
	int leftWeaponIndex = 0;
	int leftWeaponEffectIndex = 0;
    static GFX.GfxActor mObject;


    GFX.GfxSkill mSkill;
    List<string> mEffects = new List<string>();
    List<string> SkillNames = new List<string>();
    List<string> LocatorNames = new List<string>();
    int effectIndex = 0;
    int skillIndex = 0;
    int lastSkillIndex = 0;
    int lastEffectIndex = 0;
   // string mAnimateName = "";
    string mSkillName = "";
    string mBreakTime = "";
    string mHitTime = "";
    string mShakeTime = "";
    string mRepeatEffect = "";
    string mEffectName = "";
   // string mAttachPoint = "";
    string mAtttachTime = "";
    string mAttach = "";
    Vector3 mOffsetPos = Vector3.zero;
    Vector4 mOffsetRotation = Vector4.zero;
    Vector3 mOffsetScale = new Vector3(1, 1, 1);
    int mSkillAnimateNameIndex = 0;
    int mLocatorNameIndex = 0;
    bool isPlaying = false;
    bool isReadyUpdateLocater = false;
    enum GUI_STATUS
    {
        CHECKING,
        ADDEFFECT,
        ADDSKILL,
        NONE
    };
    enum GUI_ACTION
    {
        PLAY,
        STOP,
        NONE
    };
    GUI_STATUS stat = GUI_STATUS.NONE;
    string GetActionName()
    {
        if (isPlaying)
        {
            return "Stop";
        }
        return "Play";
    }

    void SetAction()
    {
        if (isPlaying)
        {
            Stop();
        }
        else
        {
            Play();
        }
        isPlaying = !isPlaying;
    }

    void Play()
    {
        if(mSkill != null)
		{
			mObject.Currentskill = mSkill;
			mObject.EnterSkill(false, mSkill.getSkillName(), true, 0.3f);
			isReadyUpdateLocater = true;
		}
    }

    void Stop()
    {
		mObject.Currentskill = null;
		CreateGameObject();
    }

    [MenuItem("SG Tools/Actor viewer")]
    static void init()
    {
		if(!EditorApplication.isPlaying) EditorApplication.isPlaying = true;
		
		ActorViewer window = (ActorViewer)GetWindow(typeof(ActorViewer));
		window.Show();
		if (!Directory.Exists(AssertBundleCreator.ActorAssetbundlePath))
             Directory.CreateDirectory(AssertBundleCreator.ActorAssetbundlePath);
		DirectoryInfo actorInfo = new DirectoryInfo(AssertBundleCreator.ActorAssetbundlePath);
        FileInfo[] actorFiles = actorInfo.GetFiles();
         string assetBundleExt = ".assetbundle";
		window.AllActors.Add("none");
		window.BodyEquips.Add("none");
		window.HairEquips.Add("none");
		window.FaceEquips.Add("none");
		window.ShoulderEquips.Add("none");
		window.AllWeapons.Add("none");
		window.AllEffects.Add("none");
		window.AnimNames.Add("none");
		
		window.AllActors.Add("wplayer");
		window.AllActors.Add("mplayer");
        foreach (FileInfo fileInfo in actorFiles)
        {
            string actorFileName = fileInfo.Name;
            if (actorFileName.Contains(assetBundleExt))
            {
                string actorName = actorFileName.Substring(0, actorFileName.Length - assetBundleExt.Length);
                window.AllActors.Add(actorName);
				window.AllWeapons.Add(actorName);
				window.AllEffects.Add(actorName);
            }
        }
		
		if (!Directory.Exists(AssertBundleCreator.EquipAssetbundlePath))
             Directory.CreateDirectory(AssertBundleCreator.EquipAssetbundlePath);
		DirectoryInfo equipInfo = new DirectoryInfo(AssertBundleCreator.EquipAssetbundlePath);
        FileInfo[] equipFiles = equipInfo.GetFiles();
		
		foreach (FileInfo fileInfo in equipFiles)
        {
            string equipFileName = fileInfo.Name;
            if (equipFileName.Contains(assetBundleExt))
            {
                string equipName = equipFileName.Substring(0, equipFileName.Length - assetBundleExt.Length);
                window.BodyEquips.Add(equipName);
				window.HairEquips.Add(equipName);
				window.FaceEquips.Add(equipName);
				window.ShoulderEquips.Add(equipName);
            }
        }


        string skillFileExt = ".txt";
        if (!Directory.Exists(AssertBundleCreator.SkillAssetbundlePath))
            Directory.CreateDirectory(AssertBundleCreator.SkillAssetbundlePath);
        DirectoryInfo skillInfo = new DirectoryInfo(AssertBundleCreator.SkillAssetbundlePath);
        FileInfo[] skillFiles = skillInfo.GetFiles();
        window.SkillNames.Add("none");
        foreach (FileInfo fileInfo in skillFiles)
        {
            string skillFileName = fileInfo.Name;
            if (skillFileName.Contains(skillFileExt))
            {
                string SkillName = skillFileName.Substring(0, skillFileName.Length - skillFileExt.Length);
                window.SkillNames.Add(SkillName);
            }
        }
        window.LocatorNames.Add("none");
		//创建game object用于每帧更新
 //		GameObject frameGO = new GameObject();
 	//	frameGO.AddComponent("FrameUpdateScript");
    }
	
	void OnGUI()
	{
		GUILayout.BeginHorizontal("box");
		GUILayout.Label("Object");
		actorIndex = EditorGUILayout.Popup(actorIndex, AllActors.ToArray());
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal("box");
		GUILayout.Label("Body");
		BodyEquipIndex = EditorGUILayout.Popup(BodyEquipIndex, BodyEquips.ToArray());
		GUILayout.Label("Effect");
		BodyEffectIndex = EditorGUILayout.Popup(BodyEffectIndex, AllEffects.ToArray());
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal("box");
		GUILayout.Label("Hair");
		HairEquipIndex = EditorGUILayout.Popup(HairEquipIndex, HairEquips.ToArray());
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal("box");
		GUILayout.Label("Face");
		FaceEquipIndex = EditorGUILayout.Popup(FaceEquipIndex, FaceEquips.ToArray());
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal("box");
		GUILayout.Label("Shoulder");
		ShoulderEquipIndex = EditorGUILayout.Popup(ShoulderEquipIndex, ShoulderEquips.ToArray());
		GUILayout.Label("Effect");
		ShoulderEffectIndex = EditorGUILayout.Popup(ShoulderEffectIndex, AllEffects.ToArray());
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal("box");
		GUILayout.Label("Right Weapon");
		rightWeaponIndex = EditorGUILayout.Popup(rightWeaponIndex, AllWeapons.ToArray());
		GUILayout.Label("Effect");
		rightWeaponEffectIndex = EditorGUILayout.Popup(rightWeaponEffectIndex, AllEffects.ToArray());
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal("box");
		GUILayout.Label("Left Weapon");
		leftWeaponIndex = EditorGUILayout.Popup(leftWeaponIndex, AllWeapons.ToArray());
		GUILayout.Label("Effect");
		leftWeaponEffectIndex = EditorGUILayout.Popup(leftWeaponEffectIndex, AllEffects.ToArray());
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal("box");
		GUILayout.Label("Animation");
		int oldAnimIndex = AnimNameIndex;
		AnimNameIndex = EditorGUILayout.Popup(AnimNameIndex, AnimNames.ToArray());
		if(oldAnimIndex != AnimNameIndex && AnimNameIndex>=0 && AnimNameIndex<AnimNames.Count && mObject != null)
				mObject.EnterSkill(true,AnimNames[AnimNameIndex]!="none"?AnimNames[AnimNameIndex]:"", true, 0.3f);
		GUILayout.EndHorizontal ();

		if(GUILayout.Button("Reset"))
		{
			reset();
		}
		GUILayout.BeginHorizontal("box");
		if(GUILayout.Button("OK"))
		{
            if (!EditorApplication.isPlaying) EditorApplication.isPlaying = true;
			else
			{
				if(AllActors[actorIndex] != "none")
				{
					CreateGameObject();
					LocatorNames.Clear();
	                LocatorNames.Add("none");
				}
				else
				{
					if(mObject != null)
					{
						GFX.GFXObjectManager.Instance.DestroyObject(mObject);
						mObject = null;
					}
	 
				}
			}

		}
// 		if(GUILayout.Button("stop"))
// 		{
// 			destroyActor();
//             AssetBundleManager.AssetBundleRequestManager.clearAssetBundles();
// 			if (EditorApplication.isPlaying) EditorApplication.isPlaying = false;
// 		}
		GUILayout.EndHorizontal ();
        DisplaySkillGUI();
	}
	void destroyActor()
	{
		if(mObject != null)
		{
			GFX.GFXObjectManager.Instance.DestroyObject(mObject);
			mObject = null;
		}
	}
	void CreateGameObject()
	{
		destroyActor();
        mObject = (GFX.GfxActor)GFX.GFXObjectManager.Instance.createObject(AllActors[actorIndex], GFX.GFXObjectType.ACTOR);

		//换装
		mObject.changePart(0, BodyEquips[BodyEquipIndex] != "none" ? BodyEquips[BodyEquipIndex]:"");
		mObject.setBodyEffect(AllEffects[BodyEffectIndex]!="none"?AllEffects[BodyEffectIndex]:"");
		mObject.changePart(1, HairEquips[HairEquipIndex] != "none" ? HairEquips[HairEquipIndex]:"");
		mObject.changePart(2, FaceEquips[FaceEquipIndex] != "none" ? FaceEquips[FaceEquipIndex]:"");
		mObject.changePart(3, ShoulderEquips[ShoulderEquipIndex] != "none" ? ShoulderEquips[ShoulderEquipIndex]:"");
		mObject.setShoulderEffect(AllEffects[ShoulderEffectIndex]!="none"?AllEffects[ShoulderEffectIndex]:"");
		mObject.changeRightWeapon(AllWeapons[rightWeaponIndex]!="none"?AllWeapons[rightWeaponIndex]:"");
		mObject.changeLeftWeapon(AllWeapons[leftWeaponIndex]!="none"?AllWeapons[leftWeaponIndex]:"");
		mObject.SetRightWeaponEffect(AllEffects[rightWeaponEffectIndex]!="none"?AllEffects[rightWeaponEffectIndex]:"");
		mObject.SetLeftWeaponEffect(AllEffects[leftWeaponEffectIndex]!="none"?AllEffects[leftWeaponEffectIndex]:"");
		
		isReadyUpdateLocater = true;
	}
	
	void Update()
	{
		if(mObject!=null)
		{
			GFX.GfxSystem.Instance.Tick();
			AnimNames = mObject.getAnimationName();
            AnimNames.Sort();
			AnimNames.Insert(0,"none");
			

			
            if(isReadyUpdateLocater)
            {
                GameObject go = mObject.getGameObject();
                if (go != null)
                {
                    Transform[] actorBones = go.GetComponentsInChildren<Transform>();
                    foreach (Transform t in actorBones)
                    {
                        if (t.name.IndexOf("Locator") != -1)
                        {
                            LocatorNames.Add(t.name);
                        }
                    }
                }
                isReadyUpdateLocater = false;
				
				if(mEffects.Count != 0 && mEffects[effectIndex] != "none" && mSkill != null)
				{
					GFX.GfxSkillEffect curSkillEffect = mSkill.getSkillEffect(effectIndex - 1);
					if(curSkillEffect != null)
					{
		                string attachPoint = curSkillEffect.getParameter("AttachPoint");
		                mLocatorNameIndex = LocatorNames.IndexOf(attachPoint);

                        if (mLocatorNameIndex == -1)
		                {
		                    mLocatorNameIndex = 0;
		                }
					}
				}
				
				if(mSkill != null)
				{
					string curAnimateName    = mSkill.getParameter("Animation");
                    mSkillAnimateNameIndex   = AnimNames.IndexOf(curAnimateName);
                    if (mSkillAnimateNameIndex == -1)
                    {
                        mSkillAnimateNameIndex = 0;
                    }
				}
            }
		}
    }

    void reset()	
	{
		actorIndex = 0;
		 BodyEquipIndex = 0;
		 BodyEffectIndex = 0;
	
	 	HairEquipIndex = 0;
	 	FaceEquipIndex = 0;

		 ShoulderEquipIndex = 0;
	 	ShoulderEffectIndex = 0;
		
		 AnimNameIndex = 0;
	 	rightWeaponIndex = 0;
		 rightWeaponEffectIndex = 0;
		leftWeaponIndex = 0;
	 	leftWeaponEffectIndex = 0;
    }
    

    void DisplaySkillGUI()
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Skill");
        skillIndex = EditorGUILayout.Popup(skillIndex, SkillNames.ToArray());
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (!GUILayout.Button("AddSkill"))
        {
            if (SkillNames[skillIndex] != "none")
            {
                if (lastSkillIndex != skillIndex)
                {
                    GFX.GfxSkill curSkill = GFX.GfxSkillManager.Instance.createAndParseSkillTemplate(SkillNames[skillIndex]);
                    if (curSkill != null)
                    {
                        mSkill = curSkill;
                        stat = GUI_STATUS.CHECKING;
                        refreshSkillEffectList();
                        string curAnimateName    = mSkill.getParameter("Animation");
                        mSkillAnimateNameIndex   = AnimNames.IndexOf(curAnimateName);
                        if (mSkillAnimateNameIndex ==-1)
                        {
                            mSkillAnimateNameIndex = 0;
                        }
                        mBreakTime      = mSkill.getParameter("BreakTime");
                        mHitTime        = mSkill.getParameter("HitTime");
                        mShakeTime      = mSkill.getParameter("ShakeTime");
                        mRepeatEffect   = mSkill.getParameter("RepeatEffect");
						lastEffectIndex = 0;
                    }
                }
                lastSkillIndex = skillIndex;
            }
        }
        else
        {
            stat = GUI_STATUS.ADDSKILL;
        }

        if (GUILayout.Button("RemoveSkill"))
        {
            removeSkill();
        }
        GUILayout.EndHorizontal();

        switch (stat)
        {
            case GUI_STATUS.ADDEFFECT:
                addSkillEffect();
                break;
            case GUI_STATUS.ADDSKILL:
                addSkill();
                break;
            case GUI_STATUS.CHECKING:
                {
                    showSkill();
                    showSkillEffect();
                    GUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("SetSkill"))
                    {
                        if(AnimNames[mSkillAnimateNameIndex] == "none")
                        {
                            mSkill.setParameter("Animation", "");
                        }
                        else
                        {
                            mSkill.setParameter("Animation", AnimNames[mSkillAnimateNameIndex]);
                        }
                        mSkill.setParameter("BreakTime", mBreakTime);
                        mSkill.setParameter("HitTime", mHitTime);
                        mSkill.setParameter("ShakeTime", mShakeTime);
                        mSkill.setParameter("RepeatEffect", mRepeatEffect);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("box");
                    if (GUILayout.Button(GetActionName()))
                    {
                        SetAction();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Save"))
                    {
                        Save();
                    }
                    GUILayout.EndHorizontal();
                }
                break;
        }
    }

    void showSkill()
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Animation");
        mSkillAnimateNameIndex = EditorGUILayout.Popup(mSkillAnimateNameIndex, AnimNames.ToArray());
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("BreakTime");
        mBreakTime = EditorGUILayout.TextField(mBreakTime);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("HitTime");
        mHitTime = EditorGUILayout.TextField(mHitTime);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("ShakeTime");
        mShakeTime = EditorGUILayout.TextField(mShakeTime);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("RepeatEffect");
        mRepeatEffect = EditorGUILayout.TextField(mRepeatEffect);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        effectIndex = EditorGUILayout.Popup(effectIndex, mEffects.ToArray());
        GUILayout.EndHorizontal();
    }

    void refreshSkillEffectList()
    {
        mEffects.Clear();
        mEffects.Add("none");
        for (int i = 0; i < mSkill.getSkillEffectCount(); i++)
        {
            mEffects.Add("Effect" + i);
        }
    }

    void showSkillEffect()
    {
        if (mEffects[effectIndex] != "none")
        {
            if (effectIndex != lastEffectIndex)
            {
                GFX.GfxSkillEffect curSkillEffect = mSkill.getSkillEffect(effectIndex - 1);
                mAtttachTime = curSkillEffect.getParameter("AttachTime");
                string attachPoint = curSkillEffect.getParameter("AttachPoint");
                mLocatorNameIndex = LocatorNames.IndexOf(attachPoint);
                if (mLocatorNameIndex == -1)
                {
                    mLocatorNameIndex = 0;
                }
                mEffectName = curSkillEffect.getParameter("EffectTemplateName");
                mOffsetPos = curSkillEffect.OffsetPos;
                mOffsetRotation = new Vector4(curSkillEffect.OffsetRotation.x,
                                              curSkillEffect.OffsetRotation.y,
                                              curSkillEffect.OffsetRotation.z,
                                              curSkillEffect.OffsetRotation.w);
                mOffsetScale = curSkillEffect.OffsetScale;
                mAttach = curSkillEffect.getParameter("Attach");
            }
            lastEffectIndex = effectIndex;
            GUILayout.BeginHorizontal("box");

            GUILayout.Label("AttachTime");
            mAtttachTime = GUILayout.TextField(mAtttachTime);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("AttachPoint");
            mLocatorNameIndex = EditorGUILayout.Popup(mLocatorNameIndex, LocatorNames.ToArray());
           // mAttachPoint = GUILayout.TextField(mAttachPoint);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("EffectTemplateName");
            mEffectName = GUILayout.TextField(mEffectName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            mOffsetPos = EditorGUILayout.Vector3Field("OffsetPos", mOffsetPos);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            mOffsetRotation = EditorGUILayout.Vector4Field("OffsetRotation", mOffsetRotation);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            mOffsetScale = EditorGUILayout.Vector3Field("OffsetScale", mOffsetScale);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Attach");
            mAttach = GUILayout.TextField(mAttach);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("SetSkillEffect"))
            {
                GFX.GfxSkillEffect curSkillEffect = mSkill.getSkillEffect(effectIndex - 1);
                curSkillEffect.setParameter("AttachTime", mAtttachTime);
                if(LocatorNames[mLocatorNameIndex] == "none")
                {
                    curSkillEffect.setParameter("AttachPoint", "");
                }
                else
                {
                    curSkillEffect.setParameter("AttachPoint",LocatorNames[mLocatorNameIndex]);
                }
                
                curSkillEffect.setParameter("EffectTemplateName", mEffectName);
                curSkillEffect.setParameter("Attach", mAttach);
                curSkillEffect.OffsetPos = mOffsetPos;
                curSkillEffect.OffsetRotation = new Quaternion(mOffsetRotation.x, mOffsetRotation.y, mOffsetRotation.z, mOffsetRotation.w);
                curSkillEffect.OffsetScale = mOffsetScale;
            }
        }

        GUILayout.BeginHorizontal("box");

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("AddEffect"))
        {
            stat = GUI_STATUS.ADDEFFECT;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("RemoveEffect"))
        {
            removeSkillEffect();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndHorizontal();
    }

    void removeSkill()
    {
        if (EditorUtility.DisplayDialog("Remove Skill", "Are you sure to remove current Skill", "OK"))
        {
            if (skillIndex > 0)
            {
                GFX.GfxSkillManager.Instance.destroySkillTemplate(SkillNames[skillIndex]);
                string skillFile = SkillNames[skillIndex];
                SkillNames.RemoveAt(skillIndex);
                string fullFilePath = AssertBundleCreator.SkillAssetbundlePath + skillFile + ".txt";
                File.Delete(fullFilePath);
                skillIndex = 0;
                stat = GUI_STATUS.NONE;
            }
        }
    }

    void addSkill()
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("AnimateName");
       // mAnimateName = GUILayout.TextField(mAnimateName);
        mSkillAnimateNameIndex = EditorGUILayout.Popup(mSkillAnimateNameIndex,AnimNames.ToArray());
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("SkillName");
        mSkillName = GUILayout.TextField(mSkillName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("OK"))
        {
            GFX.GfxSkill newSkill = GFX.GfxSkillManager.Instance.createSkillTemplate(mSkillName);
            if (newSkill != null)
            {
                if(AnimNames[mSkillAnimateNameIndex]== "none")
                {
                    newSkill.setAnimateName("");
                }
                else
                {
                    newSkill.setAnimateName(AnimNames[mSkillAnimateNameIndex]);
                }
                SkillNames.Add(mSkillName);
            }
            lastSkillIndex = 0;
            if (skillIndex != 0)
                stat = GUI_STATUS.CHECKING;
            else
                stat = GUI_STATUS.NONE;
        }

        if (GUILayout.Button("Cancel"))
        {
            lastSkillIndex = 0;
            if (skillIndex != 0)
                stat = GUI_STATUS.CHECKING;
            else
                stat = GUI_STATUS.NONE;
        }
        GUILayout.EndHorizontal();
    }

    void removeSkillEffect()
    {
        if (EditorUtility.DisplayDialog("remove SkillEffect", "Are you sure to remove current SkillEffect", "OK"))
        {
            if (effectIndex > 0)
            {
                mSkill.removeSkillEffect(effectIndex);
                effectIndex = 0;
            }
        }
    }

    void addSkillEffect()
    {
        showSkill();
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("EffectName");
        mEffectName = EditorGUILayout.TextField(mEffectName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("AttachPoint");
        mLocatorNameIndex = EditorGUILayout.Popup(mLocatorNameIndex, LocatorNames.ToArray());
        //mAttachPoint = EditorGUILayout.TextField(mAttachPoint);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("OK"))
        {
            GFX.GfxSkillEffect newSkillEffect = mSkill.addSkillEffect();
            newSkillEffect.setParameter("EffectTemplateName", mEffectName);
            if (LocatorNames[mLocatorNameIndex] == "none")
            {
                newSkillEffect.setParameter("AttachPoint", "");
            }
            else
            {
                newSkillEffect.setParameter("AttachPoint", LocatorNames[mLocatorNameIndex]);
            }
            
            refreshSkillEffectList();
            lastEffectIndex = 0;
            if (skillIndex != 0)
                stat = GUI_STATUS.CHECKING;
            else
                stat = GUI_STATUS.NONE;
        }

        if (GUILayout.Button("Cancel"))
        {
            lastEffectIndex = 0;
            if (skillIndex != 0)
                stat = GUI_STATUS.CHECKING;
            else
                stat = GUI_STATUS.NONE;
        }
        GUILayout.EndHorizontal();
    }

    void Save()
    {
        if (mSkill != null)
        {
            string skillFileName = SkillNames[skillIndex];
            string fullFilePath = AssertBundleCreator.SkillAssetbundlePath + skillFileName + ".txt";
            FileStream skillData = new FileStream(fullFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            skillData.Flush();
            StreamWriter skillWrite = new StreamWriter(skillData);
            skillWrite.WriteLine("skill " + skillFileName);
            skillWrite.WriteLine("{");
            skillWrite.WriteLine("\t\tAnimation " + mSkill.getAnimateName());
            skillWrite.WriteLine("\t\tBreakTime " + mSkill.getParameter("BreakTime"));
            skillWrite.WriteLine("\t\tHitTime " + mSkill.getParameter("HitTime"));
            skillWrite.WriteLine("\t\tShakeTime " + mSkill.getParameter("ShakeTime"));
            skillWrite.WriteLine("\t\tRepeatEffect " + mSkill.getParameter("RepeatEffect"));
            skillWrite.WriteLine("\t\tEnableRibbon " + mSkill.getParameter("EnableRibbon"));
            for (int i = 0; i < mSkill.getSkillEffectCount(); i++)
            {
                skillWrite.WriteLine("\t\tAnimEffect");
                GFX.GfxSkillEffect skillEffect = mSkill.getSkillEffect(i);
                skillWrite.WriteLine("\t\t{");
                skillWrite.WriteLine("\t\t\tAttachTime " + skillEffect.getParameter("AttachTime"));
                skillWrite.WriteLine("\t\t\tAttachPoint " + skillEffect.getParameter("AttachPoint"));
                skillWrite.WriteLine("\t\t\tEffectTemplateName " + skillEffect.getParameter("EffectTemplateName"));
                skillWrite.WriteLine("\t\t\tOffsetPos " + skillEffect.getParameter("OffsetPos"));
                skillWrite.WriteLine("\t\t\tOffsetRotation " + skillEffect.getParameter("OffsetRotation"));
                skillWrite.WriteLine("\t\t\tOffsetScale " + skillEffect.getParameter("OffsetScale"));
                skillWrite.WriteLine("\t\t\tAttach " + skillEffect.getParameter("Attach"));
                skillWrite.WriteLine("\t\t}");
            }
            skillWrite.WriteLine("}");
            skillWrite.Close();
            skillData.Close();
        }
    }
}
