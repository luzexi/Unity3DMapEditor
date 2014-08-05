using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FakeObjectScript : MonoBehaviour {

	// Use this for initialization
    public GFX.GfxObject FakeObject;
    GameObject mFakeCameraObj;
    Vector3 mFakeObjectPos = Vector3.zero;
    const int FakeTextureHeight = 256;
    RenderTexture mFakeRenderTexture;
    float ctanFOV;
    UISelfEquip mSelfEquipt = null;
    SkinnedMeshRenderer mRender = null; //当前需要渲染的mesh 人物或者npc add by ss
	void Start () {

        if (mFakeCameraObj == null)//创建摄像机
        {
            mFakeCameraObj = new GameObject("FakeCameraObj");
			Camera theCamera = mFakeCameraObj.AddComponent<Camera>();
            theCamera.fov = 45.0f;
            theCamera.near = 0.1f;
            theCamera.far = 20.0f;
            theCamera.backgroundColor = new Color(0, 0, 0, 0);
            theCamera.cullingMask = LayerManager.FakeObjectMask;
            mFakeCameraObj.transform.rotation = Quaternion.LookRotation( new Vector3(0, 0, -1 ) );
            ctanFOV = 1/(Mathf.Tan(mFakeCameraObj.camera.fov / 2.0f));
        } 
        if (mFakeRenderTexture == null && gameObject.renderer != null)//创建渲染纹理
        { 
            int texWidth =(int) ((gameObject.renderer.bounds.size.x/gameObject.renderer.bounds.size.y )* (float)FakeTextureHeight);
            mFakeRenderTexture = new RenderTexture(texWidth, FakeTextureHeight, 1, RenderTextureFormat.ARGB32);
            mFakeCameraObj.camera.aspect = ((float)texWidth) / (float)FakeTextureHeight;
			mFakeCameraObj.camera.enabled = false;
        }
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_UPDATE_PET_PAGE, OnEvent);
	}

	// Update is called once per frame
	bool needRender = false;
	void Update () 
    {
		if(needRender)
		{
			RenderObject();
		}
	}

    void OnEvent(GAME_EVENT_ID eventid,List<string> vParams)
    {
        switch (eventid)
        {
            case GAME_EVENT_ID.GE_UPDATE_PET_PAGE:
                {
					FakeObject = null;
                    mRender = null;
                }
                break;
        }
    }

	void OnEnable()
	{
		needRender = true;
	}
	void OnDisable()
	{
		needRender = false;
	}
    void RenderObject()
    {
        UpdateObject();
        if (FakeObject == null) return;
        Renderer render = FakeObject.getGameObject().renderer == null ? mRender : FakeObject.getGameObject().renderer; //设置当前renderer add by ss
        if (FakeObject == null || FakeObject.getGameObject() == null || render == null || mFakeCameraObj == null) return;
		
		Quaternion curRot = FakeObject.getGameObject().transform.localRotation;
		FakeObject.getGameObject().transform.localRotation = Quaternion.identity;
		mFakeCameraObj.SetActiveRecursively (true);//打开摄像机
        float objectHeight = render.bounds.size.y;//fake object 高度  //modified by ss
		GFX.GfxUtility.setGameObjectLayer(FakeObject.getGameObject(), LayerManager.FakeObjectLayer);
        FakeObject.SetVisible(true);//设置对象可见
        Vector3 fakeObjCenter = mFakeObjectPos;
        float dis = ctanFOV * objectHeight * 0.5f + render.bounds.size.z * 0.5f + 0.6f;
        fakeObjCenter.y += render.bounds.size.y * 0.5f;
        Vector3 camePos = fakeObjCenter - mFakeCameraObj.transform.forward * dis;//-mFakeCameraObj.transform.forward * dis + fakeObjCenter;//求摄像机位置
        mFakeCameraObj.transform.position = camePos;
        mFakeCameraObj.camera.targetTexture = mFakeRenderTexture;
		FakeObject.getGameObject().transform.localRotation = curRot;//恢复朝向
        mFakeCameraObj.camera.Render();

        //把render texture 赋予材质
        if(renderer.material != null)
        {
            renderer.material.mainTexture = mFakeRenderTexture;
        }
       FakeObject.SetVisible(false);//不可见
       mFakeCameraObj.SetActiveRecursively (false);//关闭摄像机
    }
    void UpdateObject()
    {
        if (gameObject.name == "RoleTipShowPlayer")//是角色面板
        {
            if (CObjectManager.Instance.getPlayerMySelf().getAvatar() != null &&
               CObjectManager.Instance.getPlayerMySelf().getAvatar().GetRenderInterface() != null)
            {
                FakeObject = CObjectManager.Instance.getPlayerMySelf().getAvatar().GetRenderInterface();
                FakeObject.GetLocator("FootEffectLocator", ref mFakeObjectPos);
            }
        }
        else if (gameObject.name == "PetModelShow")
        {
            if (mSelfEquipt == null)
            {
                GameObject roleTipWindow = UIWindowMng.Instance.GetWindowGo("RoleTipWindow");
                mSelfEquipt = roleTipWindow.GetComponent<UISelfEquip>();
            }

            SDATA_PET pet = CDataPool.Instance.Pet_GetValidPet(mSelfEquipt.ActivePet);//CDataPool.Instance.Pet_GetPet(mSelfEquipt.ActivePet);
            if (pet != null && pet.FakeObject != null)
            {
                //由于宠物的skinmeshrenderer是在主gameobject下面，所以需要取子节点的meshrender add by ss
                FakeObject = pet.FakeObject.GetRenderInterface();
                SkinnedMeshRenderer render = FakeObject.getGameObject().GetComponentInChildren<SkinnedMeshRenderer>();
				if (mRender != render)
                {
                    mRender = render;
                    FakeObject.GetLocator("FootEffectLocator", ref mFakeObjectPos);
                }
            }
        }
        //         else if ()//其他fake window
        //         {
        //         }
        //
    }
    void OnBecameVisible()
    {
        if (mFakeRenderTexture != null )
        {
            mFakeRenderTexture.Create();
        }
        
        
    }
    void OnBecameInvisible()
    {
        if (mFakeRenderTexture != null )
        {
            mFakeRenderTexture.Release();
        }
       
    }
}
