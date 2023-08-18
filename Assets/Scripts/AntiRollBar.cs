using UnityEngine;

//An anti-roll bar helps reduce the body roll of a vehicle during fast cornering or over road irregularities.
public class AntiRollBar : MonoBehaviour
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public float antiRollForce;

    private Rigidbody carRigidbody;

    void Start()
    {
        // As the rigidbody and the AntiRollBar script have the same parent, the Rigidbody can be accessed as follows:
        carRigidbody = GetComponent<Rigidbody>();
        antiRollForce = 10000f;
    }

    // FixedUpdate is used for physics calculations as it has the same frequency as the physics system.
    void FixedUpdate()
    {
        if (leftWheel && rightWheel)
        {
            // Calculate the vertical displacement of the wheels from the center of mass
            float leftWheelSuspensionForce = leftWheel.transform.InverseTransformPoint(leftWheel.transform.position).y - leftWheel.suspensionDistance;
            float rightWheelSuspensionForce = rightWheel.transform.InverseTransformPoint(rightWheel.transform.position).y - rightWheel.suspensionDistance;

            // Calculate the anti-roll force
            float antiRoll = (leftWheelSuspensionForce - rightWheelSuspensionForce) * antiRollForce;

            // Apply the anti-roll force to the wheels
            if (leftWheel.isGrounded)
                carRigidbody.AddForceAtPosition(leftWheel.transform.up * -antiRoll, leftWheel.transform.position);

            if (rightWheel.isGrounded)
                carRigidbody.AddForceAtPosition(rightWheel.transform.up * antiRoll, rightWheel.transform.position);
        }
    }
}