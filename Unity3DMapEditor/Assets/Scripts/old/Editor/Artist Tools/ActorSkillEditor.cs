using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(ActorSkill))]
public class ActorSkillEditor : Editor
{
    const string WEQUIPTPREFIX = "w";
    const string MEQUIPTPREFIX = "m";
    
    const string MHEQUIPPREFIX = "mh";
    const string MTEQUIPPREFIX = "mt";
    const string MSEQUIPPREFIX = "ms";
    const string MMEQUIPPREFIX = "mm";
    const string WHEQUIPPREFIX = "wh";
    const string WTEQUIPPREFIX = "wt";
    const string WSEQUIPPREFIX = "ws";
    const string WMEQUIPPREFIX = "wm";

    const string WTANIMATEPREFIX = "W_t_";
    const string WHANIMATEPREFIX = "W_h_";
    const string WMANIMATEPREFIX = "W_m_";
    const string WSANIMATEPREFIX = "W_s_";

    const string MTANIMATEPREFIX = "M_t_";
    const string MHANIMATEPREFIX = "M_h_";
    const string MMANIMATEPREFIX = "M_m_";
    const string MSANIMATEPREFIX = "M_s_";


    const string WEAPON2PREFIX = "weapon2_";
    const string WEAPON4PREFIX = "weapon4_";
    const string WEAPON3PREFIX = "weapon3_";
    const string WEAPON5PREFIX = "weapon5_";

    const string MPSKILL1PREFIX = "MP1";
    const string MPSKILL2PREFIX = "MP2";
    const string MPSKILL3PREFIX = "MP3";
    const string MPSKILL4PREFIX = "MP4";

    const string WPSKILL1PREFIX = "WP1";
    const string WPSKILL2PREFIX = "WP2";
    const string WPSKILL3PREFIX = "WP3";
    const string WPSKILL4PREFIX = "WP4";
	bool    inited = false;
   // static bool inited = false;
    List<string> AllActors = new List<string>();
    List<string> BodyEquips = new List<string>();

    List<string> whBodyEquips = new List<string>();
    List<string> mhBodyEquips = new List<string>();

    List<string> wtBodyEquips = new List<string>();
    List<string> mtBodyEquips = new List<string>();

    List<string> msBodyEquips = new List<string>();
    List<string> wsBodyEquips = new List<string>();

    List<string> wmBodyEquips = new List<string>();
    List<string> mmBodyEquips = new List<string>();

    List<string> wBodyEquips = new List<string>();
    List<string> mBodyEquips = new List<string>();

    List<string> whAnimates = new List<string>();
    List<string> mhAnimates = new List<string>();

    List<string> wtAnimates = new List<string>();
    List<string> mtAnimates = new List<string>();

    List<string> msAnimates = new List<string>();
    List<string> wsAnimates = new List<string>();

    List<string> wmAnimates = new List<string>();
    List<string> mmAnimates = new List<string>();

    List<string> HairEquips = new List<string>();
    List<string> FaceEquips = new List<string>();
    List<string> ShoulderEquips = new List<string>();

    List<string> AllWeapons = new List<string>();

    List<string> weapon2Weapons = new List<string>();
    List<string> weapon4Weapons = new List<string>();
    List<string> weapon3Weapons = new List<string>();
    List<string> weapon5Weapons = new List<string>();

    List<string> AllEffects = new List<string>();
    ActorSkill actor;
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
    bool openHitGroundEffect = false;

    List<string> mEffects    = new List<string>();
    List<string> SkillNames = new List<string>();

    List<string> w1SkillNames = new List<string>();
    List<string> w2SkillNames = new List<string>();
    List<string> w3SkillNames = new List<string>();
    List<string> w4SkillNames = new List<string>();

    List<string> m1SkillNames = new List<string>();
    List<string> m2SkillNames = new List<string>();
    List<string> m3SkillNames = new List<string>();
    List<string> m4SkillNames = new List<string>();

    int effectIndex = 0;
    int skillIndex = 0;
    int lastSkillIndex = 0;
    int lastEffectIndex = 0;
    string mSkillName = "";
    string mBreakTime = "";
    string mHitTime = "";
    string mShakeTime = "";
    bool mRepeatEffect = false;
    bool mEnableRibbon = true;
    string mEffectName = "";
    float mAtttachTime = 0f;
    bool mAttach = false;
    Vector3 mOffsetPos = Vector3.zero;
    Vector4 mOffsetRotation = Vector4.zero;
    Vector3 mOffsetScale = new Vector3(1, 1, 1);
    int mSkillAnimateNameIndex = 0;
    int mLocatorNameIndex = 0;

