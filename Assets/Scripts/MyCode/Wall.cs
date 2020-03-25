using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destructable
{

    public class Wall : MonoBehaviour
    {

        List<List<Vector2>> Holes = new List<List<Vector2>>();
        public float extrudedist = 0.1f;
        public GameObject backface = null;


        float a = 1;
        float b = 1;
        // Start is called before the first frame update
        void Start()
        {
            MakeWall(a, b);

            backface.transform.position += new Vector3(0, 0, extrudedist);


            //  Extrude();
        }

        private void Update()
        {
            //a = Mathf.Sin(Time.time)*2;
            //b = Mathf.Sin(Time.time)*2;
            //MakeWall(a,b);
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

        // This is experimental please for love of god fix this this is too bad even for me...
        void MakeHoles(Dictionary<int, int> connections, int link, List<List<Vector2>> Holes, List<Vector2> Shape)
        {
            int hook = -1;
            //just for finding hook
            if (!connections.TryGetValue(link, out hook))
            {
                Debug.Log("Didnot find the link on dictionary!:" +link);
            }
            else
            {
                //hook %= 4;
             
                Shape.Add(Holes[(hook / 4) - 1][hook % 4]);
                for (int t = hook+1; t < hook + Holes[(hook / 4) - 1].Count; t++)//11,12,13
                {
                        //Debug.Log("t:" + t);
                    if (!connections.ContainsKey((hook/4)*4+(t%4)))
                    {
                        Shape.Add(Holes[(hook / 4) - 1][t % 4]);
                    }
                    else
                    {
                        //dont use 4  use holesvert.count
                        Shape.Add(Holes[(hook / 4)-1][t % 4]);
                        //Queue Holes with recursive function
                      
                        MakeHoles(connections, (hook / 4) * 4 + (t % 4), Holes, Shape);

                        Shape.Add(Holes[(hook / 4) - 1][t % 4]);
                    }

                }
                Shape.Add(Holes[(hook / 4) - 1][hook % 4]);
            }
        }

        public void AddHole(Vector2 Pointofimpact)
        {
            List<Vector2> newHole = new List<Vector2>();
            newHole.Add(Pointofimpact + new Vector2(-0.1f, -0.1f));
            newHole.Add(Pointofimpact + new Vector2(+0.1f, -0.1f));
            newHole.Add(Pointofimpact + new Vector2(+0.1f, +0.1f));
            newHole.Add(Pointofimpact + new Vector2(-0.1f, +0.1f));
            Holes.Add(newHole);

            MakeWall(a, b);
      
            
        }
        public void MakeWall(float a , float b)
        {
            
            Mesh Mymesh = GetComponent<MeshFilter>().mesh;
            Dictionary<int, int> connections = new Dictionary<int, int>();


            int[] indices;

            List<Vector2> hull = new List<Vector2>();
            List<Vector2> Shape = new List<Vector2>();


            //Add connetion vert again after connection
            hull.Add(new Vector2(0, 0));
            hull.Add(new Vector2(0, 3));
            hull.Add(new Vector2(5, 3));
            hull.Add(new Vector2(5, 0));


            //Calculate connnetion hull to one of the edges then connect the holes 
            int hullconnection = -1;
            int holeconnection = -1;
            float hindistance = Mathf.Infinity;
            for (int h =0;h<hull.Count;h++)
            {
                //Define conneting edges    

                for (int k =0;k< Holes.Count;k++)
                {
                    for(int c =0; c< Holes[k].Count;c++)
                    {
                        float distance = Vector2.Distance(hull[h], Holes[k][c]);
                        if(distance< hindistance)
                        {
                            hindistance = distance;
                            //Debug.Log("Hole distance:"+ k +","+h+":"+ distance);
                            hullconnection = h;
                            holeconnection = 4 + (k * 4) + c;
                        }
                    }
                }
            }
            if(hullconnection!=-1)
            {
              connections.Add(hullconnection, holeconnection);
            }


            int linkconnection = -1;
            int hookconnection = -1;

            for (int k = 0; k < Holes.Count - 1; k++)
            {

                //Debug.Log("k:" + k);
                float mindistance = Mathf.Infinity;

                for (int c = 0; c < Holes[k].Count; c++)
                {

                    for (int t = k + 1; t < Holes.Count; t++)
                    {

                        for (int n = 0; n < Holes[t].Count; n++)
                        {
                            float distance = Vector2.Distance(Holes[t][n], Holes[k][c]);
                            if (distance < mindistance)
                            {
                                mindistance = distance;
                                linkconnection = 4 + (k * 4) + c;
                                hookconnection = 4 + (t * 4) + n;
                               // Debug.Log("dff distance:" + k + "," + t + " :" + distance);


                            }
                        }

                    }

                }

                if (!connections.ContainsKey(linkconnection))
                {

                    connections.Add(linkconnection, hookconnection);
                    connections.Add(hookconnection, linkconnection);
                }
                else
                {
                    hullconnection = -1;
                    holeconnection = -1;
                    hindistance = Mathf.Infinity;

                    for (int c = 0; c < Holes[k].Count; c++)
                    {

                        for (int h = 0; h < hull.Count; h++)
                        {
                            float distance = Vector2.Distance(hull[h], Holes[k][c]);
                            if (distance < hindistance)
                            {
                                hindistance = distance;
                                //Debug.Log("Hole distance:"+ k +","+h+":"+ distance);
                                hullconnection = h;
                                holeconnection = 4 + (k * 4) + c;
                            }
                        }
                    }

                    if (!connections.ContainsKey(hullconnection))
                    {
                        connections.Add(hullconnection, holeconnection);
                    }

                }

            }

 


            foreach (var kvp in connections)
                Debug.Log(kvp.Key +","+ kvp.Value);

            for (int l = 0; l < hull.Count; l++)
            {
                if (!connections.ContainsKey(l))
                {
                    Shape.Add(hull[l]);
                }
                else
                {
                    Shape.Add(hull[l]);
                    //Queue Holes with recursive function
                    MakeHoles(connections,l,Holes,Shape);
                    Shape.Add(hull[l]);
                }
            }

            EarTriangulator earTriangulator = new EarTriangulator();
            indices =  earTriangulator.Triangulate(Shape.ToArray());


            Vector3[] vertices = new Vector3[Shape.Count + 0];
            int i = 0;
            foreach (var item in Shape)
            {
                vertices[i] = new Vector3(Shape[i].x, Shape[i].y, 0);
                i++;
            }


            Mymesh.vertices = vertices;
            Mymesh.triangles = indices;

            Mymesh.RecalculateBounds();
            Mymesh.RecalculateNormals();

            GetComponent<MeshFilter>().mesh = Mymesh;
            GetComponent<MeshCollider>().sharedMesh = Mymesh;

            backface.GetComponent<MeshFilter>().mesh = Mymesh;
            backface.GetComponent<MeshCollider>().sharedMesh = Mymesh;
        }

    }
}