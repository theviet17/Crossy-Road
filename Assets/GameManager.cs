
using System.Collections;
using System.Collections.Generic;
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

    [HideInInspector] public GameData gameData;
    public Text Point1;
    public Text Point2;
    private int point;
    public Text HighestPoint;
    public Text Gold;
    private int gold;
    void Start()
    {
        gameData = selectedCharacter.gameData;

        terrainGenerator.TerrainGeneratorStart();
        playerController.PlayerControllerStart();

        playerController.MoveOn += IncreasePoint;
        playerController.MoveBack += MinusPoint;
        playerController.Gold += IncreaseGold;
        playerController.Die += EndGame;

        ShowGameData();
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
        playerController.HideScreenCapture();
        playerController.ReStart();

        menuPanel.gameObject.SetActive(false);
    }

    // OutPut show Character Panel ,  hide menu Panel
    void CharacterLoad()
    {
        playerController.HideScreenCapture();
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
        playerController.ReLoadParentStatus();
        terrainGenerator.ReStart();
        playerController.ReLoadData();
        EndGameDataSave();
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

        EditorUtility.SetDirty(gameData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

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
    public void ScreenCaptureClick()
    {
        float time = 0.2f;
        if (!openScreenCapture)
        {
            openScreenCapture = true;
            var screenCapture = playerController.screenCapture;
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
            var screenCapture = playerController.screenCapture;
            StartCoroutine(time.Tweeng((p) => screenCapture.transform.position = p,
              screenCapture.transform.position, screenCapturePose2.transform.position));

            StartCoroutine(time.Tweeng((p) => screenCapture.transform.eulerAngles = p,
              screenCapture.transform.eulerAngles, screenCapturePose2.transform.eulerAngles));

            StartCoroutine(time.Tweeng((p) => screenCapture.transform.localScale = p,
              screenCapture.transform.localScale, screenCapturePose2.transform.localScale));
        }
        
    }
    public GameObject screenCapturePose1;
    public GameObject screenCapturePose2;
  


}
