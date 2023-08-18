using System;
using UnityEngine;

// This controller is a car controller that handles cars with a steering wheel and cockpit view
// as well as wheels with a parent object of multiple children (where the wheel has multiple child components such as callipers, tyres or rims)
public class CarController: MonoBehaviour
{
    private Rigidbody playerRB;

    public GameObject FRWheel;
    public GameObject FLWheel;
    public GameObject RRWheel;
    public GameObject RLWheel;

    public WheelCollider FRWheelCollider;
    public WheelCollider FLWheelCollider;
    public WheelCollider RRWheelCollider;
    public WheelCollider RLWheelCollider;

    public float gasInput;
    public float steeringInput;

    private float speed;
    public float motorPower;
    private float brakeForce;

    public float maxSteeringAngle;

    public bool isParked;

    StateManager manager;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();
        manager = GameObject.Find("StateManager").GetComponent<StateManager>();
        isParked = false;
        maxSteeringAngle = 70f;
    }

    // Update is called once per frame
    void Update()
    {
        speed = playerRB.velocity.magnitude;
        if (manager.isPlayingCar) checkInput();
        else playerRB.velocity = Vector3.zero;

        // When parked, the car's wheels must be locked and so any function applying motorTorque to the wheels must not be called.
        if (!isParked)
        {
            // The second condition in the first if statement is needed for when the car starts moving from a standstill but is reversing.
            if (gasInput >= 0 || Math.Round(transform.InverseTransformDirection(playerRB.velocity).z, 2) <= 0) brakeForce = 0;
            else brakeForce = 1500;
            ApplyMotor();
            ApplyBrake();
        }
        ApplySteering();
        ApplyWheelPositions();
    }

    // Receives the user's input and updates the input variables.
    void checkInput()
    {
        // W, S, or Up and Down arrow keys control the input for the speed of the vehicle.
        gasInput = Input.GetAxis("Vertical");
        // A, D, or Left and Right arrow keys control the input for the direction of travel of the vehicle.
        steeringInput = Input.GetAxis("Horizontal");

        // To park, the key of P is used.
        if (Input.GetKeyDown(KeyCode.P))
        {
            // When not parked, the car will no longer be in park so the brake can be released otherwise the brakeForce 
            // can be set to a large value to simulate parking brakes.
            if (isParked) brakeForce = 0;
            else brakeForce = 3000;
            // Everytime the key P is clicked, the state of isParked will change to it's logical not of itself.
            isParked = !isParked;
            ApplyBrake();
        }
        // To flip the car, the key of F is used.
        if (Input.GetKeyDown(KeyCode.F)) FlipCar();
    }

    // Applies a torque from the brakes to the wheel colliders.
    // Which thus stops the wheels, whether they're going forwards or backwards.
    void ApplyBrake()
    {
        RRWheelCollider.brakeTorque = brakeForce;
        RLWheelCollider.brakeTorque = brakeForce;
    }

    // Applies a torque from the motor to the wheel colliders.
    // Which results in the wheels rotating.
    void ApplyMotor()
    {
        RRWheelCollider.motorTorque = motorPower * gasInput;
        RLWheelCollider.motorTorque = motorPower * gasInput;

    }

    // Sets the steering angle for the wheels, with a multiplier corresponding to the input.
    void ApplySteering()
    {
        // The steering angle is the angle between the front of the vehicle and the steered wheel direction.
        float steeringAngle = (float)steeringInput * maxSteeringAngle;

        FLWheelCollider.steerAngle = steeringAngle;
        FRWheelCollider.steerAngle = steeringAngle;

    }

    // Updates the position and rotation of all the wheelMeshes.
    void ApplyWheelPositions()
    {

        UpdateWheel(FRWheelCollider, FRWheel);

        UpdateWheel(FLWheelCollider, FLWheel);

        UpdateWheel(RRWheelCollider, RRWheel);

        UpdateWheel(RLWheelCollider, RLWheel);
    }

    // Sets the wheelMesh to match the position and rotation of it's corresponding wheel collider.
    void UpdateWheel(WheelCollider collider, GameObject wheelMesh)
    {

        Vector3 position;
        Quaternion orientation;

        // Gets the rotation and position of the collider and outputs them into the position and orientation variables.
        collider.GetWorldPose(out position, out orientation);

        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = orientation;
    }

    // Flips the car to align the terrain.
    public void FlipCar()
    {
        // If rigidbody.isKinematic is enabled, Forces, collisions or joints will not affect the rigidbody anymore
        // Thus we can enable it to prevent the car from maintaining it's velocity prior to the flip
        playerRB.isKinematic = true;

        // transform.forward is a normalized vector representing the forward direction of the car, which we want the car to maintain
        // after reseting the car's rotation
        Vector3 fwd = transform.forward;

        // resets the car's rotation to align with the axes of the world (terrain)
        transform.rotation = Quaternion.identity;

        // set the value for the direction in which the car was facing
        transform.forward = fwd;

        // Disable rigidbody.isKinematic to allow forces to act on the car again
        playerRB.isKinematic = false;
    }
}
