using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetCamera;
    public Vector3 offsetCamera;
    public Vector3 initOffsetCamera;
    public float smoothSpeed = 0.01f;

    private void Start()
    {
        initOffsetCamera = offsetCamera;
    }

    private void Update()
    {
        Vector3 vel = Vector3.zero;
        Vector3 desiredPosition = new Vector3(targetCamera.position.x + offsetCamera.z,
                                              targetCamera.position.y + offsetCamera.y,
                                              targetCamera.position.z + offsetCamera.x);
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref vel, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(targetCamera);
    }

    public void ChangeOffsetCamera(float ratio)
    {
        Vector3 offset = initOffsetCamera * ratio;
        offsetCamera = offset;
    }
}
