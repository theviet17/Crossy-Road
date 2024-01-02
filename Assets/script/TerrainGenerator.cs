using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int minDistanceFromPlayer;
    [SerializeField] private int maxTerrainCount;
    [SerializeField] private List<TerrainData> terrainDatas = new List<TerrainData>();
    [SerializeField] private Transform terrainHolder;

    [HideInInspector] public List<GameObject> currentTerrains = new List<GameObject>();
    [HideInInspector] public Vector3 currentPosition = new Vector3(-10, 0, 0);
    private int numberTerrainInStartPoint = 0;
    public void TerrainGeneratorStart()
    {
        numberTerrainInStartPoint = Random.Range(12, 20);
        for (int i = 0; i < maxTerrainCount; i++)
        {
            SpawnTerrain(true,new Vector3(0,0,0));
        }
    }
    
    public void SpawnTerrain(bool isStart, Vector3 playerPos )
    {
        if(currentPosition.x - playerPos.x < minDistanceFromPlayer)
        {
            int wichTerrain = numberTerrainInStartPoint >= 0 ? 0 : Random.Range(0, terrainDatas.Count);
            int terrainInSuccession = Random.Range(1, terrainDatas[wichTerrain].maxInSuccession);
            
            for (int i = 0; i < terrainInSuccession; i++)
            {
               GameObject terrain = Instantiate(terrainDatas[wichTerrain].possibleTerrain[Random.Range(0,terrainDatas[wichTerrain].possibleTerrain.Count)], currentPosition, Quaternion.identity, terrainHolder);
            
               currentTerrains.Add(terrain);
               GenerateObstacle(terrain);
               if (!isStart) 
               {
                   if (currentTerrains.Count > maxTerrainCount)
                   {
                       Destroy(currentTerrains[0]);
                       currentTerrains.RemoveAt(0);
                   }
               }
               currentPosition.x++;
               numberTerrainInStartPoint--;
            }
        }
    }

    public GameObject CurrentTerrainJumpIn(float x)
    {
        return currentTerrains.FirstOrDefault(terrain => terrain.transform.position.x == x);
    }

    public void GenerateObstacle(GameObject terrain)
    {
        string tag = terrain.tag;
        switch (tag)
        {
            case "Road":
                RoadObstacle(terrain);
                break;
            case "Track":
                break;
            case "Grass":
                break;
            case "Water":
                break;
        }
    }

    public void RoadObstacle(GameObject terrain)
    {
        GenerateRoadMarkings(terrain);
        GenerateVehicleInstancePoint(terrain);
    }
    [Header("Offer for road")]
    [SerializeField] private GameObject roadMarkings;
    [SerializeField] private List<GameObject> VehicleInstancePoint;
    public void GenerateRoadMarkings(GameObject terrain)
    {
        var previousTerain = currentTerrains[currentTerrains.IndexOf(terrain) - 1];
       
        if (previousTerain.tag == terrain.tag)// "Road"
        {
            var rmk = Instantiate(roadMarkings);
            rmk.transform.SetParent(terrain.transform);
            rmk.transform.localPosition = new Vector3(0, -1.77f, -0.01f);
        } 
    }
    
    public void GenerateVehicleInstancePoint(GameObject terrain)
    {
        int randomNumber = Random.Range(0, 100);
        if (randomNumber % 2 == 0)
        {
            var vhip = Instantiate(VehicleInstancePoint[0]);
            vhip.transform.SetParent(terrain.transform);
            vhip.transform.localPosition = new Vector3(0, 0.58f, 0.3f);
        }
        else
        {
            var vhip = Instantiate(VehicleInstancePoint[1]);
            vhip.transform.SetParent(terrain.transform);
            vhip.transform.localPosition = new Vector3(0, 0.58f, -0.3f);
        }
    }

}