using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uicontrol : MonoBehaviour
{

    public GameObject pnl;
    void Start()
    {
        pnl.SetActive(false);
    }


    void Update()
    {
        
    }
   
    public void readme()
    {
        pnl.SetActive(true);
    }
    public void readmeoff()
    {
        pnl.SetActive(false);
    }
}
