using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extns
{
    public static IEnumerator Tweeng(this float duration,
        System.Action<float> var, float aa, float zz)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT)
        {
            float t = (Time.time - sT) / duration;
            var(Mathf.SmoothStep(aa, zz, t));
            yield return null;
        }

        var(zz);
    }

    public static IEnumerator Tweeng(this float duration,
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
    public static IEnumerator Tweeng(this float duration,
        System.Action<Vector3> var, Vector3 aa, Vector3 zz, AnimationCurve curve)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT)
        {
            float t = (Time.time - sT) / duration;
            var(Vector3.Lerp(aa, zz, curve.Evaluate(Mathf.SmoothStep(0f, 1f, t))));
            yield return null;
        }

        var(zz);
    }
 
    public static IEnumerator DelayedAction(this float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
    public static IEnumerator ParabolJump(this float duration,
        System.Action<Vector3> var, Vector3 aa, Vector3 zz, AnimationCurve curve)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT)
        {
            float t = (Time.time - sT) / duration;
            float heightMultiplier = curve.Evaluate(t);
            var(Vector3.Lerp(aa, zz, Mathf.SmoothStep(0f, 1f, t))+ Vector3.up * heightMultiplier);
            yield return null;
        }

        var(zz);
    }
    public static IEnumerator ParabolJump(this float duration,
        System.Action<Vector3> var, Vector3 aa, Transform targetTransform, AnimationCurve curve)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT)
        {
            float t = (Time.time - sT) / duration;
            float heightMultiplier = curve.Evaluate(t);
            var(Vector3.Lerp(aa, targetTransform.localToWorldMatrix.GetPosition(), Mathf.SmoothStep(0f, 1f, t))+ Vector3.up * heightMultiplier);
            yield return null;
        }

        var(targetTransform.localToWorldMatrix.GetPosition());
    }
    public static IEnumerator TweengMinAngleRotation(this float duration, System.Action<Vector3> var, Vector3 startEulerAngles, Vector3 targetEulerAngles, AnimationCurve curve)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT )
        {
            float t = (Time.time - sT) / duration;
            float deltaAngleX = Mathf.DeltaAngle(startEulerAngles.x, targetEulerAngles.x);
            float deltaAngleY = Mathf.DeltaAngle(startEulerAngles.y, targetEulerAngles.y);
            float deltaAngleZ = Mathf.DeltaAngle(startEulerAngles.z, targetEulerAngles.z);
            
            targetEulerAngles = startEulerAngles + new Vector3(deltaAngleX, deltaAngleY, deltaAngleZ);
            
            var(Vector3.Lerp(startEulerAngles, targetEulerAngles, curve.Evaluate(Mathf.SmoothStep(0f, 1f, t))));
            yield return null;
        }

        var(targetEulerAngles);

        var(targetEulerAngles);
    }
    public static IEnumerator ScaleObject(this float duration,
        System.Action<Vector3> initialScale,Vector3 startScale, Vector3 targetScale, System.Action<Vector3> initialPosition, Vector3 startPosition, Vector3 targetPoint, AnimationCurve curve)
    {
   
        float startTime = Time.time;

        while (Time.time < startTime + duration )
        {
            float t = (Time.time - startTime) / duration;
            initialScale(Vector3.Lerp(startScale, targetScale, curve.Evaluate(t)));
            initialPosition(Vector3.Lerp(startPosition, targetPoint, curve.Evaluate(t)));
            yield return null;
        }

        initialScale(targetScale);
        initialPosition(targetPoint);
    }
    public class StopCouroutine
    {
        public bool flag = false;
    }
}