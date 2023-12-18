using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KillPlayerOnTouch : MonoBehaviour
{
    void Start()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Player>() != null)
        {
            collision.gameObject.SetActive(false);

        }
    }
   
}
