using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int minDistanceFromPlayer;
    [SerializeField] private int maxTerrainCount;
    [SerializeField] private List<TerrainData> terrainDatas = new List<TerrainData>();
    [SerializeField] private Transform terrainHolder;

    private List<GameObject> currentTerrains = new List<GameObject>();
    [HideInInspector] public Vector3 currentPosition = new Vector3(0, 0, 0);
    private void Start()
    {
        for (int i = 0; i < maxTerrainCount; i++)
        {
            SpawnTerrain(true,new Vector3(0,0,0));
        }
    }
    public void SpawnTerrain(bool isStart, Vector3 playerPos )
    {
        if(currentPosition.x - playerPos.x < minDistanceFromPlayer)
        {
        int wichTerrain = Random.Range(0, terrainDatas.Count);
        int terrainInSuccession = Random.Range(1, terrainDatas[wichTerrain].maxInSuccession);
        for (int i = 0; i < terrainInSuccession; i++)
        {
            GameObject terrain = Instantiate(terrainDatas[wichTerrain].possibleTerrain[Random.Range(0,terrainDatas[wichTerrain].possibleTerrain.Count)], currentPosition, Quaternion.identity, terrainHolder);
            terrain.transform.SetParent(gameObject.transform);
            
            currentTerrains.Add(terrain);
            if (!isStart) 
            {
                if (currentTerrains.Count > maxTerrainCount)
                {
                     Destroy(currentTerrains[0]);
                     currentTerrains.RemoveAt(0);
                }
            }
            currentPosition.x++;
        }
        }
    }

}