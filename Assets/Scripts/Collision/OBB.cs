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
        return transform.worldToLocalMatrix * _worldPoint;
    }

    public void setHalfWidth(Vector3 _halfWidth)
    {
        halfWidth = _halfWidth;
    }

    public void setAxes(Matrix3x3 _axes)
    {
        //axes[0] = new Vector3(_axes.values[0, 0], _axes.values[0, 1], _axes.values[0, 2]).normalized;
        //axes[1] = new Vector3(_axes.values[1, 0], _axes.values[1, 1], _axes.values[1, 2]).normalized;
        //axes[2] = new Vector3(_axes.values[2, 0], _axes.values[2, 1], _axes.values[2, 2]).normalized;

        axes[0] = new Vector3(_axes.values[0, 0], _axes.values[1, 0], _axes.values[2, 0]).normalized;
        axes[1] = new Vector3(_axes.values[0, 1], _axes.values[1, 1], _axes.values[2, 1]).normalized;
        axes[2] = new Vector3(_axes.values[0, 2], _axes.values[1, 2], _axes.values[2, 2]).normalized;
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

        //Get local positions based on orientation
        center = Vector3.zero;

        Vector3[] localVertices = new Vector3[8];
        localVertices[0] = center + (axes[0] * halfWidth.x) + (axes[1] * halfWidth.y) + (axes[2] * halfWidth.z);
        localVertices[1] = center + (axes[0] * halfWidth.x) + (axes[1] * halfWidth.y) - (axes[2] * halfWidth.z);
        localVertices[2] = center + (axes[0] * halfWidth.x) - (axes[1] * halfWidth.y) + (axes[2] * halfWidth.z);
        localVertices[3] = center + (axes[0] * halfWidth.x) - (axes[1] * halfWidth.y) - (axes[2] * halfWidth.z);
        localVertices[4] = center - (axes[0] * halfWidth.x) + (axes[1] * halfWidth.y) + (axes[2] * halfWidth.z);
        localVertices[5] = center - (axes[0] * halfWidth.x) + (axes[1] * halfWidth.y) - (axes[2] * halfWidth.z);
        localVertices[6] = center - (axes[0] * halfWidth.x) - (axes[1] * halfWidth.y) + (axes[2] * halfWidth.z);
        localVertices[7] = center - (axes[0] * halfWidth.x) - (axes[1] * halfWidth.y) - (axes[2] * halfWidth.z);

        //Convert local vertices to world
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = toWorld(localVertices[i]);

            Debug.DrawLine(transform.position, vertices[i], Color.white);
        }

        return vertices;
    }

    public override Shape shape => Shape.OBB;
}
