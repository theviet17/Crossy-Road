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
    [SerializeField] private AnimationCurve scaleCurve;

    [SerializeField] private float jumpTime = 0.3f;
    //private GameObject currentTerrainJumpIn;
    private float currentX = 0;

    public void PlayerControllerStart()
    {
        float currentHeight = Height(terrainGenerator.CurrentTerrainJumpIn(currentX));
        gameObject.transform.position = new Vector3(0, currentHeight, 0);
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
        if (Input.GetKeyDown(KeyCode.T))
        {
            flagContainer.flag = true;
        }

    }

    private void MoveCharacter(Vector3 difference, float angle)
    {
        //audioSource.Play();
        float currentHeight = Height(terrainGenerator.CurrentTerrainJumpIn(currentX));
        StartCoroutine(PLayerMoving(difference, currentHeight,angle));
        
        terrainGenerator.SpawnTerrain(false, transform.position);
    }
    private Extns.StopCouroutine flagContainer = new Extns.StopCouroutine();
    IEnumerator PLayerMoving(Vector3 difference, float currentHeight , float angle)
    {
        var childObject = gameObject.transform.GetChild(0);
        IEnumerator TweenScale(
            Vector3 startScale, Vector3 targetScale, 
            Vector3 startPosition, Vector3 targetPosition)
        {
            yield return 0.03f.ScaleObject(
                (s) => childObject.localScale = s, startScale, targetScale,
                (p) => childObject.localPosition = p, startPosition, targetPosition,
                scaleCurve, flagContainer);
        }

        yield return TweenScale(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.3f, 0.7f),
            new Vector3(0, -0.162f, 0), new Vector3(0, -0.31f, 0));

        yield return new WaitForSeconds(0.03f);
        
        MoveSmooth(difference, currentHeight);
        RotateSmooth(angle);
        
        yield return TweenScale(new Vector3(0.5f, 0.3f, 0.7f), new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0, -0.31f, 0), new Vector3(0, -0.162f, 0));
        
        yield return new WaitForSeconds(jumpTime);
   
        
    }
    void MoveSmooth(Vector3 difference, float height)
    {
        var newPosition = transform.position + difference;
        newPosition = new Vector3(newPosition.x, height, newPosition.z);
        
        StartCoroutine(jumpTime.Tweeng((p) => gameObject.transform.position = p, 
            gameObject.transform.position, newPosition, jumpCurve,flagContainer));

    }
    void RotateSmooth(float angle)
    {
        if (gameObject.transform.eulerAngles.y == angle) return;
        StartCoroutine(jumpTime.TweengMinAngleRotation((p) => gameObject.transform.eulerAngles = p, 
            gameObject.transform.eulerAngles, new Vector3(0,angle,0) , rotateCurve,flagContainer));
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
