using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public TerrainGenerator terrainGenerator;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveCharacter(new Vector3(1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveCharacter(new Vector3(-1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveCharacter(new Vector3(0, 0, -1));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveCharacter(new Vector3(0, 0, 1));
        }
        

    }

    private void MoveCharacter(Vector3 difference)
    {
        //audioSource.Play();
        //animator.SetTrigger("hop");
        //transform.position = Vector3.Lerp(transform.position, transform.position + difference, Time.deltaTime * speed);
        transform.position += difference;
        terrainGenerator.SpawnTerrain(false, transform.position);
    }
}
