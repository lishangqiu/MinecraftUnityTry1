using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        mesh.Clear();

        Vector3 p0 = new Vector3(0, 0, 0);
        Vector3 p1 = new Vector3(1, 0, 0);
        Vector3 p2 = new Vector3(0, 1, 0);
        Vector3 p3 = new Vector3(0, 0, 1);
        Vector3 p4 = new Vector3(0, 1, 1);
        Vector3 p5 = new Vector3(1, 1, 0);
        Vector3 p6 = new Vector3(1, 0, 1);
        Vector3 p7 = new Vector3(1, 1, 1);
        Vector3 p8 = new Vector3(0, 1, 2);
        Vector3 p9 = new Vector3(1, 1, 2);

        mesh.vertices = new Vector3[]{
            p0,p1,p2,p5, // front
            p3,p6,p4,p7, // back
            p0,p3,p2,p4, // left
            p1,p6,p5,p7, // right
            p2,p5,p4,p7, // top
            p0,p1,p3,p6, // bottom

            p4,p7,p8,p9
        };

        mesh.triangles = new int[]{
            2,1,0,     1,2,3, // front
            4,5,6,     7,6,5, // back
            8,9,10,    11,10,9, // left
            14,13,12,  13,14,15, // right
            18,17,16,  17,18,19, // top
            20,21,22,  23,22,21, // bottom

            26,25,24,  25,26,27
        };

        Vector2 uv0 = new Vector2(0, 0);
        Vector2 uv1 = new Vector2(0.33f, 0);
        Vector2 uv2 = new Vector2(0.66f, 0);
        Vector2 uv3 = new Vector2(1, 0);
        Vector2 uv4 = new Vector2(0, 0.5f);
        Vector2 uv5 = new Vector2(0.33f, 0.5f);
        Vector2 uv6 = new Vector2(0.66f, 0.5f);
        Vector2 uv7 = new Vector2(1, 0.5f);
        Vector2 uv8 = new Vector2(0, 1);
        Vector2 uv9 = new Vector2(0.33f, 1);
        Vector2 uv10 = new Vector2(0.66f, 1);
        Vector2 uv11 = new Vector2(1, 1);

        mesh.uv = new Vector2[]{
            uv0, uv1, uv4, uv5, // front
            uv4, uv5, uv8, uv9, // back
            uv1, uv2, uv5, uv6, // left
            uv5, uv6, uv9, uv10, // right
            uv2, uv3, uv6, uv7, // top
            uv6, uv7, uv10, uv11, // bottom

            uv2, uv3, uv6, uv7, // top
        };

        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 forward = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normals = new Vector3[]
        {
	        forward, forward, forward, forward,	// Front
	        back, back, back, back,             // Back
            left, left, left, left,             // Left
	        right, right, right, right,         // Right
	        up, up, up, up,                      // Top
            down, down, down, down,             // Bottom

            up, up, up, up                      // Top
        };

       mesh.normals = normals;
       mesh.RecalculateBounds();
       mesh.Optimize();
    }
}
