using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extrude : MonoBehaviour
{
    float extrudeDistance = 1f;

    // Start is called before the first frame update
    void Start()
    {
        ExtrudeMesh(extrudeDistance);
    }

    void ExtrudeMesh (float extrudeDist)
    {
        MeshFilter MymeshFilter = GetComponent<MeshFilter>();
        Mesh mesh = MymeshFilter.mesh;




        mesh.vertices = GenerateVerts(mesh);
        mesh.triangles = GenerateTries(mesh);


        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MymeshFilter.mesh = mesh;
    }

    private Vector3[] GenerateVerts(Mesh mesh)
    {
        //if want to hard shade edges you need duplicate
        int meshvertcount = mesh.vertices.Length;
        var newverts = new Vector3[meshvertcount*2];


        //make front and back face
        for (int i = 0; i < meshvertcount; i++)
        {
            newverts[i] = mesh.vertices[i];
            
            newverts[meshvertcount + i-1] = mesh.vertices[i] +new Vector3(0,0, meshvertcount);
        }
        //make sides

        return newverts;
    }
    private int [] GenerateTries(Mesh mesh)
    {
        var newtries = new int[mesh.triangles.Length*2];

        for (int i = 0; i < mesh.triangles.Length; i++) 
        {
            newtries[i] = mesh.triangles[i];
            newtries[mesh.triangles.Length + i] = mesh.triangles[i] + (mesh.vertices.Length/2);
        }


        return newtries;
    }
}
