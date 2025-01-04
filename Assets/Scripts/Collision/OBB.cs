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
        return transform.TransformPoint(_localPoint);
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

    //SAT functions
    public static bool SATintersect(OBB b1, OBB b2)
    {
        Vector3[] axesToTest = b1.getAxes();
        for (int i = 0; i < axesToTest.Length; i++)
        {
            if (!overlapOnAxis(b1, b2, axesToTest[i]))
            {
                return false;
            }
        }

        axesToTest = b2.getAxes();
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

    //Collision resolution functions
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
    public static void vertexFaceCollision(OBB b1, OBB b2, Vector3 _centers, int _minIndex, float _penetration, out Vector3 normal, out Vector3 contactPoint)
    {
        //Pick one of two faces on axis of box1 that is in the direction of box2
        normal = b1.getAxis(_minIndex);
        if (Vector3.Dot(b1.getAxis(_minIndex), _centers) > 0)
        {
            normal *= -1.0f;
        }

        //Find vertex of box2 that is colliding with box1
        Vector3 localVertex = b2.halfWidth;
        if (Vector3.Dot(b2.getAxis(0), normal) < 0) localVertex.x = -localVertex.x;
        if (Vector3.Dot(b2.getAxis(1), normal) < 0) localVertex.y = -localVertex.y;
        if (Vector3.Dot(b2.getAxis(2), normal) < 0) localVertex.z = -localVertex.z;

        //Convert to world coordinates
        contactPoint = b2.toWorld(localVertex);
    }

    public override Shape shape => Shape.OBB;
}
