using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionFunction
{
    public static bool ChanceToBool(this float val)
    {
        return Random.value <= val;
    }

    public static Vector2 GetLeftDirection(this Vector2 currDir)
    {
        float radians = -45 * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = currDir.x;
        float ty = currDir.y;
 
        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
        // return (currDir - 1 + 8) % 8;
    }
    
    public static Vector2 GetRightDirection(this Vector2 currDir)
    {
        float radians = 45 * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = currDir.x;
        float ty = currDir.y;
 
        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);

        // return (currDir + 1) % 8;
    }

    public static void DirectionToIndex(Vector2 dir, out int resX, out int resY)
    {
        float signedAngle = Vector2.SignedAngle(dir, new Vector2(0, 1));
        if (Mathf.Abs(signedAngle) <= 22.5f)
        {
            resX = 0;
            resY = 1;
        }
        else if (signedAngle > 22.5f && signedAngle <= 67.5f)
        {
            resX = 1;
            resY = 1;
        }
        else if (signedAngle > 67.5f && signedAngle <= 112.5f)
        {
            resX = 1;
            resY = 0;
        }
        else if (signedAngle > 112.5f && signedAngle <= 157.5f)
        {
            resX = 1;
            resY = -1;
        }
        else if (signedAngle < -22.5f && signedAngle >= -67.5f)
        {
            resX = -1;
            resY = 1;
        }
        else if (signedAngle < -67.5f && signedAngle >= -112.5f)
        {
            resX = -1;
            resY = 0;
        }
        else if (signedAngle < -112.5f && signedAngle >= -157.5f)
        {
            resX = -1;
            resY = -1;
        }
        else
        {
            resX = 0;
            resY = -1;
        }
    }
    
    public static Vector3 RemoveY(this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }
    
    public static Vector3 RemoveZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

}
