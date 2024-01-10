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

    [Header("GOLD")]
    [SerializeField] private GameObject gold;
    [SerializeField] private int goldProbability = 3;
    public void ReStart()
    {
        DestroyAllChildrenObjects();
        currentTerrains.Clear();
        currentPosition = new Vector3(-10, 0, 0);
        numberTerrainInStartPoint = 0;
        TerrainGeneratorStart();
    }
    void DestroyAllChildrenObjects()
    {
        foreach (Transform child in terrainHolder)
        {
            Destroy(child.gameObject);
        }
    }
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
                TrackObstacle(terrain);
                break;
            case "Grass":
                GrassObstacle(terrain);
                break;
            case "Water":
                WaterObstacle(terrain);
                break;
        }
    }

   
    [Header("OFFER FOR ROAD")]
    [SerializeField] private GameObject roadMarkings;
    [SerializeField] private List<GameObject> VehicleInstancePoint;

    public void RoadObstacle(GameObject terrain)
    {
        GenerateRoadMarkings(terrain);
        GenerateVehicleInstancePoint(terrain);
        GenerateRoadGold(terrain);
    }
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
            Instance(terrain, VehicleInstancePoint[0],  new Vector3(0, 0.58f, 0.3f));
        }
        else
        {
            Instance(terrain, VehicleInstancePoint[1],  new Vector3(0, 0.58f, -0.3f));
        }
    }
    public void GenerateRoadGold(GameObject terrain)
    {
        List<float> position = CreateListFromRange(-4, 5);
        if (CheckRandomPercentage(goldProbability))
        {
            SpawnGold(position, terrain, 0.931f);
        }
    }
        
    [Header("OFFER FOR TRACK")]
    [SerializeField] private List<GameObject> TrainInstancePoint;

    public void TrackObstacle(GameObject terrain)
    {
        GenerateTrainInstancePoint(terrain);
        GenerateTrackGold(terrain);
    }
    public void GenerateTrainInstancePoint(GameObject terrain)
    {
        int randomNumber = Random.Range(0, 100);
        if (randomNumber % 2 == 0)
        {
            var vhip = Instance(terrain, TrainInstancePoint[0], new Vector3(0, 1.3f, 0.3f));

            WarningLightRegister(terrain, vhip.GetComponent<MovingObjectInstancePoint>());
        }
        else
        {
            var vhip = Instance(terrain, TrainInstancePoint[1], new Vector3(0, 1.3f, -0.3f));
            WarningLightRegister(terrain, vhip.GetComponent<MovingObjectInstancePoint>());
        }
    }
    public void WarningLightRegister(GameObject terrain , MovingObjectInstancePoint movingObjectInstancePoint)
    {
        var wrl = terrain.transform.GetChild(0).GetComponent<WarningLight>();
        wrl.movingObjectInstancePoint = movingObjectInstancePoint;
        wrl.Register();
    }
    public void GenerateTrackGold(GameObject terrain)
    {
        List<float> position = CreateListFromRange(-4, 5);
        if (CheckRandomPercentage(goldProbability))
        {
            SpawnGold(position, terrain, 1.086f);
        }
    }

    [Header("OFFER FOR WATER")] 
    private int CurrentPlankType = 0;
    [SerializeField] private GameObject duckweed;
    [SerializeField] private List<GameObject> PlankInstancePoint;
    public void WaterObstacle(GameObject terrain)
    {
        if(CheckRandomPercentage(70) || currentTerrains[currentTerrains.IndexOf(terrain) - 1].tag == "Grass")
        {
            GeneratePlankInstancePoint(terrain);
        }
        else
        {
            GenerateDuckweed(terrain);
        }
        
    }
    public void GeneratePlankInstancePoint(GameObject terrain)
    {
        var previousTerain = currentTerrains[currentTerrains.IndexOf(terrain) - 1];
        int randomNumber = 0;
        if (previousTerain.tag != terrain.tag)
        {
            randomNumber = Random.Range(0, 100);
        }
        else
        {
            randomNumber = CurrentPlankType == 0 ? 1 : 0;
        }
         
        if (randomNumber % 2 == 0)
        {
            Instance(terrain, PlankInstancePoint[0], new Vector3(0, -0.05f, 0.3f));
            CurrentPlankType = 0;
        }
        else
        {
            Instance(terrain, PlankInstancePoint[1], new Vector3(0, -0.05f, -0.3f));
            CurrentPlankType = 1;
        }
    }
    public void GenerateDuckweed(GameObject terrain)
    {
        terrain.name = "RiverWithDuckweed";
        List<float> position = new List<float>();
        var previousTerain = currentTerrains[currentTerrains.IndexOf(terrain) - 1];
        if (previousTerain.name == terrain.name)
        {
            for(int i = 2; i < previousTerain.transform.childCount; i++)
            {
                var x = terrain.transform.localToWorldMatrix.GetPosition().x;
                var z = previousTerain.transform.GetChild(i).transform.position.z;
                var ob = Instantiate(duckweed, new Vector3(x, 0, z), Quaternion.identity);
                ob.transform.SetParent(terrain.transform);
                position.Add(z);
            }
        }
        else
        {
            int numberDuckweed;
            var randomNumber = Random.Range(0, 100);
            if (randomNumber < 60)
            {
                numberDuckweed = 2;
            }
            else if (randomNumber < 90)
            {
                numberDuckweed = 1;
            }
            else
            {
                numberDuckweed = 3;
            }
            var currentZ = 100;
            for (int i = 1; i <= numberDuckweed; i++)
            {
                var z = Random.Range(-3, 3);
                if (z != currentZ)
                {
                    currentZ = z;
                    var x = terrain.transform.localToWorldMatrix.GetPosition().x;
                    var ob = Instantiate(duckweed, new Vector3(x, 0, z), Quaternion.identity);
                    ob.transform.SetParent(terrain.transform);
                    position.Add(z);
                }

            }
        }
        GenerateDuckweedGold(terrain,position);
    }
    public void GenerateDuckweedGold(GameObject terrain, List<float> position)
    {
        if (CheckRandomPercentage(goldProbability))
        {
            SpawnGold(position, terrain, 0.9f);
        }
    }

    public MovingObjectInstancePoint Instance(GameObject terrain,GameObject gameObject, Vector3 position)
    {
        var vhip = Instantiate(gameObject);
        vhip.GetComponent<MovingObjectInstancePoint>().terrain = terrain.transform;
        vhip.transform.SetParent(terrain.transform);
        vhip.transform.localPosition = position;
        return vhip.GetComponent<MovingObjectInstancePoint>();
    }

    [Header("OFFER FOR GRASS")]
    [SerializeField] private List<GameObject> Obstacle;
    public void GrassObstacle(GameObject terrain)
    {
        GenerateGrassObstacle(terrain);
    }
    public void GenerateGrassObstacle(GameObject terrain)
    {
        List<float> position = CreateListFromRange(-4, 5);
        bool CanSpawn = true;
        if(currentPosition.x > 0)
        {
            var previousTerain = currentTerrains[currentTerrains.IndexOf(terrain) - 1];
            if (previousTerain.name == "RiverWithDuckweed")
            {
                CanSpawn = false;
            }
        }
        for (int i = -10; i <= 10; i++)
        {
            if(i >= -4 && i <= 4)
            {
                if (CanSpawn)
                {
                    if (currentPosition.x < -5)
                    {
                        var x = terrain.transform.localToWorldMatrix.GetPosition().x;
                        var ob = Instantiate(Obstacle[Random.Range(2, Obstacle.Count)], new Vector3(x, 1, i), Quaternion.identity);
                        ob.transform.SetParent(terrain.transform);
                    }
                    else
                    {
                        if (CheckRandomPercentage(20))
                        {
                            var x = terrain.transform.localToWorldMatrix.GetPosition().x;
                            if (new Vector3(x, 1, i) != new Vector3(0, 1, 0))
                            {
                                var ob = Instantiate(Obstacle[Random.Range(0, Obstacle.Count - 1)], new Vector3(x, 1, i), Quaternion.identity);
                                ob.transform.SetParent(terrain.transform);
                            }
                            else
                            {
                                Debug.Log("Spawn trung vao vi tri nhan vat chinh");
                            }
                            position.Remove(i);
                        }
                    }
                   
                }
               
            }
            else
            {
                var x = terrain.transform.localToWorldMatrix.GetPosition().x;
                var ob = Instantiate(Obstacle[Random.Range(2, Obstacle.Count)], new Vector3(x, 1, i), Quaternion.identity);
                ob.transform.SetParent(terrain.transform);
            }
        }
        GenerateGrassGold(terrain, position);
    }
    public void GenerateGrassGold(GameObject terrain, List<float> position)
    {
        if (CheckRandomPercentage(goldProbability))
        {
            SpawnGold(position, terrain, 1);
        }
    }
    public bool CheckRandomPercentage(float percentage)
    {
        // Tạo một số ngẫu nhiên từ 0 đến 100
        var randomValue = Random.Range(0, 100);
        // Kiểm tra xem giá trị ngẫu nhiên có nằm trong tỷ lệ phần trăm hay không
        return randomValue < percentage;
    }

    public void SpawnGold(List<float> position, GameObject terrain, float y)
    {
        var select = position[Random.Range(0, position.Count)];
        var x = terrain.transform.localToWorldMatrix.GetPosition().x;
        var ob = Instantiate(gold, new Vector3(x, y, select), Quaternion.identity);
        ob.transform.SetParent(terrain.transform);
    }
    List<float> CreateListFromRange(int start, int end)
    {
        List<float> result = new List<float>();

        for (int i = start; i < end; i++)
        {
            result.Add((float)i);
        }

        return result;
    }


}