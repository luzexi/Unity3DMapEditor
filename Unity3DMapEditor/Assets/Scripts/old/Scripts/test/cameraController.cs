using UnityEngine;
using System.Collections;
public class cameraController : MonoBehaviour {
	public Transform targetTransform;
	public float     pitch 				= 60;
	public float 	 rotateSpeed 	  	= 2.0f;
	public float     mouseWheelSpeed  	= 8.0f;
	public float     pivotDistance    	= 8;
	public float     maxPivotDistance 	= 10;
	public float     minPivotDistance 	= 2;
	private bool     isRotateCamera 	= false;
	private Vector3  targetLastPosition	= Vector3.zero;
	public  Vector3  shakeMount = new Vector3(1, 1, 1);
	public  float    duration = 6;
    public float leftTime = 0;
	const int RAND_MAX = 12;
    const int RAND_MIN = 1;
	System.Random   mShakeRand      = new System.Random();
    public float    shakeMagnitude  = 0.8f;
    public float shakeRequency = 0.5f;
    private float signal = 1;
    private float Offset = 0;
    private float lastLeftTime = 0;
    private float timeStamp = 0;
    private int nextShakeCount = 0;
    private int ShakeCount = 0;

	// Use this for initialization
	void Start () 
	{
        Camera.main.transform.position = new Vector3(targetTransform.root.position.x - pivotDistance * Mathf.Cos(Mathf.Deg2Rad * pitch),
                                        targetTransform.root.position.y + pivotDistance * Mathf.Sin(Mathf.Deg2Rad * pitch),
                                        targetTransform.root.position.z - pivotDistance * Mathf.Cos(Mathf.Deg2Rad * pitch));

        Vector3 relPos = targetTransform.root.position - Camera.main.transform.position;
        Camera.main.transform.rotation = Quaternion.LookRotation(relPos);
		targetLastPosition = targetTransform.root.position;
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		//rotate camera
        if (leftTime > 0)
        {
            if (lastLeftTime < leftTime)
            {
                lastLeftTime = leftTime;
                timeStamp = UnityEngine.Time.realtimeSinceStartup;
                nextShakeCount = 1;
                ShakeCount = Mathf.RoundToInt(duration / shakeRequency);
            }
            float nextTime = nextShakeCount * shakeRequency;
            float deltTime = UnityEngine.Time.realtimeSinceStartup - timeStamp;
            if (nextTime < deltTime)
            {
                int magnitude = (mShakeRand.Next() % RAND_MAX);
                if (magnitude < RAND_MIN)
                {
                    magnitude = RAND_MIN;
                }

                Debug.Log(magnitude);
                float lastOffset = Offset;
                Offset = magnitude * shakeMagnitude * signal * leftTime / duration;
                Camera.main.transform.RotateAround(targetTransform.root.position, Vector3.up, -lastOffset + Offset);
                Vector3 pos = Camera.main.transform.position;
                pos.y -= lastOffset;
                pos.y += Offset;
                Camera.main.transform.position = pos;
                Vector3 relPos = targetTransform.root.position - Camera.main.transform.position;
                Camera.main.transform.rotation = Quaternion.LookRotation(relPos);
                signal = -signal;
                nextShakeCount++;
            }

            leftTime -= UnityEngine.Time.deltaTime;
            if (leftTime <= 0)
            {
                Camera.main.transform.position = new Vector3(targetTransform.root.position.x - pivotDistance * Mathf.Cos(Mathf.Deg2Rad * pitch),
                                        targetTransform.root.position.y + pivotDistance * Mathf.Sin(Mathf.Deg2Rad * pitch),
                                        targetTransform.root.position.z - pivotDistance * Mathf.Cos(Mathf.Deg2Rad * pitch));

                Vector3 relPos = targetTransform.root.position - Camera.main.transform.position;
                Camera.main.transform.rotation = Quaternion.LookRotation(relPos);
                targetLastPosition = targetTransform.root.position;
            }
        }
        else
        {
            //Debug.Log(Camera.main.transform.localPosition);
            lastLeftTime = 0;
            if (Input.GetMouseButtonDown(1))
            {
                isRotateCamera = true;
            }

            if (isRotateCamera)
            {
                float h = rotateSpeed * Input.GetAxis("Mouse X");
                float v = rotateSpeed * Input.GetAxis("Mouse Y");
                float speed;
                if (Mathf.Abs(h) > Mathf.Abs(v))
                {
                    speed = h;
                }
                else
                {
                    speed = v;
                }

                Camera.main.transform.RotateAround(targetTransform.root.position, Vector3.up, speed);
                Vector3 relPos = targetTransform.root.position - Camera.main.transform.position;
                Camera.main.transform.rotation = Quaternion.LookRotation(relPos);
            }

            if (Input.GetMouseButtonUp(1))
            {
                isRotateCamera = false;
            }

            //scale camera
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (scrollDelta != 0)
            {
                scrollDelta = scrollDelta * mouseWheelSpeed;
                Vector3 faceTargetDir = targetTransform.root.position - Camera.main.transform.position;
                float distance = faceTargetDir.magnitude - scrollDelta;
                if (distance > maxPivotDistance)
                {
                    distance = maxPivotDistance;

                }
                else if (distance < minPivotDistance)
                {
                    distance = minPivotDistance;
                }
                faceTargetDir.Normalize();
                Camera.main.transform.position = targetTransform.root.position - faceTargetDir * distance;
            }

            //translate camera
            Vector3 step = targetTransform.root.position - targetLastPosition;
            Camera.main.transform.position += step;
            targetLastPosition = targetTransform.root.position;
        }	
	}
	
	void FixedUpdate()
	{
		
	}
}
