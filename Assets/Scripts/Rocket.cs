using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject locationOfThrust;
    public GameObject payload;
    public GameObject payloadCapsule;

    public GameObject particleObject;
    public ParticleSystem particleSystem;
    public GameObject fumes;
    public ParticleSystem fumeSystem;

    public float thrustForce;
    public float localAtmosphere;
    private float AtmosphericChange;

    public bool hasLaunched;
    public bool isLanding;

    public AudioSource source;
    public AudioClip clip;

    private Vector3 payloadOffset;
    public Vector3 startPosition;
    public Vector3 payloadStart;

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            // Add wait time for particle system to load
            TotalReset();
        }
        // To launch the rocket, the key of L is used.
        // To prevent key spamming, the condition will only evaluate if the rocket has not already launched.
        if (Input.GetKeyDown(KeyCode.L) && hasLaunched == false)
        {
            if (transform.position != startPosition) TotalReset();
            // Activates the rocket's exhaust and launch pad fumes.
            ParticleManager(particleSystem, true);
            ParticleManager(fumeSystem, true);
            hasLaunched = true;
            rb.useGravity = true;
            source.PlayOneShot(clip);
            // Stops the rocket's fumes that occur during launch.
            StartCoroutine(StopFumes());
        }

        if (hasLaunched && !isLanding)
        {
            // At an atmospheric thickness of 0.1, the rocket boosters will separate.
            // Due to the way the forces of separation are applied on the rocket boosters
            // we must apply an additional force to the main rocket to prevent collision with the separating rocket boosters.
            if ((float)System.Math.Round(localAtmosphere, 1) == 0.1f)
            {
                rb.AddForceAtPosition(transform.TransformVector(new Vector3(0, 1, 0).normalized) * Time.deltaTime * thrustForce, locationOfThrust.transform.position);
            }
            localAtmosphere -= AtmosphericChange;
            // Applies an upwards force on the rocket at the position of the nozzle.
            rb.AddForceAtPosition(transform.TransformVector(new Vector3(0, 1, 0).normalized) * Time.deltaTime * thrustForce, locationOfThrust.transform.position);
            // Move the payload to match the rocket's position with an offset.
            payload.transform.position = transform.position + payloadOffset;

            // When the atmospheric thickness is 0.01, the payload will be released,
            // so we can toggle the visibility of the capsule to allow the payload to be released.
            if ((float)System.Math.Round(localAtmosphere, 2) == 0.01f)
            {
                payloadCapsule.SetActive(false);
                AtmosphericChange = -0.00001f;
                if(particleSystem.isPlaying) particleSystem.Stop();
                isLanding = true;
                rb.useGravity = false;
            }
        }
    }

    // Stops the fumes from the rocket at launch.
    IEnumerator StopFumes()
    {
        yield return new WaitForSeconds(4f); // Wait for 4 seconds
        ParticleManager(fumeSystem, false);
    }

    // Resets the rocket and it's components.
    void Reset()
    {
        // Reset variables and stop particles
        hasLaunched = false;
        isLanding = false;
        ParticleManager(particleSystem, false);
        ParticleManager(fumeSystem, false);

        // Stop any ongoing force application
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Stop audio
        source.Stop();

        // Reset payload and rocket positions
        payload.transform.position = payloadStart;
        payloadCapsule.SetActive(true);
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        // Reset gravity and kinematic state
        rb.isKinematic = false;
        rb.useGravity = false;
    }

    // Resets the program with values from the initial scene.
    void TotalReset()
    {
        // Reset the atmospheric thickness to 1
        RenderSettings.skybox.SetFloat("_AtmosphereThickness", 1f);
        // Reset the rocket and initialise the variables with values from the initial scene.
        Initialise();
        Reset();
    }

    // Initialises variables with values from the intial scene.
    void Initialise()
    {
        rb = GetComponent<Rigidbody>();

        particleObject = locationOfThrust.transform.Find("Particle System").gameObject;
        particleSystem = particleObject.GetComponent<ParticleSystem>();
        fumeSystem = fumes.GetComponent<ParticleSystem>();

        ParticleManager(particleSystem, false);
        ParticleManager(fumeSystem, false);

        thrustForce = 500000000f;
        localAtmosphere = 1f;
        AtmosphericChange = 0.0001f;

        rb.useGravity = false;

        payloadOffset = new Vector3(0, 25, 0);
        startPosition = new Vector3(337.57f, 79.78f, 369.16f);
        payloadStart = new Vector3(337.569f, 103.84f, 369.444f);
    }

    // Manages the particleSystems of the Rocket.
    void ParticleManager(ParticleSystem ps, bool active)
    {
        if (active)
        {
            if (!ps.isPlaying) ps.Play();
        }
        else if (ps.isPlaying) ps.Stop();
    }
}