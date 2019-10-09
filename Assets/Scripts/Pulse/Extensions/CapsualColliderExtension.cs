using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CapsualColliderExtension
{
    public static void InitiLineCollider(this CapsuleCollider collider, Transform right )
    {
        collider.direction = 2;
        collider.height = Vector3.Distance(collider.transform.position, right.position);
        collider.radius = 0.05f;
        collider.center = Vector3.forward * collider.height * 0.5f;
        collider.isTrigger = false;
    }
}
