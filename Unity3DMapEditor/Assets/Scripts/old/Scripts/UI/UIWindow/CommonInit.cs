using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommonInit : MonoBehaviour {

    Vector3 oldPos = Vector3.zero;
    void Awake()
    {
        oldPos = gameObject.transform.localPosition;
        gameObject.transform.localPosition = Vector3.zero;
        CEventSystem.Instance.RegisterEventHandle(GAME_EVENT_ID.GE_APPLICATION_INITED, FirstEnter);
    }

    public void FirstEnter(GAME_EVENT_ID gAME_EVENT_ID, List<string> vParam)
    {
        gameObject.transform.localPosition = oldPos;
		EZScreenPlacement ScreenPlacement = gameObject.GetComponent<EZScreenPlacement>();
		if(ScreenPlacement != null)
			ScreenPlacement.SetCamera(UISystem.Instance.UiCamrea);
    }
}
