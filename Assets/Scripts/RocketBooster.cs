using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBooster : MonoBehaviour
{
    Rigidbody rb;

    public float thrustForce;
    public float localAtmosphere;
    public float AtmosphericChange;
    public float BoosterID;
    public float gravity;
    public float landingDuration;

    public Vector3 landingPosition;
    public Vector3 thrustDir;
    public Vector3 startPosition;

    public GameObject locationOfThrust;
    public GameObject particleObject;
    public ParticleSystem particleSystem;

    public bool hasLaunched;
    public bool isLanding;

    public Material skyboxMaterial;

    public float speed;
    public float speedChange = 30.0f;

    public float rotationSpeed = 45.0f; // Set the desired rotation speed in degrees per second
    public Vector3 targetRotationEulerAngles = Vector3.zero;

    // Start is called before the first frame update void Start()
    void Start()
    {
        Initialise();
    }

    // Update is called once per frame
    void Update()
    {
        // To reset the rocket, the key of E is used.
        if (Input.GetKey(KeyCode.E))
        {
            StopAllCoroutines();
            TotalReset();
        }
        thrustDir = new Vector3(0, 1, 0).normalized;
        // To launch the rocket, the key of L is used.
        if (Input.GetKeyDown(KeyCode.L) && hasLaunched == false)
        {
            if (transform.position != startPosition) TotalReset();
            ParticleManager(true);
            StartCoroutine(DelayLaunch());
            //rb.useGravity = true;
            //hasLaunched = true;
        }
        // Applies an upwards force on the rocket at the position of the nozzle.
        if (hasLaunched && !isLanding)
        {
            rb.AddForceAtPosition(transform.TransformVector(new Vector3(0, 1, 0).normalized) * Time.deltaTime * thrustForce, locationOfThrust.transform.position);
        }

        if (!isLanding)
        {
            // If the atmospheric thickness is 0.1, the rocket boosters should separate from the rocket.
            if ((float)System.Math.Round(skyboxMaterial.GetFloat("_AtmosphereThickness"), 1) == 0.1f)
            {
                localAtmosphere = skyboxMaterial.GetFloat("_AtmosphereThickness");
                // If the rocket booster is on the left, a force should be applied to the rocket booster in the left direction.
                if (BoosterID == 0)
                {
                    rb.AddForceAtPosition(transform.TransformVector(new Vector3(-1f, 1, 0).normalized) * Time.deltaTime * thrustForce, locationOfThrust.transform.position);
                }
                // If the rocket booster is on the right, a force should be applied to the rocket booster in the right direction.
                else
                {
                    rb.AddForceAtPosition(transform.TransformVector(new Vector3(1f, 1, 0).normalized) * Time.deltaTime * thrustForce, locationOfThrust.transform.position);
                }
                // Now the AtmosphericChange will be set to negative, such that when it is subtracted from the localAtmosphere
                // the atmospheric thickness increases as the booster falls towards the world.
                AtmosphericChange = -0.00001f;
                // Now the thruster is no longer be used, so the particleSystem for the thruster can be deactivated.
                ParticleManager(false);
                //hasLaunched = false;
                StartCoroutine(DelayLanding());
            }
        }
        else
        {
            // Prevent forces from acting on the rocket to achieve smooth movement.
            rb.isKinematic = true;

            // Rotates the booster towards a rotation of 0, 0, 0 to match the landing pad's rotation.
            Quaternion targetRotation = Quaternion.Euler(targetRotationEulerAngles);

            float Rotationalstep = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Rotationalstep);

            // Calculates the distance between the booster and the landing pad.
            float totalDistance = Vector3.Distance(transform.position, landingPosition);

            // The speed will be calculated as the ratio between the remaining distance and a constant speedChange
            // such that as the rocket moves closer to the landing pad, the speed will decrease, thereby simulating 
            // the thrusters being applied to slow the rocket down.
            float speed = totalDistance / speedChange;

            // If the rocket's height is less than 2000, the the exhuast fuel should be visible,
            // to indicate that thrust is being applied to slow the rocket down.
            if (transform.position.y < 2000)
            {
                ParticleManager(true);
                // As the distance of the rocket to the landingPosition decreases, the time to reach the landing pad
                // will increase too much, so the time can be decreased by increasing the speed by a constant factor of 10.
                speed += 10;
            }

            float step = speed * Time.deltaTime; // calculate distance to move

            // Moves the rocket towards the landing pad incrementally by a value of step.
            transform.position = Vector3.MoveTowards(transform.position, landingPosition, step);

            // If the rocket is at a height of the landing pad, the rocket is no longer landing and the thrusters can be shut off.
            if ((int)transform.position.y == landingPosition.y)
            {
                ParticleManager(false);
                isLanding = false;
            }
            speedChange -= 0.001f;
        }

        UpdateParticleSystem();

        if (isLanding && localAtmosphere >= 0.99f) { }
        else localAtmosphere -= AtmosphericChange;
    }

    IEnumerator DelayLanding()
    {
        yield return new WaitForSeconds(4f); // Wait for 4 seconds
        AtmosphericChange = -1 * Time.deltaTime * 0.011f;
        isLanding = true; // Set isLanding to true after the delay
    }

    IEnumerator DelayLaunch()
    {
        yield return new WaitForSeconds(10f);
        rb.useGravity = true;
        hasLaunched = true;
    }

    // Updates the particleSystem's position to the nozzle's position with a small offset.
    void UpdateParticleSystem()
    {
        particleObject.transform.position = locationOfThrust.transform.position + new Vector3(0, -1, 0);
    }

    // Handles the activity of the particleSystem.
    void ParticleManager(bool active)
    {
        if (active)
        {
            if (!particleSystem.isPlaying) particleSystem.Play();
        }
        else
        {
            if (particleSystem.isPlaying) particleSystem.Stop();
        }
    }

    // Resets the rocket booster's position to the launchpad.
    void Reset()
    {
        // Reset variables and stop particles
        hasLaunched = false;
        isLanding = false;
        ParticleManager(false);

        // Stop any ongoing force application (this may not be necessary)
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset the rocket booster position
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        // Reset gravity and kinematic state
        rb.isKinematic = false;
        rb.useGravity = false;
    }

    // Initialises variables with values from the intial Scene.
    void Initialise()
    {
        rb = GetComponent<Rigidbody>();
        particleSystem = particleObject.GetComponent<ParticleSystem>();

        ParticleManager(false);

        thrustForce = 500000000f;
        localAtmosphere = 1f;
        AtmosphericChange = 0.0001f;
        gravity = 9.81f;
        landingDuration = 20f;

        skyboxMaterial = RenderSettings.skybox;
        rb.useGravity = false;

        if (BoosterID == 0)
        {
            landingPosition = new Vector3(1483, 64, 703);
            startPosition = new Vector3(339.42f, 74.14f, 369.16f);
        }
        else
        {
            landingPosition = new Vector3(1054, 64, 703);
            startPosition = new Vector3(335.75f, 74.14f, 369.16f);
        }
    }

    // Resets all of the values in the program and the rocket booster.
    void TotalReset()
    {
        Initialise();
        Reset();
    }
}