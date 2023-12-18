using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fly : MonoBehaviour
{
    //ublic AudioClip 1 ;
    private AudioSource audioSource;
    int check = 0;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(check == 1)
        {
            transform.position = transform.position + new Vector3(-Time.deltaTime * 20, 0, 0);
            //udio.Play();
        }
        if(transform.position.x <= -100)
        {
            Destroy(gameObject);
        }

    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            check = 1;
            audioSource.Play();
            //ansform.position = transform.position + new Vector3(-0.5f, 0, 0);
        }
    }
    
}
