using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destructable
{
    using ClipperLib;
    using Path = List<ClipperLib.IntPoint>;
    using Paths = List<List<ClipperLib.IntPoint>>;
    using Polygon = EarTriangulator.Polygon;


    public class Hole : MonoBehaviour
    {
        //Adomet



        public bool once = false;
        public float extrudewidth = 1f;


        // Start is called before the first frame update
        void Start()
        {
            if (once)
                Cut();

            Extrude();
        }

        public void Update()
        {
            //if (!once)
            //    Cut();
        }

        void AnalyzeMesh()
        {
            Mesh mesh = GetComponent<MeshFilter>().mesh;

            foreach (var item in mesh.vertices)
            {
                Debug.Log("Vert:" + item);
            }
            foreach (var item in mesh.triangles)
            {
                Debug.Log("Tri:" + item);
            }

        }




        Vector2[] Path2Vector2(Path myPath)
        {
            Vector2[] newVectors = new Vector2[myPath.Count];
            int a = 0;
            foreach (var pnt in myPath)
            {
                newVectors[a] = new Vector2(pnt.X, pnt.Y);
                a++;
            }
            return newVectors;

        }

        Vector2[][] Paths2Vector2(Paths myPaths)
        {
            Vector2[][] newVectors = new Vector2[myPaths.Count][];
            int a = 0;
            foreach (var pth in myPaths)
            {
                newVectors[a] = Path2Vector2(pth);
                a++;
            }
            return newVectors;
        }

        void Print2DArray(Vector2[][] vector2arrs)
        {
            foreach (var vectorarr in vector2arrs)
            {
                foreach (var vector in vectorarr)
                {
                    Debug.Log("vector x:" + vector);
                }
            }
        }


        void Extrude()
        {
            Mesh Mymesh = GetComponent<MeshFilter>().mesh;

            Vector3[] newverts = new Vector3[6*Mymesh.vertices.Length];

            int vertCount = Mymesh.vertices.Length;

            int[] indices = new int[2*Mymesh.triangles.Length + vertCount*6];


            Debug.Log("verts:"+ Mymesh.vertices.Length);
            Debug.Log("triangles:"+Mymesh.triangles.Length);

            // Make first face again
            int c = 0;
            foreach (var item in Mymesh.triangles)
            {
                indices[c] = item;
                c++;
            }



            // Construct second face of the mesh based on other face
            for (int i = 0; i < Mymesh.triangles.Length/3; i++)
            {
                indices[c] = Mymesh.triangles[(i * 3)] + vertCount;
                c++;
                indices[c] = Mymesh.triangles[(i * 3) + 2] + vertCount;
                c++;
                indices[c] = Mymesh.triangles[(i * 3) + 1] + vertCount;
                c++;
            }

            //Construct middle portion of the model
            for (int i = 0; i < vertCount; i++)
            {
                //face 1 of the quad
                indices[c] = Mymesh.triangles[(i)] + vertCount*2;
                c++;
                indices[c] = Mymesh.triangles[(i)] + vertCount * 3;
                c++;
                indices[c] = Mymesh.triangles[(i) + 1] + vertCount*2;
                c++;
                //face 2 of the quad
                indices[c] = Mymesh.triangles[(i) + 1] + vertCount*2;
                c++;
                indices[c] = Mymesh.triangles[(i)] + vertCount * 3;
                c++;
                indices[c] = Mymesh.triangles[(i) + 1] + vertCount * 3;
                c++;

            
            
            }





     
            // Add verts for existing faces
            int t = 0;
            foreach (var item in Mymesh.vertices)
            {
                newverts[t] = item;
                t++;
            }
            // Add verts for new face
            foreach (var item in Mymesh.vertices)
            {
                newverts[t] = new Vector3(item.x,item.y,item.z+extrudewidth);
                t++;
            }
           
            //Add verts for middle faces
           
            foreach (var item in Mymesh.vertices)
            {
                newverts[t] = item;
                t++;
            }
           
            foreach (var item in Mymesh.vertices)
            {
                newverts[t] = new Vector3(item.x, item.y, item.z + extrudewidth);
                t++;
            }



            Mymesh.vertices = newverts;
            Mymesh.triangles = indices;

           
            Mymesh.RecalculateBounds();
            Mymesh.RecalculateTangents();
            Mymesh.RecalculateNormals();
            GetComponent<MeshFilter>().mesh = Mymesh;

        }


        // polygon clipping
        void Cut()
        {
            Mesh Mymesh = GetComponent<MeshFilter>().mesh;

            List<int> indices = new List<int>();

            Vector2[] hull = new Vector2[4];

            hull[0] = new Vector2(0,0);
            hull[1] = new Vector2(10,0);
            hull[2] = new Vector2(10,10);
            hull[3] = new Vector2(0,10);


            Vector2[][] holes = new Vector2[1][];

            holes[0] = new Vector2[4];

            holes[0][0] = new Vector2(3, 3);
            holes[0][1] = new Vector2(3, 6);
            holes[0][2] = new Vector2(6, 6);
            holes[0][3] = new Vector2(6, 3);


            Polygon polygon = new Polygon(hull,holes);

            EarTriangulator earTriangulator = new EarTriangulator(polygon);


            foreach (var item in earTriangulator.Triangulate())
            {
                //change k*4
                indices.Add(item);

            }


            // Create the Vector3 vertices

             // Vector3[] vertices = new Vector3[hull.Length];
             // int i = 0;
             // foreach (var item in hull)
             // {
             //     vertices[i] = new Vector3(hull[i].x, hull[i].y, 0);
             //     i++;
             // }

             //Create the Vector3 vertices
              Vector3[] vertices = new Vector3[hull.Length + holes[0].Length];
              int i = 0;
              foreach (var item in hull)
              {
                  vertices[i] = new Vector3(hull[i].x, hull[i].y, 0);
                  i++;
              }
           
             int c = 0;
             foreach (var item in holes[0])
            
             {
                 vertices[i] = new Vector3(holes[0][c].x, holes[0][c].y, 0);
                 i++;
                 c++;
             }


            Mymesh.vertices = vertices;
            Mymesh.triangles = indices.ToArray();

            Mymesh.RecalculateBounds();
            Mymesh.RecalculateNormals();

            GetComponent<MeshFilter>().mesh = Mymesh;
        }
    }
}