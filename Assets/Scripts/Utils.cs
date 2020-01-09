using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static float GenerateRandomValue(float min, float max)
    {
        var val = Random.Range(min,max);
        FlipDirectiononRandom(ref val);
        
        return val;
    }

    private static void FlipDirectiononRandom(ref float value)
    {
        if ((Random.value > 0.5f))
        {
            value *= -1;
        }
    }
}
