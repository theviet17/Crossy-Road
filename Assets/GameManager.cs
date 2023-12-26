using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private PlayerController playerController;
    void Start()
    {
        terrainGenerator.TerrainGeneratorStart();
        playerController.PlayerControllerStart();
    }

}