    int     mHitGroundAnimateNameIndex = 0;
    int     mHitGroundLocatorNameIndex = 0;
    Vector3 mHitGroundOffsetPos = Vector3.zero;
    Vector4 mHitGroundOffsetRotation = Vector4.zero;
    Vector3 mHitGroundOffsetScale = new Vector3(1, 1, 1);
    bool    mHitGroundAttach = false;
    float   mHitGroundAttachTime = 0;
    float   mHitGroundShakeTime = 0;
    string  mHitGroundEffectName = "";


    bool isPlaying = false;
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

    List<string> getCurrentBodyEquipList()
    {
        if (AllActors[actorIndex].IndexOf("w") == 0)
        {
            return wBodyEquips;
        }

        if (AllActors[actorIndex].IndexOf("m") == 0)
        {
            return mBodyEquips;
        }
        return BodyEquips;
    }

    List<string> getCurrentAnimateList()
    {
        List<string> curBodyEquipt = getCurrentBodyEquipList();
        if (curBodyEquipt.Count > BodyEquipIndex)
        {
            if (curBodyEquipt[BodyEquipIndex].IndexOf(MHEQUIPPREFIX) == 0)
            {
                return actor.MHAnimateNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(WHEQUIPPREFIX) == 0)
            {
                return actor.WHAnimateNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(MTEQUIPPREFIX) == 0)
            {
                return actor.MTAnimateNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(WTEQUIPPREFIX) == 0)
            {
                return actor.WTAnimateNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(MSEQUIPPREFIX) == 0)
            {
                return actor.MSAnimateNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(WSEQUIPPREFIX) == 0)
            {
                return actor.WSAnimateNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(MMEQUIPPREFIX) == 0)
            {
                return actor.MMAnimateNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(WMEQUIPPREFIX) == 0)
            {
                return actor.WMAnimateNames;
            }
        }
        else
        {
            BodyEquipIndex = 0;
        }
        return actor.AnimateNames;
    }

    List<string> getCurrrentSkillList()
    {
        List<string>  curBodyEquipt = getCurrentBodyEquipList();
        if (curBodyEquipt.Count > BodyEquipIndex)
        {
            if (curBodyEquipt[BodyEquipIndex].IndexOf(MHEQUIPPREFIX) == 0)
            {
                return m4SkillNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(WHEQUIPPREFIX) == 0)
            {
                return w4SkillNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(MTEQUIPPREFIX) == 0)
            {
                return m1SkillNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(WTEQUIPPREFIX) == 0)
            {
                return w1SkillNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(MSEQUIPPREFIX) == 0)
            {
                return m2SkillNames;
            }

            if(curBodyEquipt[BodyEquipIndex].IndexOf(WSEQUIPPREFIX) == 0)
            {
                return w2SkillNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(MMEQUIPPREFIX) == 0)
            {
                return m3SkillNames;
            }

            if (curBodyEquipt[BodyEquipIndex].IndexOf(WMEQUIPPREFIX) == 0)
            {
                return w3SkillNames;
            }
        }
        else
        {
            BodyEquipIndex = 0;
        }
        return SkillNames;
    }

    List<string> getCurrentWeaponList()
    {
        string bodyEquipName = getCurrentBodyEquipName();
        if (bodyEquipName.IndexOf(MHEQUIPPREFIX) == 0 ||
            bodyEquipName.IndexOf(WHEQUIPPREFIX) == 0)
        {
            return weapon4Weapons;
        }

        if (bodyEquipName.IndexOf(MTEQUIPPREFIX) == 0 ||
            bodyEquipName.IndexOf(WTEQUIPPREFIX) == 0)
        {
            return weapon5Weapons;
        }

        if (bodyEquipName.IndexOf(MSEQUIPPREFIX) == 0 ||
            bodyEquipName.IndexOf(WSEQUIPPREFIX) == 0)
        {
            return weapon3Weapons;
        }

        if (bodyEquipName.IndexOf(MMEQUIPPREFIX) == 0 ||
            bodyEquipName.IndexOf(WMEQUIPPREFIX) == 0)
        {
            return weapon2Weapons;
        }
        return AllWeapons;
    }

    string getCurrentSkillName()
    {
        List<string> skillList = getCurrrentSkillList();
        if (skillList.Count <= skillIndex)
        {
            skillIndex = 0;
        }
        return skillList[skillIndex];
    }

    string getCurrentBodyEquipName()
    {
        List<string> bodyEquipList = getCurrentBodyEquipList();
        if (bodyEquipList.Count <= BodyEquipIndex)
        {
            BodyEquipIndex = 0;
        }
        return bodyEquipList[BodyEquipIndex];
    }

    string getCurrentAnimateName()
    {
        List<string> animateList = getCurrentAnimateList();
        if (animateList.Count <= AnimNameIndex)
        {
            AnimNameIndex = 0;
        }
        return animateList[AnimNameIndex];
    }

    string getCurrentSkillAnimateName()
    {
        List<string> animateList = getCurrentAnimateList();
        if (animateList.Count <= mSkillAnimateNameIndex)
        {
            mSkillAnimateNameIndex = 0;
        }
        return animateList[mSkillAnimateNameIndex];
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
        actor.play();
    }

