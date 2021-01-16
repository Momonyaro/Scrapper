using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonalNavMesh : MonoBehaviour
{
    private PolygonCollider2D polyCollider;
    public bool drawVisibilityLines = true;
    public Transform playerTest;
    public Transform endTest;
    
    // Start is called before the first frame update
    void Start()
    {
        if (polyCollider == null) polyCollider = GetComponent<PolygonCollider2D>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (polyCollider == null) polyCollider = GetComponent<PolygonCollider2D>();
        Vector2[] points = polyCollider.points;
        List<Vector2> concavePoints = new List<Vector2>();
        for (int i = 0; i < points.Length; i++)
        {
            if (IsVertexConcave(points.ToList(), i))
            {
                concavePoints.Add(points[i]);
                Gizmos.DrawLine(new Vector3(points[i].x - 0.5f, points[i].y), new Vector3(points[i].x + 0.5f, points[i].y));
                Gizmos.DrawLine(new Vector3(points[i].x, points[i].y - 0.5f), new Vector3(points[i].x, points[i].y + 0.5f));
            }
        }
        
        concavePoints.Add(playerTest.position);
        concavePoints.Add(endTest.position);

        if (drawVisibilityLines)
        {
            Gizmos.color = new Color(0.4f, 0, 0, 1);

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

        bool inLineOfSight = InLineOfSight(playerTest.position, endTest.position);
        Gizmos.color = (inLineOfSight) ? Color.green : Color.red;
        
        Gizmos.DrawLine(playerTest.position, endTest.position);
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
}
