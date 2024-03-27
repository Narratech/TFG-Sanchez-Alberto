using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    public string nombre = "Escena2";
    private GPTController gptController; // Referencia al controlador de GPT

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si el personaje ha cruzado el límite (extremo derecho de la pantalla)
        if (other.CompareTag("Naeve"))
        {
            // Buscamos el GPT de Naeve
            GameObject gptNaeveObject = GameObject.Find("Naeve GPT");

            if (gptNaeveObject != null)
            {
                // Si lo encontramos, guardamos todos los prompts generados para utilizarlos en el resumen de la siguiente escena
                gptController = gptNaeveObject.GetComponent<GPTController>();
                // Serializar el string a JSON y guardar en un archivo
                PlayerPrefs.SetString("GptPromptsData", gptController.GetAllPrompts());
                PlayerPrefs.Save();
            }
            // Cambia a la siguiente escena
            SceneManager.LoadScene(nombre);
        }
    }
}