    void OnEnable()
    {
        actor = (ActorSkill)target;
        Debug.LogWarning("OnEnable");
    }

    void Stop()
    {
        actor.stop();
        CreateGameObject();
    }

    void Init()
    {
        if (!Directory.Exists(AssertBundleCreator.ActorAssetbundlePath))
            Directory.CreateDirectory(AssertBundleCreator.ActorAssetbundlePath);
        DirectoryInfo actorInfo = new DirectoryInfo(AssertBundleCreator.ActorAssetbundlePath);
        FileInfo[] actorFiles = actorInfo.GetFiles();
        string assetBundleExt = ".assetbundle";
        AllActors.Add("none");
        BodyEquips.Add("none");
        HairEquips.Add("none");
        FaceEquips.Add("none");
        ShoulderEquips.Add("none");
        AllWeapons.Add("none");
        AllEffects.Add("none");

        weapon2Weapons.Add("none");
        weapon4Weapons.Add("none");
        weapon3Weapons.Add("none");
        weapon5Weapons.Add("none");
        whBodyEquips.Add("none");
        wtBodyEquips.Add("none");
        wsBodyEquips.Add("none");
        wmBodyEquips.Add("none");
        mhBodyEquips.Add("none");
        mtBodyEquips.Add("none");
        msBodyEquips.Add("none");
        mmBodyEquips.Add("none");
        w1SkillNames.Add("none");
        w2SkillNames.Add("none");
        w3SkillNames.Add("none");
        w4SkillNames.Add("none");
        m1SkillNames.Add("none");
        m2SkillNames.Add("none");
        m3SkillNames.Add("none");
        m4SkillNames.Add("none");
        wBodyEquips.Add("none");
        mBodyEquips.Add("none");

        stat = GUI_STATUS.NONE;
        AllActors.Add("wplayer");
        AllActors.Add("mplayer");
        foreach (FileInfo fileInfo in actorFiles)
        {
            string actorFileName = fileInfo.Name;
            if (actorFileName.Contains(assetBundleExt))
            {
                string actorName = actorFileName.Substring(0, actorFileName.Length - assetBundleExt.Length);
                AllActors.Add(actorName);
                AllWeapons.Add(actorName);
                int index2 = actorName.IndexOf(WEAPON2PREFIX);          
                int index4 = actorName.IndexOf(WEAPON4PREFIX);          
                int index3 = actorName.IndexOf(WEAPON3PREFIX);
                int index5 = actorName.IndexOf(WEAPON5PREFIX);
                if(index2 == 0)
                {
                    weapon2Weapons.Add(actorName);
                }
                if(index4 == 0)
                {
                    weapon4Weapons.Add(actorName);
                }
                if(index3 == 0)
                {
                    weapon3Weapons.Add(actorName);
                }
                if(index5 == 0)
                {
                    weapon5Weapons.Add(actorName);
                }
                AllEffects.Add(actorName);
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
                BodyEquips.Add(equipName);
                int wIndex = equipName.IndexOf(WEQUIPTPREFIX);
                int mIndex = equipName.IndexOf(MEQUIPTPREFIX);
                if (wIndex == 0)
                {
                    wBodyEquips.Add(equipName);
                }

                if (mIndex == 0)
                {
                    mBodyEquips.Add(equipName);
                }
                int whIndex = equipName.IndexOf(WHEQUIPPREFIX);
                int wtIndex = equipName.IndexOf(WTEQUIPPREFIX);
                int wsIndex = equipName.IndexOf(WSEQUIPPREFIX);
                int wmIndex = equipName.IndexOf(WMEQUIPPREFIX);
                if (whIndex == 0)
                {
                    whBodyEquips.Add(equipName);
                }
                if (wtIndex == 0)
                {
                    wtBodyEquips.Add(equipName);
                }
                if (wsIndex == 0)
                {
                    wsBodyEquips.Add(equipName);
                }
                if (wmIndex == 0)
                {
                    wmBodyEquips.Add(equipName);
                }

                int mhIndex = equipName.IndexOf(MHEQUIPPREFIX);
                int msIndex = equipName.IndexOf(MSEQUIPPREFIX);
                int mtIndex = equipName.IndexOf(MTEQUIPPREFIX);
                int mmIndex = equipName.IndexOf(MMEQUIPPREFIX);

                if (mhIndex == 0)
                {
                    mhBodyEquips.Add(equipName);
                }
                if (msIndex == 0)
                {
                    msBodyEquips.Add(equipName);
                }
                if (mtIndex == 0)
                {
                    mtBodyEquips.Add(equipName);
                }
                if (mmIndex == 0)
                {
                    mmBodyEquips.Add(equipName);
                }
                HairEquips.Add(equipName);
                FaceEquips.Add(equipName);
                ShoulderEquips.Add(equipName);
            }
        }

        string skillFileExt = ".txt";
        if (!Directory.Exists(AssertBundleCreator.SkillAssetbundlePath))
            Directory.CreateDirectory(AssertBundleCreator.SkillAssetbundlePath);
        DirectoryInfo skillInfo = new DirectoryInfo(AssertBundleCreator.SkillAssetbundlePath);
        FileInfo[] skillFiles = skillInfo.GetFiles();
        SkillNames.Add("none");

        foreach (FileInfo fileInfo in skillFiles)
        {
            string skillFileName = fileInfo.Name;
            if (skillFileName.Contains(skillFileExt))
            {
                string SkillName = skillFileName.Substring(0, skillFileName.Length - skillFileExt.Length);
                SkillNames.Add(SkillName);
                int m1Index = SkillName.IndexOf(MPSKILL1PREFIX);
                int m2Index = SkillName.IndexOf(MPSKILL2PREFIX);
                int m3Index = SkillName.IndexOf(MPSKILL3PREFIX);
                int m4Index = SkillName.IndexOf(MPSKILL4PREFIX);
                if (m1Index == 0)
                {
                    m1SkillNames.Add(SkillName);
                }
                if (m2Index == 0)
                {
                    m2SkillNames.Add(SkillName);
                }
                if (m3Index == 0)
                {
                    m3SkillNames.Add(SkillName);
                }
                if (m4Index == 0)
                {
                    m4SkillNames.Add(SkillName);
                }
                int w1Index = SkillName.IndexOf(WPSKILL1PREFIX);
                int w2Index = SkillName.IndexOf(WPSKILL2PREFIX);
                int w3Index = SkillName.IndexOf(WPSKILL3PREFIX);
                int w4Index = SkillName.IndexOf(WPSKILL4PREFIX);
                if (w1Index == 0)
                {
                    w1SkillNames.Add(SkillName);
                }
                if (w2Index == 0)
                {
                    w2SkillNames.Add(SkillName);
                }
                if (w3Index == 0)
                {
                    w3SkillNames.Add(SkillName);
                }
                if (w4Index == 0)
                {
                    w4SkillNames.Add(SkillName);
                }
            }
        }
    }

