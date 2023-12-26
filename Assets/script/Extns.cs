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
    public static IEnumerator Tweeng(this float duration,
        System.Action<Vector3> var, Vector3 aa, Vector3 zz, AnimationCurve curve, float ID)
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
    public static IEnumerator TweengRotation(this float duration, System.Action<Vector3> var, Vector3 startEulerAngles, Vector3 targetEulerAngles, AnimationCurve curve)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT)
        {
            float t = (Time.time - sT) / duration;
            float deltaAngleX = Mathf.DeltaAngle(startEulerAngles.x, targetEulerAngles.x);
            float deltaAngleY = Mathf.DeltaAngle(startEulerAngles.y, targetEulerAngles.y);
            float deltaAngleZ = Mathf.DeltaAngle(startEulerAngles.z, targetEulerAngles.z);

            Vector3 deltaAngles = new Vector3(
                Mathf.SmoothStep(0f, deltaAngleX, t),
                Mathf.SmoothStep(0f, deltaAngleY, t),
                Mathf.SmoothStep(0f, deltaAngleZ, t)
            );

            var(deltaAngles);
            yield return null;
        }

        var(targetEulerAngles);
    }
}