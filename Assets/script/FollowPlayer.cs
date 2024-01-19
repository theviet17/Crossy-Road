using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothness;
    [HideInInspector] public bool inGame = true;
    [HideInInspector] public bool moveCam = false;
    public float camMovingSpeed = 5;
    public float hawkEyesDistance = 6;

    private void Start()
    {
        player.GetComponent<PlayerController>().Die += EndGame;
        player.GetComponent<PlayerController>().Start += StartGame;
        player.GetComponent<PlayerController>().MoveCam += StartMoveCam;
        
    }
    private void Update()
    {
        if (inGame)
        {
            var changePosition = new Vector3(0, 0, 0);
            if (player.transform.position.x + offset.x >= transform.position.x)
            {
                changePosition = new Vector3(transform.position.x + offset.x, transform.position.y, player.transform.position.z + offset.z);
            }
            else
            {
                changePosition = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + offset.z);
            }
            transform.position = Vector3.Lerp(transform.position, changePosition, smoothness);
        }
        if (moveCam)
        {
            transform.position += Vector3.right * camMovingSpeed * Time.deltaTime;
        }
      

    }
    IEnumerator EventSeenByHawks()
    {
        while(transform.position.x - player.transform.position.x < hawkEyesDistance)
        {
            yield return null;
        }
        player.GetComponent<PlayerController>().EventSeenByHawks();
    }
    public void EndGame()
    {
        inGame = false;
        moveCam = false;
    }
    public void StartGame()
    {
        inGame = true;
        transform.position = player.transform.position + offset;
    }
    public void StartMoveCam()
    {
        moveCam = true;
        StartCoroutine(EventSeenByHawks());
    }
}
