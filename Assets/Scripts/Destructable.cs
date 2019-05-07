using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destructable
{
    using ClipperLib;
    using Path = List<ClipperLib.IntPoint>;
    using Paths = List<List<ClipperLib.IntPoint>>;
    using Polygon = EarTriangulator.Polygon;


    public class Destructable : MonoBehaviour
    {
        //Adomet

        public int clipy = 100;
        public int clipy2 = 100;
        public int clipx = 100;
        public int clipx2 = 100;

        public int clip2y = 100;
        public int clip2y2 = 100;
        public int clip2x = 100;
        public int clip2x2 = 100;

        public bool once = false;

        public Mesh DrawMesh;

        // Start is called before the first frame update
        void Start()
        {
            if (once)
                Cut();
        }

        public void Update()
        {
            if (!once)
                Cut();
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

        void Extrude()
        {
            Mesh Mymesh = GetComponent<MeshFilter>().mesh;
        }

        Path Mesh2Path(Mesh myMesh)
        {
            Path myPath = new Path(myMesh.vertices.Length);

            foreach (var vert in myMesh.vertices)
            {
                myPath.Add(new IntPoint(vert.x * 100, vert.y * 100));
                //  Debug.Log("Vert X:" + vert.x + " Y:" + vert.y);
            }
            return myPath;
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

        // polygon clipping
        void Cut()
        {
            Mesh Mymesh = GetComponent<MeshFilter>().mesh;

            Paths subj = new Paths(1);
            subj.Add(new Path(4));
            subj[0].Add(new IntPoint(300, 0));
            subj[0].Add(new IntPoint(-300, 0));
            subj[0].Add(new IntPoint(-300, 300));
            subj[0].Add(new IntPoint(300, 300));


            Paths clip = new Paths(1);
            clip.Add(new Path(4));
            clip[0].Add(new IntPoint(-clipx2 + 50, clipy2 - 100));
            clip[0].Add(new IntPoint(-clipx, clipy2 - 100));
            clip[0].Add(new IntPoint(-clipx, clipy));
            clip[0].Add(new IntPoint(-clipx2 + 50, clipy));

            Paths solutions = new Paths();
            Clipper c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctDifference, solutions);


        //    if(DrawMesh !=null)
        //    {
        //       Path DrawPath = Mesh2Path(DrawMesh);
        //       Paths DrawPaths = new Paths(1);
        //        DrawPaths.Add(DrawPath);
        //        solutions = DrawPaths;
        //    }




            int t = 0;
            foreach (var item in solutions)
            {
                t += item.Count;
            }
            Vector2[] vertices2D = new Vector2[t];

              int a = 0;
              int z = 0;
            
              foreach (var solution in solutions)
              {
                  foreach (var pth in solution)
                  {
                      vertices2D[a] = new Vector2(pth.X, pth.Y);
                      Debug.Log("Path X:" + pth.X + " Y:" + pth.Y + "a:" + a + "z:" + z);
                      a++;
                  }
                  z++;
              }

            List<int> indices = new List<int>();

            Vector2[][] verticesarr = Paths2Vector2(solutions);
            int k = 0;

            foreach (var vector2s in verticesarr)
            {
                Polygon polygon = new Polygon(vector2s);

                EarTriangulator earTriangulator = new EarTriangulator(polygon);
                foreach (var item in earTriangulator.Triangulate())
                {
                    //change k*4
                    indices.Add(item +(k*4));
                   
                }
                k++;
            }


            foreach (var item in vertices2D)
            {
                Debug.Log("Vert:"+item);
            }


            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[vertices2D.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
            }

           

            Mymesh.vertices = vertices;
            Mymesh.triangles = indices.ToArray();

            Mymesh.RecalculateBounds();
            Mymesh.RecalculateNormals();

            GetComponent<MeshFilter>().mesh = Mymesh;
        }
    }
}