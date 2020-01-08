using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    static private Vector3[] AllVector2To3(Vector2[] Vectors)
    {
        Vector3[] Result = new Vector3[Vectors.Length];
        for (int I = 0; I < Vectors.Length; I++)
            Result[I] = new Vector3(Vectors[I].x, Vectors[I].y, 0);
        return Result;
    }

    static private float GetClockwiseCoef(Vector2 v1, Vector2 v2)
    { 
        return Mathf.Sign(v1.x * v2.y - v1.y * v2.x); 
    }

    static private bool TriangleStrictlyAnyContains(Vector2 a, Vector2 b, Vector2 c, List<Vector2> points)
    {
        if (points.Count == 0)
            return false;
        var ky1 = (b.y - a.y);
        var ky2 = (c.y - b.y);
        var ky3 = (a.y - c.y);
        var kx1 = (b.x - a.x);
        var kx2 = (c.x - b.x);
        var kx3 = (a.x - c.x);
        for (int i = 0; i < points.Count; i++)
        {
            var point = points[i];
            var a1 = (a.x - point.x) * ky1 - kx1 * (a.y - point.y);
            var b1 = (b.x - point.x) * ky2 - kx2 * (b.y - point.y);
            var c1 = (c.x - point.x) * ky3 - kx3 * (c.y - point.y);
            if ((a1 < 0 && b1 < 0 && c1 < 0) || (a1 > 0 && b1 > 0 && c1 > 0))
                return true;
        }
        return false;
    }

    static private int[] GetTriangles(Vector3[] aPoints)
    {
        List<Vector3> points = new List<Vector3>(aPoints);
        var Result = new List<int>();
        var checkPoints = new List<Vector2>();
        for (int j = points.Count - 1; j >= 0; j--)
        {
            points[j] = new Vector3(points[j].x, points[j].y, j);
            var a = points[(j + points.Count - 1) % points.Count];
            var b = points[j];
            var c = points[(j + 1) % points.Count];
            if (GetClockwiseCoef(a - b, c - b) < 0f)
                checkPoints.Add(b);
        }
        for (int j = points.Count - 1; j >= 0; j--)
        {
            var a = points[(j + points.Count - 1) % points.Count];
            var b = points[j];
            var c = points[(j + 1) % points.Count];
            if (GetClockwiseCoef(a - b, c - b) > 0f && !TriangleStrictlyAnyContains(a, b, c, checkPoints))
            {
                Result.AddRange(new[] { (int)a.z, (int)b.z, (int)c.z });
                points.RemoveAt(j);
                if (points.Count == 2)
                    break;
                j = points.Count - 1;
            }
        }
        return Result.ToArray();
    }

    static private Vector2[] GetUV(Vector3[] aPoints)
    {
        const float TextureSize = 1f;
        Vector2[] Result = new Vector2[aPoints.Length];
        float minX = aPoints[0].x, minY = aPoints[0].y;
        for (int I = 1; I < aPoints.Length; I++)
        {
            if (aPoints[I].x < minX)
                minX = aPoints[I].x;
            if (aPoints[I].y < minY)
                minY = aPoints[I].y;
        }
        for (int I = 0; I < aPoints.Length; I++)
        {
            Result[I] = new Vector2(
                (aPoints[I].x - minX) / TextureSize,
                (aPoints[I].y - minY) / TextureSize);
        }
        return Result;
    }

    static private Mesh GetBlockMesh(List<Vector2> Coords)
    {
        var Mesh = new Mesh {};
        Mesh.vertices = AllVector2To3(Coords.ToArray());
        Mesh.triangles = GetTriangles(Mesh.vertices);
        Mesh.uv = GetUV(Mesh.vertices);
        Mesh.RecalculateNormals();
        return Mesh;
    }

    public void DoShow(List<Vector2> BlockVertices)
    {
        GetComponent<MeshFilter>().mesh = GetBlockMesh(BlockVertices);
    }
}