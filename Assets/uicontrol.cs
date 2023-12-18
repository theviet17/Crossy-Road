using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uicontrol : MonoBehaviour
{
    //[SerializeField] public List<GameObject> ui = new List<GameObject>();
   // public GameObject player;
    //public GameObject pnl;
   // public Button btn;
    // Start is called before the first frame update
    void Start()
    {
       // pnl.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        //if (other.GetComponent<Player>() == ui)
       // {
           // collision.
            //Destroy(collision.gameObject);
            //collision.gameObject.SetActive(false);
            
            //SceneManager.LoadScene(0);
            //ui.End();
            //  animator.SetBool("die", true);

       // }


    }
    public void End()
    {
       // pnl.SetActive(true);
    }
}
