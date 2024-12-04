using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BalancingAgent : Agent
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public Transform carTransform;
    public Rigidbody carRigidbody;
    public float maxTorque;
    public float maxTiltAngle;
    private Vector3 initialPosition;

    public override void OnEpisodeBegin()
    {
        carTransform.localPosition = new Vector3(-0.709614f, -1.378614f, -1.941332f);
        carTransform.localRotation = Quaternion.identity;
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        initialPosition = carTransform.localPosition;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float actionvalue = actions.ContinuousActions[0];
        float torque = Mathf.Lerp(carRigidbody.angularVelocity.x, actionvalue * maxTorque, Time.deltaTime * 1.5f);
        //Debug.Log("Torque: " + torque);

        leftWheel.motorTorque = torque;
        rightWheel.motorTorque = torque;

        float tiltAngle = carTransform.localEulerAngles.x;
        if (tiltAngle > 180f) tiltAngle -= 360f;

        float tiltPenalty = Mathf.Abs(tiltAngle) / maxTiltAngle;

        if (Mathf.Abs(tiltAngle) <= 5f)
        {
            AddReward(2.0f);
        }
        else if (Mathf.Abs(tiltAngle) <= 10f)
        {
            AddReward(0.5f);
        }
        else if (Mathf.Abs(tiltAngle) <= maxTiltAngle)
        {
            AddReward(-tiltPenalty * 1.0f);
        }
        else
        {
            AddReward(-2.0f);
            EndEpisodeWithFloorColorChange();
        }

        if (Mathf.Abs(actionvalue) > 0.9f)
        {
            AddReward(-0.1f);
        }

        if (Mathf.Abs(tiltAngle) < 2f && Mathf.Abs(actionvalue) < 0.1f)
        {
            AddReward(1.0f);
        }

        if (carRigidbody.velocity.magnitude > 0.1f)
        {
            AddReward(0.2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "wall1" || collision.gameObject.name == "wall2" ||
            collision.gameObject.name == "wall3" || collision.gameObject.name == "wall4")
        {
            AddReward(-1.0f);
            EndEpisodeWithFloorColorChange();
        }
    }

    private void EndEpisodeWithFloorColorChange()
    {
        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float tiltAngle = carTransform.localEulerAngles.x;
        if (tiltAngle > 180f) tiltAngle -= 360f;
        sensor.AddObservation(tiltAngle / maxTiltAngle);
        sensor.AddObservation(carRigidbody.angularVelocity.x);
        sensor.AddObservation(carTransform.localPosition);
        sensor.AddObservation(carRigidbody.velocity);
        sensor.AddObservation(leftWheel.rpm);
        sensor.AddObservation(rightWheel.rpm);
    }
}