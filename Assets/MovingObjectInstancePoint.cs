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

    public bool rightDrection;

    public bool isTrain = false;
    public bool isPlank = false;
    public event Action instance;
    [HideInInspector] public Transform terrain;
    private void Start()
    {
        StartCoroutine(InstanceObject(objects[UnityEngine.Random.Range(0, objects.Count)]));
    }

    IEnumerator InstanceObject(GameObject slelectedObject)
    {
        if (!isTrain)
        {
            Instance(slelectedObject); // frist time Instance
        }
        while (true)
        {
            yield return new WaitForSeconds(timeDelay.RandomValue());
            EventInstance();
            if (isTrain)
            {
                yield return new WaitForSeconds(1);
                Instance(slelectedObject);
            }
            else
            {
                Instance(slelectedObject);
            }
            
        }
    }

    void Instance(GameObject slelectedObject)
    {
        if (isPlank)
        {
            slelectedObject = objects[UnityEngine.Random.Range(0, objects.Count)];
        }
        var vhc = Instantiate(slelectedObject, gameObject.transform.localToWorldMatrix.GetPosition(), Quaternion.identity);
        vhc.transform.SetParent(terrain.transform);
        var vehicle = vhc.GetComponent<Vehicle>();
        var randomSpeed = baseSpeed.RandomValue();
        vehicle.movingSpeed = rightDrection ? randomSpeed : -1 * randomSpeed;
        vehicle.isPlank = isPlank;

    }
    public void EventInstance()
    {
        instance?.Invoke();
    }
}
