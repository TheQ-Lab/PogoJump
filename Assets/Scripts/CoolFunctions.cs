using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolFunctions : MonoBehaviour
{
    public static float Remap(float value, int start1, int stop1, int start2, int stop2)
    {
        float outgoing =
            start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
        return outgoing;
    }

    public static float Remap(float value, float start1, float stop1, float start2, float stop2)
    {
        float outgoing =
            start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
        return outgoing;
    }

    public static double Remap(double value, double start1, double stop1, double start2, double stop2)
    {
        double outgoing =
            start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
        return outgoing;
    }
}
