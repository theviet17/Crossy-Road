using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TerrainGenerator mainCamrera;


    [SerializeField] private GameObject logoPanel;
    [SerializeField] private Vector3 logoStart;
    [SerializeField] private Vector3 logoPose;
    [SerializeField] private Vector3 logoEnd;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float logoLoadTime = 0.3f;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private SelectedCharacter selectedCharacter;
    void Start()
    {
        terrainGenerator.TerrainGeneratorStart();
        playerController.PlayerControllerStart();      
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(logoPanel.transform.position);
        }
    }
    void ReStart()
    {
        terrainGenerator.ReStart();
        playerController.ReStart();

        menuPanel.gameObject.SetActive(false);
    }
    void CharacterLoad()
    {
        selectedCharacter.gameObject.SetActive(true);
        selectedCharacter.StartCharacterPanel();


        menuPanel.gameObject.SetActive(false);
    }
    void CharacterClose()
    {
       
        selectedCharacter.gameObject.SetActive(false);

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
            Debug.Log(1);
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
    void ShowMenuPanel()
    {
        menuPanel.gameObject.SetActive(true);
        MenuPanelEnabledButton();
    }

}
