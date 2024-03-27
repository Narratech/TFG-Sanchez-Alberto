using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneScript : MonoBehaviour
{
    protected GPTController gptNaeveController; // Referencia al controlador de GPT
    protected GPTController gptActionController; // Referencia al controlador de GPT

    private PromptManager promptManager = new PromptManager();

    protected abstract string GetScenePrompt(); // Clase abstracta para incluir los diferentes prompts de cada escena
    protected abstract Task<string> GetScenePrompt(string allPrompts); // Clase abstracta para incluir los diferentes prompts de cada escena

    // Start is called before the first frame update
    protected virtual async void Start()
    {
        // Obtén una referencia al GPTController
        GameObject gptNaeveObject = GameObject.Find("Naeve GPT");
        GameObject gptActionObject = GameObject.Find("Action GPT");


        if (gptNaeveObject != null && gptActionObject != null)
        {
            gptNaeveController = gptNaeveObject.GetComponent<GPTController>();
            gptActionController = gptActionObject.GetComponent<GPTController>();


            // Verifica si existe el GPTController antes de intentar enviar el prompt
            if (gptNaeveController != null && gptActionController != null)
            {
                // Comprueba si la escena en la que estamos es la primera ya que esta no tendrá resumen y utilizaremos la función que no recibe parámetros, ya que no hace falta pasarle el contexto de la escena anterior.
                if (SceneManager.GetActiveScene().name == "Escena1")
                {
                    // Incorpora el prompt específico de la escena 1 al prompt general
                    string prompt = GetScenePrompt();
                    gptNaeveController.SetPrompt(prompt);
                }
                else
                {
                    string gptPrompts = PlayerPrefs.GetString("GptPromptsData");
                    // Incorpora el prompt específico del resto de escenas al prompt general
                    string prompt = await GetScenePrompt(gptPrompts);
                    gptNaeveController.SetPrompt(prompt);
                }

                // Inicializa ChatGPT enviando el primer prompt para la respuesta formal
                string actionText = setActionText();
                gptActionController.SetPrompt(actionText);

                gptActionController.InitGPT();
                await gptActionController.SendReply("");
                gptActionController.UpdateGPT();

                // Inicializa ChatGPT enviando el primer prompt del narrador
                gptNaeveController.InitGPT();
                await gptNaeveController.SendReply("");
                gptNaeveController.UpdateGPT();
            }
            else
            {
                Debug.LogError("GPTController no encontrado.");
            }
        }
    }

    private string setActionText()
    {
        return promptManager.getActionPrompt();
    }
}
