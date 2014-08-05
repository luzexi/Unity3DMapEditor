using UnityEngine;
using System.Collections;

public class Utility {
    public static float GetYAngle( Vector2 src,  Vector2 dest)//计算角色旋转到 向量dest-src的角度,换回弧度值
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

    public static float GetDistance(float x1, float y1,  float x2, float y2 )//计算平面上点(x1,y1)到点(x2, y2)的距离
    {
        return Mathf.Sqrt( (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) );
    }

    public static float TDU_GetDistSq(Vector3 fvPos1, Vector3 fvPos2)
    {
	    return (fvPos1.x-fvPos2.x)*(fvPos1.x-fvPos2.x) + (fvPos1.y-fvPos2.y)*(fvPos1.y-fvPos2.y) +  (fvPos1.z-fvPos2.z)*(fvPos1.z-fvPos2.z);
    }

    public static float TDU_GetDist(Vector3 fvPos1, Vector3 fvPos2)
    {
        return Mathf.Sqrt((fvPos1.x - fvPos2.x) * (fvPos1.x - fvPos2.x) + (fvPos1.y - fvPos2.y) * (fvPos1.y - fvPos2.y) + (fvPos1.z - fvPos2.z) * (fvPos1.z - fvPos2.z));
    }
}


