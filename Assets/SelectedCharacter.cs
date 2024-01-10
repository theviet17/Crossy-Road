using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public struct Character
{
    public GameObject prefab;
    public int type; // 1 lock , 2 unlock, 3 select
    public int cost;
    public Character(GameObject prefab, int type, int cost)
    {
        this.prefab = prefab;
        this.type = type;
        this.cost = cost;
    }
    public void ChangeType(int newType)
    {
        this.type = newType;
    }
}
public class SelectedCharacter : MonoBehaviour
{
    [SerializeField] List<Transform> pose;
    [SerializeField] Transform pointStart;
    [SerializeField] Transform pointEnd;
    [SerializeField] GameData gameData;
    [SerializeField] List<Character> characters = new List<Character>();
    int centerPosition;
    int leftPoint;
    int leftPoint2;
    int rightPoint;
    int rightPoint2;
    [SerializeField] float ChangeViewCharacterPanelTime = 0.5f;
    [SerializeField]  GameObject ViewCharacterPanel;
    [SerializeField]  GameObject UnlockButton;
    [SerializeField] List<Color> colors;
    [SerializeField]  GameObject SelectButton;
    [SerializeField]  Text Gold;

    [SerializeField] GameObject SelectedCharacterrPanel;
    [SerializeField] GameObject SelectedCharacterrCamera;
    [SerializeField] GameObject MainCamera;


    //private void Start()
    //{
    //    StartCharacterPanel();
    //}
    private void OnEnable()
    {
        SelectedCharacterrPanel.gameObject.SetActive(true);
        SelectedCharacterrCamera.gameObject.SetActive(true);
        MainCamera.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        SelectedCharacterrPanel.gameObject.SetActive(false);
        SelectedCharacterrCamera.gameObject.SetActive(false);
        MainCamera.gameObject.SetActive(true);
    }
    public void StartCharacterPanel()
    {
        characters = new List<Character> ( gameData.characters );
        Instance3DModel();
    }

