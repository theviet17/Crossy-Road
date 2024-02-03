
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private PlayerController playerController;


    [SerializeField] private GameObject logoPanel;
    [SerializeField] private Vector3 logoStart;
    [SerializeField] private Vector3 logoPose;
    [SerializeField] private Vector3 logoEnd;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float logoLoadTime = 0.3f;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private SelectedCharacter selectedCharacter;

    [SerializeField] public GameData gameData;
    public Text Point1;
    public Text Point2;
    private int point;
    public Text HighestPoint;
    public Text Gold;
    private int gold;
    void Start()
    {
        Screen.SetResolution(1080, 1920, fullscreen: true);

        if (File.Exists(Application.dataPath + "/GameData.json"))
        {
            // Tệp tồn tại
            Debug.Log("Tệp tồn tại: ");
            GameData loadedData = gameData.LoadNewGameData();
            gameData = loadedData;
        }
        else
        {
            // Tệp không tồn tại
            Debug.Log("Tệp không tồn tại: ");
            gameData.SaveToJson();
        }
        Debug.Log(Application.dataPath + "/GameData.json");
        playerController.gameData = gameData;
        terrainGenerator.gameData = gameData;
        selectedCharacter.gameData = gameData;
        //gameData = selectedCharacter.gameData;

        terrainGenerator.TerrainGeneratorStart();
        playerController.PlayerControllerStart();

        playerController.MoveOn += IncreasePoint;
        //playerController.MoveBack += MinusPoint;
        playerController.Gold += IncreaseGold;
        playerController.Die += EndGame;

        ShowGameData();
    }
    public void Update()
    {
        Detector();
    }

    // Input form gameData
    // OutPut show highestPoint, show gold 
    void ShowGameData() 
    {
        HighestPoint.text = gameData.highestPoint.ToString();
        gold = gameData.gold;
        Gold.text = gameData.gold.ToString();
    }


    // OutPut regenerate map ,  clear player data
    void ReStart()
    {
        //if()
        HideScreenCapture();
        playerController.ReStart();

        menuPanel.gameObject.SetActive(false);
    }

    // OutPut show Character Panel ,  hide menu Panel
    void CharacterLoad()
    {
        HideScreenCapture();
        selectedCharacter.gameObject.SetActive(true);
        selectedCharacter.StartCharacterPanel();


        menuPanel.gameObject.SetActive(false);
    }

    // OutPut hide Character Panel ,  show menu Panel
    void CharacterClose()
    {
       
        selectedCharacter.gameObject.SetActive(false);

        ShowMenuPanel();
    }

   
    void EndGame()
    {
        if (point > gameData.highestPoint)
        {
            TakeScreenShots();
        }
        StartCoroutine(WaitReLoadNewGame());
    }
 
    

    // OutPut Save gamedata, show menu panel
    IEnumerator WaitReLoadNewGame()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(LogoLoad(ReGenerateMapAndShowUIPanel));
    }

    void ReGenerateMapAndShowUIPanel()
    {
        EndGameDataSave();

        playerController.ReLoadParentStatus();
        terrainGenerator.ReStart();
        playerController.ReLoadData();
        ShowMenuPanel();
    }



    public void Play_Button()
    {
        StartCoroutine(LogoLoad(ReStart));
    }

    public void Character_Button()
    {
        StartCoroutine(LogoLoad(CharacterLoad));
    }
    public void CloseCharacter_Button()
    {
        StartCoroutine(LogoLoad(CharacterClose));
    }

    void ShowMenuPanel()
    {
        menuPanel.gameObject.SetActive(true);
        MenuPanelEnabledButton();
    }

    IEnumerator LogoLoad(System.Action action)
    {
        logoPanel.GetComponent<AudioSource>().Play();

        MenuPanelDisableButton();

        StartCoroutine(logoLoadTime.Tweeng((p) => logoPanel.transform.position = p,
              logoStart, logoEnd, curve));

        yield return new WaitForSeconds(logoLoadTime/2);

        action.Invoke();
    }

    void MenuPanelDisableButton()
    {
        List<Button> button = menuPanel.GetComponentsInChildren<Button>().ToList();
        foreach (Button bt in button)
        {
            bt.enabled = false;
        }
    }
    void MenuPanelEnabledButton()
    {
        List<Button> button = menuPanel.GetComponentsInChildren<Button>().ToList();
        foreach (Button bt in button)
        {
            bt.enabled = true;
        }
    }


    void EndGameDataSave()
    {
        if (point > gameData.highestPoint)
        {
            gameData.highestPoint = point;
        }
        gameData.gold = gold;

        gameData.SaveToJson();
        gameData = gameData.LoadNewGameData();

        //#if UNITY_EDITOR
        //        EditorUtility.SetDirty(gameData);
        //        AssetDatabase.SaveAssets();
        //        AssetDatabase.Refresh();
        //#endif

        point = 0;
        ShowUIPoint();
        ShowGameData();
    }
     

    
    void IncreasePoint()
    {
        point++;
        ShowUIPoint();
    }
    void MinusPoint()
    {
        point--;
    }
    void IncreaseGold()
    {
        gold++;
        Gold.text = gold.ToString();
    }

    void ShowUIPoint()
    {
        Point1.text = point.ToString();
        Point2.text = Point1.text;
    }
    bool openScreenCapture = false;

    [Header("Capture")]
    public GameObject screenCapturePose1;
    public GameObject screenCapturePose2;

    public Image screenCapture;
    Sprite previewImage;
    public void TakeScreenShots()
    {
        StartCoroutine(CaptureScreenshot());
    }
    IEnumerator CaptureScreenshot()
    {
        yield return new WaitForEndOfFrame(); // Chờ đến cuối của frame hiện tại

        var time = DateTime.Now.ToString("dd_MM_yyyy_HH-mm-ss");
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        var screenshotname = Application.dataPath + "/Screen_" + time + ".png";
        File.WriteAllBytes(screenshotname, bytes);

        //ScreenCapture.CaptureScreenshot(screenshotname);

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

    public void HideScreenCapture()
    {
        screenCapture.gameObject.SetActive(false);
    }
    public void ScreenCaptureClick()
    {
        float time = 0.2f;
        if (!openScreenCapture)
        {
            openScreenCapture = true;
            StartCoroutine(time.Tweeng((p) => screenCapture.transform.position = p,
              screenCapture.transform.position, screenCapturePose1.transform.position));

            StartCoroutine(time.Tweeng((p) => screenCapture.transform.eulerAngles = p,
              screenCapture.transform.eulerAngles, screenCapturePose1.transform.eulerAngles));

            StartCoroutine(time.Tweeng((p) => screenCapture.transform.localScale = p,
              screenCapture.transform.localScale, screenCapturePose1.transform.localScale));
        }
        else
        {
            openScreenCapture = false;
            StartCoroutine(time.Tweeng((p) => screenCapture.transform.position = p,
              screenCapture.transform.position, screenCapturePose2.transform.position));

            StartCoroutine(time.Tweeng((p) => screenCapture.transform.eulerAngles = p,
              screenCapture.transform.eulerAngles, screenCapturePose2.transform.eulerAngles));

            StartCoroutine(time.Tweeng((p) => screenCapture.transform.localScale = p,
              screenCapture.transform.localScale, screenCapturePose2.transform.localScale));
        }
        
    }

    public void Quit()
    {
        Application.Quit();
    }
    


    [Header("SwipeDetector")]
    public float mMinSwipeDist = 50.0f;
    private Vector2 mXAxis = new Vector2(1, 0);
    private Vector2 mYAxis = new Vector2(0, 1);

    private float mAngleRange = 30;
    

    private Vector2 mStartPosition;

    void Detector()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mStartPosition = new Vector2(Input.mousePosition.x,
                                         Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {

            Vector2 endPosition = new Vector2(Input.mousePosition.x,
                                               Input.mousePosition.y);
            Vector2 swipeVector = endPosition - mStartPosition;

            if (swipeVector.magnitude > mMinSwipeDist)
            {
                swipeVector.Normalize();

                float angleOfSwipe = Vector2.Dot(swipeVector, mXAxis);
                angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;

                if (angleOfSwipe < mAngleRange)
                {
                    OnSwipeRight();
                }
                else if ((180.0f - angleOfSwipe) < mAngleRange)
                {
                    OnSwipeLeft();
                }
                else
                {
                    angleOfSwipe = Vector2.Dot(swipeVector, mYAxis);
                    angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;
                    if (angleOfSwipe < mAngleRange)
                    {
                        OnSwipeTop();
                    }
                    else if ((180.0f - angleOfSwipe) < mAngleRange)
                    {
                        OnSwipeBottom();
                    }
                    else
                    {

                    }
                }
            }

        }
    }
    private void OnSwipeLeft()
    {
        playerController.OnSwipeLeft();
    }

    private void OnSwipeRight()
    {
        playerController.OnSwipeRight();
    }

    private void OnSwipeTop()
    {
        playerController.OnSwipeUp();
    }

    private void OnSwipeBottom()
    {
        playerController.OnSwipeBack();
    }



}
