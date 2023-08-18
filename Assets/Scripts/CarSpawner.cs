using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public static int selectedCar = 0;

    public GameObject spawnedCar;

    public Vector3 currentPosition;
    public Vector3 startPosition;

    // List of the names of car prefabs.
    private string[] cars = {
        "Mustang2018",
        "Chevrolet",
        "Ferrari488"
    };

    // Keycodes for the different cars.
    private KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4
    };

    // Start is called before the first frame update
    private void Start()
    {
        startPosition = new Vector3(1228.7f, 18.91f, 309f);
        SpawnCar();
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            // If the users clicks a numeric key in the range of [1, cars.Length]
            // We must despawn the currently selected car and spawn the newly selected car.
            if (Input.GetKeyDown(keyCodes[i]))
            {
                selectedCar = i;
                DespawnCar();
                SpawnCar();
            }
        }

        // To reset the car, the key of R is used.
        if (Input.GetKeyDown(KeyCode.R))
        {
            DespawnCar();
            SpawnCar();
            spawnedCar.transform.position = startPosition;
        }
    }

    // Spawns the newly selected car
    private void SpawnCar()
    {
        if (spawnedCar != null)
        {
            spawnedCar.SetActive(false);
        }

        if (!spawnedCar || spawnedCar.name != cars[selectedCar])
        {
            if (spawnedCar != null)
            {
                Destroy(spawnedCar);
            }

            // All cars are stored in the Resources Folder of the Prefabs folder
            // With the file's name being the name of the car
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + cars[selectedCar]);
            if (currentPosition == Vector3.zero) currentPosition = startPosition;

            // Instantiate will load the prefab into the scene with a rotation of (0,0,0)
            spawnedCar = Instantiate(prefab, currentPosition, Quaternion.identity);
        }

        spawnedCar.SetActive(true);
    }

    // Despawns the currently selected car
    private void DespawnCar()
    {
        if (spawnedCar != null)
        {
            currentPosition = spawnedCar.transform.position;
            spawnedCar.SetActive(false);
        }
    }
}