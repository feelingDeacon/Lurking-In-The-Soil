using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionFunction
{
    public static bool ChanceToBool(this float val)
    {
        return Random.value <= val;
    }

    public static int GetLeftDirection(int currDir)
    {
        return (currDir - 1) % 8;
    }
    
    public static int GetRightDirection(int currDir)
    {
        return (currDir + 1) % 8;
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
