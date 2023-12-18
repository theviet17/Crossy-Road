using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] public TerrainGenerator terrainGenerator;
   // [SerializeField] private GameObject vehicle;
    ///public int speed;
    private Animator animator;

    // private bool isHopping;
    public float y = 0.1f;
    public float speed = 5f;
    public float speeds = 5f;
    // Start is called before the first frame update

    void Start()
    {
        animator = GetComponent<Animator>();


    }

    void Update()
    {
        //if (transform.position.y == 0.55 || transform.position.y == 0.65 || transform.position.y == 0.45)
        // { 
        float zDifference2 = 0;
        if (Input.GetKeyUp(KeyCode.W) /*&& !isHopping*/)
            {
            //Debug.Log(transform.position.y);
            //Debug.Log(transform.position.y);
            //animator.SetBool("walk", true);
            float zDifference = 0;
               
            
               

                //   Debug.Log(zDifference2);
                if (transform.position.z % 1 == 0)
                {
                    zDifference = Mathf.Round(transform.position.z) - transform.position.z ;
                }

                float xDifference = 0;

                xDifference = 1 - (transform.position.x % 1 - 0.082f);

                MoveCharacter(new Vector3(xDifference, 0, zDifference));
            Debug.Log(transform.position.z % 1);
            if (Math.Abs(transform.position.z % 1) < 0.5)
            {
                zDifference2 = 1 * (0.5f - Math.Abs(transform.position.z % 1));
            }
            if (Math.Abs(transform.position.z % 1) > 0.5)
            {
                zDifference2 = -1 * (transform.position.z % 1 - 0.5f);
            }
            transform.position = (transform.position + new Vector3(0, 0, zDifference2));
                gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 180, 0), speeds);
            }

        if (Input.GetKeyUp(KeyCode.D))
        {
            float xDifference = 0;

            xDifference = 0 - (transform.position.x % 1 - 0.082f);
            MoveCharacter(new Vector3(xDifference, 0, -1));
            gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 270, 0), speeds);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            float xDifference = 0;

            xDifference = 0 - (transform.position.x % 1 - 0.082f);
            MoveCharacter(new Vector3(xDifference, 0, 1));
            gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), speeds);
        }

        if (Input.GetKeyUp(KeyCode.S) /*&& !isHopping*/)
        {
            float xDifference = 0;

            xDifference = -1 - (transform.position.x % 1 - 0.082f);
            float zDifference = 0;
            if (transform.position.z % 1 == 0)
            {
                zDifference = Mathf.Round(transform.position.z) - transform.position.z;
            }
            MoveCharacter(new Vector3(xDifference, 0, zDifference));
            gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), speeds);

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, y, 0), 3);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, y, 0), 3);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, y, 0), 3);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, y, 0), 3);
        }



    }
  //}
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<Vehicle>()!= null)
        {
            if (collision.collider.GetComponent<Vehicle>().isLog)
            {
               // transform.position = vehicle.transform.position ;
               transform.parent = collision.collider.transform;
           }
        }
        else
        {
            transform.parent = null;
        }
    }
    private void MoveCharacter(Vector3 difference)
    {
        animator.SetTrigger("hop");
      //  isHopping = true;
       // transform.position = transform.position + difference;
        transform.position = Vector3.Lerp(transform.position, transform.position + difference, Time.deltaTime * speed);
        terrainGenerator.SpawnTerrain(false, transform.position);
    }
    
     
    // public void FinishHop()
    //  {
    //  isHopping = false;
    //}
}
