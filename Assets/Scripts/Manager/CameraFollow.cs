using System;
using UnityEngine;

public class CameraFollow: MonoBehaviour
{
    [SerializeField] private Transform target; // Camera target to follow
    [SerializeField] private float smoothSpeed = 5f; // Smoothing speed
    [SerializeField] private Vector3 offset; // Camera offset in relation to target
    private void LateUpdate()
    {
        if (target == null) return;
        
        // Camera desired position with offset
        Vector3 desiredPosition = target.position + offset;
        
        // Smooth transaction from camera position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
