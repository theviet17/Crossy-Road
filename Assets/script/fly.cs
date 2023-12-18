using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fly : MonoBehaviour
{
    private AudioSource audioSource;
    bool check;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        if(check == true)
        {
            transform.position = transform.position + new Vector3(-Time.deltaTime * 20, 0, 0);
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
            check = true;
            audioSource.Play();
        }
    }
    
}
