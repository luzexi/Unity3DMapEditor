using UnityEngine;
using System.Collections;
namespace GFX
{
	public class GfxUtility {
        public static readonly float MAX_SCENE_HEIGHT = 100000.0f;
        //获取场景的高度（包括地形和建筑）
        public static float getSceneHeight(float x, float z)
        {
            float SceneHeight = -MAX_SCENE_HEIGHT;
            Ray ray = new Ray();//构造射线
            ray.direction = -Vector3.up;
            ray.origin = new Vector3(x, MAX_SCENE_HEIGHT, z);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, GfxActor.ActorLayerUnMask))//排除actor
            {
                SceneHeight = hitInfo.point.y;
            }
            return SceneHeight;
        }
		public static void attachGameObject(GameObject src, GameObject attTo)
		{
			  Vector3 locPos = src.transform.localPosition;//保存本地变换
			  Quaternion locRot = src.transform.localRotation;
			  Vector3 locScale = src.transform.localScale;
              src.transform.parent = attTo.transform;//挂接到object
			  src.transform.localPosition = locPos;//挂接后还原本地变换
			  src.transform.localRotation = locRot;
			  src.transform.localScale = locScale;
		}
        public static void setGameObjectLayer(GameObject go, int layer)//设置go和go所有子节点的层
        {
            if (go == null) return;
            go.layer = layer;
            Transform[] allTrans = go.GetComponentsInChildren<Transform>(); 
            foreach(Transform trans in allTrans)
            {
                trans.gameObject.layer = layer;
            }
        }
        public static void setGameObjectVisible(GameObject gameObject, bool bVisible)
        {
            if (gameObject)
            {
                //gameObject.SetActiveRecursively(bVisible);
                Renderer[] allRenderer = gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in allRenderer)
                {
                    r.enabled = bVisible;   
                }
                Behaviour[] allBehaviours = gameObject.GetComponentsInChildren<Behaviour>();
                foreach(Behaviour b in allBehaviours)
                {
                    b.enabled = bVisible;
                }
            }
        }
        public static float GetYAngle( Vector2 src,  Vector2 dest)//计算角色旋转到 向量dest-src的角度,返回弧度值
        {
            Vector2 moveDir = dest - src;
            if (moveDir.magnitude<0.01)
            {
                return 0;
            }
           
            moveDir.Normalize();
            float angle  = Mathf.Acos(moveDir.y);
            if (moveDir.x < 0)//180-360
            {
                angle = 2 * Mathf.PI - angle;
            }
            return angle;
            
        }
		
		public static bool IsLessEqual(float a,float b)
		{
			return (a < b || IsEqual(a,b));
		}
		
		public static bool IsEqual(float a,float b)
		{
			return Mathf.Approximately(a,b);
		}
	}
}

