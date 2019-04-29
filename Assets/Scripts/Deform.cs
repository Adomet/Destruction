using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deform : MonoBehaviour
{



    public float value_1, value_2, value_3;

    Mesh newMesh;

    // Use this for initialization
    void Start()
    {

        value_1 = Random.Range(0f, value_1);
        value_2 = Random.Range(0f, value_2);
        value_3 = Random.Range(0f, value_3);


        newMesh = GetComponent<MeshFilter>().mesh;


    }

    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] += normals[i]* Mathf.Sin(Time.time) * Random.Range(-1f,1f)/10000f;
        }

        mesh.vertices = vertices;
    }
}
