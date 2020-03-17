using System.Collections.Generic;
using UnityEngine;


namespace Destructable
{

    public class EarTriangulator
    {
        //    EarClippingTriangualtion algorithm
        //
        //    Input: A simple polygon P with n vertices V(v0, v1,…, vn-1)
        //    Output: A triangulation T with n–2 triangles
        //    1:  Compute interior angles of each vertex in P.
        //    2:  Indentify each vertex whether it is an ear tip or not. 
        //    3:  while number of triangles in T<n–2 do 
        //    4:  Find the ear tip vi which has the smallest interior angle.
        //    5:  Construct a triangle (vi－1, vi, vi+1) and add it onto T. 
        //    6:  Let vi be no longer an ear tip. 
        //    7:  Update connection relationship of vi－1 and vi, vi and vi+1, vi－1 and vi+1.
        //    8:  Compute the interior angles of vi－1 and vi+1.
        //    9:  Refresh the ear tip status of vi－1 and vi+1.
        //    10: end while
        //


      // LinkedList<Vector2> VertsOfShape = new LinkedList<Vector2>();
 

        public EarTriangulator()
        {

        }
        bool IsConvex(Vector2 v0, Vector2 v1, Vector2 v2)
        {
            return SideOfLine(v0, v1, v2) == -1;
        }

        public static int SideOfLine(Vector2 a, Vector2 b, Vector2 c)
        {
            // sing of the dot product gives a angle is convex or not a · b = ax × bx + ay × by) 1 or -1
            return (int)Mathf.Sign((c.x - a.x) * (-b.y + a.y) + (c.y - a.y) * (b.x - a.x));
        }


        bool TriangleContainsVertex(Vector2 v0, Vector2 v1, Vector2 v2,LinkedList<Vertex> VertsOfShape)
        {
            LinkedListNode<Vertex> vertexNode = VertsOfShape.First;
            for (int i = 0; i < VertsOfShape.Count; i++)
            {
                if (!vertexNode.Value.isConvex) // convex verts will never be inside triangle
                {
                    Vector2 vertexToCheck = vertexNode.Value.position;
                    if (vertexToCheck != v0 && vertexToCheck != v1 && vertexToCheck != v2) // dont check verts that make up triangle
                    {
                        if (PointInTriangle(v0, v1, v2, vertexToCheck))
                        {
                            return true;
                        }
                    }
                }
                vertexNode = vertexNode.Next;
            }
        
            return false;
        }
        bool PointsAreCounterClockwise(Vector2[] testPoints)
        {
            float signedArea = 0;
            for (int i = 0; i < testPoints.Length; i++)
            {
                int nextIndex = (i + 1) % testPoints.Length;
                signedArea += (testPoints[nextIndex].x - testPoints[i].x) * (testPoints[nextIndex].y + testPoints[i].y);
            }

            return signedArea < 0;
        }

        bool PointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
        {
            float area = 0.5f * (-b.y * c.x + a.y * (-b.x + c.x) + a.x * (b.y - c.y) + b.x * c.y);
            float s = 1 / (2 * area) * (a.y * c.x - a.x * c.y + (c.y - a.y) * p.x + (a.x - c.x) * p.y);
            float t = 1 / (2 * area) * (a.x * b.y - a.y * b.x + (a.y - b.y) * p.x + (b.x - a.x) * p.y);
            return s >= 0 && t >= 0 && (s + t) <= 1;
        
        }

        LinkedList<Vertex> ToLinkedList(Vector2[] list)
        {
            LinkedList<Vertex> VertexList = new LinkedList<Vertex>();
            for (int i = 0; i < list.Length; i++)
            {
                int prevPointIndex = (i - 1 + list.Length) % list.Length;
                int nextPointIndex = (i + 1) % list.Length;

              //  Debug.Log(IsConvex(list[prevPointIndex], list[i], list[nextPointIndex]));
                VertexList.AddLast(new Vertex(list[i],i,IsConvex(list[prevPointIndex],list[i],list[nextPointIndex])));
            }
            
            return VertexList;
        }


        public int[] Triangulate(Vector2[] PoligonVectors)
        {
            LinkedList<Vertex> VertsOfShape = ToLinkedList(PoligonVectors);
            List<int> tris = new List<int>();
            int triIndex=0;

            while (VertsOfShape.Count >= 3)
            {
                bool hasRemovedEarThisIteration = false;
                LinkedListNode<Vertex> vertexNode = VertsOfShape.First;

                for (int i = 0; i < VertsOfShape.Count; i++)
                {
                    LinkedListNode<Vertex> prevVertexNode = vertexNode.Previous ?? VertsOfShape.Last;
                    LinkedListNode<Vertex> nextVertexNode = vertexNode.Next ?? VertsOfShape.First;
                   // Debug.Log(vertexNode.Value.isConvex);
                    if (vertexNode.Value.isConvex)
                    {
                        if (!TriangleContainsVertex(prevVertexNode.Value.position, vertexNode.Value.position, nextVertexNode.Value.position,VertsOfShape))
                        {

                            // check if removal of ear makes prev/next vertex convex (if was previously reflex)
                            if (!prevVertexNode.Value.isConvex)
                            {
                                LinkedListNode<Vertex> prevOfPrev = prevVertexNode.Previous ?? VertsOfShape.Last;

                                prevVertexNode.Value.isConvex = IsConvex(prevOfPrev.Value.position, prevVertexNode.Value.position, nextVertexNode.Value.position);
                            }
                            if (!nextVertexNode.Value.isConvex)
                            {
                                LinkedListNode<Vertex> nextOfNext = nextVertexNode.Next ?? VertsOfShape.First;
                                nextVertexNode.Value.isConvex = IsConvex(prevVertexNode.Value.position, nextVertexNode.Value.position, nextOfNext.Value.position);
                            }

                            // add triangle to tri array
                            if (true)
                            {
                                tris.Add(vertexNode.Value.index);
                                tris.Add(nextVertexNode.Value.index);
                                tris.Add(prevVertexNode.Value.index);
                                triIndex++;
                            }
                            

                            hasRemovedEarThisIteration = true;
                            VertsOfShape.Remove(vertexNode);
                            break;
                        }

                    }

                    vertexNode = nextVertexNode;
                }

                if (!hasRemovedEarThisIteration)
                {
                    Debug.LogError("Error triangulating mesh. Aborted.");
                    return null;
                }
            }
            return tris.ToArray();
        }


        public class Vertex
        {
            public readonly Vector2 position;
            public readonly int index;
            public bool isConvex;

            public Vertex(Vector2 position, int index, bool isConvex)
            {
                this.position = position;
                this.index = index;
                this.isConvex = isConvex;
            }
        }
    }
}
