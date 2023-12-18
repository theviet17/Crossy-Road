using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class canpasstrigger : MonoBehaviour
{
    int check = 0;
    public bool isLog;
    void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            check = 3;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            check = 1;
            //transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, y, 0), 3);
            //Debug.Log(check);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            check = 2;

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            check = 4;
        }

    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            //Debug.Log(collision.name);
            if (isLog)
            {
                collision.gameObject.SetActive(false);
               // SceneManager.LoadScene(0);
            }
            else { 
            if (check == 1)
            {
                collision.transform.position += Vector3.back;
            }
            if (check == 2)
            {
                collision.transform.position -= Vector3.back;
            }
            if (check == 3)
            {
                collision.transform.position += Vector3.back - new Vector3(1, 0, -1);
            }
            if (check == 4)
            {
                collision.transform.position -= Vector3.back - new Vector3(1, 0, -1);
            }
            }

        }
    }
}
