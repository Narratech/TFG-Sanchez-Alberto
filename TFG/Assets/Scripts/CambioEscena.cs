using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    public string nombre = "Escena2";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si el personaje ha cruzado el límite (extremo derecho de la pantalla)
        if (other.CompareTag("player"))
        {
            // Cambia a la siguiente escena
            SceneManager.LoadScene(nombre);
        }
    }
}

