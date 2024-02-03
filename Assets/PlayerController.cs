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

    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private AnimationCurve rotateCurve;
    [SerializeField] private AnimationCurve scaleCurve;

    [SerializeField] private float jumpTime = 0.3f;
    [SerializeField] private float scaleTime = 0.03f;

    private bool canJump = false;
    private bool onWater = false;

    private GameObject rayCastPoint;
    private Vector3 direction;
    private bool onPlank = false;
    public LayerMask plank;
    private GameObject planket;

    public LayerMask obstacle;

    private float currentX = 0;

    public GameData gameData;

    public GameObject waterEffect;
    public GameObject cashEffect;

    public LayerMask vehicle;
    

    public void ReStart()
    {
        canJump = true;
        EventMoveCam();
        ReLoadData();
    }
    public void ReLoadData()
    {
        gameObject.transform.position = new Vector3(0, 1, 0);
        gameObject.GetComponent<BoxCollider>().enabled = true;
        hawk.transform.position = new Vector3(-20, 1, 0);
 
        currentX = 0;
        onWater = false;
        hawkSee = false;

        ReLoadParentStatus();

        cashWihtCar = false;

        planket = null;

        if (transform.childCount > 2)
        {
            Destroy(transform.GetChild(2).gameObject);
        }

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
    public void HideScreenCapture()
    {
        screenCapture.gameObject.SetActive(false);
    }
    public void ReLoadParentStatus()
    {
        onPlank = false;
        onCar = false;
        onHawks = false;
        transform.parent = null;
    }
    public void PlayerControllerStart()
    {
        float currentHeight = Height(terrainGenerator.CurrentTerrainJumpIn(currentX));
        gameObject.transform.position = new Vector3(0, currentHeight, 0);
        rayCastPoint = gameObject.transform.GetChild(1).gameObject;
    }
      

    private void OnSwipeLeft()
        {
        direction = new Vector3(0, 0, 1);
        if (!HaveObstacleInThisDirection())
        {
            if (planket != null)
            {
                planket.GetComponent<Planket>().currentJumpPoint--;
            }
            MoveCharacter(new Vector3(0, 0, 1), 270);
        }
        else
        {
            RotateSmooth(270);

        }

    }

    private void OnSwipeRight()
    {
        direction = new Vector3(0, 0, -1);
        if (!HaveObstacleInThisDirection())
        {
            if (planket != null)
            {
                planket.GetComponent<Planket>().currentJumpPoint++;
            }
            MoveCharacter(new Vector3(0, 0, -1), 90);
        }
        else
        {
            RotateSmooth(90);

        }
    }

    private void OnSwipeUp()
    {
        direction = new Vector3(1, 0, 0);
        if (!HaveObstacleInThisDirection())
        {
            currentX++;
            if (planket != null)
            {
                planket.GetComponent<Planket>().currentJumpPoint = 100;
            }
            MoveCharacter(new Vector3(1, 0, 0), 0);
        }
        else
        {
            RotateSmooth(0);
        }
        //floppyControll.JumpAnim();
    }

    private void OnSwipeBack()
    {
        direction = new Vector3(-1, 0, 0);
        if (!HaveObstacleInThisDirection())
        {
            EventMoveBack();
            currentX--;
            if (planket != null)
            {
                planket.GetComponent<Planket>().currentJumpPoint = 100;
            }
            direction = new Vector3(-1, 0, 0);
            MoveCharacter(new Vector3(-1, 0, 0), 180);
        }
        else
        {
            RotateSmooth(180);
        }
    }


    private void Update()
    {
        if (canJump)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                OnSwipeUp();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                OnSwipeBack();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                OnSwipeRight();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                OnSwipeLeft();
            }
        }

        if (onPlank)
        {
            transform.parent = planket.transform.GetChild(0).transform;
        }
        else if (onCar)
        {
            transform.parent = car.transform;
        }
        else if (onHawks)
        {
            transform.parent = hawk.transform;
        }
        else
        {
            transform.parent = null;
        }
    }
  
    public bool HaveObstacleInThisDirection()
    {
        RaycastHit hit;
        if (Physics.Raycast(rayCastPoint.transform.position, direction, out hit, 1, obstacle, QueryTriggerInteraction.UseGlobal))
        {
            return true;
        }
        return false;
    }

    private void MoveCharacter(Vector3 difference, float angle)
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

                    CheckNearestJumpPoint();

                }
                else
                {
                    currentHeight = Height(terrainGenerator.CurrentTerrainJumpIn(currentX));
                }

            }
            //audioSource.Play();


            StartCoroutine(PLayerMoving(difference, currentHeight, angle, stillOnPlank));

            terrainGenerator.SpawnTerrain(false, transform.position);
        }
        

    }
    GameObject car;
    bool onCar = false;
    bool cashWihtCar = false;
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
    public AnimationCurve CashDieCurve;
    IEnumerator CashDie(Vector3 difference, float currentHeight, float angle)
    {
        yield return TweenScale(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.3f, 0.7f),
          new Vector3(0, -0.162f, 0), new Vector3(0, -0.31f, 0));

        yield return new WaitForSeconds(scaleTime);

        cashWihtCar = true;

        difference = difference == new Vector3(1, 0, 0) ? new Vector3(0.5f, 0, 0) : new Vector3(-0.5f, 0, 0);

        MoveSmooth(difference, currentHeight, false);
        RotateSmooth(angle);

        var childObject = gameObject.transform.GetChild(2);
        StartCoroutine(jumpTime.Tweeng((p) => childObject.localScale = p,
              childObject.localScale,  new Vector3(0.1f, 0.5f,0.5f), CashDieCurve));

        yield return new WaitForSeconds(jumpTime);
        InstanceCashEffect();
        onCar = true;
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

    IEnumerator PLayerMoving(Vector3 difference, float currentHeight, float angle, bool stillOnPlank)
    {
        yield return TweenScale(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.3f, 0.7f),
            new Vector3(0, -0.162f, 0), new Vector3(0, -0.31f, 0));

        yield return new WaitForSeconds(scaleTime);

        MoveSmooth(difference, currentHeight, stillOnPlank);

        RotateSmooth(angle);

        yield return TweenScale(new Vector3(0.5f, 0.3f, 0.7f), new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0, -0.31f, 0), new Vector3(0, -0.162f, 0));

        if (!stillOnPlank)
        {
            onPlank = false;
            this.planket = null;
        }

        yield return new WaitForSeconds(jumpTime);

        if (!hawkSee)
        {
            canJump = true;
        }
        
        GetComponent<BoxCollider>().enabled = true;
        if (stillOnPlank)
        {
            planket.GetComponent<Planket>().Floating();
            onPlank = true;
        }

        EndGameProcessing(angle);

    }
    public void EndGameProcessing(float angle)
    {
        if (onWater)
        {
            EventDie();

            MoveSmooth(new Vector3(0, 0, 0), -2, false);

            var ef = Instantiate(waterEffect);
            Destroy(ef, 3);
            ef.transform.position = gameObject.transform.position - new Vector3(0, 0.8f, 0);


        }
        else if (angle == 0)
        {
            EventMoveOn();
        }
    }
    public void InstanceCashEffect()
    {
        var ef = Instantiate(cashEffect);
        Destroy(ef, 3);
        ef.transform.position = gameObject.transform.position;
    }
    public
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
    void MoveSmooth(Vector3 difference, float height, bool stillOnPlank)
    {
        if (!stillOnPlank)
        {
            var newPosition = transform.position + difference;
            newPosition = new Vector3(newPosition.x, height, newPosition.z);

            var nextTerrain = terrainGenerator.CurrentTerrainJumpIn(currentX);

            newPosition = onPlank ? new Vector3((float)Math.Floor(newPosition.x + 0.5f), newPosition.y, (float)Math.Floor(newPosition.z + 0.5f)) : newPosition;

            if (nextTerrain.tag == "Grass")
            {
                for (int i = 1; i < nextTerrain.transform.childCount - 1; i++)
                {
                    if (nextTerrain.transform.GetChild(i).transform.position == newPosition)
                    {
                        var planketSpeed = planket.GetComponent<Vehicle>().movingSpeed;
                        var newZ = planketSpeed > 0 ? newPosition.z - 1 : newPosition.z + 1;
                        newPosition = new Vector3(newPosition.x, newPosition.y, newZ);
                        Debug.Log("nhay vao chuong ngai vat");
                        break;
                    }

                }
            }


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
                float changingZ = 0;
                if (nextTerrain.name != "RiverWithDuckweed")
                {
                    var moip = nextTerrain.transform.GetChild(2).GetComponent<MovingObjectInstancePoint>();
                    var plankSpeed = moip.rightDrection ? moip.plankSpeed : -1 * moip.plankSpeed;
                    changingZ = plankSpeed * (jumpTime + scaleTime);
                }
                else if (planket != null) // mục tiêu chính là nếu đang trên planket có thể nhảy chính sác vào duckweed
                {
                    var plankSpeed = -1 * this.planket.GetComponent<Vehicle>().movingSpeed;
                    changingZ = plankSpeed * (jumpTime + scaleTime);

                }
                //Debug.Log(changingZ);

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
            onPlank = false;
            onHawks = true;
            direction = new Vector3(0, 0, 0);
            gameObject.GetComponent<BoxCollider>().enabled = false;
            InstanceCashEffect();
            EventDie();
        }
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
        if(transform.position.x > gameData.highestPoint)
        {
            TakeScreenShots();
        }
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

    public GameObject hawk;
    public AnimationCurve hawkCurve;
    
    bool onHawks = false;
    bool hawkSee = false;
    public void EventSeenByHawks()
    {
        hawkSee = true;
        CaughtByHawks();
    }

    public void CaughtByHawks()
    {
        canJump = false;
        hawk.transform.position = new Vector3(gameObject.transform.position.x - 10, hawk.transform.position.y, gameObject.transform.position.z);
        hawk.GetComponent<AudioSource>().Play();
        StartCoroutine(1f.Tweeng((p) => hawk.transform.position = p,
              hawk.transform.position, gameObject.transform.position + new Vector3(20, 0, 0), hawkCurve));
    }

    public Image screenCapture;
    Sprite previewImage;
    public void TakeScreenShots()
    {
        var time = DateTime.Now.ToString("dd_MM_yyyy_HH-mm-ss");
        var screenshotname = $"Assets/ScreenCapture/Screen" + "_" + time + ".png";

        ScreenCapture.CaptureScreenshot(screenshotname);

        StartCoroutine(WaitScreenShot(screenshotname));
        
    }
    public IEnumerator WaitScreenShot(string screenshotname)
    {
        yield return new WaitForSeconds(0.1f);

        // Đọc dữ liệu ảnh từ tệp hình ảnh
        byte[] fileData = File.ReadAllBytes(screenshotname);
        Texture2D texture = new Texture2D(2, 2); 
        texture.LoadImage(fileData); // Đọc dữ liệu hình ảnh

        // Tạo Sprite từ Texture2D
        previewImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        LoadScreenShots();
        File.Delete(screenshotname);
    }


    void LoadScreenShots()
    {
        screenCapture.gameObject.SetActive(true);
        screenCapture.transform.GetChild(0).GetComponent<Image>().sprite = previewImage;
    }
}
