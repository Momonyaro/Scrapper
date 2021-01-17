using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingCamera : MonoBehaviour
{
    public Transform camTarget;
    public float minDistance = 0.5f;
    public float lerpSpeed = 0.3f;

    private void Update()
    {
        if (camTarget == null) return;
        if (Vector2.Distance(camTarget.position, transform.position) > minDistance)
        {
            transform.position = Vector2.Lerp(transform.position, camTarget.position, lerpSpeed * Time.deltaTime);
            transform.position += new Vector3(0, 0, -10);
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            Camera.main.orthographicSize += Time.deltaTime * 1.2f;
        }
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            Camera.main.orthographicSize -= Time.deltaTime * 2f;
        }
    }

    public void SetTarget(Transform target)
    {
        camTarget = target;
    }
}
