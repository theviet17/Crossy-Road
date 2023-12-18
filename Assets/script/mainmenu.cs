using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainmenu : MonoBehaviour
{
    public GameObject pnl;
    public Button bt1;
    public Button bt2;
    private void Start()
    {
        pnl.SetActive(false);
    }
    public void openyesno()
    {
        pnl.SetActive(true);
    }
    public void closeyesno()
    {
        pnl.SetActive(false);
    }
    public void quitegame ()

    {
        Application.Quit();
    }
        
}
