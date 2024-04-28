using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Referencia al transform del personaje que seguir� la c�mara
    public float smoothSpeed = 0.125f; // Velocidad de suavizado para el seguimiento
    public Vector3 offset; // Desplazamiento adicional de la c�mara

    void LateUpdate()
    {
        if (target != null)
        {
            // Calcula la posici�n deseada de la c�mara en funci�n de la posici�n del personaje
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);

            // Interpolaci�n suavizada entre la posici�n actual de la c�mara y la posici�n deseada
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Actualizaci�n de la posici�n de la c�marad
            transform.position = smoothedPosition;
        }
    }
}


