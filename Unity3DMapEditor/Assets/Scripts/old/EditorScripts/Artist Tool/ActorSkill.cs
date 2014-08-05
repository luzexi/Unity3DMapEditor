using UnityEngine;
using System.Collections.Generic;
using System.IO;
public class ActorSkill : MonoBehaviour
{
    const string WTANIMATEPREFIX = "W_t_";
    const string WHANIMATEPREFIX = "W_h_";
    const string WMANIMATEPREFIX = "W_m_";
    const string WSANIMATEPREFIX = "W_s_";

    const string MTANIMATEPREFIX = "M_t_";
    const string MHANIMATEPREFIX = "M_h_";
    const string MMANIMATEPREFIX = "M_m_";
    const string MSANIMATEPREFIX = "M_s_";

    GFX.GfxActor mObject = null;
    GFX.GfxSkill mSkill = null;
    List<string> mAnimateNames = new List<string>();
    List<string> mLocatorNames  = new List<string>();
    
    List<string> whAnimates = new List<string>();
    List<string> mhAnimates = new List<string>();
    List<string> wtAnimates = new List<string>();
    List<string> mtAnimates = new List<string>();
    List<string> msAnimates = new List<string>();
    List<string> wsAnimates = new List<string>();
    List<string> wmAnimates = new List<string>();
    List<string> mmAnimates = new List<string>();

    bool mUpdated = false;
    static bool isInited = false;
    public GFX.GfxActor Object
    {
        get { return mObject; }
        set { mObject = value; }
    }
    public bool isInit()
    {
        return isInited;
    }
    public void setInit()
    {
        isInited = true;
    }

    public GFX.GfxSkill Skill
    {
        get { return mSkill; }
        set { mSkill = value; }
    }

    public List<string> AnimateNames
    {
        get { return mAnimateNames; }
    }

    public List<string> LocatorNames
    {
        get { return mLocatorNames; }
    }

    public List<string> WTAnimateNames
    {
        get { return wtAnimates; }
    }

    public List<string> WHAnimateNames
    {
        get { return whAnimates; }
    }

    public List<string> WMAnimateNames
    {
        get { return wmAnimates; }
    }

    public List<string> WSAnimateNames
    {
        get { return wsAnimates; }
    }

    public List<string> MTAnimateNames
    {
        get { return mtAnimates; }
    }

    public List<string> MHAnimateNames
    {
        get { return mhAnimates; }
    }

    public List<string> MSAnimateNames
    {
        get { return msAnimates; }
    }

    public List<string> MMAnimateNames
    {
        get { return mmAnimates; }
    }

    public bool isUpdated()
    {
        bool updated = mUpdated;
        mUpdated = false;
        return updated;
    }

    void Start()
    {
        mAnimateNames.Add("none");
		whAnimates.Add("none");
        mhAnimates.Add("none");
        wtAnimates.Add("none");
        mtAnimates.Add("none");
        msAnimates.Add("none");
        wsAnimates.Add("none");
        wmAnimates.Add("none");
        mmAnimates.Add("none");
        mLocatorNames.Add("none");
    }

    void OnDrawGizmos()
    {

    }

    void Update()
    {
        bool isUpdated = false;
        if (mObject != null)
        {
            mAnimateNames = mObject.getAnimationName();
            mAnimateNames.Sort();
            mAnimateNames.Insert(0, "none");
            GameObject go = mObject.getGameObject();
            LocatorNames.Clear();
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
            LocatorNames.Sort();
            LocatorNames.Insert(0, "none");

            whAnimates.Clear();
            mhAnimates.Clear();
            wtAnimates.Clear();
            mtAnimates.Clear();
            msAnimates.Clear();
            wsAnimates.Clear();
            wmAnimates.Clear();
            mmAnimates.Clear();

            foreach (string animateName in mAnimateNames)
            {
                if (animateName.IndexOf(WTANIMATEPREFIX) == 0)
                {
                    wtAnimates.Add(animateName);
                }
                if (animateName.IndexOf(WHANIMATEPREFIX) == 0)
                {
                    whAnimates.Add(animateName);
                }
                if (animateName.IndexOf(WMANIMATEPREFIX) == 0)
                {
                    wmAnimates.Add(animateName);
                }
                if (animateName.IndexOf(WSANIMATEPREFIX) == 0)
                {
                    wsAnimates.Add(animateName);
                }
                if (animateName.IndexOf(MTANIMATEPREFIX) == 0)
                {
                    mtAnimates.Add(animateName);
                }
                if (animateName.IndexOf(MHANIMATEPREFIX) == 0)
                {
                    mhAnimates.Add(animateName);
                }
                if (animateName.IndexOf(MMANIMATEPREFIX) == 0)
                {
                    mmAnimates.Add(animateName);
                }
                if (animateName.IndexOf(MSANIMATEPREFIX) == 0)
                {
                    msAnimates.Add(animateName);
                }
            }

            whAnimates.Sort();
            mhAnimates.Sort();
            wtAnimates.Sort();
            mtAnimates.Sort();
            msAnimates.Sort();
            wsAnimates.Sort();
            wmAnimates.Sort();
            mmAnimates.Sort();

            whAnimates.Insert(0, "none");
            mhAnimates.Insert(0, "none");
            wtAnimates.Insert(0, "none");
            mtAnimates.Insert(0, "none");
            msAnimates.Insert(0, "none");
            wsAnimates.Insert(0, "none");
            wmAnimates.Insert(0, "none");
            mmAnimates.Insert(0, "none");

            isUpdated = true;
        }

        if (isUpdated)
        {
            mUpdated = true;
        }
        GFX.GfxSystem.Instance.Tick();
    }

    public GFX.GfxSkill createSkill(string skillName)
    {
        mSkill = GFX.GfxSkillManager.Instance.createSkillTemplate(skillName);
        return mSkill;
    }

    public void create(string actorName)
    {
        mObject = (GFX.GfxActor)GFX.GFXObjectManager.Instance.createObject(actorName, GFX.GFXObjectType.ACTOR);
    }

    public GFX.GfxSkill getSkill(string skillName)
    {
        mSkill = GFX.GfxSkillManager.Instance.createAndParseSkillTemplate(skillName);
        return mSkill;
    }

    public void destroy()
    {
        GFX.GFXObjectManager.Instance.DestroyObject(mObject);
    }

    public void destroySkill(string skillName)
    {
        GFX.GfxSkillManager.Instance.destroySkillTemplate(skillName);
    }

    public void play()
    {
        if (mSkill != null)
        {
            mObject.Currentskill = mSkill;
            mObject.EnterSkill(false, mSkill.getSkillName(), true, 0.3f);
        }
    }

    public void stop()
    {
        mObject.Currentskill = null;
    }
}