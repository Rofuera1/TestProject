using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour, IPathFinder
{
    // Легче было использовать обратную кинематику, но о ней я узнал только после выполнения тестового. Постараюсь переписать!
    private List<Rectangle> rects;

    private List<(Vector2, Vector2)> leftRightVectors;
    private List<Vector2> focalPoints;
    private List<int> newPointsIDs;
    private bool isEndPointInLastSector;

    void Start()
    {
        Test();
        Debug.Break();  
    }
    public void Test()
    {
        rects = new List<Rectangle>();
        rects.Add(new Rectangle(new Vector2(-15, 15), new Vector2(2, 25)));
        //rects.Add(new Rectangle(new Vector2(-3, 25), new Vector2(17, 40))); // Когда квардат достаточно высокий
        rects.Add(new Rectangle(new Vector2(-3, 25), new Vector2(17, 32))); // Когда квадрат недостаточно высокий
        rects.Add(new Rectangle(new Vector2(17, 20), new Vector2(37, 30)));
        rects.Add(new Rectangle(new Vector2(37, 10), new Vector2(47, 25)));
        rects.Add(new Rectangle(new Vector2(47, 5), new Vector2(57, 20)));
        rects.Add(new Rectangle(new Vector2(57, 0), new Vector2(67, 15))); 
        rects.Add(new Rectangle(new Vector2(67, -5), new Vector2(77, 10))); 
        rects.Add(new Rectangle(new Vector2(77, -10), new Vector2(87, 5))); 
        rects.Add(new Rectangle(new Vector2(85, -20), new Vector2(95, -10))); 
        rects.Add(new Rectangle(new Vector2(87, -30), new Vector2(100, -20))); 
        rects.Add(new Rectangle(new Vector2(82, -40), new Vector2(97, -30))); 
        rects.Add(new Rectangle(new Vector2(90, -50), new Vector2(110, -40)));

        List<Vector2> edgeLines = new List<Vector2>();
        edgeLines.Add(new Vector2(-3, 25));
        edgeLines.Add(new Vector2(2, 25));
        edgeLines.Add(new Vector2(17, 25));
        edgeLines.Add(new Vector2(17, 30));
        edgeLines.Add(new Vector2(37, 20));
        edgeLines.Add(new Vector2(37, 25));
        edgeLines.Add(new Vector2(47, 10));
        edgeLines.Add(new Vector2(47, 20));
        edgeLines.Add(new Vector2(57, 10)); 
        edgeLines.Add(new Vector2(57, 15)); 
        edgeLines.Add(new Vector2(67, 5));  
        edgeLines.Add(new Vector2(67, 10)); 
        edgeLines.Add(new Vector2(77, 0));  
        edgeLines.Add(new Vector2(77, 5));  
        edgeLines.Add(new Vector2(85, -10));  
        edgeLines.Add(new Vector2(87, -10));  
        edgeLines.Add(new Vector2(87, -20));  
        edgeLines.Add(new Vector2(95, -20));  
        edgeLines.Add(new Vector2(87, -30));  
        edgeLines.Add(new Vector2(97, -30));  
        edgeLines.Add(new Vector2(90, -40));  
        edgeLines.Add(new Vector2(97, -40));  

        List<Edge> edges = new List<Edge>
        {
            new Edge(
                rects[0],
                rects[1],
                edgeLines[0],
                edgeLines[1]
            ),
            new Edge(
                rects[1],
                rects[2],
                edgeLines[2],
                edgeLines[3]
            ),
            new Edge(
                rects[2],
                rects[3],
                edgeLines[4],
                edgeLines[5]
            ),
            new Edge(
                rects[3],
                rects[4],
                edgeLines[6],
                edgeLines[7]
            ),
            new Edge(
                rects[4],
                rects[5],
                edgeLines[8],
                edgeLines[9]
            ),
            new Edge(
                rects[5],
                rects[6],
                edgeLines[10],
                edgeLines[11]
            ),
            new Edge(
                rects[6],
                rects[7],
                edgeLines[12],
                edgeLines[13]
            ),
            new Edge(
                rects[7],
                rects[8],
                edgeLines[14],
                edgeLines[15]
            ),
            new Edge(
                rects[8],
                rects[9],
                edgeLines[16],
                edgeLines[17]
            ),
            new Edge(
                rects[9],
                rects[10],
                edgeLines[18],
                edgeLines[19]
            ),
            new Edge(
                rects[10],
                rects[11],
                edgeLines[20],
                edgeLines[21]
            )
        };

        VisualizeRects(rects.ToArray(), Color.red);
        VisualizeEdges(edgeLines.ToArray(), Color.green);

        Vector2 A = new Vector2(-6.5f, 15f);
        //Vector2 B = new Vector2(93f, -50f); // Когда конец строится ровно
        Vector2 B = new Vector2(110f, -42f); // Когда конец строится криво

        IEnumerable<Vector2> vecs = GetPath(A, B, edges);
        VisualizePath(vecs, Color.green);

        Debug.DrawRay(A, Vector2.up * 10f + Vector2.right * 10f, Color.white);
        Debug.DrawRay(B, Vector2.up * 10f + Vector2.right * 10f, Color.white);

        //VisualizeBetaPath(Color.black, 5f);
    }

    public IEnumerable<Vector2> GetPath(Vector2 A, Vector2 C, IEnumerable<Edge> edges)
    {
        isEndPointInLastSector = false;
        leftRightVectors = new List<(Vector2, Vector2)>();
        focalPoints = new List<Vector2>();
        newPointsIDs = new List<int>();

        Vector2 curPoint = A;
        Vector2 rightVec, leftVec;
        bool doHavePoint = true;
        Edge edgeForPoint = edges.First();

        (rightVec, leftVec) = calculateVectorsThroughEdge(edges.First(), curPoint);

        int id = 0;

        foreach (Edge edge in edges.Skip(1))
        {
            id++;

            if (!doHavePoint)
            {
                curPoint = calculateNewFocalPoint(edgeForPoint, edge);
                (rightVec, leftVec) = calculateVectorsThroughEdge(edge, curPoint);
                doHavePoint = true;
                continue;
            }

            Vector2 newRightVec, newLeftVec;
            (newRightVec, newLeftVec) = calculateVectorsThroughEdge(edge, curPoint);

            (Vector2, Vector2) resultRef = (Vector2.zero, Vector2.zero);

            if (calculateNewAreaContainedInsideOther((rightVec, leftVec), (newRightVec, newLeftVec), ref resultRef))
            {
                (leftVec, rightVec) = resultRef;
            }
            else
            {
                leftRightVectors.Add((leftVec, rightVec));
                focalPoints.Add(curPoint);

                doHavePoint = false;
                edgeForPoint = edge;

                newPointsIDs.Add(id);
            }
        }

        Vector2 endVector = C - curPoint;
        (Vector2, Vector2) endRef = (Vector2.zero, Vector2.zero);

        List<Vector2> result = new List<Vector2>() { C };
        List<Vector2> result2 = new List<Vector2>() { C };

        if (calculateNewAreaContainedInsideOther((rightVec, leftVec), (endVector, endVector), ref endRef))
        {
            (leftVec, rightVec) = endRef;
            curPoint = C;
            isEndPointInLastSector = true;
        }

        result.AddRange(buildPathToStart(leftRightVectors.Count - 1, new List<Vector2>() , leftVec, curPoint));
        result2.AddRange(buildPathToStart(leftRightVectors.Count - 1, new List<Vector2>(), rightVec, curPoint));

        result.Add(A);
        result2.Add(A);

        if (result.Count != 0 && result.Count < result2.Count)
        {
            if(!isEndPointInLastSector)
            {
                Vector2 lastPointRotation = IntersectionOfVectorWithRect(result[1], leftVec, rects.Last())[1];
                result.Insert(1, lastPointRotation);
            }

            return result;
        }
        else
        {
            if (!isEndPointInLastSector)
            {
                Vector2 lastPointRotation = IntersectionOfVectorWithRect(result2[1], rightVec, rects.Last())[1];
                result2.Insert(1, lastPointRotation);
            }

            return result2;
        }
    }

    private List<Vector2> buildPathToStart(int id, List<Vector2> vecs, Vector2 curVec, Vector2 curPoint)
    {
        if (id < 0)
            return vecs;

        List<Vector2> newVec_1 = new List<Vector2>();
        List<Vector2> newVec_2 = new List<Vector2>();

        int idOfRect = newPointsIDs[id];

        Vector2 intersection_1 = computeCrossPoint(curPoint, curVec, focalPoints[id], leftRightVectors[id].Item1);

        Debug.DrawRay(intersection_1, -Vector2.one, Color.yellow, 1f);

        if (doesPointLandInRect(intersection_1, rects[idOfRect]))
        {
            newVec_1.Add(intersection_1);

            List<Vector2> res = buildPathToStart(id - 1, newVec_1, leftRightVectors[id].Item1, intersection_1);

            if (res != null)
            {
                newVec_1.AddRange(res);
            }
        }
        else
        {
            Vector2 intersectionWithRectFirst = IntersectionOfVectorWithRect(curPoint, -curVec, rects[idOfRect])[1];
            Vector2 intersectionWithRectSecond = IntersectionOfVectorWithRect(intersection_1, -leftRightVectors[id].Item1, rects[idOfRect])[0];

            newVec_1.Add(intersectionWithRectFirst);
            newVec_1.Add(intersectionWithRectSecond);
        }
        
        Vector2 intersection_2 = computeCrossPoint(curPoint, curVec, focalPoints[id], leftRightVectors[id].Item2);
        Debug.DrawRay(intersection_2, -Vector2.one, Color.yellow, 1f);

        if (doesPointLandInRect(intersection_2, rects[idOfRect]))
        {
            newVec_2.Add(intersection_2);

            List<Vector2> res = buildPathToStart(id - 1, newVec_2, leftRightVectors[id].Item2, intersection_2);

            if (res != null)
            {
                newVec_2.AddRange(res);
            }
        }
        else
        {
            Vector2 intersectionWithRectFirst = IntersectionOfVectorWithRect(curPoint, -curVec, rects[idOfRect])[1];
            Vector2 intersectionWithRectSecond = IntersectionOfVectorWithRect(intersection_2, -leftRightVectors[id].Item2, rects[idOfRect])[0];

            newVec_2.Add(intersectionWithRectFirst);
            newVec_2.Add(intersectionWithRectSecond);
        }

        if (newVec_1.Count != 0 && newVec_1.Count < newVec_2.Count)
            return newVec_1;
        else
            return newVec_2;
    }

    private List<Vector2> IntersectionOfVectorWithRect(Vector2 startPoint, Vector2 vec, Rectangle rect)
    {
        List<Vector2> intersections = new List<Vector2>();

        Vector2 topLeft = new Vector2(rect.Min.x, rect.Max.y);
        Vector2 topRight = rect.Max;
        Vector2 bottomLeft = rect.Min;
        Vector2 bottomRight = new Vector2(rect.Max.x, rect.Min.y);

        Vector2[] edgesStart = { bottomLeft, topLeft, topRight, bottomRight };
        Vector2[] edgesEnd = { topLeft, topRight, bottomRight, bottomLeft };

        for (int i = 0; i < edgesStart.Length; i++)
        {
            Vector2? intersection = RaySegmentIntersection(startPoint, vec, edgesStart[i], edgesEnd[i]);
            if (intersection.HasValue)
            {
                intersections.Add(intersection.Value);
            }
        }

        return intersections;
    }
    private Vector2? RaySegmentIntersection(Vector2 rayStart, Vector2 rayVec, Vector2 segStart, Vector2 segEnd)
    {
        Vector2 segVec = segEnd - segStart;
        float cross = rayVec.x * segVec.y - rayVec.y * segVec.x;

        if (Mathf.Abs(cross) < 1e-6)
        {
            return null;
        }

        Vector2 delta = segStart - rayStart;

        float t = (delta.x * segVec.y - delta.y * segVec.x) / cross;
        float u = (delta.x * rayVec.y - delta.y * rayVec.x) / cross;

        if (u < 0 || u > 1)
        {
            return null;
        }

        if (t < 0)
        {
            return null;
        }

        return rayStart + t * rayVec;
    }

    private bool doesPointLandInRect(Vector2 point, Rectangle rect)
    {
        return (point.x >= rect.Min.x && point.x <= rect.Max.x && point.y >= rect.Min.y && point.y <= rect.Max.y);
    }

    private Vector2 computeCrossPoint(Vector2 a1, Vector2 d1, Vector2 a2, Vector2 d2)
    {
        float delta = d1.x * d2.y - d1.y * d2.x;

        if (Mathf.Abs(delta) < 1e-6)
        {
            Debug.LogWarning("Прямые совпадают или параллельны");
            return Vector2.zero;
        }

        float t = ((a2.x - a1.x) * d2.y - (a2.y - a1.y) * d2.x) / delta;
        Vector2 intersection = a1 + t * d1;

        return intersection;
    }

    private Vector2 calculateNewFocalPoint(Edge prevEdge, Edge newEdge)
    {
        Vector2 lowPoint1, highPoint1, lowPoint2, highPoint2;
        (lowPoint1, highPoint1) = alignLowHighPoint(prevEdge);
        (lowPoint2, highPoint2) = alignLowHighPoint(newEdge);

        Vector2 firstCross = highPoint2 - lowPoint1;
        Vector2 secondCross = highPoint1 - lowPoint2;
        
        float det = firstCross.x * secondCross.y - firstCross.y * secondCross.x;

        Vector2 delta = lowPoint2 - lowPoint1;
        float t = (delta.x * secondCross.y - delta.y * secondCross.x) / det;

        return lowPoint1 + t * firstCross;
    }

    private (Vector2, Vector2) calculateVectorsThroughEdge(Edge edge, Vector2 point)
    {
        Vector2 lowPoint, highPoint;
        (lowPoint, highPoint) = alignLowHighPoint(edge);

        return (lowPoint - point, highPoint - point);
    }

    private bool calculateNewAreaContainedInsideOther((Vector2, Vector2) neededArea, (Vector2, Vector2) newArea, ref (Vector2, Vector2) resultArea)
    {
        float CalculateAngle(Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x);
        }

        float angleNeeded1 = CalculateAngle(neededArea.Item1);
        float angleNeeded2 = CalculateAngle(neededArea.Item2);
        float angleNew1 = CalculateAngle(newArea.Item1);
        float angleNew2 = CalculateAngle(newArea.Item2);

        if (angleNeeded1 > angleNeeded2)
        {
            (angleNeeded1, angleNeeded2) = (angleNeeded2, angleNeeded1);
        }

        bool isNew1Inside = angleNew1 >= angleNeeded1 && angleNew1 <= angleNeeded2;
        bool isNew2Inside = angleNew2 >= angleNeeded1 && angleNew2 <= angleNeeded2;

        if (isNew1Inside && isNew2Inside)
        {
            resultArea = (newArea.Item1, newArea.Item2);
            return true;
        }
        else if (isNew1Inside)
        {
            var nearestNeededVector = Mathf.Abs(angleNew1 - angleNeeded1) < Mathf.Abs(angleNew1 - angleNeeded2)
                ? neededArea.Item1
                : neededArea.Item2;
            resultArea = (newArea.Item1, nearestNeededVector);
            return true;
        }
        else if (isNew2Inside)
        {
            var nearestNeededVector = Mathf.Abs(angleNew2 - angleNeeded1) < Mathf.Abs(angleNew2 - angleNeeded2)
                ? neededArea.Item1
                : neededArea.Item2;
            resultArea = (newArea.Item2, nearestNeededVector);
            return true;
        }

        return false;
    }

    private (Vector2, Vector2) alignLowHighPoint(Edge edge)
    {
        Vector2 lowPoint = edge.Start;
        Vector2 highPoint = edge.End;

        if ((lowPoint.x == highPoint.x && lowPoint.y > highPoint.y)
            || (lowPoint.y == highPoint.y && lowPoint.x > highPoint.x))
        {
            (lowPoint, highPoint) = (highPoint, lowPoint);
        }

        return (lowPoint, highPoint);
    }

    private void VisualizePath(List<Vector2> points, Color col, float duration = 5f)
    {
        for (int i = 0; i < points.Count() - 1; i++)
            Debug.DrawLine(points[i], points[i + 1], col, duration);
    }

    private void VisualizeBetaPath(Color col, float duration = 5f)
    {
        for(int i = 0; i < leftRightVectors.Count; i++)
        {
            Debug.DrawRay(focalPoints[i], leftRightVectors[i].Item1.normalized * 100f, col, duration);
            Debug.DrawRay(focalPoints[i], leftRightVectors[i].Item2.normalized * 100f, col, duration);
        }
    }

    public void VisualizePath(IEnumerable<Vector2> path, Color color, float duration = 5f)
    {
        if (path == null || !path.Any())
        {
            return;
        }

        var points = path.ToArray();
        for (int i = 0; i < points.Length - 1; i++)
        {
            Debug.DrawLine(points[i], points[i + 1], color, duration);
        }
    }

    public void VisualizeEdges(Vector2[] vecs, Color col, float dur = 5f)
    {
        for(int i = 0; i < vecs.Length-1; i+=2)
        {
            Debug.DrawLine(vecs[i], vecs[i + 1], col, dur);
        }
    }

    public void VisualizeRects(Rectangle[] rects, Color color, float duration = 5f)
    {
        foreach(Rectangle rec in rects)
        {
            Debug.DrawLine(rec.Min, new Vector3(rec.Min.x, rec.Max.y), color, duration);
            Debug.DrawLine(new Vector3(rec.Min.x, rec.Max.y), rec.Max, color, duration);
            Debug.DrawLine(rec.Max, new Vector3(rec.Max.x, rec.Min.y), color, duration);
            Debug.DrawLine(new Vector3(rec.Max.x, rec.Min.y), rec.Min, color, duration);
        }
    }
}

public interface IPathFinder
{
    IEnumerable<Vector2> GetPath(Vector2 A, Vector2 C, IEnumerable<Edge> edges);
}

public struct Edge
{
    public Rectangle First;
    public Rectangle Second;
    public Vector2 Start;
    public Vector2 End;

    public Edge(Rectangle first, Rectangle second, Vector2 start, Vector2 end)
    {
        First = first;
        Second = second;
        Start = start;
        End = end;
    }
}

public struct Rectangle
{
    public Vector2 Min;
    public Vector2 Max;

    public Rectangle(Vector2 min, Vector2 max)
    {
        Min = min;
        Max = max;
    }
}
