using System;
using UnityEngine;

public static class FloatExtensions
{
    public static float MoveOverTime(this float number, float destination, float time)
    {
        return Mathf.Lerp(number, destination, 1 - Mathf.Pow(time, Time.deltaTime));
    }
}