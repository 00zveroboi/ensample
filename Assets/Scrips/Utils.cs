using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    static public float GetCircleLength(float Radius)
    {
        return 2 * Mathf.PI * Radius;
    }

    static public int Mod(int F1, int F2)
    {
        int Result = F1 % F2; 
        if (F1 < 0)
            Result += F2;
        return Result;
    }

    static public void ReverseList(ref List<string> aList)
    {
        for (int I = 0, J = aList.Count - 1; J - I > 0; I++, J--)
        {
            string Temp = aList[I];
            aList[I] = aList[J];
            aList[J] = Temp;
        }
    }

    static public float GetAngleByVector(Vector2 Vector)
    {
        float Angle = Mathf.Atan2(Vector.y, Vector.x);
        if (Angle < 0)
            Angle = 2 * Mathf.PI + Angle; 
        return Angle;
    }

    static public float GetAngleBetween(float Angle1, float Angle2)
    {
        float Angle = Mathf.Abs(Angle1 - Angle2);
        if (Angle > Mathf.PI)
            Angle = 2 * Mathf.PI - Angle;
        return Angle;
    }
}
