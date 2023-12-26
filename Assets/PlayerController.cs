using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public TerrainGenerator terrainGenerator;
    [SerializeField] private float speed;

    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private AnimationCurve rotateCurve;

    [SerializeField] private float jumpTime = 0.3f;
    //private GameObject currentTerrainJumpIn;
    private float currentX = 0;

    public void PlayerControllerStart()
    {
        //currentTerrainJumpIn = terrainGenerator.CurrentTerrainJumpIn(currentX);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentX++;
            MoveCharacter(new Vector3(1, 0, 0), 0);
           
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentX--;
            MoveCharacter(new Vector3(-1, 0, 0),180);
            
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveCharacter(new Vector3(0, 0, -1),90);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveCharacter(new Vector3(0, 0, 1),270);
        }
        

    }

    private void MoveCharacter(Vector3 difference, float angle)
    {
        //audioSource.Play();
        //animator.SetTrigger("hop");
        //transform.position = Vector3.Lerp(transform.position, transform.position + difference, Time.deltaTime * speed);
        float currentHeight = Height(terrainGenerator.CurrentTerrainJumpIn(currentX));
        
        MoveSmooth(difference, currentHeight);
        RotateSmooth(angle);
        
        terrainGenerator.SpawnTerrain(false, transform.position);
    }

    void MoveSmooth(Vector3 difference, float height)
    {
        var newPosition = transform.position + difference;
        newPosition = new Vector3(newPosition.x, height, newPosition.z);
        
        StartCoroutine(jumpTime.Tweeng((p) => gameObject.transform.position = p, 
            gameObject.transform.position, newPosition, jumpCurve,0));

    }
    void RotateSmooth(float angle)
    {
        if (gameObject.transform.eulerAngles.y != angle)
        {
            float deltaAngle = Mathf.DeltaAngle(gameObject.transform.eulerAngles.y, angle); // tính góc nhỏ nhất từ góc ban đầu đến góc cần quay tới
            Debug.Log(1);
            var targetAngle = gameObject.transform.eulerAngles + new Vector3(0, deltaAngle, 0);
        
            StartCoroutine(jumpTime.Tweeng((p) => gameObject.transform.eulerAngles = p, 
                gameObject.transform.eulerAngles,targetAngle , rotateCurve));
        }
    }

    float Height(GameObject currentTerrainJumpIn)
    {
        //Road = 0.931
        //Track = 1.086
        //Grass = 1
        //Water = 0.871
        string terrainType = currentTerrainJumpIn.tag;
        switch (terrainType)
        {
            case "Road":
                return 0.931f;
            case "Track":
                return 1.086f;
            case "Grass":
                return 1f;
            case "Water":
                return 0.871f;
        }
        return 0;
    }
}
