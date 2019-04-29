using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sebastian.Geometry
{
    using ClipperLib;
    using Path = List<ClipperLib.IntPoint>;
    using Paths = List<List<ClipperLib.IntPoint>>;


    public class Destructable : MonoBehaviour
    {
        //Adomet


        public float Len = 1f;
        public int x = 0;

        public int clipy = 100;
        public int clipy2 = 100;
        public int clipx = 100;
        public int clipx2 = 100;

        public int clip2y = 100;
        public int clip2y2 = 100;
        public int clip2x = 100;
        public int clip2x2 = 100;

        public Mesh stencil;

        public bool once = false;


        // Start is called before the first frame update
        void Start()
        {
            if(once)
            Cut();
            

            
        }

        public void Update()
        {
            if(!once)
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



        Path Mesh2Path (Mesh myMesh)
        {
            Path myPath = new Path();

            foreach (var vert in myMesh.vertices)
            {
                myPath.Add(new IntPoint(vert.x *100, vert.y *100));
              //  Debug.Log("Vert X:" + vert.x + " Y:" + vert.y);
            }
            return myPath;
        }

        Mesh Path2Mesh(Path myPath)
        {
            Mesh myMesh = new Mesh();
            Vector3 [] newVertices = new Vector3 [myPath.Count];
            int a = 0;
            foreach (var pnt in myPath)
            {
                newVertices[a] = new Vector3(pnt.X / 100f, pnt.Y /100f, 0f);
                a++;
            }
            myMesh.vertices = newVertices;
            return myMesh;
        }

        // polygon clipping
        void Cut()
        {
            Mesh Mymesh = GetComponent<MeshFilter>().mesh;

              Paths subj = new Paths(1);
              subj.Add(new Path(4));
              subj[0].Add(new IntPoint(0,0));
              subj[0].Add(new IntPoint(-500,0));
              subj[0].Add(new IntPoint(-500,200));
              subj[0].Add(new IntPoint(0,200));

           
              Paths clip = new Paths(1);
              clip.Add(new Path(4));
              clip[0].Add(new IntPoint(-clipx2 + 50,clipy2 - 100));
              clip[0].Add(new IntPoint(-clipx, clipy2 - 100));
              clip[0].Add(new IntPoint(-clipx, clipy));
              clip[0].Add(new IntPoint(-clipx2 + 50,   clipy));

            Paths solutions = new Paths();
              Clipper c = new Clipper();
              c.AddPaths(subj, PolyType.ptSubject,true);
              c.AddPaths(clip, PolyType.ptClip, true);
              c.Execute(ClipType.ctDifference, solutions);


          

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





                Polygon polygon = new Polygon(vertices2D);

            Triangulator tr = new Triangulator(polygon);
            




           // int p = 0;
           // int k = 0;
           // foreach (var part in partindices)
           // {
           //     foreach (var item in part)
           //     {
           //
           //         // how much vert fist parth has ?
           //         indices.Add(item+(k*solutions[k].Capacity));
           //         
           //         Debug.Log("indices:" + (item + (k * solutions[k].Capacity)) + " p:"+p +" k:"+k);
           //        p++;
           //     }
           //     k++;
           // }
            




            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[vertices2D.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
            }

            Mymesh.vertices = vertices;



            Mymesh.triangles = tr.Triangulate();
            Mymesh.RecalculateBounds();
            Mymesh.RecalculateNormals();
            GetComponent<MeshFilter>().mesh = Mymesh;

   


        }
    }
}
