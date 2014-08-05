using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GFX
{
    public  enum GFXObjectType { NONE, ACTOR, EFFECT, STATICOBJECT,DUMY }
	public class GFXObjectManager {
        static readonly GFXObjectManager sInstance = new GFXObjectManager();

        static public GFXObjectManager Instance
        {
            get
            {
                return sInstance;
            }
        }
	List<GfxObject> mObjectList = new List<GfxObject>();
    List<GfxObject>.Enumerator mCurEnumertor;
	public GfxObject createObject(string objName, GFXObjectType objType)
	{
		GfxObject gfxObject=null;
        switch (objType)
        {
            case GFXObjectType.ACTOR:
                {
                    gfxObject = new GfxActor(objName);
                }
                break;
            case GFXObjectType.EFFECT:
                {
                    gfxObject = new GfxEffect(objName);
                }
                break;
            case GFXObjectType.DUMY:
                {
                 gfxObject = new GfxDummyObject();
                }
                break;
        }
        mObjectList.Add(gfxObject);
        mCurEnumertor = mObjectList.GetEnumerator();
		return gfxObject;
	}
	public void DestroyObject(GfxObject gfxObject)
	{
        if (gfxObject != null)
        {
            gfxObject.destroy();
            mObjectList.Remove(gfxObject);
            mCurEnumertor = mObjectList.GetEnumerator();
        }
	}

	public void Update () 
    {
		//更新objects
        mCurEnumertor = mObjectList.GetEnumerator();
        while (mCurEnumertor.MoveNext())
        {
            mCurEnumertor.Current.update();
        }
	}
}
}