    override public void OnInspectorGUI()
    {
        if (!inited)
        {
            Debug.LogWarning("begin init");
            Init();
            inited = true;
        }

        GUILayout.BeginHorizontal("box");
		GUILayout.Label("Object");
		actorIndex = EditorGUILayout.Popup(actorIndex, AllActors.ToArray());
		GUILayout.EndHorizontal ();

        List<string> bodyEquipList = getCurrentBodyEquipList();
        if (bodyEquipList.Count <= BodyEquipIndex)
        {
            BodyEquipIndex = 0;
        }
        GUILayout.BeginHorizontal("box");
		GUILayout.Label("Body");
        BodyEquipIndex = EditorGUILayout.Popup(BodyEquipIndex, bodyEquipList.ToArray());
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

        List<string> weaponList = getCurrentWeaponList();
        GUILayout.BeginHorizontal("box");
		GUILayout.Label("Right Weapon");
        if (weaponList.Count <= rightWeaponIndex)
        {
            rightWeaponIndex = 0;
        }
        rightWeaponIndex = EditorGUILayout.Popup(rightWeaponIndex, weaponList.ToArray());
		GUILayout.Label("Effect");
		rightWeaponEffectIndex = EditorGUILayout.Popup(rightWeaponEffectIndex, AllEffects.ToArray());
		GUILayout.EndHorizontal ();

        GUILayout.BeginHorizontal("box");
		GUILayout.Label("Left Weapon");
        if (weaponList.Count <= leftWeaponIndex)
        {
            leftWeaponIndex = 0;
        }
        leftWeaponIndex = EditorGUILayout.Popup(leftWeaponIndex, weaponList.ToArray());
		GUILayout.Label("Effect");
		leftWeaponEffectIndex = EditorGUILayout.Popup(leftWeaponEffectIndex, AllEffects.ToArray());
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal("box");
		GUILayout.Label("Animation");
        List<string> animateNames = getCurrentAnimateList();
        if (AnimNameIndex >= animateNames.Count)
        {
            EditorGUILayout.Popup(animateNames.Count - 1, animateNames.ToArray());
        }
        else
        {
            AnimNameIndex = EditorGUILayout.Popup(AnimNameIndex, animateNames.ToArray());
        }
		GUILayout.EndHorizontal ();

		if(GUILayout.Button("Reset"))
		{
			reset();
		}
		if(GUILayout.Button("OK"))
		{
			if(AllActors[actorIndex] != "none")
			{
				CreateGameObject();
			}
			else
			{
                actor.destroy();
			}
		}
        DisplaySkillGUI();
	}
	
