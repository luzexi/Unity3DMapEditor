using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateUIManager : MonoBehaviour {

	// 由于摄像机动态加载的时候名字会改变，所以需要重新设置ui需要的摄像机 [1/12/2012 Ivan]
	void Start () {
        Camera uiCamera = null;
        foreach (Camera camera in Camera.allCameras)
	    {
            if (camera.cullingMask == LayerManager.UIMask)
            {
                uiCamera = camera;
                break;
            }
	    }
        if (!uiCamera)
            return;
        GameObject goUi = GameObject.Find("UIManager");
        if(goUi != null)
        {
            UIManager uiManager = goUi.GetComponent<UIManager>();
            uiManager.rayCamera = uiCamera;

            EZCameraSettings[] ezCamera = new EZCameraSettings[1];
            ezCamera[0] = new EZCameraSettings();
            ezCamera[0].camera = uiCamera;
            ezCamera[0].mask = 1 << 10;
            uiManager.uiCameras = ezCamera;
        }

        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_APPLICATION_INITED, OnUIStart);
	}

    public void OnUIStart(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        //this.gameObject.SetActiveRecursively(true);
    }
}
