using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TypeUtilities
{
    public static bool DerivesFrom(this Type type, Type baseType)
    {
        if (type == null || baseType == null)
            return false;

        if (type == baseType)
            return true;

        return type.IsAssignableFrom(baseType);
    }
    
}
