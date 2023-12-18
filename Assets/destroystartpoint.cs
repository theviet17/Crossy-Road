using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroystartpoint : MonoBehaviour
{
    [SerializeField] private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x >= 20)
        {
            Destroy(gameObject);
        }
    }
}
