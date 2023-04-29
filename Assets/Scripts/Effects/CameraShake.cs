using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.5f;
    public float smoothness = 0.02f;

    private Vector3 originalPosition;

    public void Shake(Camera camera)
    {
        originalPosition = camera.transform.localPosition;
        InvokeRepeating("DoShake", 0f, smoothness);
        Invoke("StopShake", shakeDuration);
    }

    private void DoShake()
    {
        float offsetX = Random.Range(-1f, 1f) * shakeIntensity;
        float offsetY = Random.Range(-1f, 1f) * shakeIntensity;
        float offsetZ = Random.Range(-1f, 1f) * shakeIntensity;

        Vector3 newPosition = originalPosition + new Vector3(offsetX, offsetY, offsetZ);
        transform.localPosition = newPosition;
    }

    private void StopShake()
    {
        CancelInvoke("DoShake");
        transform.localPosition = originalPosition;
    }
}
