using UnityEngine;
using System.Collections;

public class GameCameraScript : MonoBehaviour
{

    // Use this for initialization
    public float CameraSpeed = 1.0f;
    public float WheelSpeed = 10.0f;
    public float cameraRotSpeed = 3.0f;
    public bool useGameCamera = false;
    bool isMidButtonDown = false;
    bool isAltDown = false;
    bool isLeftButtonDown = false;
    GFX.SceneCamera mSceneCamera = null;
    Vector3 mLastHitPos;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.mainCamera == null) return;
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        float sw = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetMouseButtonDown(0)) isLeftButtonDown = true;
        if (Input.GetMouseButtonUp(0)) isLeftButtonDown = false;
        if (Input.GetMouseButtonDown(2)) isMidButtonDown = true;
        if (Input.GetMouseButtonUp(2)) isMidButtonDown = false;
        if (Input.GetKeyDown(KeyCode.LeftAlt)) isAltDown = true;
        if (Input.GetKeyUp(KeyCode.LeftAlt)) isAltDown = false;
        if (useGameCamera)
        {
            if (mSceneCamera == null)
            {
                mSceneCamera = new GFX.SceneCamera();
                cameraLook();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                mSceneCamera.ResetCamera();
            }
            if (Input.GetMouseButtonDown(0))
            {
                cameraLook();
            }
            mSceneCamera.setScrollWheel(sw);
            if(Mathf.Abs(sw) > 0.0001f)
            {
                mSceneCamera.lookAt(ref mLastHitPos);
            }
        }
        else
        {
            if (Camera.mainCamera.orthographic == false)
                Camera.mainCamera.gameObject.transform.Translate(0, 0, sw * WheelSpeed);
            else
            {
                Camera.mainCamera.transform.rotation = Quaternion.LookRotation(new Vector3(0, -1, 0), new Vector3(0, 0, 1));
                Camera.mainCamera.orthographicSize -= sw * WheelSpeed;
            }
            if (isMidButtonDown)
            {
                Camera.mainCamera.gameObject.transform.Translate(-x * CameraSpeed, 0, 0);
                Camera.mainCamera.gameObject.transform.Translate(0, -y * CameraSpeed, 0);
            }

            if (isAltDown && isLeftButtonDown && Camera.mainCamera.orthographic == false)
            {
                Camera.mainCamera.gameObject.transform.Rotate(Vector3.right, -cameraRotSpeed * y);
                Camera.mainCamera.gameObject.transform.Rotate(Vector3.up, cameraRotSpeed * x, Space.World);
            }
        }
    }
    void cameraLook()
    {
        Vector3 fvMouseHitPlan = Vector3.zero;
        Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo,Mathf.Infinity, 1 << 9);
        if (hit)
        {
            Vector3 hitPos = hitInfo.point;
            mLastHitPos = hitPos;
            mSceneCamera.lookAt(ref hitPos);
        }
    }
}
