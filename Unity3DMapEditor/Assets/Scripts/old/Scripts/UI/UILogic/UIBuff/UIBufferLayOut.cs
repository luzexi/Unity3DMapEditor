using UnityEngine;
public class UIBufferLayOut : MonoBehaviour
{
    public string AlignmentWindow = "MiniMapWindow";
    void Awake()
    {
        GameObject miniMap = UIWindowMng.Instance.GetWindowGo(AlignmentWindow);
        GetComponent<EZScreenPlacement>().relativeObject = miniMap.transform;
        gameObject.SetActiveRecursively(true);
    }
}