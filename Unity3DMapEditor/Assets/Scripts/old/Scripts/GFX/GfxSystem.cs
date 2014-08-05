using UnityEngine;
namespace GFX
{
	public class GfxSystem
	{
		
        static readonly GfxSystem sInstance = new GfxSystem();//唯一的实例

        static public GfxSystem Instance
        {
            get
            {
                return sInstance;
            }
        }
		public  void Tick()
		{
			GFX.GFXObjectManager.Instance.Update();
		}

	}
}