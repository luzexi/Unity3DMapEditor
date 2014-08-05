using UnityEngine;
using System.Collections;
//该脚本挂接到地形对象，用于设置场景的雾、环境光等参数  [2/3/2012 ZZY]
public class EnviromentSetting : MonoBehaviour
{
    public bool UseFog = true;
    public FogMode fogModel = FogMode.ExponentialSquared;
    public float FogDensity = 0.03f;
    public float FogStart = 20.0f;
    public float FogEnd = 50.0f;
    public Color FogColor = new Color(0.8f,0.8f,1.0f);
    public Color AmbientLight = 0.3f * Color.white;
	// Use this for initialization
	void Start () {
        setAmbient();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void setAmbient()
    {
        RenderSettings.fog = UseFog;
        RenderSettings.fogMode = fogModel;
        RenderSettings.fogDensity = FogDensity;
        RenderSettings.fogStartDistance = FogStart;
        RenderSettings.fogEndDistance = FogEnd;
        RenderSettings.fogColor = FogColor;
        RenderSettings.ambientLight = AmbientLight;
        //摄像机的背景颜色设为雾的颜色
        if(Camera.mainCamera != null)
        {
            Camera.mainCamera.backgroundColor = RenderSettings.fogColor;
            Camera.mainCamera.farClipPlane = FogEnd;//摄像机的远裁剪面等于雾的远距离
        }
    }
}
