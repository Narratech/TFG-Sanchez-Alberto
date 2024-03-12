using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1Script : SceneScript
{
    private PromptManager promptManager = new PromptManager();

    protected override string GetScenePrompt()
    {
        string prompt = promptManager.getNaevePrompt() + promptManager.getScene1Prompt();
        // Devuelve el prompt espec�fico de la escena 1
        return prompt;
    }
    protected override void Start()
    {
        base.Start(); // Llama al m�todo Start de la clase base
    }

}