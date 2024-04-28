using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Referencia al transform del personaje que seguirá la cámara
    public float smoothSpeed = 0.125f; // Velocidad de suavizado para el seguimiento
    public Vector3 offset; // Desplazamiento adicional de la cámara

    void LateUpdate()
    {
        if (target != null)
        {
            // Calcula la posición deseada de la cámara en función de la posición del personaje
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);

            // Interpolación suavizada entre la posición actual de la cámara y la posición deseada
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Actualización de la posición de la cámarad
            transform.position = smoothedPosition;
        }
    }
}


