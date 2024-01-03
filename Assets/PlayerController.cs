using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public TerrainGenerator terrainGenerator;
    [SerializeField] private float speed;

    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private AnimationCurve rotateCurve;
    [SerializeField] private AnimationCurve scaleCurve;

    [SerializeField] private float jumpTime = 0.3f;
    [SerializeField] private float scaleTime = 0.03f;

    private bool canJump = true;
    
    private GameObject rayCastPoint;
    private Vector3 direction;
    private bool onPlank = false;
    public LayerMask plank;
    private GameObject planket;
 
    private float currentX = 0;

    public void PlayerControllerStart()
    {
        float currentHeight = Height(terrainGenerator.CurrentTerrainJumpIn(currentX));
        gameObject.transform.position = new Vector3(0, currentHeight, 0);
        rayCastPoint = gameObject.transform.GetChild(1).gameObject;
    }

    private void Update()
    {
        if (canJump)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                currentX++;
                if (planket != null)
                {
                    planket.GetComponent<Planket>().currentJumpPoint = 100;
                }
                direction = new Vector3(1, 0, 0);
                MoveCharacter(new Vector3(1, 0, 0), 0);
           
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                currentX--;
                if (planket != null)
                {
                    planket.GetComponent<Planket>().currentJumpPoint = 100;
                }
                direction = new Vector3(-1, 0, 0);
                MoveCharacter(new Vector3(-1, 0, 0),180);
            
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                direction = new Vector3(0, 0, -1);
                if (planket != null)
                {
                    planket.GetComponent<Planket>().currentJumpPoint++;
                }
                MoveCharacter(new Vector3(0, 0, -1),90);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (planket != null)
                {
                    planket.GetComponent<Planket>().currentJumpPoint--;
                }
                direction = new Vector3(0, 0, 1);
                MoveCharacter(new Vector3(0, 0, 1),270);
            }
        }

        if (onPlank)
        {
            transform.parent = planket.transform.GetChild(0).transform;
        }
        else
        {
            transform.parent = null;
        }
        
    }

    private void MoveCharacter(Vector3 difference, float angle)
    {
        canJump = false;
        float currentHeight = 1;
        bool stillOnPlank = false;
        
        if (onPlank && StillOnPlanket())
        {
            stillOnPlank = true;
        }
        else
        {
            if (CanJumpOnPlank())
            {
                stillOnPlank = true;
                
                CheckNearestJumpPoint();
                
            }
            else
            {
                currentHeight = Height(terrainGenerator.CurrentTerrainJumpIn(currentX));
            }

        }
        //audioSource.Play();
           
        
        StartCoroutine(PLayerMoving(difference, currentHeight,angle,stillOnPlank));
        
        terrainGenerator.SpawnTerrain(false, transform.position);
    
    }
    
    private Extns.StopCouroutine flagContainer = new Extns.StopCouroutine();
    IEnumerator PLayerMoving(Vector3 difference, float currentHeight , float angle, bool stillOnPlank)
    {
        yield return TweenScale(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.3f, 0.7f),
            new Vector3(0, -0.162f, 0), new Vector3(0, -0.31f, 0));

        yield return new WaitForSeconds(scaleTime);
        
        MoveSmooth(difference, currentHeight, stillOnPlank);
        RotateSmooth(angle);
        
        yield return TweenScale(new Vector3(0.5f, 0.3f, 0.7f), new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0, -0.31f, 0), new Vector3(0, -0.162f, 0));
        
        yield return new WaitForSeconds(jumpTime);

        canJump = true;
        if (stillOnPlank)
        {
            planket.GetComponent<Planket>().Floating();
            onPlank = true;
        }
        else
        {
            onPlank = false;
        }
    }
    IEnumerator TweenScale(
            Vector3 startScale, Vector3 targetScale,
            Vector3 startPosition, Vector3 targetPosition)
    {
        var childObject = gameObject.transform.GetChild(0);
        yield return scaleTime.ScaleObject(
            (s) => childObject.localScale = s, startScale, targetScale,
            (p) => childObject.localPosition = p, startPosition, targetPosition,
            scaleCurve);
    }
    void MoveSmooth(Vector3 difference, float height, bool stillOnPlank)
    {
        if (!stillOnPlank)
        {
            var newPosition = transform.position + difference;
            newPosition = new Vector3(newPosition.x, height, newPosition.z);

            newPosition = onPlank ? new Vector3((float)Math.Floor(newPosition.x+0.5f), newPosition.y, newPosition.z) : newPosition;

            StartCoroutine(jumpTime.ParabolJump((p) => gameObject.transform.position = p, 
                gameObject.transform.position, newPosition, jumpCurve));
        }
        else
        {
            var plk = planket.GetComponent<Planket>();
            var jumpPoint = plk.JumpPoint[plk.currentJumpPoint];
            
            StartCoroutine(jumpTime.ParabolJump((p) => gameObject.transform.position = p, 
                gameObject.transform.position, jumpPoint.transform, jumpCurve));
        }
        

    }
    void RotateSmooth(float angle)
    {
        if (gameObject.transform.eulerAngles.y == angle) return;
        StartCoroutine(jumpTime.TweengMinAngleRotation((p) => gameObject.transform.eulerAngles = p, 
            gameObject.transform.eulerAngles, new Vector3(0,angle,0) , rotateCurve));
    }

    float Height(GameObject currentTerrainJumpIn)
    {
        //Road = 0.931
        //Track = 1.086
        //Grass = 1
        //Water = 0.871
        string tag = currentTerrainJumpIn.tag;
        switch (tag)
        {
            case "Road":
                return 0.931f;
            case "Track":
                return 1.086f;
            case "Grass":
                return 1f;
            case "Water":
                return 0.871f;
            case "Plank":
                return 1f;
        }
        return 0;
    }
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Debug.DrawLine(rayCastPoint.transform.position , rayCastPoint.transform.position  + direction * 1);
    //}

    public bool CanJumpOnPlank()
    {
        RaycastHit hit;

        if (Physics.Raycast(rayCastPoint.transform.position, direction, out hit, 1, plank, QueryTriggerInteraction.UseGlobal))
        {
            if (planket == null || hit.collider.gameObject != planket.gameObject)
            {
                planket = hit.transform.gameObject;
                return true;
            }
        }
        else
        {
            var nextTerrain = terrainGenerator.CurrentTerrainJumpIn(currentX);

            if (nextTerrain.tag == "Water")
            {
                var moip = nextTerrain.transform.GetChild(2).GetComponent<MovingObjectInstancePoint>();
                var plankSpeed = moip.rightDrection ? moip.plankSpeed : -1 * moip.plankSpeed;
                var changingZ = plankSpeed * (jumpTime + scaleTime);
                Debug.Log(changingZ);

                if (Physics.Raycast(rayCastPoint.transform.position - new Vector3(0, 0, changingZ), direction, out hit, 1, plank, QueryTriggerInteraction.UseGlobal))
                {
                    if (planket == null || hit.collider.gameObject != planket.gameObject)
                    {
                        planket = hit.transform.gameObject;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void CheckNearestJumpPoint()
     {
         var planket = this.planket.GetComponent<Planket>();
         float minDistance = 100;
         for (int i = 0; i < planket.JumpPoint.Count; i++)
         {
             float distance = Vector3.Distance(gameObject.transform.position, planket.JumpPoint[i].transform.localToWorldMatrix.GetPosition());
             if (minDistance > distance)
             {
                 minDistance = distance;
                 planket.currentJumpPoint = i;
             }
         }
     }

     public bool StillOnPlanket()
     {
         var planket = this.planket.GetComponent<Planket>();
         if (planket.currentJumpPoint >= 0 && planket.currentJumpPoint <= planket.JumpPoint.Count - 1)
         {
             return true;
         }
         return false;
     }
}
