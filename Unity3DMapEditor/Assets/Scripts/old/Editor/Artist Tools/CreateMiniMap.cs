using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
class CaptureMiniMap
{
    [MenuItem("SG Tools/Mini Map")]
    static void Execute()
    {
			if(Terrain.activeTerrain == null) return;
			TerrainData terrainData = Terrain.activeTerrain.terrainData  ;
			Vector3 cameraPos = new Vector3(terrainData.size.x*0.5f,  500,terrainData.size.z*0.5f);
			Camera.mainCamera.orthographic = true;
			Camera.mainCamera.far = 1000;
			Camera.mainCamera.farClipPlane = 1000;
			Camera.mainCamera.orthographicSize = terrainData.size.x*0.5f;
			Camera.mainCamera.transform.position = cameraPos;
			Camera.mainCamera.transform.rotation = Quaternion.LookRotation(new Vector3(0,-1,0),new Vector3(0,0,1));
			Camera.mainCamera.aspect = 1.0f;
			RenderSettings.fog = false;
			string sceneName = EditorApplication.currentScene;
			string[] path = sceneName.Split('.');
			
			Application.CaptureScreenshot(path[0]+".png");
    }
}