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
        if (other.CompareTag("player"))
        {
            // Buscamos el GPT de Naeve
            GameObject gptNaeveObject = GameObject.Find("Naeve GPT");

            if (gptNaeveObject != null)
            {
                Debug.Log("GPT encontrado:" + gptNaeveObject.name);
                // Si lo encontramos, guardamos todos los prompts generados para utilizarlos en el resumen de la siguiente escena
                gptController = gptNaeveObject.GetComponent<GPTController>();
                // Serializar el string a JSON y guardar en un archivo
                PlayerPrefs.SetString("GptPromptsData", gptController.GetAllPrompts());
                PlayerPrefs.Save();
                //string jsonData = JsonUtility.ToJson(gptController.GetAllPrompts());
                //string path = Application.persistentDataPath + "/gptPrompts.json";
                //File.WriteAllText(path, jsonData);
                //Debug.Log("Ubicación de los datos: " + Application.persistentDataPath);
                //Debug.Log("Contenido de los datos: " + path);
                Debug.Log("Todos los prompts: " + gptController.GetAllPrompts());
                //Debug.Log("json data de los prompts: " + jsonData);

            }
            // Cambia a la siguiente escena
            SceneManager.LoadScene(nombre);
        }
    }
}

