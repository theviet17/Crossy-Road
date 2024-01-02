using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    
    public float distance;
    [HideInInspector] public float movingSpeed;
    void Start()
    {
        StartCoroutine(Moving(movingSpeed));
    }

    IEnumerator Moving(float time)
    {
        StartCoroutine(Tweeng(time,(p) => gameObject.transform.position = p,
            gameObject.transform.position, gameObject.transform.position + new Vector3(0, 0, distance)));
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
    public IEnumerator Tweeng(float duration,
        System.Action<Vector3> var, Vector3 aa, Vector3 zz)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT)
        {
            float t = (Time.time - sT) / duration;
            var(Vector3.Lerp(aa, zz, Mathf.SmoothStep(0f, 1f, t)));
            yield return null;
        }

        var(zz);
    }
    // void Update()
    // {
    //     transform.Translate(Vector3.forward * (-(Random.Range(minspeed, maxspeed)) * Time.deltaTime));
    //     if (transform.position.y <= -1)
    //     {
    //         Destroy(gameObject);
    //     }
    //    
    //
    // }
   
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
