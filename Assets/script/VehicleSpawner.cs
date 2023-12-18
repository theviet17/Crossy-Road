using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject vehicle;
    [SerializeField] private Transform spawmPos;
    [SerializeField] private float minSeparationTime;
    [SerializeField] private float maxSeparationTime;
    private void Start()
    {
        StartCoroutine(SpawnVehicle());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator SpawnVehicle()
    {
        while (true) { 
        yield return new WaitForSeconds(Random.Range(minSeparationTime,maxSeparationTime));
        Instantiate(vehicle, spawmPos.position, Quaternion.identity);
        }
    }
}
