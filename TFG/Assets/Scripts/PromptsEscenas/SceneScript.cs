using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneScript : MonoBehaviour
{
    protected GPTController gptController; // Referencia al controlador de GPT
    protected abstract string GetScenePrompt(); // Clase abstracta para incluir los diferentes prompts de cada escena
    protected abstract Task<string> GetScenePrompt(string allPrompts); // Clase abstracta para incluir los diferentes prompts de cada escena

    // Start is called before the first frame update
    protected virtual async void Start()
    {
        // Obtén una referencia al GPTController
        GameObject gptNaeveObject = GameObject.Find("Naeve GPT");

        if (gptNaeveObject != null)
        {
            gptController = gptNaeveObject.GetComponent<GPTController>();

            // Verifica si existe el GPTController antes de intentar enviar el prompt
            if (gptController != null)
            {
                // Comprueba si la escena en la que estamos es la primera ya que esta no tendrá resumen y utilizaremos la función que no recibe parámetros, ya que no hace falta pasarle el contexto de la escena anterior.
                if (SceneManager.GetActiveScene().name == "Escena1")
                {
                    // Incorpora el prompt específico de la escena 1 al prompt general
                    string prompt = GetScenePrompt();
                    gptController.SetPrompt(prompt);
                }
                else
                {
                    Debug.Log("Entrada en el acceso de dartosa");
                    // Cargar el JSON de todos los prompts de la escena anterior desde el archivo y deserializar
                    //string jsonData = File.ReadAllText(Application.persistentDataPath + "/gptPrompts.json");
                    //string gptPrompts = JsonUtility.FromJson<string>(jsonData);
                    //Debug.Log("Json data obtenido: " + jsonData);
                    //Debug.Log("Promps obtenidos: " + gptPrompts);
                    string gptPrompts = PlayerPrefs.GetString("GptPromptsData");
                    Debug.Log("Promps obtenidos: " + gptPrompts);
                    // Incorpora el prompt específico del resto de escenas al prompt general
                    string prompt = await GetScenePrompt(gptPrompts);
                    gptController.SetPrompt(prompt);
                }

                // Inicializa ChatGPT enviando el primer prompt de contexto
                gptController.InitGPT();

                await gptController.SendReply("");

                gptController.UpdateGPT();
            }
            else
            {
                Debug.LogError("GPTController no encontrado.");
            }
        }
    }
}
