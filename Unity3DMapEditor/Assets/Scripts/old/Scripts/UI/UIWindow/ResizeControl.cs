using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ResizeControl : MonoBehaviour {

    public int width = 100;
    public int height = 100;

    public UIButton ltWin;
    public UIButton lbWin;
    public UIButton rtWin;
    public UIButton rbWin;

    public UIButton lWin;
    public UIButton rWin;
    public UIButton tWin;
    public UIButton bWin;

    public UIButton centerWin;

    void Awake()
    {
        InitialWins();

        ReCalc();
    }

	// Use this for initialization
	void Start () {

        ReCalc();
	}

    int lastWidth;
    int lastHeight;

    void UpdateSize()
    {
        if (lastHeight != height || lastWidth != width)
        {
            ReCalc();

            lastWidth = width;
            lastHeight = height;
        }
    }

    void ReCalc()
    {
        // rePostion [4/19/2012 Ivan]
        RePosition();
        // resize [4/19/2012 Ivan]
        ReSize();
    }

    private void ReSize()
    {
        lWin.SetSize(lWin.width, height - ltWin.height - lbWin.height);
        //lWin.SetUVsFromPixelCoords(new Rect(0, 0, lWin.width, height - ltWin.height - lbWin.height));
        rWin.SetSize(lWin.width, height - rtWin.height - rbWin.height);
        //rWin.SetUVsFromPixelCoords(new Rect(0, 0, lWin.width, height - rtWin.height - rbWin.height));
        tWin.SetSize(width - ltWin.width - rtWin.width, tWin.height);
        //tWin.SetUVsFromPixelCoords(new Rect(0, 0, width - ltWin.width - rtWin.width, tWin.height));
        bWin.SetSize(width - lbWin.width - rbWin.width, bWin.height);
        //bWin.SetUVsFromPixelCoords(new Rect(0, 0, width - lbWin.width - rbWin.width, bWin.height));
        centerWin.SetSize(width - lWin.width - rWin.width, height - tWin.height - bWin.height);
    }

    private void InitialWins()
    {
        ltWin.Anchor = lbWin.Anchor = rtWin.Anchor = rbWin.Anchor = lWin.Anchor
            = rWin.Anchor = tWin.Anchor = bWin.Anchor = centerWin.Anchor = SpriteRoot.ANCHOR_METHOD.UPPER_LEFT;
        ltWin.controlIsEnabled = lbWin.controlIsEnabled = rtWin.controlIsEnabled = rbWin.controlIsEnabled = lWin.controlIsEnabled
            = rWin.controlIsEnabled = tWin.controlIsEnabled = bWin.controlIsEnabled = centerWin.controlIsEnabled = false;

        ltWin.pixelPerfect = lbWin.pixelPerfect = rtWin.pixelPerfect = rbWin.pixelPerfect = true;
        ltWin.autoResize = lbWin.autoResize = rtWin.autoResize = rbWin.autoResize = true;

        lWin.pixelPerfect = rWin.pixelPerfect = tWin.pixelPerfect = bWin.pixelPerfect = centerWin.pixelPerfect = false;
        lWin.autoResize = rWin.autoResize = tWin.autoResize = bWin.autoResize = centerWin.autoResize = false;

        lWin.gameObject.renderer.sharedMaterial.mainTexture.wrapMode = rWin.gameObject.renderer.sharedMaterial.mainTexture.wrapMode =
            tWin.gameObject.renderer.sharedMaterial.mainTexture.wrapMode = bWin.gameObject.renderer.sharedMaterial.mainTexture.wrapMode =
            centerWin.gameObject.renderer.sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Clamp;
    }

    void RePosition()
    {
        lbWin.transform.localPosition = new Vector3(0, lbWin.height, 0);
        rbWin.transform.localPosition = new Vector3(width - rtWin.width, rbWin.height, 0);
        ltWin.transform.localPosition = new Vector3(0, height, 0);
        rtWin.transform.localPosition = new Vector3(width - rtWin.width, height, 0);

        lWin.transform.localPosition = new Vector3(0, height-ltWin.height, 0);
        tWin.transform.localPosition = new Vector3(ltWin.width, height, 0);
        rWin.transform.localPosition = new Vector3(width - rWin.width, height - rtWin.width, 0);
        bWin.transform.localPosition = new Vector3(lbWin.width, bWin.height, 0);

        centerWin.transform.localPosition = new Vector3(lWin.width, height - tWin.height, 0);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateSize();
	}
}
