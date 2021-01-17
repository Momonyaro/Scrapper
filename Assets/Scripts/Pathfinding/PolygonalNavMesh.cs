using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Scrapper.Pathfinding
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class PolygonalNavMesh : MonoBehaviour
    {
        private PolygonCollider2D polyCollider;
        public bool drawVisibilityLines = true;
        public bool drawNavMesh = true;
        
        [SerializeField] private Vector2[] path = new Vector2[0];
    
        // Start is called before the first frame update
        void Start()
        {
            if (polyCollider == null) polyCollider = GetComponent<PolygonCollider2D>();
        }

        public Vector2[] GetShortestPath(Vector2 start, Vector2 end)
        {
            List<DijkstraNode> vertexSet = new List<DijkstraNode>();
            List<DijkstraNode> graph = BuildGraph(start, end);

            for (int i = 0; i < graph.Count; i++)
            {
                DijkstraNode v = graph[i];
                v.SetNodeDistance(float.MaxValue);
                if (v.GetPosition() == start)
                    v.SetNodeDistance(0);
                v.SetPrevious(null);
                vertexSet.Add(v);
            }

            while (vertexSet.Count > 0)
            {
                DijkstraNode u = vertexSet[0];
                for (int i = 0; i < vertexSet.Count; i++)
                {
                    float distance = vertexSet[i].GetDistance();
                    if (distance < u.GetDistance())
                        u = vertexSet[i];
                }

                vertexSet.Remove(u);

                if (u.GetPosition() == end)
                {
                    Stack<DijkstraNode> s = new Stack<DijkstraNode>();

                    while (true)
                    {
                        s.Push(u);
                        if (u.GetPrevious() == null) break;
                        u = u.GetPrevious();
                    }

                    List<Vector2> path = new List<Vector2>();
                    while (s.Count > 0)
                    {
                        path.Add(s.Pop().GetPosition());
                    }

                    return path.ToArray();
                }

                foreach (var neighbor in u.GetNeighbors())
                {
                    float alt = u.GetDistance() + Vector2.Distance(u.GetPosition(), neighbor.GetPosition());
                    if (alt < neighbor.GetDistance())
                    {
                        neighbor.SetNodeDistance(alt);
                        neighbor.SetPrevious(u);
                    }
                }
            }

            return new Vector2[0];
        }

        private List<DijkstraNode> BuildGraph(Vector2 start, Vector2 end)
        {
            List<DijkstraNode> toReturn = new List<DijkstraNode>();

            List<Vector2> points = polyCollider.points.ToList();

            List<Vector2> concavePoints = new List<Vector2>();
            
            concavePoints.Add(start);
            for (int i = 0; i < points.Count; i++)
            {
                if (IsVertexConcave(points, i))
                {
                    concavePoints.Add(points[i]);
                }
            }
            concavePoints.Add(end);
            
            //Convert to dijkstra nodes.
            for (int i = 0; i < concavePoints.Count; i++)
            {
                toReturn.Add(new DijkstraNode(concavePoints[i], new List<DijkstraNode>()));
            }

            for (int i = 0; i < toReturn.Count; i++)
            {
                for (int j = 0; j < toReturn.Count; j++)
                {
                    if (i == j) continue;

                    if (InLineOfSight(toReturn[i].GetPosition(), toReturn[j].GetPosition()))
                    {
                        toReturn[i].AddNeighbor(toReturn[j]);
                        toReturn[j].AddNeighbor(toReturn[i]);
                    }
                }
            }

            return toReturn;
        }

        private void OnDrawGizmos()
        {
            if (polyCollider == null)
                polyCollider = GetComponent<PolygonCollider2D>();
            
            Gizmos.color = new Color(0.4f, 0, 0);
            if (drawNavMesh)
            {
                for (int i = 0; i < polyCollider.points.Length; i++)
                {
                    Gizmos.DrawLine(polyCollider.points[i], (i == 0) ? polyCollider.points[polyCollider.points.Length - 1] 
                        : polyCollider.points[i - 1]);
                }
            }

            if (drawVisibilityLines)
            {
                drawVisibilityLines = false;
                List<Vector2> points = polyCollider.points.ToList();
                List<Vector2> concavePoints = new List<Vector2>();
                for (int i = 0; i < points.Count; i++)
                {
                    if (IsVertexConcave(points, i))
                    {
                        concavePoints.Add(points[i]);
                    }
                }
                
                for (int i = 0; i < concavePoints.Count; i++)
                {
                    for (int j = 0; j < concavePoints.Count; j++)
                    {
                        if (i == j) continue;

                        if (InLineOfSight(concavePoints[i], concavePoints[j]))
                        {
                            Gizmos.DrawLine(concavePoints[i], concavePoints[j]);
                        }
                    }
                }
            }
            
            Gizmos.color = Color.green;
            for (int i = 1; i < path.Length; i++)
            {
                Gizmos.DrawLine(path[i], path[i - 1]);
            }
        }

        bool InLineOfSight(Vector2 start, Vector2 end)
        {
            // Not in LOS if any of the ends is outside the polygon
            if (!Inside(new List<Vector2>(polyCollider.points), start) || !Inside(new List<Vector2>(polyCollider.points), end)) return false;

            // In LOS if it's the same start and end location
            if (Vector2.Distance(start, end) < float.Epsilon) return true;

            // Not in LOS if any edge is intersected by the start-end line segment
            int n = polyCollider.points.Length;
            for (int i = 0; i < n; i++)
                if (LineSegmentsCross(start, end, polyCollider.points[i], polyCollider.points[(i+1)%n]))
                    return false;

            // Finally the middle point in the segment determines if in LOS or not
            return Inside(polyCollider.points.ToList(), (start + end) / 2f);
        }
    
        public static bool IsVertexConcave(List<Vector2> vertices, int vertex)
        {
            Vector2 current = vertices[vertex];
            Vector2 next = vertices[(vertex + 1) % vertices.Count];
            Vector2 previous = vertices[vertex == 0 ? vertices.Count - 1 : vertex - 1];

            Vector2 left = new Vector2(current.x - previous.x, current.y - previous.y);
            Vector2 right = new Vector2(next.x - current.x, next.y - current.y);

            float cross = (left.x * right.y) - (left.y * right.x);

            return cross < 0;
        }
    
        public static bool LineSegmentsCross(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            float denominator = ((b.x - a.x) * (d.y - c.y)) - ((b.y - a.y) * (d.x - c.x));

            if (denominator == 0)
            {
                return false;
            }

            float numerator1 = ((a.y - c.y) * (d.x - c.x)) - ((a.x - c.x) * (d.y - c.y));

            float numerator2 = ((a.y - c.y) * (b.x - a.x)) - ((a.x - c.x) * (b.y - a.y));

            if (numerator1 == 0 || numerator2 == 0)
            {
                return false;
            }

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;

            return (r > 0 && r < 1) && (s > 0 && s < 1);
        }
    
        public static bool Inside(List<Vector2> polygon, Vector2 position, bool toleranceOnOutside = true)
        {
            Vector2 point = position;

            const float epsilon = 0.5f;

            bool inside = false;

            // Must have 3 or more edges
            if (polygon.Count < 3) return false;

            Vector2 oldPoint = polygon[polygon.Count - 1];
            float oldSqDist = Mathf.Pow(Vector2.Distance(oldPoint, point), 2);

            for (int i = 0; i < polygon.Count; i++)
            {
                Vector2 newPoint = polygon[i];
                float newSqDist = Mathf.Pow(Vector2.Distance(newPoint, point), 2);

                if (oldSqDist + newSqDist + 2.0f * System.Math.Sqrt(oldSqDist * newSqDist) - Mathf.Pow(Vector2.Distance(newPoint, oldPoint) , 2) < epsilon)
                    return toleranceOnOutside;

                Vector2 left;
                Vector2 right;
                if (newPoint.x > oldPoint.x)
                {
                    left = oldPoint;
                    right = newPoint;
                }
                else
                {
                    left = newPoint;
                    right = oldPoint;
                }

                if (left.x < point.x && point.x <= right.x && (point.y - left.y) * (right.x - left.x) < (right.y - left.y) * (point.x - left.x))
                    inside = !inside;

                oldPoint = newPoint;
                oldSqDist = newSqDist;
            }

            return inside;
        }

        public class DijkstraNode
        {
            private float distance;
            private Vector2 position;
            private DijkstraNode previous;
            private List<DijkstraNode> neighbors;

            public DijkstraNode(Vector2 position, List<DijkstraNode> neighbors, float distance = -1)
            {
                this.position = position;
                this.neighbors = neighbors;
                this.distance = distance;
                previous = null;
            }

            public void SetNodePosition(Vector2 position)
            {
                this.position = position;
            }
            
            public void SetNodeDistance(float newDistance)
            {
                distance = newDistance;
            }

            public void SetPrevious(DijkstraNode previous)
            {
                this.previous = previous;
            }

            public List<DijkstraNode> GetNeighbors()
            {
                return neighbors;
            }

            public void AddNeighbor(DijkstraNode newNeighbor)
            {
                if (neighbors.Contains(newNeighbor)) return;
                
                neighbors.Add(newNeighbor);
            }
            
            public Vector2 GetPosition()
            {
                return position;
            }
            
            public float GetDistance()
            {
                return distance;
            }

            public DijkstraNode GetPrevious()
            {
                return previous;
            }
        }
    }
}
