using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Clase para mover el texto de un bot�n al clickar en este, con el objetivo de que siga la animaci�n del pulsado del bot�n.
public class OnClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI textToMove;
    private float moveAmount = 19f;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Se ejecuta cuando se pulsa el bot�n
        MoverTextoAbajo();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Se ejecuta cuando se suelta el bot�n
        VolverPosicionOriginal();
    }

    private void MoverTextoAbajo()
    {
        // L�gica para mover el texto hacia abajo
        // Puedes usar la posici�n actual y modificar el eje Y, por ejemplo:
        textToMove.rectTransform.anchoredPosition -= new Vector2(0f, moveAmount);
    }

    private void VolverPosicionOriginal()
    {
        // L�gica para volver el texto a su posici�n original
        // Puedes establecer la posici�n original o cualquier l�gica espec�fica
        // en tu caso podr�a ser texto.rectTransform.anchoredPosition = [posici�n original];
        textToMove.rectTransform.anchoredPosition -= new Vector2(0f, -moveAmount);
    }
}
