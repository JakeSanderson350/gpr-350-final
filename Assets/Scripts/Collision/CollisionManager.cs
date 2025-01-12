using System.Collections;
using System.Collections.Generic;
using static CollisionDetection;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollisionManager : MonoBehaviour
{
    public TextMesh text;

    private void StandardCollisionResolution()
    {
        Sphere[] spheres = FindObjectsOfType<Sphere>();
        PlaneCollider[] planes = FindObjectsOfType<PlaneCollider>();
        for (int i = 0; i < spheres.Length; i++)
        {
            Sphere s1 = spheres[i];
            for (int j = i + 1; j < spheres.Length; j++)
            {
                Sphere s2 = spheres[j];
                ApplyCollisionResolution(s1, s2);
            }
            foreach (PlaneCollider plane in planes)
            {
                ApplyCollisionResolution(s1, plane);
            }
        }
    }

    private void CheckOBBCollision()
    {
        RectRigidBody[] rigidBodies = FindObjectsOfType<RectRigidBody>();
        for (int i = 0; i < rigidBodies.Length; i++)
        {
            OBB b1 = rigidBodies[i].rbOBB;
            for (int j = i + 1; j < rigidBodies.Length; j++)
            {
                OBB b2 = rigidBodies[j].rbOBB;
                if (OBB.SATintersect(b1, b2))
                {
                    //Debug.Log("Colliding");
                    text.text = "Colliding: True";
                    ApplyCollisionResolution(rigidBodies[i], rigidBodies[j]);
                }
                else
                {
                    text.text = "Colliding: False";
                }
            }
        }
    }

    private void FixedUpdate()
    {
        CollisionChecks = 0;

        CheckOBBCollision();
    }
}
