using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float minspeed;
    [SerializeField] private float maxspeed;
    public bool isLog;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * -(Random.Range(minspeed, maxspeed)) * Time.deltaTime);
        if(transform.position.y <= -1)
        {
            Destroy(gameObject);
        }
    }
    /*private void OnCollisionEnter(Collision collision) { 
        if(collision.collider.GetComponent<Player>()!= null)
        {
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);

        }
       

    }*/
}
