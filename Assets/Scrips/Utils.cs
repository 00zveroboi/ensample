using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static int Max(int Val1, int Val2)
    {
        if (Val1 >= Val2)
            return Val1;
        else
            return Val2;
    }

    public static int Min(int Val1, int Val2)
    {
        if (Val1 <= Val2)
            return Val1;
        else
            return Val2;
    }

    public static float Max(float Val1, float Val2)
    {
        if (Val1 >= Val2)
            return Val1;
        else
            return Val2;
    }

    public static float Min(float Val1, float Val2)
    {
        if (Val1 <= Val2)
            return Val1;
        else
            return Val2;
    }

    public static int Ceil(float Val)
    {
        if (Val % 1 != 0)
        {
            return ((int)Val) + 1;
        }
        else
            return (int)Val;
    }

    public static int Floor(float Val)
    {
        return (int)Val;
    }


}
