using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogFollow : MonoBehaviour
{
    public Transform characterTransform; // Referencia al transform del personaje
    public Transform dialogBoxTransform; // Referencia al transform del cuadro de di�logo
    private Vector3 offset = new Vector3(-5.0f, 16.5f, 0.0f); // Offset inicial en X, Y


    void Update()
    {
        // Aseg�rate de que characterTransform y dialogBoxTransform no sean nulos
        if (characterTransform != null && dialogBoxTransform != null)
        {
            // Actualiza la posici�n del cuadro de di�logo para seguir al personaje
            dialogBoxTransform.position = characterTransform.position + offset;
        }
    }
}
