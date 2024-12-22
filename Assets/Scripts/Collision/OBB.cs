using UnityEngine;
using System.Collections;

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

        Debug.DrawLine(transform.position, transform.position + axes[0], Color.red);
        Debug.DrawLine(transform.position, transform.position + axes[1], Color.green);
        Debug.DrawLine(transform.position, transform.position + axes[2], Color.blue);
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

    public override Shape shape => Shape.OBB;
}
