using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    static public float GetCircleLength(float Radius)
    {
        return 2 * Mathf.PI * Radius;
    }

}
