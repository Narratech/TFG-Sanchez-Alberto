using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneScript : MonoBehaviour
{
    protected GPTController gptController; // Referencia al controlador de GPT
    protected abstract string GetScenePrompt(); // Clase abstracta para incluir los diferentes prompts de cada escena

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
                // Incorpora el prompt específico de la escena al prompt general
                gptController.SetPrompt(GetScenePrompt());

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
