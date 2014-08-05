using UnityEngine;
using System;
namespace GFX
{
    public class SceneCamera
    {
         const float  ZoomSpeed  =1.5f;
        //固定的摄像机参数
        const float FOV = 45.0f; //45度角
        const float MinDis = 4.5f;//最近8米
        const float MaxDis = 20.0f;//最远20米
        const float CamAngle = Mathf.PI/4.0f;//摄像机初始角度
        const float CamLookAngle = Mathf.PI * 2.0f / 4.0f; ////90度角是正常XY地图视角 //135度角度观看
        const float CamRotateSpeed = 4.5f;
        const int RAND_MAX = 0x7fff;
        Vector3 Scale = new Vector3(1, 1, 1);

        const float zoomStep = 0.2f;
        static readonly SceneCamera sInstance = new SceneCamera();//唯一的实例
        Camera mUnityMainCamera;
        public UnityEngine.Camera UnityMainCamera
        {
            get { return mUnityMainCamera; }
        }
        float mZoom = 0.5f;
        public float getZoom()
        {
             return mZoom; 
        }
        float mCameraDis;
        static public SceneCamera Instance
        {
            get
            {
                return sInstance;
            }
        }

        float           mShakeDuration  = 0.2f;
        float           mShakeLeft      = 0;
        float           mShakeMagnitude = 0.5f;
        System.Random   mShakeRand      = new System.Random();
        Vector3         mLastCameraForword = new Vector3();
        const int       SHAKE_RAND_MAX = 12;
        const int       SHAKE_RAND_MIN = 1;
        float           mLastShakeOffset = 0;
        float           mShakeIncrement = 1;

        public float ShakeDuration
        {
            get { return mShakeDuration; }
            set { mShakeDuration = value; }
        }
        public float ShakeLeft
        {
            get { return mShakeLeft; }
            set { mShakeLeft = value; }
        }

        public SceneCamera()
        {
            ResetCamera();
        }
        public void ResetCamera()
        {
            setZoom(0.5f);
            mUnityMainCamera = Camera.mainCamera;
            mUnityMainCamera.cullingMask &= ~LayerManager.FakeObjectMask;
            mUnityMainCamera.fov = FOV;//设置fov
            Vector3 camLookDir = new Vector3();
            camLookDir.y = -Mathf.Sin(CamAngle);
            camLookDir.x = Mathf.Cos(CamAngle) * Mathf.Cos(CamLookAngle);
            camLookDir.z = Mathf.Cos(CamAngle) * Mathf.Sin(CamLookAngle);
            mUnityMainCamera.gameObject.transform.rotation = Quaternion.LookRotation(camLookDir);
        }
        public void lookAt(ref Vector3 lookAtPos)
        {
            if (mShakeLeft > 0)
            {
                int magnitude = (mShakeRand.Next() % SHAKE_RAND_MAX);
                if (magnitude < SHAKE_RAND_MIN)
                {
                    magnitude = SHAKE_RAND_MIN;
                }
                mShakeLeft          -= Time.deltaTime;
                float offset        = mLastShakeOffset;
                mLastShakeOffset    = magnitude * mShakeMagnitude * mShakeIncrement * mShakeLeft / mShakeDuration;
                Vector3 pos         = Camera.main.transform.position;
                pos.y -= offset;
                pos.y += mLastShakeOffset;
                mUnityMainCamera.gameObject.transform.position = pos;
                Vector3 relPos = lookAtPos - mUnityMainCamera.gameObject.transform.position;
                mUnityMainCamera.gameObject.transform.rotation = Quaternion.LookRotation(relPos);
                if (mShakeLeft < 0)
                {
                   mUnityMainCamera.gameObject.transform.forward = mLastCameraForword;
                }
            }
            else
            {
                mLastCameraForword.x = mUnityMainCamera.gameObject.transform.forward.x;
                mLastCameraForword.y = mUnityMainCamera.gameObject.transform.forward.y;
                mLastCameraForword.z = mUnityMainCamera.gameObject.transform.forward.z;
                Vector3 camDir = mUnityMainCamera.gameObject.transform.forward;
                Vector3 camPos = lookAtPos - camDir * mCameraDis;//计算摄像机位置
                mUnityMainCamera.gameObject.transform.position = camPos;
            }
        }
        public void setScrollWheel(float scrollWheelValue)//鼠标滚轮
        {
            if (Math.Abs(scrollWheelValue) < 0.0001f) return;
            mZoom = mZoom - scrollWheelValue * GFX.SceneCamera.ZoomSpeed;
            mZoom = Mathf.Clamp01(mZoom);

            setZoom(mZoom);
        }
        void setZoom(float zoom)
        {
            mZoom = zoom;
            mCameraDis = Mathf.Lerp(MinDis, MaxDis, mZoom);
        }
//         public void ZoomIn()
//         {
//             float zoomTemp = mZoom - zoomStep;
//             if (zoomTemp >= 0)
//             {
//                 setZoom(zoomTemp);
//             }
//         }
//         public void ZoomOut()
//         {
//             float zoomTemp = mZoom + zoomStep;
//             if (zoomTemp <= 1)
//             {
//                 setZoom(zoomTemp);
//             }
//         }

        public void OnShake()
        {
            ShakeLeft = mShakeDuration;
            mLastCameraForword.x = mUnityMainCamera.gameObject.transform.forward.x;
            mLastCameraForword.y = mUnityMainCamera.gameObject.transform.forward.y;
            mLastCameraForword.z = mUnityMainCamera.gameObject.transform.forward.z;
        }
        public void AddDirection(float deltaDir)
        {
            mUnityMainCamera.gameObject.transform.Rotate(Vector3.up, deltaDir * CamRotateSpeed, Space.World);
        }
    }
}