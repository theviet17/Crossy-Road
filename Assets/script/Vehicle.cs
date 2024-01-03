using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public float rayCastDistance = 2;
    public LayerMask car;
    public float lifeTime = 5;
    Vector3 origin;
    Vector3 direction;
    float distance;
    [HideInInspector] public float movingSpeed;
    [HideInInspector] public bool isPlank;
    private void Awake()
    {
        Destroy(gameObject, lifeTime);
    }
    //void Start()
    //{
    //    StartCoroutine(Moving(movingSpeed));
    //}

    //IEnumerator Moving(float time)
    //{
    //    StartCoroutine(time.Tweeng((p) => gameObject.transform.position = p,
    //        gameObject.transform.position, gameObject.transform.position + new Vector3(0, 0, distance)));
    //    yield return new WaitForSeconds(time);
    //    Destroy(gameObject);
    //}

    void Update()
    {
        if (!isPlank)
        {
            origin = transform.position;
        }
        else
        {
            origin = gameObject.transform.GetChild(0).localToWorldMatrix.GetPosition();
        }
       
        direction = (movingSpeed > 0 ? 1 : -1) * transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, rayCastDistance, car, QueryTriggerInteraction.UseGlobal))
        {
            if (hit.collider != null && hit.collider.gameObject != this.gameObject)
            {
                Debug.Log("Vachamvoixephiatruoc");
                rayCastDistance = 0;
                movingSpeed = hit.collider.gameObject.GetComponent<Vehicle>().movingSpeed;
            }

        }
        transform.Translate(Vector3.forward * movingSpeed * Time.deltaTime);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(origin, origin + direction * rayCastDistance);
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
