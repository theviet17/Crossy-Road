using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLight : MonoBehaviour
{
    public float timeFlicker = 1;

    [HideInInspector] public MovingObjectInstancePoint movingObjectInstancePoint;
    GameObject light1;
    GameObject light2;
    public void Register()
    {
        movingObjectInstancePoint.instance += OnLight;
        light1 = gameObject.transform.GetChild(1).gameObject;
        light2 = gameObject.transform.GetChild(2).gameObject;
    }

    public void OnLight()
    {
        StartCoroutine(Flicker());
        gameObject.GetComponent<AudioSource>().Play();
    }
    public IEnumerator Flicker()
    {
        float sT = Time.time;
        float eT = sT + timeFlicker;
        light1.gameObject.SetActive(true);
        while (Time.time < eT)
        {
            yield return new WaitForSeconds(0.15f);
            if (light1.activeSelf)
            {
                light1.gameObject.SetActive(false);
                light2.gameObject.SetActive(true);
            }
            else
            {
                light1.gameObject.SetActive(true);
                light2.gameObject.SetActive(false);
            }
          
        }
        light1.gameObject.SetActive(false);
        light2.gameObject.SetActive(false);
    }
}
