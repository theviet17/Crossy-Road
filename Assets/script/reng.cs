using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reng : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.mute = true;
    }
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider collision)
    {

        if (collision.GetComponent<Vehicle>() != null)
        {
            audioSource.mute = false;
            audioSource.Play();
        }


    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.GetComponent<Vehicle>() != null)
        {

            audioSource.mute = true;
        }

    }
}
