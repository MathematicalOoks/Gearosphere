using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Car variables
    public Transform playerTransform;
    Rigidbody playerRB;

    public Vector3[] Offsets;
    public Vector3 localOffset;

    public float positionSmoothing;
    public float rotationSmoothing;
    public float angleSum;

    public int carCameraPosition;
    public bool isReversing;

    CarSpawner spawnerClass;
    CarController controller;

    // Rocket variables
    public GameObject Rocket;
    public GameObject LeftRocketBooster;
    public GameObject RightRocketBooster;

    public Material skyboxMaterial;

    private float offsetDistance;
    private float ySpeed;

    private float deltaAngle;
    private float angle;
    private float rad;
    private float radius;

    private Vector3 targetPosition;
    public Vector3 startPosition;

    Rocket RocketController;
    RocketBooster LeftBoosterController;
    RocketBooster RightBoosterController;
    public int cameraPosition;

    public Vector3 LeftLandingPad;
    public Vector3 RightLandingPad;

    public float LeftBoosterAngle;
    public float RightBoosterAngle;
    public float LeftBoosterOffset;
    public float RightBoosterOffset;

    public Transform childCamera;
    public bool waitingForLaunch;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseRocket();
        InitialiseCar();
        // Set the initial atmosphere thickness
        RenderSettings.skybox.SetFloat("_AtmosphereThickness", 1f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.E))
        {
            InitialiseRocket();
            transform.position = startPosition;
        }
        if (waitingForLaunch || (Input.GetKeyDown(KeyCode.L) && !RocketController.hasLaunched))
        {
            transform.position = new Vector3(Rocket.transform.position.x + radius, Rocket.transform.position.y, Rocket.transform.position.z + 35f);
            transform.LookAt(Rocket.transform);
            waitingForLaunch = true;
        }
        if (RocketController.hasLaunched)
        {
            waitingForLaunch = false;
            childCamera.localPosition = Vector3.zero;
            // Only allows the camera to follow the different objects when the rocket and rocket boosters are separated.
            if (Input.GetKeyDown(KeyCode.V))
            {
                // There are a maximum of 6 cameraPositions and so we can use modular arithmetic to reset the cameraPosition.
                cameraPosition = (cameraPosition + 1) % 6;
            }
            if (RocketController.hasLaunched)
            {
                // The first camera position follows the main rocket.
                if (cameraPosition == 0)
                {
                    RenderSettings.skybox.SetFloat("_AtmosphereThickness", RocketController.localAtmosphere);
                    Follow(Rocket, ref angle, ref offsetDistance);
                }
                // The second camera position follows the Rocket's Left booster.
                else if (cameraPosition == 1)
                {
                    RenderSettings.skybox.SetFloat("_AtmosphereThickness", LeftBoosterController.localAtmosphere);
                    if (LeftBoosterController.isLanding)
                    {
                        Follow(LeftRocketBooster, ref LeftBoosterAngle, ref LeftBoosterOffset);
                    }
                    else
                    {
                        Follow(LeftRocketBooster, ref angle, ref offsetDistance);
                    }
                }
                // The third camera position follows the Rocket's Right booster.
                else if (cameraPosition == 2)
                {
                    RenderSettings.skybox.SetFloat("_AtmosphereThickness", RightBoosterController.localAtmosphere);
                    if (RightBoosterController.isLanding)
                    {
                        Follow(RightRocketBooster, ref RightBoosterAngle, ref RightBoosterOffset);
                    }
                    else
                    {
                        Follow(RightRocketBooster, ref angle, ref offsetDistance);
                    }
                }
                // The fourth camera position follows the payload.
                else if (cameraPosition == 3)
                {
                    RenderSettings.skybox.SetFloat("_AtmosphereThickness", RocketController.localAtmosphere);
                    PayloadFollow(RocketController.payload);
                }
                // The fifth camera position is at a stationary position placed near the landing pad of the Rocket's Left booster.
                else if (cameraPosition == 4)
                {
                    RenderSettings.skybox.SetFloat("_AtmosphereThickness", 1);
                    transform.position = LeftLandingPad;
                    transform.rotation = Quaternion.Euler(Vector3.zero);
                }
                // The fifth camera position is at a stationary position placed near the landing pad of the Rocket's Right booster.
                else if (cameraPosition == 5)
                {
                    RenderSettings.skybox.SetFloat("_AtmosphereThickness", 1);
                    transform.position = RightLandingPad;
                    transform.rotation = Quaternion.Euler(Vector3.zero);
                }
            }
        }
        else if(!waitingForLaunch)
        {
            childCamera.localPosition = localOffset;
            // To change camera position, the key of V is used.
            if (Input.GetKeyDown(KeyCode.V))
            {
                // At the final camera position, reset the camera's position.
                carCameraPosition = (carCameraPosition + 1) % Offsets.Length;
            }
            if (playerTransform != null)
            {
                targetPosition = playerTransform.position + playerTransform.rotation * Offsets[carCameraPosition];
                if (carCameraPosition > 0)
                {
                    // Makes the camera follow the vehicle and offsets it with it's current rotation.
                    transform.position = targetPosition;
                    transform.rotation = playerTransform.rotation;
                }
                else
                {
                    // When the car is reversing, to give a reversing view we must rotate the camera 180 degrees in the y axis.
                    if (controller.gasInput == -1)
                    {
                        if (isReversing)
                        {
                            if ((int)angleSum > -180)
                            {
                                transform.Rotate(0, -90 * Time.deltaTime, 0);
                                angleSum += -90 * Time.deltaTime;
                            }
                        }
                    }

                    // Makes the camera follow the vehicle.
                    //targetPosition = playerTransform.position + Offsets[carCameraPosition];
                    transform.position = targetPosition;

                    if (!isReversing)
                    {
                        // Spherically interpolates betweeh the camera's rotation and the car's rotation
                        transform.rotation = Quaternion.Slerp(transform.rotation, playerTransform.rotation, rotationSmoothing);

                        // Only allows rotation about the y axis (yaw)
                        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                    }

                    // Only start the reverse view camera if the gasInput is -1 and the velocity in the z direction is negative
                    // otherwise it may activate at a high speed (where the car is just braking) 
                    // here we are using the localspace as we want to know the direction of the velocity vector relative to the car's orientation
                    if (playerTransform.InverseTransformDirection(playerRB.velocity).z < 0) isReversing = true;
                    else
                    {
                        isReversing = false;
                        angleSum = 0;
                    }
                }
            }
            // Reset vehicle variables if the spawnedCar is no longer avaliable.
            else if (spawnerClass.spawnedCar != null)
            {
                playerTransform = spawnerClass.spawnedCar.transform;
                controller = spawnerClass.spawnedCar.GetComponent<CarController>();
                playerRB = spawnerClass.spawnedCar.GetComponent<Rigidbody>();
            }
        }
    }

    // Makes the camera follow a gameobject by moving spirally and up to a maximum height and back down the oject.
    void Follow(GameObject obj, ref float Angle, ref float Offset)
    {
        Angle += deltaAngle;
        targetPosition = new Vector3(obj.transform.position.x + radius * Mathf.Cos(Mathf.Deg2Rad * Angle), obj.transform.position.y + Offset, obj.transform.position.z + 35f + radius * Mathf.Sin(Mathf.Deg2Rad * Angle));
        transform.position = targetPosition;

        Offset += ySpeed;

        if ((int)Offset == 20) ySpeed *= -1;
        else if ((int)Offset == 0 && ySpeed < 0) ySpeed *= -1;

        transform.LookAt(obj.transform);
    }

    // Makes camera follow the Payload.
    void PayloadFollow(GameObject payload)
    {
        // The camera will be positioned at the payload's position with a fixed offset, allowing the camera to view the cockpit of the car (payload).
        transform.position = payload.transform.position + (Quaternion.Euler(new Vector3(-90f, 180f, 0))) * (new Vector3(0f, -0.007465f, 0.00236f)) + new Vector3(0f, -0.6f, 0.2f);
        // The rotation of the camera will be set to match that of the payload.
        transform.rotation = Quaternion.Euler(new Vector3(-90f, 180f, 0));
    }

    void InitialiseRocket()
    {
        RocketController = Rocket.GetComponent<Rocket>();
        LeftBoosterController = LeftRocketBooster.GetComponent<RocketBooster>();
        RightBoosterController = RightRocketBooster.GetComponent<RocketBooster>();

        skyboxMaterial = RenderSettings.skybox;

        ySpeed = 0.002f;
        deltaAngle = 0.06f;

        angle = 0;
        LeftBoosterAngle = 0;
        RightBoosterAngle = 0;

        offsetDistance = 0;
        LeftBoosterOffset = 0;
        RightBoosterOffset = 0;

        radius = 25f;

        LeftLandingPad = new Vector3(1048.9f, 96.8f, 624);
        RightLandingPad = new Vector3(1455, 96.8f, 624);

        startPosition = new Vector3(372.2f, 64.1f, 579.8f);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        waitingForLaunch = false;
    }
    void InitialiseCar()
    {
        // Car variables

        spawnerClass = GameObject.Find("Spawner").GetComponent<CarSpawner>();

        carCameraPosition = 0;

        isReversing = false;

        positionSmoothing = 0.5f;
        rotationSmoothing = 0.15f;

        // Offsets holds the different offsets (relative to the vehicle's position) for each camera position.
        Offsets = new[] { new Vector3(0f, 0f, 0f), new Vector3(0f, -0.042996f, 5.28988f), new Vector3(1.6f, -0.3f, 2.7f), new Vector3(-1.6f, -0.3f, 2.7f) };
        localOffset = new Vector3(0f, 1.599996f, -5.18988f);
    }
}