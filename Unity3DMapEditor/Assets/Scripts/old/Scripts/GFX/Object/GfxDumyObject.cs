using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssetBundleManager;

namespace GFX
{
	public class GfxDummyObject:GfxObject
	{
        public GfxDummyObject()
        {
            mGameObject = new GameObject("DummyObject");
            mGameObjectOutOfDate = false;
        }
        public override GFXObjectType getObjectType() { return GFXObjectType.DUMY; }
        public override void update()
        {
            base.update();
        }
	}

}

