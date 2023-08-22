using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Speedometer;
    [SerializeField] TextMeshProUGUI SpeedText;
    [SerializeField] TextMeshProUGUI AltitudeText;
    [SerializeField] TextMeshProUGUI PayloadStatus;

    CameraFollow followClass;
    Rocket rocketController;
    public GameObject rocket;

    public float mphConversion;
    public float speedometerConversion;
    public float altitudeConversion;
    public float startingAltitude;

    public bool isTelemetryVisible;
    public bool isSpeedometerVisible;
    public bool isPayloadStatusVisible;

    CarSpawner spawnerClass;
    Rigidbody playerRB;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseRocket();
        InitialiseCar();
    }

    // Update is called once per frame
    void Update()
    {
        // To reset the program, the key of E is used.
        if (Input.GetKey(KeyCode.E))
        {
            // All of variables should be reset as well, which can be acheived by calling the Initialise function.
            InitialiseRocket();
        }
        if (rocketController.hasLaunched)
        {
            isSpeedometerVisible = false;
            // Only make the speed and altitude telemetry visible if the camera is following the rocket
            // otherwise make the text invisible.
            if (followClass.cameraPosition == 0)
            {
                // As the rocket is travelling straight upwards, all of the speed will be measured from the rocket's velocity in the y direction.
                SpeedText.text = (Mathf.Abs(rocketController.rb.velocity.y) * mphConversion).ToString("0") + " mph";
                AltitudeText.text = ((rocket.transform.position.y - startingAltitude) * altitudeConversion).ToString("0") + " km";
                isTelemetryVisible = true;
            }
            else
            {
                isTelemetryVisible = false;
            }
            if (rocketController.isPayloadReleased && !isPayloadStatusVisible)
            {
                PayloadStatus.canvasRenderer.SetAlpha(1.0f);
                isPayloadStatusVisible = true;
                StartCoroutine(ClearPayloadStatus());
            }
        }
        // If the rocket has not launched or the camera is not following the rocket, the telemetry should be invisible.
        else
        {
            if (!followClass.waitingForLaunch)
            {
                isSpeedometerVisible = true;
                if (playerRB != null)
                {
                    // Here we are calculating the magnitude of the velocity of the car only in the x and z directions (as the y direction is not influenced by the motor)
                    // and converting the velocity into mph using the conversion unit.
                    Speedometer.text = (Mathf.Abs(Mathf.Sqrt((float)Math.Pow(playerRB.velocity.x, 2) + (float)Math.Pow(playerRB.velocity.z, 2)) * speedometerConversion)).ToString("0") + " mph";
                }
                else
                {
                    // If there is a change in car or the car becomes null, we must reset the rigidbody component for the player.
                    playerRB = spawnerClass.spawnedCar.GetComponent<Rigidbody>();
                }
                isTelemetryVisible = false;
            }
            else isSpeedometerVisible = false;
        }

        // To simulate a rocket launch, the rocket's speed near launch will increase as the rocket travels further upwards
        // with the exclusion of drag, so we can incrementally increase the conversion rate for speed and altitude over time.
        if (rocketController.hasLaunched)
        {
            if ((int)mphConversion < 31) mphConversion += 0.01f;
            if ((int)altitudeConversion < 1) altitudeConversion += 0.0000008f;
        }

        UpdateVisibility();
    }

    // Updates the conversion unit for mph
    IEnumerator ChangeConversion()
    {
        yield return new WaitForSeconds(0.1f); // Wait for 1 second
        mphConversion++;
    }

    // Clears payload status after 4 seconds
    IEnumerator ClearPayloadStatus()
    {
        yield return new WaitForSeconds(4f);
        PayloadStatus.canvasRenderer.SetAlpha(0.0f);
    }

    // Initialises rocket telemetry variables with values from initial scene.
    void InitialiseRocket()
    {
        // Rocket Telemetry variables
        rocket = GameObject.Find("Rocket");
        rocketController = rocket.GetComponent<Rocket>();
        followClass = GameObject.Find("CameraHandler").GetComponent<CameraFollow>();

        isTelemetryVisible = false;
        mphConversion = 0.3f;
        altitudeConversion = 0.001f;
        startingAltitude = rocket.transform.position.y;

        PayloadStatus.canvasRenderer.SetAlpha(0.0f);
        isPayloadStatusVisible = false;
    }

    // Initialises car speedometer variables with values from initial scene.
    void InitialiseCar()
    {
        // Car telemetry variables
        spawnerClass = GameObject.Find("Spawner").GetComponent<CarSpawner>();
        if (spawnerClass.spawnedCar != null) playerRB = spawnerClass.spawnedCar.GetComponent<Rigidbody>();
        isSpeedometerVisible = true;
        speedometerConversion = 4.23623629f;
    }

    // Updates the visibility of each element in the canvas.
    void UpdateVisibility()
    {
        SpeedText.canvasRenderer.SetAlpha(isTelemetryVisible ? 1.0f : 0.0f);
        AltitudeText.canvasRenderer.SetAlpha(isTelemetryVisible ? 1.0f : 0.0f);
        Speedometer.canvasRenderer.SetAlpha(isSpeedometerVisible ? 1.0f : 0.0f);
    }
}