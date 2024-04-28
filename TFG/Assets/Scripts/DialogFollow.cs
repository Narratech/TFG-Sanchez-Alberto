using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogFollow : MonoBehaviour
{
    public Transform characterTransform; // Referencia al transform del personaje
    public Transform dialogBoxTransform; // Referencia al transform del cuadro de diálogo
    public Vector3 offset = new Vector3(0.5f, 16f, 0.0f); // Offset inicial en X, Y


    void Update()
    {
        // Asegúrate de que characterTransform y dialogBoxTransform no sean nulos
        if (characterTransform != null && dialogBoxTransform != null)
        {
            // Actualiza la posición del cuadro de diálogo para seguir al personaje
            dialogBoxTransform.position = characterTransform.position + offset;
        }
    }
}
