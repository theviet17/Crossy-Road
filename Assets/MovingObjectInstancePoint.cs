using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct Between
{
   public float minValue;
   public float maxValue;

   public float RandomValue()
   {
      return UnityEngine.Random.Range(minValue, maxValue);
   }
}
public class MovingObjectInstancePoint : MonoBehaviour
{
   public List<GameObject> objects;
   public Between timeDelay;
   public Between baseSpeed;
   private void Start()
   {
      StartCoroutine(InstanceObject( objects[UnityEngine.Random.Range(0,objects.Count)]));
   }

   IEnumerator InstanceObject(GameObject slelectedObject)
   {
      Instance(slelectedObject); // frist time Instance
      while (true)
      {
         yield return new WaitForSeconds(timeDelay.RandomValue());
         Instance(slelectedObject);
      }
   }

   void Instance(GameObject slelectedObject)
   {
      var vhc = Instantiate(slelectedObject, gameObject.transform.localToWorldMatrix.GetPosition(), Quaternion.identity);
      var vehicle = vhc.GetComponent<Vehicle>();
      vehicle.movingSpeed = baseSpeed.RandomValue();
   }
}