	void CreateGameObject()
	{
        actor.destroy();
        actor.create(AllActors[actorIndex]);

		//换装

        actor.Object.changePart(0, getCurrentBodyEquipName() != "none" ? getCurrentBodyEquipName() : "");
        actor.Object.setBodyEffect(AllEffects[BodyEffectIndex] != "none" ? AllEffects[BodyEffectIndex] : "");
        actor.Object.changePart(1, HairEquips[HairEquipIndex] != "none" ? HairEquips[HairEquipIndex] : "");
        actor.Object.changePart(2, FaceEquips[FaceEquipIndex] != "none" ? FaceEquips[FaceEquipIndex] : "");
        actor.Object.changePart(3, ShoulderEquips[ShoulderEquipIndex] != "none" ? ShoulderEquips[ShoulderEquipIndex] : "");
        actor.Object.setShoulderEffect(AllEffects[ShoulderEffectIndex] != "none" ? AllEffects[ShoulderEffectIndex] : "");
        List<string> weaponList = getCurrentWeaponList();
        if (weaponList.Count <= rightWeaponIndex)
        {
            rightWeaponIndex = 0;
        }
        if (weaponList.Count <= leftWeaponIndex)
        {
            leftWeaponIndex = 0;
        }
        actor.Object.changeRightWeapon(weaponList[rightWeaponIndex] != "none" ? weaponList[rightWeaponIndex] : "");
        actor.Object.changeLeftWeapon(weaponList[leftWeaponIndex] != "none" ? weaponList[leftWeaponIndex] : "");
        actor.Object.SetRightWeaponEffect(AllEffects[rightWeaponEffectIndex] != "none" ? AllEffects[rightWeaponEffectIndex] : "");
        actor.Object.SetLeftWeaponEffect(AllEffects[leftWeaponEffectIndex] != "none" ? AllEffects[leftWeaponEffectIndex] : "");
        if (AnimNameIndex >= actor.AnimateNames.Count)
        {
            actor.Object.EnterSkill(true, "", true, 0.3f);
        }
        else
        {
            actor.Object.EnterSkill(true, getCurrentAnimateName() != "none" ? getCurrentAnimateName() : "", true, 0.3f);
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
        List<string> skillList = getCurrrentSkillList();
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Skill");
        if (skillList.Count <= skillIndex)
        {
            skillIndex = 0;
        }
        skillIndex = EditorGUILayout.Popup(skillIndex, skillList.ToArray());
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (!GUILayout.Button("AddSkill"))
        {
            if (skillList[skillIndex] != "none")
            {
                if (lastSkillIndex != skillIndex)
                {
                    string skillName = getCurrentSkillName();
                    actor.getSkill(skillName);
                    GFX.GfxSkill curSkill = actor.Skill;
                    if (curSkill != null)
                    {
                        stat = GUI_STATUS.CHECKING;
                        refreshSkillEffectList();
                        string curAnimateName = curSkill.getParameter("Animation");
                        mSkillAnimateNameIndex = getCurrentAnimateList().IndexOf(curAnimateName);
                        if (mSkillAnimateNameIndex ==-1)
                        {
                            mSkillAnimateNameIndex = 0;
                        }
                        mBreakTime = curSkill.getParameter("BreakTime");
                        mHitTime = curSkill.getParameter("HitTime");
                        mShakeTime = curSkill.getParameter("ShakeTime");
                        mRepeatEffect = curSkill.RepeatEffect;
                        mEnableRibbon = curSkill.EnableRibbon;
						lastEffectIndex = 0;
                        openHitGroundEffect = curSkill.isHitGroundEffectExist();
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

                    ShowHitGroundEffect();

                    GUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("SetSkill"))
                    {
                        if (getCurrentSkillAnimateName() == "none")
                        {
                            actor.Skill.setParameter("Animation", "");
                        }
                        else
                        {
                            actor.Skill.setParameter("Animation", getCurrentSkillAnimateName());
                        }
                        actor.Skill.setParameter("BreakTime", mBreakTime);
                        actor.Skill.setParameter("HitTime", mHitTime);
                        actor.Skill.setParameter("ShakeTime", mShakeTime);
                        actor.Skill.RepeatEffect = mRepeatEffect;
                        actor.Skill.EnableRibbon = mEnableRibbon;
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("box");
                    bool  changed = EditorGUILayout.Toggle("Play", this.isPlaying);
                    if (changed != this.isPlaying)
                    //if (GUILayout.Button(GetActionName()))
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

    void refreshSkillEffectList()
    {
        mEffects.Clear();
        mEffects.Add("none");
        for (int i = 0; i < actor.Skill.getSkillEffectCount(); i++)
        {
            mEffects.Add("Effect" + i);
        }

        if (mEffects.Count <= effectIndex)
        {
            effectIndex = 0;
        }
    }

    void Update()
    {
        if (actor.isUpdated())
        {
            this.Repaint();
          //  Debug.Log("Enter repaint");
        }
    }

    void showSkill()
    {
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Animation");
        if (mSkillAnimateNameIndex >= getCurrentAnimateList().Count)
        {
            EditorGUILayout.Popup(getCurrentAnimateList().Count - 1, getCurrentAnimateList().ToArray());
        }
        else
        {
            mSkillAnimateNameIndex = EditorGUILayout.Popup(mSkillAnimateNameIndex, getCurrentAnimateList().ToArray());
        }
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
        mRepeatEffect = EditorGUILayout.Toggle("RepeatEffect", mRepeatEffect);
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal("box");
        mEnableRibbon = EditorGUILayout.Toggle("EnableRibbon", mEnableRibbon);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        effectIndex = EditorGUILayout.Popup(effectIndex, mEffects.ToArray());
        GUILayout.EndHorizontal();
    }

    void showSkillEffect()
    {
        if (mEffects[effectIndex] != "none")
        {
            if (effectIndex != lastEffectIndex)
            {
                GFX.GfxSkillEffect curSkillEffect = actor.Skill.getSkillEffect(effectIndex - 1);
                mAtttachTime = curSkillEffect.AttachTime;
                string attachPoint = curSkillEffect.getParameter("AttachPoint");
                mLocatorNameIndex = actor.LocatorNames.IndexOf(attachPoint);
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
                mAttach = curSkillEffect.Attach;
            }
            lastEffectIndex = effectIndex;
            GUILayout.BeginHorizontal("box");
            mAtttachTime = EditorGUILayout.Slider("AttachTime",mAtttachTime,0f,10f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("AttachPoint");
            mLocatorNameIndex = EditorGUILayout.Popup(mLocatorNameIndex, actor.LocatorNames.ToArray());
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
            mAttach = EditorGUILayout.Toggle("Attach", mAttach);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("SetSkillEffect"))
            {
                GFX.GfxSkillEffect curSkillEffect = actor.Skill.getSkillEffect(effectIndex - 1);
                curSkillEffect.AttachTime = mAtttachTime;
                if (actor.LocatorNames[mLocatorNameIndex] == "none")
                {
                    curSkillEffect.setParameter("AttachPoint", "");
                }
                else
                {
                    curSkillEffect.setParameter("AttachPoint", actor.LocatorNames[mLocatorNameIndex]);
                }
                
                curSkillEffect.setParameter("EffectTemplateName", mEffectName);
                curSkillEffect.Attach = mAttach;
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
                string skillName = getCurrentSkillName();
                actor.destroySkill(skillName);
                removeSkill(skillName);
                string fullFilePath = AssertBundleCreator.SkillAssetbundlePath + skillName + ".txt";
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
        if (mSkillAnimateNameIndex >= getCurrentAnimateList().Count)
        {
            EditorGUILayout.Popup(getCurrentAnimateList().Count - 1, getCurrentAnimateList().ToArray());
        }
        else
        {
            mSkillAnimateNameIndex = EditorGUILayout.Popup(mSkillAnimateNameIndex, getCurrentAnimateList().ToArray());
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("SkillName");
        mSkillName = GUILayout.TextField(mSkillName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("OK"))
        {
            GFX.GfxSkill newSkill = actor.createSkill(mSkillName);
            if (newSkill != null)
            {
                if (getCurrentSkillAnimateName() == "none")
                {
                    newSkill.setAnimateName("");
                }
                else
                {
                    newSkill.setAnimateName(getCurrentSkillAnimateName());
                }
                addSkill(mSkillName);
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

    void addSkill(string skillName)
    {
        SkillNames.Add(skillName);
        if(skillName.IndexOf(MPSKILL1PREFIX) == 0)
        {
             m1SkillNames.Add(skillName);
        }

        if(skillName.IndexOf(MPSKILL2PREFIX) == 0)
        {
             m2SkillNames.Add(skillName);
        }

        if(skillName.IndexOf(MPSKILL3PREFIX) == 0)
        {
            m3SkillNames.Add(skillName);
        }

        if(skillName.IndexOf(MPSKILL4PREFIX) == 0)
        {
            m4SkillNames.Add(skillName);
        }

        if(skillName.IndexOf(WPSKILL1PREFIX) == 0)
        {
            w1SkillNames.Add(skillName);
        }

        if(skillName.IndexOf(WPSKILL2PREFIX) == 0)
        {
            w2SkillNames.Add(skillName);
        }

        if(skillName.IndexOf(WPSKILL3PREFIX) == 0)
        {
            w3SkillNames.Add(skillName);
        }

        if(skillName.IndexOf(WPSKILL4PREFIX) == 0)
        {
            w4SkillNames.Add(skillName);
        }
        refreshSkillEffectList();
    }

    void removeSkill(string skillName)
    {
        SkillNames.Remove(skillName);
        if (skillName.IndexOf(MPSKILL1PREFIX) == 0)
        {
            m1SkillNames.Remove(skillName);
        }

        if (skillName.IndexOf(MPSKILL2PREFIX) == 0)
        {
            m2SkillNames.Remove(skillName);
        }

        if (skillName.IndexOf(MPSKILL3PREFIX) == 0)
        {
            m3SkillNames.Remove(skillName);
        }

        if (skillName.IndexOf(MPSKILL4PREFIX) == 0)
        {
            m4SkillNames.Remove(skillName);
        }

        if (skillName.IndexOf(WPSKILL1PREFIX) == 0)
        {
            w1SkillNames.Remove(skillName);
        }

        if (skillName.IndexOf(WPSKILL2PREFIX) == 0)
        {
            w2SkillNames.Remove(skillName);
        }

        if (skillName.IndexOf(WPSKILL3PREFIX) == 0)
        {
            w3SkillNames.Remove(skillName);
        }

        if (skillName.IndexOf(WPSKILL4PREFIX) == 0)
        {
            w4SkillNames.Remove(skillName);
        }
    }

    void removeSkillEffect()
    {
        if (EditorUtility.DisplayDialog("remove SkillEffect", "Are you sure to remove current SkillEffect", "OK"))
        {
            if (effectIndex > 0)
            {
                actor.Skill.removeSkillEffect(effectIndex-1);
                refreshSkillEffectList();
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
        mLocatorNameIndex = EditorGUILayout.Popup(mLocatorNameIndex, actor.LocatorNames.ToArray());
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("OK"))
        {
            GFX.GfxSkillEffect newSkillEffect = actor.Skill.addSkillEffect();
            newSkillEffect.setParameter("EffectTemplateName", mEffectName);
            if (actor.LocatorNames[mLocatorNameIndex] == "none")
            {
                newSkillEffect.setParameter("AttachPoint", "");
            }
            else
            {
                newSkillEffect.setParameter("AttachPoint", actor.LocatorNames[mLocatorNameIndex]);
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

    void ShowHitGroundEffect()
    {
       // GUILayout.BeginHorizontal("box");

        GUILayout.BeginHorizontal("box");
        bool result = EditorGUILayout.Toggle("HitGroundSetting", openHitGroundEffect);
        GUILayout.EndHorizontal();
        if (result && result != openHitGroundEffect)
        {
            if (actor.Skill.HitGroundEffect != null)
            {
                GFX.GfxSkillHitGroundEffect curEffect = actor.Skill.HitGroundEffect;
                mHitGroundOffsetPos = curEffect.OffsetPos;
                mHitGroundOffsetRotation = new Vector4(curEffect.OffsetRotation.x, curEffect.OffsetRotation.y,
                                                       curEffect.OffsetRotation.z, curEffect.OffsetRotation.w);
                List<string> animList = getCurrentAnimateList();
                mHitGroundAnimateNameIndex = animList.IndexOf(curEffect.AnimateName);
                mHitGroundLocatorNameIndex = actor.LocatorNames.IndexOf(curEffect.AttachPoint);
                mHitGroundOffsetScale = curEffect.OffsetScale;
                mHitGroundAttachTime = curEffect.AttachTime;
                mHitGroundEffectName = curEffect.EffectName;
                mHitGroundAttach = curEffect.Attach;
                mHitGroundShakeTime = curEffect.ShakeTime;
            }
        }
        openHitGroundEffect = result;
        if (openHitGroundEffect)
        {
            GFX.GfxSkillHitGroundEffect curEffect = actor.Skill.HitGroundEffect;
            GUILayout.BeginHorizontal("box");
            mHitGroundOffsetPos = EditorGUILayout.Vector3Field("OffsetPos", mHitGroundOffsetPos);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            mHitGroundOffsetRotation = EditorGUILayout.Vector3Field("OffsetRotation", mHitGroundOffsetRotation);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            mHitGroundOffsetScale = EditorGUILayout.Vector3Field("OffsetScale", mHitGroundOffsetScale);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            mHitGroundAttachTime = EditorGUILayout.Slider("AttachTime", mHitGroundAttachTime, 0f, 10f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("AnimateName");
            if (mHitGroundAnimateNameIndex >= getCurrentAnimateList().Count)
            {
                EditorGUILayout.Popup(getCurrentAnimateList().Count - 1, getCurrentAnimateList().ToArray());
            }
            else
            {
                mHitGroundAnimateNameIndex = EditorGUILayout.Popup(mHitGroundAnimateNameIndex, getCurrentAnimateList().ToArray());
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            mHitGroundEffectName = EditorGUILayout.TextField("EffectTemplateName", mHitGroundEffectName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            mHitGroundAttach = EditorGUILayout.Toggle("Attach", mHitGroundAttach);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("AttachPoint");
            mHitGroundLocatorNameIndex = EditorGUILayout.Popup(mHitGroundLocatorNameIndex, actor.LocatorNames.ToArray());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("box");
            mHitGroundShakeTime = EditorGUILayout.Slider("ShakeTime", mHitGroundShakeTime, 0f, 10f);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("SetHitGroundEffect"))
            {
                GFX.GfxSkillHitGroundEffect newEffect = new GFX.GfxSkillHitGroundEffect();
                newEffect.OffsetScale                 = mHitGroundOffsetScale;
                newEffect.OffsetPos                   = mHitGroundOffsetPos;
                newEffect.OffsetRotation = new Quaternion(mHitGroundOffsetRotation.x, 
                                                          mHitGroundOffsetRotation.y, 
                                                          mHitGroundOffsetRotation.z, 
                                                          mHitGroundOffsetRotation.w);
                newEffect.AttachTime = mHitGroundAttachTime;
                newEffect.Attach = mHitGroundAttach;
                if (actor.LocatorNames[mHitGroundLocatorNameIndex] == "none")
                {
                    newEffect.AttachPoint = "";
                }
                else
                {
                    newEffect.AttachPoint = actor.LocatorNames[mHitGroundLocatorNameIndex];
                }
                newEffect.EffectName = mHitGroundEffectName;
                newEffect.ShakeTime  = mHitGroundShakeTime;
                List<string> animateList = getCurrentAnimateList();
                if (animateList.Count <= mHitGroundAnimateNameIndex)
                {
                    newEffect.AnimateName = "";
                }
                else
                {
                    newEffect.AnimateName = animateList[mHitGroundAnimateNameIndex];
                }
                actor.Skill.HitGroundEffect = newEffect;
                
            }
            GUILayout.EndHorizontal();
        }
        //GUILayout.EndHorizontal();
    }

    void Save()
    {
        if (actor.Skill != null)
        {
            string skillFileName = getCurrentSkillName();
            string fullFilePath = AssertBundleCreator.SkillAssetbundlePath + skillFileName + ".txt";
            StreamWriter skillWrite = new StreamWriter(fullFilePath);
            skillWrite.Flush();
            skillWrite.WriteLine("skill " + skillFileName);
            skillWrite.WriteLine("{");
            skillWrite.WriteLine("\t\tAnimation " + actor.Skill.getAnimateName());
            skillWrite.WriteLine("\t\tBreakTime " + actor.Skill.getParameter("BreakTime"));
            skillWrite.WriteLine("\t\tHitTime " + actor.Skill.getParameter("HitTime"));
            skillWrite.WriteLine("\t\tShakeTime " + actor.Skill.getParameter("ShakeTime"));
            skillWrite.WriteLine("\t\tRepeatEffect " + actor.Skill.getParameter("RepeatEffect"));
            skillWrite.WriteLine("\t\tEnableRibbon " + actor.Skill.getParameter("EnableRibbon"));
            for (int i = 0; i < actor.Skill.getSkillEffectCount(); i++)
            {
                skillWrite.WriteLine("\t\tAnimEffect");
                GFX.GfxSkillEffect skillEffect = actor.Skill.getSkillEffect(i);
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

            if (actor.Skill.isHitGroundEffectExist())
            {
                skillWrite.WriteLine("\t\tHitGroundEffect");
                skillWrite.WriteLine("\t\t{");
                skillWrite.WriteLine("\t\t\tAttachTime " + actor.Skill.HitGroundEffect.getParameter("AttachTime"));
                skillWrite.WriteLine("\t\t\tShakeTime " + actor.Skill.HitGroundEffect.getParameter("ShakeTime"));
                skillWrite.WriteLine("\t\t\tAnimation " + actor.Skill.HitGroundEffect.getParameter("Animation"));
                skillWrite.WriteLine("\t\t\tAttachPoint " + actor.Skill.HitGroundEffect.getParameter("AttachPoint"));
                skillWrite.WriteLine("\t\t\tEffectTemplateName " + actor.Skill.HitGroundEffect.getParameter("EffectTemplateName"));
                skillWrite.WriteLine("\t\t\tOffsetPos " + actor.Skill.HitGroundEffect.getParameter("OffsetPos"));
                skillWrite.WriteLine("\t\t\tOffsetRotation " + actor.Skill.HitGroundEffect.getParameter("OffsetRotation"));
                skillWrite.WriteLine("\t\t\tOffsetScale " + actor.Skill.HitGroundEffect.getParameter("OffsetScale"));
                skillWrite.WriteLine("\t\t\tAttach " + actor.Skill.HitGroundEffect.getParameter("Attach"));
                skillWrite.WriteLine("\t\t}");
            }

            skillWrite.WriteLine("}");

            skillWrite.Close();
            EditorUtility.DisplayDialog("Save Skill File","Success!","Ok");
        }
        else
        {
            Debug.LogError("skill instance is none");
        }
    }

    public void OnSceneGUI()
    {
        if (actor.isUpdated())
        {
            this.Repaint();
        }
    }
}