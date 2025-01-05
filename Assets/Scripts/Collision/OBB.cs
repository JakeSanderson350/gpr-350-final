using UnityEngine;
using System.Collections;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class OBB : PhysicsCollider
{
    // TODO: YOUR CODE HERE
    private Vector3 center;
    private Vector3 halfWidth = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3[] axes = new Vector3[3];

    Vector3 toWorld(Vector3 _localPoint)
    {
        //return transform.localToWorldMatrix * _localPoint;
        //return transform.TransformPoint(_localPoint);
        Vector3 rotatedDirection = (axes[0] * _localPoint.x) + (axes[1] * _localPoint.y) + (axes[2] * _localPoint.z);
        return transform.position + rotatedDirection;
    }
    Vector3 toLocal(Vector3 _worldPoint)
    {
        return transform.InverseTransformPoint(_worldPoint);
    }

    public void setHalfWidth(Vector3 _halfWidth)
    {
        halfWidth = _halfWidth;
    }

    public void setAxes(Matrix3x3 _axes)
    {
        axes[0] = new Vector3(_axes.values[0, 0], _axes.values[1, 0], _axes.values[2, 0]).normalized;
        axes[1] = new Vector3(_axes.values[0, 1], _axes.values[1, 1], _axes.values[2, 1]).normalized;
        axes[2] = new Vector3(_axes.values[0, 2], _axes.values[1, 2], _axes.values[2, 2]).normalized;

        //Debug.DrawLine(transform.position, transform.position + axes[0], Color.red);
        //Debug.DrawLine(transform.position, transform.position + axes[1], Color.green);
        //Debug.DrawLine(transform.position, transform.position + axes[2], Color.blue);
    }

    public Vector3[] getAxes()
    {
        return axes;
    }

    public Vector3 getAxis(int _index) 
    {
        if (_index < axes.Length || _index >= 0)
        {
            return axes[_index];
        }
        else
        {
            Debug.Log("Index out of Bounds");
            return Vector3.zero;
        }
    }

    public void getClosestPoint (Vector3 _worldPoint, out Vector3 closestPoint)
    {
        Vector3 sphereCenterLocal = toLocal(_worldPoint);
        center = toLocal(transform.position);

        Vector3 min = center - halfWidth;
        Vector3 max = center + halfWidth;

        closestPoint = new Vector3(
            Mathf.Clamp(sphereCenterLocal.x, min.x, max.x),
            Mathf.Clamp(sphereCenterLocal.y, min.y, max.y),
            Mathf.Clamp(sphereCenterLocal.z, min.z, max.z)
         );

        closestPoint = toWorld(closestPoint);
    }

    public Vector3[] getVertices()
    {
        Vector3[] vertices = new Vector3[8];

        // Pre-calculate all possible combinations of extents
        Vector3[] directions = new Vector3[8];
        directions[0] = new Vector3(halfWidth.x, halfWidth.y, halfWidth.z);
        directions[1] = new Vector3(halfWidth.x, halfWidth.y, -halfWidth.z);
        directions[2] = new Vector3(halfWidth.x, -halfWidth.y, halfWidth.z);
        directions[3] = new Vector3(halfWidth.x, -halfWidth.y, -halfWidth.z);
        directions[4] = new Vector3(-halfWidth.x, halfWidth.y, halfWidth.z);
        directions[5] = new Vector3(-halfWidth.x, halfWidth.y, -halfWidth.z);
        directions[6] = new Vector3(-halfWidth.x, -halfWidth.y, halfWidth.z);
        directions[7] = new Vector3(-halfWidth.x, -halfWidth.y, -halfWidth.z);

        for (int i = 0; i < 8; i++)
        {
            // Apply axes to each direction vector
            Vector3 rotatedDir = (axes[0] * directions[i].x) + (axes[1] * directions[i].y) + (axes[2] * directions[i].z);

            // Convert to world space by adding the position
            vertices[i] = transform.position + rotatedDir;

            //Debug.DrawLine(transform.position, vertices[i], Color.white);
        }

        return vertices;
    }
    //********************************************************************************
    //* SAT functions
    //********************************************************************************
    public static bool SATintersect(OBB b1, OBB b2)
    {
        //List of axes to test
        Vector3[] axesToTest = new Vector3[15];

        //Axes of b1
        axesToTest[0] = b1.getAxis(0);
        axesToTest[1] = b1.getAxis(1);
        axesToTest[2] = b1.getAxis(2);

        //Axes of b2
        axesToTest[3] = b2.getAxis(0);
        axesToTest[4] = b2.getAxis(1);
        axesToTest[5] = b2.getAxis(2);

        //Edges of both boxes
        axesToTest[6] = Vector3.Cross(b1.getAxis(0), b2.getAxis(0));
        axesToTest[7] = Vector3.Cross(b1.getAxis(0), b2.getAxis(1));
        axesToTest[8] = Vector3.Cross(b1.getAxis(0), b2.getAxis(2));
        axesToTest[9] = Vector3.Cross(b1.getAxis(1), b2.getAxis(0));
        axesToTest[10] = Vector3.Cross(b1.getAxis(1), b2.getAxis(1));
        axesToTest[11] = Vector3.Cross(b1.getAxis(1), b2.getAxis(2));
        axesToTest[12] = Vector3.Cross(b1.getAxis(2), b2.getAxis(0));
        axesToTest[13] = Vector3.Cross(b1.getAxis(2), b2.getAxis(1));
        axesToTest[14] = Vector3.Cross(b1.getAxis(2), b2.getAxis(2));

        for (int i = 0; i < axesToTest.Length; i++)
        {
            if (!overlapOnAxis(b1, b2, axesToTest[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool overlapOnAxis(OBB b1, OBB b2, Vector3 _axis)
    {
        Vector2 interval1 = getInterval(b1, _axis);
        Vector2 interval2 = getInterval(b2, _axis);

        return (interval2.x <= interval1.y) && (interval1.x <= interval2.y);
    }

    private static Vector2 getInterval(OBB _rect, Vector3 _axis)
    {
        Vector2 result = Vector2.zero;

        Vector3[] vertices = _rect.getVertices();

        result.x = Vector3.Dot(_axis, vertices[0]);
        result.y = result.x;
        //Loop through vertices and find lowest point on axis and highest point on axis
        foreach (Vector3 v in vertices)
        {
            float projection = Vector3.Dot(_axis, v);

            result.x = Mathf.Min(result.x, projection);
            result.y = Mathf.Max(result.y, projection);
        }

        return result;
    }

    //********************************************************************************
    //* Collision resolution functions
    //********************************************************************************
    private static float transformToAxis(OBB _rect, Vector3 _axis)
    {
        return _rect.halfWidth.x * Mathf.Abs(Vector3.Dot(_axis, _rect.getAxis(0))) +
               _rect.halfWidth.y * Mathf.Abs(Vector3.Dot(_axis, _rect.getAxis(1))) +
               _rect.halfWidth.z * Mathf.Abs(Vector3.Dot(_axis, _rect.getAxis(2)));
    }

    private static float penetrationOnAxis(OBB b1, OBB b2, Vector3 _axis, Vector3 _centers)
    {
        //Project half lengths onto axis
        float b1Projection = transformToAxis(b1, _axis);
        float b2Projection = transformToAxis(b2, _axis);

        //Project difference of centers onto axis
        float distance = Mathf.Abs(Vector3.Dot(_centers, _axis));

        //Return overlap, positive = penetration, negative = separation
        return b1Projection + b2Projection - distance;
    }

    public static bool tryAxis(OBB b1, OBB b2, Vector3 _axis, Vector3 _centers, int _index, ref float minPenetration, ref int minIndex)
    {
        float penetration = penetrationOnAxis(b1, b2, _axis, _centers);

        if (penetration < 0)
        {
            return false;
        }
        if (penetration < minPenetration)
        {
            minPenetration = penetration;
            minIndex = _index;
        }
        return true;
    }

    //Called when vertex from box2 is in a face of box1
    public static void vertexFaceCollision(OBB b1, OBB b2, Vector3 _centers, int _minIndex, out Vector3 normal, out Vector3 contactPoint)
    {
        //Pick one of two faces on axis of box1 that is in the direction of box2
        normal = b1.getAxis(_minIndex);
        if (Vector3.Dot(b1.getAxis(_minIndex), _centers) < 0)
        {
            normal *= -1.0f;
        }

        //Find vertex of box2 that is colliding with box1
        Vector3 localVertex = b2.halfWidth;
        if (Vector3.Dot(b2.getAxis(0), normal) > 0) localVertex.x = -localVertex.x;
        if (Vector3.Dot(b2.getAxis(1), normal) > 0) localVertex.y = -localVertex.y;
        if (Vector3.Dot(b2.getAxis(2), normal) > 0) localVertex.z = -localVertex.z;

        //Convert to world coordinates
        contactPoint = b2.toWorld(localVertex);
    }

    private static Vector3 getContactPoint(Vector3 _onePoint, Vector3 _oneAxis, float _oneSize, Vector3 _twoPoint, Vector3 _twoAxis, float _twoSize, bool useOne)
    {
        Vector3 midpoints, contactOne, contactTwo;
        float dpMidOne, dpMidTwo, dpOneTwo, smOne, smTwo;
        float denom, a, b;

        //Calculate squared magnitudes of each edge direction
        smOne = _oneAxis.sqrMagnitude;
        smTwo = _twoAxis.sqrMagnitude;
        //Get dot product between edge directions which tells us how parallel they are
        dpOneTwo = Vector3.Dot(_oneAxis, _twoAxis);

        //Vector between midpoints
        midpoints = _onePoint - _twoPoint;
        //Dot product of midpoint vector and axes help determine where closest points on each edge are
        dpMidOne = Vector3.Dot(_oneAxis, midpoints);
        dpMidTwo = Vector3.Dot(_twoAxis, midpoints);

        denom = (smOne * smTwo) - (dpOneTwo * dpOneTwo);

        //Zero denominator means parralel edges
        if (Mathf.Abs(denom) < 0.0001f)
        {
            return useOne ? _onePoint : _twoPoint;
        }

        //Parameters for where along each edge the closest points occur
        a = ((dpOneTwo * dpMidTwo) - (smTwo * dpMidOne)) / denom;
        b = ((smOne * dpMidTwo) - (dpOneTwo * dpMidOne)) / denom;

        //Check if nearest point is out of bounds
        if (a > _oneSize || a < -_oneSize ||
            b > _twoSize ||  b < -_twoSize)
        {
            return useOne ? _onePoint : _twoPoint;
        }
        else
        {
            contactOne = _onePoint + (_oneAxis * a);
            contactTwo = _twoPoint + (_twoAxis * b);

            //Return midpoint between closest points
            return (contactOne * 0.5f) + (contactTwo * 0.5f);
        }
    }

    public static void edgeEdgeCollision(OBB b1, OBB b2, Vector3 _centers, int _minIndex, int _bestSingleAxis, out Vector3 normal, out Vector3 contactPoint)
    {
        //Get axes on which edge-edge collision happened
        int oneAxisIndex = _minIndex / 3;
        int twoAxisIndex = _minIndex % 3;
        Vector3 oneAxis = b1.getAxis(oneAxisIndex);
        Vector3 twoAxis = b2.getAxis(twoAxisIndex);
        Vector3 axis = (Vector3.Cross(oneAxis, twoAxis)).normalized;

        //Get axis pointing from b1 to b2
        if (Vector3.Dot(axis, _centers) > 0)
        {
            axis *= -1.0f;
        }

        //Find midpoints of edges involved
        Vector3 ptOnOneEdge = b1.halfWidth;
        Vector3 ptOnTwoEdge = b2.halfWidth;

        for (int i = 0; i < 3; i++)
        {
            if (i == oneAxisIndex) ptOnOneEdge[i] = 0;
            else if (Vector3.Dot(b1.getAxis(i), axis) > 0) ptOnOneEdge[i] = -ptOnOneEdge[i];

            if (i == twoAxisIndex) ptOnTwoEdge[i] = 0;
            else if (Vector3.Dot(b2.getAxis(i), axis) < 0) ptOnTwoEdge[i] = -ptOnTwoEdge[i];
        }

        //Get world coordinates of midpoints
        ptOnOneEdge = b1.toWorld(ptOnOneEdge);
        ptOnTwoEdge = b2.toWorld(ptOnTwoEdge);

        //Set collsion normal and contact point
        normal = axis;
        contactPoint = getContactPoint(ptOnOneEdge, oneAxis, b1.halfWidth[oneAxisIndex], ptOnTwoEdge, twoAxis, b2.halfWidth[twoAxisIndex], _bestSingleAxis > 2);
    }

    public override Shape shape => Shape.OBB;
}
