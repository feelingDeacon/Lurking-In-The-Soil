using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionFunction
{
    public static bool ChanceToBool(this float val)
    {
        return Random.value <= val;
    }
}
