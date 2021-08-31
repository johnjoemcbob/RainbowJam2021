using UnityEngine;
public static class MathExt
{
    public static float RemapRange(this float input, float srcMin, float srcMax, float dstMin, float dstMax)
    {
        float t = Mathf.InverseLerp(srcMin, srcMax, input);
        return Mathf.Lerp(dstMin, dstMax, t);
    }

    public static float RemapRangeClamped(this float input, float srcMin, float srcMax, float dstMin, float dstMax)
    {
        float val = Mathf.Clamp(input, srcMin, srcMax);
        return RemapRange(val, srcMin, srcMax, dstMin, dstMax);
    }

    public static float RemapClamp01(this float input, float srcMin, float srcMax)
    {
        return RemapRangeClamped(input, srcMin, srcMax, 0.0f, 1.0f);
    }

    public static float LinearToDecibels(this float linear)
    {
        return Mathf.Clamp(Mathf.Log10(linear) * 20.0f, -80.0f, 0.0f);
    }
}
