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
        return transform.localToWorldMatrix * _localPoint;
    }
    Vector3 toLocal(Vector3 _worldPoint)
    {
        return transform.worldToLocalMatrix * _worldPoint;
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

    public override Shape shape => Shape.OBB;
}
