using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public WheelCollider wheelCollider1;
    public WheelCollider wheelCollider2;
    public float motorTorque = 1500f;

    void Update()
    {
        float move = Input.GetAxis("Vertical");
        wheelCollider1.motorTorque = move * motorTorque;
        wheelCollider2.motorTorque = move * motorTorque;
    }
}