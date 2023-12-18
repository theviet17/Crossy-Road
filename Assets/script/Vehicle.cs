using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{

    [SerializeField] private float minspeed;
    [SerializeField] private float maxspeed;

    public bool isLog;
    void Start()
    {
        //collision.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * -(Random.Range(minspeed, maxspeed)) * Time.deltaTime);
        if (transform.position.y <= -1)
        {
            Destroy(gameObject);
        }
       

    }
   
    private void OnTriggerEnter(Collider collision)
    {
        
        if (collision.gameObject.tag == "l1")
        {
            collision.transform.GetChild(0).gameObject.SetActive(false);
        }
       

    }
   private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "l1")
        {
            collision.transform.GetChild(0).gameObject.SetActive(true);
        }
       
    }

}
