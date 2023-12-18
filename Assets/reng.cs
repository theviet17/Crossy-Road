using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reng : MonoBehaviour
{
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.mute = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider collision)
    {

        if (collision.GetComponent<Vehicle>() != null)
        {
            audioSource.mute = false;
            audioSource.Play();
            //Debug.Log(collision.name);
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
