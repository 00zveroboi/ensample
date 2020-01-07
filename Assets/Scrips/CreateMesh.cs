using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMesh : MonoBehaviour
{

    public static Mesh Quad(Vector3 origin, Vector3 width, Vector3 length)
    {
        var normal = Vector3.Cross(length, width).normalized;

        var mesh = new Mesh
        {
            vertices = new[] { origin, origin + length, origin + length + width, origin + width, origin + width + width, origin - width },
            normals = new[] {normal, normal, normal, normal, normal, normal },
            uv = new[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 0), new Vector2(3, 0), new Vector2(0, 0) },
            triangles = new[] { 0, 1, 2, 0, 1, 5, 0, 2, 3, 2, 3, 4}
        };
        return mesh;
    }

    public static Mesh Triangle(Vector3 origin, Vector3 vertex1, Vector3 vertex2)
    {
        var normal = Vector3.Cross((vertex1 - origin), (vertex2 - origin)).normalized;
        var mesh = new Mesh
        {
            vertices = new[] { origin, vertex1, vertex2 },
            normals = new[] { normal, normal, normal },
            uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) },
            triangles = new[] { 0, 1, 2 }
        };
        return mesh;
    }

    public static Mesh MultiMesh(List<Vector3> Coords)
    {
        for(int I = 0; I < Coords.Count; I++)
        {
            
        }
        var mesh = new Mesh
        {

        };
        return mesh;
    }

    public static Mesh Cube(Vector3 width, Vector3 length, Vector3 height)
    {
        var corner0 = -width / 2 - length / 2 - height / 2;
        var corner1 = width / 2 + length / 2 + height / 2;

        var combine = new CombineInstance[6];
        combine[0].mesh = Quad(corner0, length, width);
        combine[1].mesh = Quad(corner0, width, height);
        combine[2].mesh = Quad(corner0, height, length);
        combine[3].mesh = Quad(corner1, -width, -length);
        combine[4].mesh = Quad(corner1, -height, -width);
        combine[5].mesh = Quad(corner1, -length, -height);

        var mesh = new Mesh();
        mesh.CombineMeshes(combine, true, false);
        return mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh CubeS = new Mesh();
        CubeS = Quad(new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0));
        mf.mesh = CubeS;

    }

}
