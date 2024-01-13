using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedSpawner : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private List<GameObject> vehicles = new List<GameObject>();
    [SerializeField] private float maxTime,minTime;
    [SerializeField] private float maxSpeed,minSpeed;
    float timer,randomTime;

    void Start()
    {
        randomTime = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= randomTime)
        {
            // Get a random position x/y/z
            float randX = Random.Range(-distance, distance);
            float randY = Random.Range(-distance, distance);
            float randZ = Random.Range(-distance, distance);
            Vector3 randPos = new Vector3(randX, randY, randZ);

            // Spawn a random vehicle.
            int veh_spawnnum = Random.Range(0, vehicles.Count);
            GameObject spawnedVehicle = Instantiate(vehicles[veh_spawnnum], transform.position + randPos, transform.rotation);

            // Allow vehicle deletion.
            spawnedVehicle.AddComponent<DeleteZoneScript>();

            // Send it outward
            Rigidbody veh_rb = spawnedVehicle.AddComponent<Rigidbody>();
            veh_rb.useGravity = false;
            veh_rb.freezeRotation = true;
            veh_rb.velocity = transform.forward * Random.Range(minSpeed, maxSpeed); 

            // Reset timer
            randomTime = Random.Range(minTime, maxTime);
            timer = 0f;
        }
    }
}
