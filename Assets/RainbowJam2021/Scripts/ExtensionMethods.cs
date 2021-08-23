using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static float RemapClamped( this float aValue, float aIn1, float aIn2, float aOut1, float aOut2 )
    {
        float t = (aValue - aIn1) / (aIn2 - aIn1);
        if ( t > 1f )
            return aOut2;
        if ( t < 0f )
            return aOut1;
        return aOut1 + ( aOut2 - aOut1 ) * t;
    }
}
