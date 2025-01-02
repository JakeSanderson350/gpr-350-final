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
        OBB[] oBBs = FindObjectsOfType<OBB>();
        for (int i = 0; i < oBBs.Length; i++)
        {
            OBB b1 = oBBs[i];
            for (int j = i + 1; j < oBBs.Length; j++)
            {
                OBB b2 = oBBs[j];
                if (OBB.SATintersect(b1, b2))
                {
                    Debug.Log("Colliding");
                    text.text = "Colliding: True";
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
