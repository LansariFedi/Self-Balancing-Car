using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_Wheels : MonoBehaviour
{
    public WheelCollider wheelCollider1;
    public WheelCollider wheelCollider2;
    public Transform wheelTransform1;
    public Transform wheelTransform2;

    void Update()
    {
        UpdateWheel(wheelCollider1, wheelTransform1);
        UpdateWheel(wheelCollider2, wheelTransform2);
    }

    void UpdateWheel(WheelCollider collider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        transform.position = position;
        transform.rotation = rotation;
    }
}