    public void Instance3DModel()
    {
        if(transform.childCount > 5)
        {
            for (int i = 5; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        
        for (int i = 0; i < characters.Count; i++)
        {
            var che = Instantiate(characters[i].prefab, pointStart.position, Quaternion.identity);
            che.transform.SetParent(gameObject.transform);
            characters[i] = new Character(che, characters[i].type, characters[i].cost);
        }
        for (int i = 0; i < characters.Count; i++)
        {
            if(characters[i].type == 3)
            {
                centerPosition = i;
            }
        }
        ChangeViewCharacterPanel(0);
    }
    public void LeftArrow_Button()
    {
        CharacterWithAnimation(true);

    }
    public void RightArrow_Button()
    {
        CharacterWithAnimation(false);

    }
    public void ChangeViewCharacterPanel(int value)
    {
        ViewCharacterPanel.gameObject.SetActive(true);

        centerPosition += value;
        centerPosition = centerPosition >= 0 ? (centerPosition > characters.Count - 1 ? 0 : centerPosition) : characters.Count - 1;


        leftPoint = centerPosition -1;
        leftPoint = leftPoint < 0 ? characters.Count - 1 : leftPoint;

        leftPoint2 = leftPoint - 1;
        leftPoint2 = leftPoint2 < 0 ? characters.Count - 1 : leftPoint2;


        rightPoint = centerPosition + 1;
        rightPoint = rightPoint > characters.Count - 1 ? 0 : rightPoint;

        rightPoint2 = rightPoint + 1;
        rightPoint2 = rightPoint2 > characters.Count - 1 ? 0 : rightPoint2;

        CharacterWithoutAnimation();

        ButtonData();


    }
    public void CharacterWithoutAnimation()
    {
        Pose(characters[centerPosition].prefab, pose[1]);

        Pose(characters[leftPoint].prefab, pose[0]);

        Pose(characters[leftPoint2].prefab, pointStart);

        Pose(characters[rightPoint].prefab, pose[2]);

        Pose(characters[rightPoint2].prefab, pointEnd);  
    }
    public void CharacterWithAnimation(bool arrowLeft)
    {
        if (!arrowLeft)
        {
            PoseWithAnim(characters[centerPosition].prefab, pose[0]);

            PoseWithAnim(characters[leftPoint].prefab, pointStart);

            characters[leftPoint2].prefab.SetActive(false);

            PoseWithAnim(characters[rightPoint].prefab, pose[1]);

            PoseWithAnim(characters[rightPoint2].prefab, pose[2]);

            ViewCharacterPanel.gameObject.SetActive(false);

            StartCoroutine(ChangeViewCharacterPanelTime.DelayedAction(() => ChangeViewCharacterPanel(1)));
        }
        else
        {
            PoseWithAnim(characters[centerPosition].prefab, pose[2]);

            PoseWithAnim(characters[leftPoint].prefab, pose[1]);

            PoseWithAnim(characters[leftPoint2].prefab, pose[0]);

            PoseWithAnim(characters[rightPoint].prefab, pointEnd);

            characters[rightPoint2].prefab.SetActive(false);

            ViewCharacterPanel.gameObject.SetActive(false);

            StartCoroutine(ChangeViewCharacterPanelTime.DelayedAction(() => ChangeViewCharacterPanel(-1)));
        }
    }

    public void Pose(GameObject gameObject, Transform pose)
    {
        gameObject.gameObject.SetActive(true);
        gameObject.transform.localPosition = pose.localPosition;
        gameObject.transform.localEulerAngles = pose.localEulerAngles;
        gameObject.transform.localScale = pose.localScale;
    }

    public void PoseWithAnim(GameObject gameObject,Transform nextPose)
    {
        StartCoroutine(ChangeViewCharacterPanelTime.Tweeng((p) => gameObject.transform.localPosition = p,
              gameObject.transform.localPosition, nextPose.localPosition));

        StartCoroutine(ChangeViewCharacterPanelTime.Tweeng((p) => gameObject.transform.localEulerAngles = p,
              gameObject.transform.localEulerAngles, nextPose.localEulerAngles));

        StartCoroutine(ChangeViewCharacterPanelTime.Tweeng((p) => gameObject.transform.localScale = p,
              gameObject.transform.localScale, nextPose.localScale));
    }
       
    public void ButtonData()
    {
        if(characters[centerPosition].type == 1)
        {
            SelectButton.gameObject.SetActive(true);
            UnlockButton.gameObject.SetActive(false);
        }
        else if (characters[centerPosition].type == 2)
        {
            SelectButton.gameObject.SetActive(false);
            UnlockButton.gameObject.SetActive(true);
            UnlockButton.transform.GetChild(0).GetComponent<Text>().text = characters[centerPosition].cost.ToString();
            if(characters[centerPosition].cost <= gameData.gold)
            {
                UnlockButton.GetComponent<Image>().color = colors[0];
                
            }
            else
            {
                UnlockButton.GetComponent<Image>().color = colors[1];
            }
        }
        else 
        {
            SelectButton.gameObject.SetActive(false);
            UnlockButton.gameObject.SetActive(false);
        }

        Gold.text = gameData.gold.ToString();
    }
    public void Unlock_Button()
    {
       if(gameData.gold >= gameData.characters[centerPosition].cost) // dieu kien o day
        {
            gameData.gold -= gameData.characters[centerPosition].cost;
            SelectedCharactor();
        }    
    }
    public void Selected_Button()
    {
        SelectedCharactor();
    }
    public void SelectedCharactor()
    {
        ChangeType(gameData.characters);
        ChangeType(characters);

        EditorUtility.SetDirty(gameData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        ButtonData();
    }
    public void ChangeType( List<Character> characterData)
    {
        for (int i = 0; i < characterData.Count; i++)
        {
            if (characterData[i].type == 3)
            {
                var charI = characterData[i];
                charI.ChangeType(1);
                characterData[i] = charI;
                break;
            }
        }
        var charC = characterData[centerPosition];
        charC.ChangeType(3);
        characterData[centerPosition] = charC;
    }
}
