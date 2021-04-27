using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraTransform : MonoBehaviour
{

    private void LateUpdate()
    {
        if (Camera.current == null) return;
        Transform cameraTransform = Camera.current.transform;
        transform.LookAt(transform.position 
                         + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
    }
}
