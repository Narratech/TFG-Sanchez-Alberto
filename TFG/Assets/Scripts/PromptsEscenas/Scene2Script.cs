using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Scene2Script : SceneScript
{
    private PromptManager promptManager = new PromptManager();
    protected override async Task<string> GetScenePrompt(string allPrompts)
    {
        // Llama a la función y espera a que se complete
        string resumenPrompt = "";
        string prompt = "";
        if (!string.IsNullOrEmpty(allPrompts))
        {
            resumenPrompt = await promptManager.getResumenPrompt(allPrompts);
            prompt = promptManager.getNarraevePrompt() + "\nResumen de la escena anterior: " + resumenPrompt + "\n" + promptManager.getScene2Prompt();

        }
        else
        {
            prompt = promptManager.getNarraevePrompt() + "\n" + promptManager.getScene2Prompt();
        }
        Debug.Log("Resumen" + resumenPrompt);
        // Devuelve el prompt específico de la escena 2
        return prompt;
    }

    protected override string GetScenePrompt()
    {
        throw new System.NotImplementedException();
    }

    protected override void Start()
    {
        base.Start(); // Llama al método Start de la clase base
    }
}
