using System;
using UnityEngine;

public class CameraFollow: MonoBehaviour
{
    [SerializeField] private Transform target; // alvo para a câmera seguir
    [SerializeField] private float smoothSpeed = 5f; // velocidade de suavização
    [SerializeField] private Vector3 offset; // offset da câmera em relação ao alvo
    private void LateUpdate()
    {
        if (target == null) return;
        
        // posição desejada da câmera com offset
        Vector3 desiredPosition = target.position + offset;
        
        // suaviza a transição da posição da câmera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
