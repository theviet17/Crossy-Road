using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public TerrainGenerator terrainGenerator;
    [SerializeField] private float speed;

    [Header("jump curve")]
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private AnimationCurve rotateCurve;
    [SerializeField] private AnimationCurve scaleCurve;

    [Header("1 jump time")]
    [SerializeField] private float jumpTime = 0.3f;
    [SerializeField] private float scaleTime = 0.03f;

    [Header("raycast")]
    private GameObject rayCastPoint;
    private Vector3 direction;

    [Header("check plank")]
    public LayerMask plank;
    private GameObject planket;
    private bool onPlank = false;


    [Header("check vehicle")]
    public LayerMask vehicle;
    GameObject car;
    bool cashWihtCar = false;

    [Header("check hard obstacle")]
    public LayerMask obstacle;

    [Header("die effect")]
    public GameObject waterEffect;
    public GameObject cashEffect;

    [Header("current position")]
    private float currentX = 0;


    [Header("other")]
    public GameData gameData;
    public AnimationCurve CashDieCurve;
    private bool canJump = false;
    private bool onWater = false;
    

    private void Update()
    {
        if (canJump)
        {
            HandleSwipeInput();
        }

        UpdateParentTransform();
    }
  
    public void ReStart()
    {
        canJump = true;
        EventMoveCam();
        ReLoadData();
    }
    
    public void PlayerControllerStart()
    {
        float currentHeight = Height(terrainGenerator.CurrentTerrainJumpIn(currentX));
        gameObject.transform.position = new Vector3(0, currentHeight, 0);
        rayCastPoint = gameObject.transform.GetChild(1).gameObject;
    }
      
    private void HandleObstacles(Vector3 difference, float angle)
    {
        canJump = false;

        GetComponent<BoxCollider>().enabled = false;

        float currentHeight = 1;

        bool stillOnPlank = false;

        if (CashWithVehicle())
        {
            onPlank = false;
            this.planket = null;
            StartCoroutine(CashDie(difference,1.5f,angle));
        }

        else
        {
            if (onPlank && StillOnPlanket())
            {
                stillOnPlank = true;
            }
            else
            {
                if (CanJumpOnPlank())
                {
                    stillOnPlank = true;

                    CheckNearestPointOnPlanket();

                }
                else
                {
                    currentHeight = Height(terrainGenerator.CurrentTerrainJumpIn(currentX));
                }

            }
            StartCoroutine(Jump(difference, currentHeight, angle, stillOnPlank));

            terrainGenerator.SpawnTerrain(false, transform.position);
        }
    }
    IEnumerator TweenScale(
           Vector3 startScale, Vector3 targetScale,
           Vector3 startPosition, Vector3 targetPosition)
    {
        var childObject = gameObject.transform.GetChild(2);
        yield return scaleTime.ScaleObject(
            (s) => childObject.localScale = s, startScale, targetScale,
            (p) => childObject.localPosition = p, startPosition, targetPosition,
            scaleCurve);
    }

    IEnumerator Jump(Vector3 difference, float currentHeight, float angle, bool stillOnPlank)
    {
        yield return TweenScale(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.3f, 0.7f),
            new Vector3(0, -0.162f, 0), new Vector3(0, -0.31f, 0));

        yield return new WaitForSeconds(scaleTime);

        AnimationJump(difference, currentHeight, stillOnPlank);

        AnimationRotate(angle);

        yield return TweenScale(new Vector3(0.5f, 0.3f, 0.7f), new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0, -0.31f, 0), new Vector3(0, -0.162f, 0));

        StartCoroutine(HandleAfterJump(stillOnPlank, angle));
  
    }
    IEnumerator HandleAfterJump(bool stillOnPlank, float angle)
    {
        if (!stillOnPlank)
        {
            ClearPlanket();
        }

        yield return new WaitForSeconds(jumpTime);

        // Note: khong the check cung luc 2 stillOnPlank 
        if (stillOnPlank)
        {
            DoAnimationPlanketFLoating();
            onPlank = true;
        }


        if (!hawkSee)
        {
            canJump = true;
        }

        GetComponent<BoxCollider>().enabled = true;

        HandlePoint(angle);
        
    }

    
    void HandlePoint(float angle)
    {
        if (!onWater)
        {
            if (angle == 0) // tien len
            {
                MoveOn();
            }
        }
        else
        {
            HandleJumpInWater();
        }
       
    }

    void HandleJumpInWater()
    {
        EventDie();

        AnimationJump(new Vector3(0, 0, 0), -2, false);

        var ef = Instantiate(waterEffect);
        Destroy(ef, 3);
        ef.transform.position = gameObject.transform.position - new Vector3(0, 0.8f, 0);
    }
    
    public void InstanceCashEffect()
    {
        var ef = Instantiate(cashEffect);
        Destroy(ef, 3);
        ef.transform.position = gameObject.transform.position;
    }
    public
   
    void AnimationJump(Vector3 difference, float height, bool stillOnPlank)
    {
        if (!stillOnPlank)
        {
            Vector3 newPosition = CalculateNewPosition(transform.position, difference, height);

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
    Vector3 CalculateNewPosition(Vector3 currentPosition, Vector3 difference, float height) // làm tròn giá trị khi nhảy lên bờ
    {
        Vector3 newPosition = currentPosition + difference;
        newPosition = new Vector3(newPosition.x, height, newPosition.z);

        GameObject nextTerrain = terrainGenerator.CurrentTerrainJumpIn(currentX);

        newPosition = onPlank ? new Vector3((float)Math.Floor(newPosition.x + 0.5f), newPosition.y, (float)Math.Floor(newPosition.z + 0.5f)) : newPosition;

        if (nextTerrain.tag == "Grass")
        {
            HandleGrassObstacle(ref newPosition, nextTerrain);
        }

        return newPosition;
    }

    void HandleGrassObstacle(ref Vector3 newPosition, GameObject nextTerrain) // nếu vị trí làm tròn là chướng ngại vật thì sẽ lùi sang trái hoặc sang phải  1 đơn vị tùy theo hướng di chuyển của planket 
    {
        for (int i = 1; i < nextTerrain.transform.childCount - 1; i++)
        {
            if (nextTerrain.transform.GetChild(i).transform.position == newPosition)
            {
                var planketSpeed = planket.GetComponent<Vehicle>().movingSpeed;
                var newZ = planketSpeed > 0 ? newPosition.z - 1 : newPosition.z + 1;
                newPosition = new Vector3(newPosition.x, newPosition.y, newZ);
                Debug.Log("Nhảy vào chướng ngại vật");
                break;
            }
        }
    }

    void AnimationRotate(float angle)
    {
        if (gameObject.transform.eulerAngles.y == angle) return;
        StartCoroutine(jumpTime.TweengMinAngleRotation((p) => gameObject.transform.eulerAngles = p,
            gameObject.transform.eulerAngles, new Vector3(0, angle, 0), rotateCurve));
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
                onWater = true;
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



    public bool CashWithVehicle()
    {
        RaycastHit hit;
        var nextTerrain = terrainGenerator.CurrentTerrainJumpIn(currentX);
        if (nextTerrain.tag == "Road" || nextTerrain.tag == "Track")
        {
            var vhip = nextTerrain.GetComponentInChildren<MovingObjectInstancePoint>();

            float vehicleSpeed = (vhip.baseSpeed.maxValue + vhip.baseSpeed.minValue) / 2;

            vehicleSpeed = vhip.rightDrection ? vehicleSpeed : -1 * vehicleSpeed;

            if (Physics.Raycast(rayCastPoint.transform.position - new Vector3(0, 0, vehicleSpeed * (jumpTime + scaleTime)), direction, out hit, 1, vehicle, QueryTriggerInteraction.UseGlobal))
            {
                car = hit.transform.gameObject;

                Debug.Log("Vachamvoixe");

                return true;
            }
        }

        return false;
    }



    public bool CanJumpOnPlank()
    {
        RaycastHit hit;
        // kiểm tra trực tiêp phía trước có planket hay không???
        if (Physics.Raycast(rayCastPoint.transform.position, direction, out hit, 1, plank, QueryTriggerInteraction.UseGlobal))
        {
            if (planket == null || hit.collider.gameObject != planket.gameObject)
            {
                planket = hit.transform.gameObject;
                return true;
            }
        }

        else
        { // nếu không sẽ thay đổi position của raycast dựa theo speed của planket
            var nextTerrain = terrainGenerator.CurrentTerrainJumpIn(currentX);

            if (nextTerrain.tag == "Water")
            {
                float changingZ = CalculateChangingZ(nextTerrain);

                if (Physics.Raycast(rayCastPoint.transform.position - new Vector3(0, 0, changingZ), direction, out hit, 1, plank, QueryTriggerInteraction.UseGlobal))
                {
                    if (ShouldUpdatePlanket(hit.collider.gameObject))
                    {
                        planket = hit.transform.gameObject;
                    }
                    return true;
                }
            }
        }

        return false;
    }
    private float CalculateChangingZ(GameObject nextTerrain)
    {
        float changingZ = 0;

        if (nextTerrain.name != "RiverWithDuckweed")
        {
            var moip = nextTerrain.transform.GetChild(2).GetComponent<MovingObjectInstancePoint>();
            var plankSpeed = moip.rightDrection ? moip.plankSpeed : -moip.plankSpeed;
            changingZ = plankSpeed * (jumpTime + scaleTime);
        }

        else if (planket != null)
        {
            var plankSpeed = -planket.GetComponent<Vehicle>().movingSpeed;
            changingZ = plankSpeed * (jumpTime + scaleTime);
        }

        return changingZ;
    }

    private bool ShouldUpdatePlanket(GameObject colliderObject)
    {
        return planket == null || colliderObject != planket.gameObject;
    }

    public void CheckNearestPointOnPlanket()
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Gold")
        {
            EventGold();
            Destroy(other.gameObject, 0.1f);
        }
        if (other.gameObject.tag == "Car")
        {
            if (!cashWihtCar)
            {
                CarCash();
            }
        }
        if (other.gameObject.tag == "Wave")
        {
            EventDie();
        }
        if (other.gameObject.name == "Hawks")
        {
            hawkCatch = true;

            //direction = new Vector3(0, 0, 0);

            gameObject.GetComponent<BoxCollider>().enabled = false;
            InstanceCashEffect();
            EventDie();
        }
    }
    
    public GameObject hawk;
    
    bool hawkCatch = false;
    bool hawkSee = false;
    public void EventSeenByHawks()
    {
        hawkSee = true;
        CaughtByHawks();
    }

    public void CaughtByHawks()
    {
        canJump = false;
        onPlank = false;
        hawk.transform.position = new Vector3(gameObject.transform.position.x - 10, hawk.transform.position.y, gameObject.transform.position.z);

        hawk.GetComponent<AudioSource>().Play();

        StartCoroutine(1f.Tweeng((p) => hawk.transform.position = p,
              hawk.transform.position, gameObject.transform.position + new Vector3(25, 0, 0)));
    }

    public void OnSwipeLeft()
    {

        //direction = new Vector3(0, 0, 1);
        //if (!HaveObstacleInThisDirection())
        //{
        //    if (planket != null)
        //    {
        //        planket.GetComponent<Planket>().currentJumpPoint--;
        //    }
        //    MoveCharacter(new Vector3(0, 0, 1), 270);
        //}
        //else
        //{
        //    RotateSmooth(270);

        //}
        Swipe(new Vector3(0, 0, 1), 270, 0, -1);

    }
    private void HandleSwipeInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) OnSwipeUp();
        if (Input.GetKeyDown(KeyCode.S)) OnSwipeBack();
        if (Input.GetKeyDown(KeyCode.D)) OnSwipeRight();
        if (Input.GetKeyDown(KeyCode.A)) OnSwipeLeft();
    }
    private void UpdateParentTransform()
    {
        Transform newParent = GetNewParentTransform();

        if (newParent != null)
        {
            transform.parent = newParent;
        }
        else
        {
            transform.parent = null;
        }
    }
    private Transform GetNewParentTransform()
    {
        if (onPlank) return planket.transform.GetChild(0).transform;
        if (cashWihtCar && car != null) return car.transform;
        if (hawkCatch) return hawk.transform;

        return null;
    }


    public void OnSwipeRight()
    {
        //direction = new Vector3(0, 0, -1);
        //if (!HaveObstacleInThisDirection())
        //{
        //    if (planket != null)
        //    {
        //        planket.GetComponent<Planket>().currentJumpPoint++;
        //    }
        //    MoveCharacter(new Vector3(0, 0, -1), 90);
        //}
        //else
        //{
        //    RotateSmooth(90);

        //}
        Swipe(new Vector3(0, 0, -1), 90, 0, 1);
    }

    public void OnSwipeUp()
    {
        //direction = new Vector3(1, 0, 0);
        //if (!HaveObstacleInThisDirection())
        //{
        //    currentX++;
        //    if (planket != null)
        //    {
        //        planket.GetComponent<Planket>().currentJumpPoint = 100;
        //    }
        //    MoveCharacter(new Vector3(1, 0, 0), 0);
        //}
        //else
        //{
        //    RotateSmooth(0);
        //}

        Swipe(new Vector3(1, 0, 0), 0, 1, 99);
        //floppyControll.JumpAnim();
    }


    public void OnSwipeBack()
    {
        //direction = new Vector3(-1, 0, 0);
        //if (!HaveObstacleInThisDirection())
        //{
        //    EventMoveBack();
        //    currentX--;
        //    if (planket != null)
        //    {
        //        planket.GetComponent<Planket>().currentJumpPoint = 100;
        //    }
        //    direction = new Vector3(-1, 0, 0);
        //    MoveCharacter(new Vector3(-1, 0, 0), 180);
        //}
        //else
        //{
        //    RotateSmooth(180);
        //}
        Swipe(new Vector3(-1, 0, 0), 180, -1, 99);
    }
    void Swipe(Vector3 _direction, float angle, float currentXExtraValue, int currentJumpPointExtraValue)
    {
        this.direction = _direction;
        if (!HaveHardObstacle())
        {
            currentX += currentXExtraValue;
            if (planket != null)
            {
                planket.GetComponent<Planket>().currentJumpPoint += currentJumpPointExtraValue;
            }
            HandleObstacles(_direction, angle);
        }
        else
        {
            AnimationRotate(angle);
        }
    }

    public bool HaveHardObstacle()
    {
        RaycastHit hit;
        if (Physics.Raycast(rayCastPoint.transform.position, direction, out hit, 1, obstacle, QueryTriggerInteraction.UseGlobal))
        {
            return true;
        }
        return false;
    }

    public void ReLoadData()
    {
        gameObject.transform.position = new Vector3(0, 1, 0);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.GetComponent<BoxCollider>().enabled = true;

        hawk.transform.position = new Vector3(-20, 1, 0);

        currentX = 0;
        onWater = false;
        hawkSee = false;

        ReLoadParentStatus();

        //Clear chill
        if (transform.childCount > 2)
        {
            Destroy(transform.GetChild(2).gameObject);
        }
        //Instance Chill
        for (int i = 0; i < gameData.characters.Count; i++)
        {
            if (gameData.characters[i].type == 3)
            {
                var playerModel = Instantiate(gameData.characters[i].prefab);
                playerModel.gameObject.SetActive(true);
                playerModel.transform.SetParent(gameObject.transform);
                playerModel.transform.localPosition = transform.GetChild(0).localPosition;
                playerModel.transform.localEulerAngles = transform.GetChild(0).localEulerAngles;
                playerModel.transform.localScale = transform.GetChild(0).localScale;
                transform.GetChild(0).gameObject.SetActive(false);
                break;
            }
        }

        EventStart();


        PlayerControllerStart();
    }

    public void ReLoadParentStatus()
    {
        onPlank = false;
        cashWihtCar = false;
        hawkCatch = false;
        planket = null;
        car = null;

        transform.parent = null;
    }

    IEnumerator CashDie(Vector3 difference, float currentHeight, float angle)
    {
        yield return TweenScale(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.3f, 0.7f),
          new Vector3(0, -0.162f, 0), new Vector3(0, -0.31f, 0));

        yield return new WaitForSeconds(scaleTime);

        difference = difference == new Vector3(1, 0, 0) ? new Vector3(0.5f, 0, 0) : new Vector3(-0.5f, 0, 0);

        AnimationJump(difference, currentHeight, false);
        AnimationRotate(angle);

        var childObject = gameObject.transform.GetChild(2);
        StartCoroutine(jumpTime.Tweeng((p) => childObject.localScale = p,
              childObject.localScale, new Vector3(0.1f, 0.5f, 0.5f), CashDieCurve));

        yield return new WaitForSeconds(jumpTime);
        InstanceCashEffect();

        cashWihtCar = true;

        EventDie();
    }

    void CarCash()
    {
        cashWihtCar = true;
        EventDie();
        var childObject = gameObject.transform.GetChild(2);
        StartCoroutine(scaleTime.Tweeng((p) => childObject.localScale = p,
              childObject.localScale, new Vector3(0.7f, 0.05f, 0.7f), CashDieCurve));

        StartCoroutine(scaleTime.Tweeng((p) => childObject.localPosition = p,
              childObject.localPosition, new Vector3(0, -0.4f, 0), CashDieCurve));
        InstanceCashEffect();

    }
    void ClearPlanket()
    {
        onPlank = false;
        this.planket = null;
    }
    void DoAnimationPlanketFLoating()
    {
        planket.GetComponent<Planket>().Floating();
    }

    // Event
    public event Action Gold;
    public void EventGold()
    {
        Gold?.Invoke();
    }

    public event Action MoveOn;
    public void EventMoveOn()
    {
        MoveOn?.Invoke();
    }

    public event Action MoveBack;
    public void EventMoveBack()
    {
        MoveBack?.Invoke();
    }

    public event Action Die;
    public void EventDie()
    {
        canJump = false;

        //
        Die?.Invoke();
    }

    public event Action Start;
    public void EventStart()
    {
        Start?.Invoke();
    }

    public event Action MoveCam;
    public void EventMoveCam()
    {
        MoveCam?.Invoke();
    }



}
