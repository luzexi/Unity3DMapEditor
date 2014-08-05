using System;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    protected SpriteRoot ezGuiRoot;
    protected UIButton ezButton;
    protected bool     colorInit = false;
    protected Color    originColor;
    Texture2D orgTexture;
    void Awake()
    {
        InitialControls();
        Initial();
    }

    public void InitialControls()
    {
        ezGuiRoot = gameObject.GetComponent<SpriteRoot>();
        ezButton = ezGuiRoot as UIButton;
        orgTexture = gameObject.renderer.material.mainTexture as Texture2D;
    }

    public virtual void Initial()
    {
    }

    public void SetIcon(string iconName)
    {
        if (ezGuiRoot == null)
            return;

        if (iconName == "")
        {
            ezGuiRoot.SetTexture(orgTexture);
        }
        else
        {
            Texture2D icon = IconManager.Instance.GetIcon(iconName);
            ezGuiRoot.SetTexture(icon);
            ezGuiRoot.SetUVs(new Rect(0, 0, 1, 1));
        }
    }
    public void SetMaskTexture(string maskTex)
    {
        Texture2D maskTexture = null;
        if (!string.IsNullOrEmpty(maskTex))
            maskTexture = IconManager.Instance.GetIcon(maskTex);
        
        Material mat = ezGuiRoot.renderer.material;
        if (mat.HasProperty("_OccluderTex"))
        {
            mat.SetTexture("_OccluderTex", maskTexture);
            mat.SetFloat("_Occluder", maskTexture == null ? 0.0f : 1.0f);
        }
        ezGuiRoot.SetMaterial(mat);

    }
    //调用此接口 表明 明确这个材质使用的是可以改变color的shader
    public void SetDisableColor()
    {
        if (!colorInit)
        {
            colorInit = true;
            originColor.a = gameObject.renderer.material.color.a;
            originColor.r = gameObject.renderer.material.color.r;
            originColor.g = gameObject.renderer.material.color.g;
            originColor.b = gameObject.renderer.material.color.b;
        }
        gameObject.renderer.material.color = originColor / 2;
       
    }
    //调用此接口 表明 明确这个材质使用的是可以改变color的shader
    public void SetEnableColor()
    {
        if (!colorInit)
        {
            colorInit = true;
            originColor.a = gameObject.renderer.material.color.a;
            originColor.r = gameObject.renderer.material.color.r;
            originColor.g = gameObject.renderer.material.color.g;
            originColor.b = gameObject.renderer.material.color.b;
        }
        gameObject.renderer.material.color = originColor * 2;
    }
    public void setTransColor()
    {
        if (!colorInit)
        {
            colorInit = true;
            originColor.a = gameObject.renderer.material.color.a;
            originColor.r = gameObject.renderer.material.color.r;
            originColor.g = gameObject.renderer.material.color.g;
            originColor.b = gameObject.renderer.material.color.b;
        }
        Color transColor = originColor;
        transColor.a /= 2;
        gameObject.renderer.material.color = transColor;
    }
    public void resetTransColor()
    {
        if (!colorInit)
        {
            colorInit = true;
            originColor.a = gameObject.renderer.material.color.a;
            originColor.r = gameObject.renderer.material.color.r;
            originColor.g = gameObject.renderer.material.color.g;
            originColor.b = gameObject.renderer.material.color.b;
        }

        gameObject.renderer.material.color = originColor;
    }

    public Texture GetIcon()
    {
        return gameObject.renderer.material.mainTexture;
    }

    public void Show()
    {
        gameObject.SetActiveRecursively(true);
    }

    public void Hide()
    {
        gameObject.SetActiveRecursively(false);
    }
}
