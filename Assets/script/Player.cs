using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    [SerializeField] public TerrainGenerator terrainGenerator;

    private AudioSource audioSource;
    public Animator animator;

    public GameObject pnl;
    public Button btn;
    public GameObject pnl1;
    public GameObject pnl2;


    [SerializeField] private Text scrtext1;
    [SerializeField] private Text scrtext2;
    [SerializeField] private Text scrtext3;
    private int score;

    public float y = 0.1f;
    public float speed = 5f;
    public float speeds = 5f;

    bool check = true;  

    public Transform target;
    float t;
    float t2 = 0f;
    int deathpoint = 0;

    float zDifference = 0;
    float zDifference2 = 0;
    float xDifference = 0;
    void Start()
    {
        animator = GetComponent<Animator>();
        pnl.SetActive(false);
        audioSource = gameObject.GetComponent<AudioSource>();
        pnl2.SetActive(true);

    }
   
    private void FixedUpdate()
    {
        if (transform.position.x >= -1 && transform.position.x < 0)
        {
            score = 1;
        }
        if (transform.position.x >= 0) 
        { 
        if (transform.position.x - t2 >= 1)
        {
           t2 = transform.position.x- (transform.position.x % 1);
           if (deathpoint >= 0)
                {
                    score++;
                }
        }
        }
        t = t + Time.deltaTime;;
        if (t > 1f)
        {   
            pnl1.SetActive(false);
        }

    }
   
    void Update()
    {
        
        scrtext1.text = Convert.ToString(score);
        scrtext2.text = Convert.ToString(score);
        scrtext3.text = Convert.ToString(score);
        if (deathpoint <= -3 )
        {
            target.position = Vector3.Lerp(target.position, transform.position + new Vector3(-2, -1.5f, -2), Time.deltaTime * 3);
        }
        
        if (deathpoint >-3)
        {
            target.position = transform.position + new Vector3(10, 10, -3);

        }
        
        if (check == true) 
        { 


        if (Input.GetKeyUp(KeyCode.W) /*&& !isHopping*/)
            {
                
                check = false;
                if (transform.position.z % 1 == 0)
                {
                    zDifference = Mathf.Round(transform.position.z) - transform.position.z ;
                }
                if (transform.position.x >= 0)
                { 
                xDifference = 1 - (transform.position.x % 1 - 0.082f);
                }
                if (transform.position.x % 1 < 0)
                {
                    if (transform.position.x  >= -1 )
                    {
                        xDifference = 1.11f - (Math.Abs(transform.position.x % 1) - 0.082f)  ;
                    }
                    if (transform.position.x  < -1) 
                    { 
                    xDifference = 1 + (Math.Abs(transform.position.x % 1) - 0.082f);
                    }
                }

                MoveCharacter(new Vector3(xDifference, 0, zDifference));

            if ((transform.position.z % 1) < 0.5)
            {
                if ((transform.position.z % 1) >= 0) 
                 {
                       zDifference2 = 1 * (0.5f -(transform.position.z % 1));
                 }
                if (Math.Abs(transform.position.z % 1) < 0)
                 {
                        zDifference2 = -1 * (0.5f - Math.Abs(transform.position.z % 1));
                 }
                if (Math.Abs(transform.position.z % 1) >= 0.5)
                 {
                        zDifference2 = +1 * ( Math.Abs(transform.position.z % 1) -  0.5f );
                 }

             }
            if ((transform.position.z % 1) > 0.5)
            {
                zDifference2 = -1 * (transform.position.z % 1 - 0.5f);
            }
            transform.position = (transform.position + new Vector3(0, 0, zDifference2));
                gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 180, 0), speeds);
            }

        if (Input.GetKeyUp(KeyCode.D))
        {
                check = false;
              
                if (transform.position.x % 1 >= 0f)
                {
                    xDifference = 0 - (transform.position.x % 1 - 0.082f);
                }
                if (transform.position.x % 1 < 0)
                {
                    xDifference = 0 + (Math.Abs(transform.position.x % 1) - 0.082f);
                }
                    MoveCharacter(new Vector3(xDifference, 0, -1));
                    gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 270, 0), speeds);
                

                //xDifference = 0 - (transform.position.x % 1 - 0.082f);
               
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
                check = false;

                if (transform.position.x % 1 >= 0f)
                {
                    xDifference = 0 - (transform.position.x % 1 - 0.082f);
                }
                if (transform.position.x % 1 < 0)
                {
                    xDifference = 0+ (Math.Abs(transform.position.x % 1) - 0.082f);
                }
                    MoveCharacter(new Vector3(xDifference, 0, 1));
                    gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), speeds);
                

                
        }

        if (Input.GetKeyUp(KeyCode.S) /*&& !isHopping*/)
        {
            
                check = false;
            
                if (transform.position.x % 1 >= 0f)
                {
                    xDifference = -1 - (transform.position.x % 1 - 0.082f);
                }
                if (transform.position.x % 1 < 0)
                {
                    xDifference = -1 - (Math.Abs(transform.position.x % 1) - 0.082f);
                }

                
            if (transform.position.z % 1 == 0)
            {
                zDifference = Mathf.Round(transform.position.z) - transform.position.z;
            }
            MoveCharacter(new Vector3(xDifference, 0, zDifference));
            gameObject.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), speeds);

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
                if (deathpoint < 0 && deathpoint > -3 )
                {
                    deathpoint = deathpoint + 1;

                }
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
            deathpoint = deathpoint - 1;
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, y, 0), 3);
        }

        

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<Vehicle>()!= null)
        {
            // if (collision.collider.GetComponent<Vehicle>().isLog)
            // {
            //    transform.parent = collision.collider.transform;
            // }
        }
        else
        {
            transform.parent = null;
        }
        if (collision.gameObject.tag == "die")
        {
            pnl.SetActive(true);
            pnl2.SetActive(false);

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.GetComponent<Vehicle>() != null)
        {
            // if (collision.collider.GetComponent<Vehicle>().isLog)
            // {
            //     transform.parent = null;
            // }
        }
        
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "die")
        {
            pnl.SetActive(true);
            pnl2.SetActive(false);
        }
    }
        void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag != "nopoint")
        {
            check = true;
            
        }
        // check neu player chua cham dat thi se khong duoc nhay tiep
        
    }
    void StartGame()
    {
        SceneManager.LoadScene(0);
    }
    public void Restart()
    {
        StartGame();
    }
   

    private void MoveCharacter(Vector3 difference)
    {
        audioSource.Play();
        animator.SetTrigger("hop");
        transform.position = Vector3.Lerp(transform.position, transform.position + difference, Time.deltaTime * speed);
        terrainGenerator.SpawnTerrain(false, transform.position);
    }
    
}
