using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planket : MonoBehaviour
{
    public AnimationCurve floatingCurve;
    public float floatingTime = 0.05f;
    public int currentJumpPoint;
    public List<GameObject> JumpPoint;
    public void Floating()
    {
       StartCoroutine(Heaving());
    }
    private IEnumerator Heaving()
    {
        var childObject = this.transform.GetChild(0);
        StartCoroutine(floatingTime.Tweeng((p) =>childObject.transform.localPosition = p,
              childObject.transform.localPosition, childObject.transform.localPosition - new Vector3 (0, 0.1f,0), floatingCurve));

        yield return new WaitForSeconds(floatingTime);

        StartCoroutine(floatingTime.Tweeng((p) => childObject.transform.localPosition = p,
             childObject.transform.localPosition, childObject.transform.localPosition  + new Vector3(0, 0.1f,0 ), floatingCurve));
    }
}
