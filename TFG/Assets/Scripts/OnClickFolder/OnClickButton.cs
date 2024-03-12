using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Clase para mover el texto de un botón al clickar en este, con el objetivo de que siga la animación del pulsado del botón.
public class OnClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI textToMove;
    private float moveAmount = 19f;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Se ejecuta cuando se pulsa el botón
        MoverTextoAbajo();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Se ejecuta cuando se suelta el botón
        VolverPosicionOriginal();
    }

    private void MoverTextoAbajo()
    {
        // Lógica para mover el texto hacia abajo
        // Puedes usar la posición actual y modificar el eje Y, por ejemplo:
        textToMove.rectTransform.anchoredPosition -= new Vector2(0f, moveAmount);
    }

    private void VolverPosicionOriginal()
    {
        // Lógica para volver el texto a su posición original
        // Puedes establecer la posición original o cualquier lógica específica
        // en tu caso podría ser texto.rectTransform.anchoredPosition = [posición original];
        textToMove.rectTransform.anchoredPosition -= new Vector2(0f, -moveAmount);
    }
}
