using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public GameObject rocket;
    Rocket rocketController;

    public bool isPlayingCar;
    // Start is called before the first frame update
    void Start()
    {
        rocketController = rocket.GetComponent<Rocket>();
        isPlayingCar = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isPlayingCar = true;
        }
        if (Input.GetKeyDown(KeyCode.L) && rocketController.hasLaunched == false)
        {
            isPlayingCar = false;
        }
    }
}