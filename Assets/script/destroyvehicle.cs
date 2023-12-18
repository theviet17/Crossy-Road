using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyvehicle : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Vehicle>() != null)
        {
            Destroy(collision.gameObject);
        }

    }
}
