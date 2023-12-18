using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cantpass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Player>() != null)
        {
            //Destroy(collision.gameObject);
            collision.collider.transform.position -= Vector3.back - new Vector3(-1,0,-1);

        }


    }
}
