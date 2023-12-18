using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroystartpoint : MonoBehaviour
{
    [SerializeField] private GameObject player;
    void Start()
    {
        
    }
    void Update()
    {
        if (player.transform.position.x >= 20)
        {
            Destroy(gameObject);
        }
    }
}
