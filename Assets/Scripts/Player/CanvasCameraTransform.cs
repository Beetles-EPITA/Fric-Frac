using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraTransform : MonoBehaviour
{
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.current.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position 
                                   + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
    }
}
