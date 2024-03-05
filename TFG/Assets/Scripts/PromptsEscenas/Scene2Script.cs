using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2Script : SceneScript
{
    private string promptScene2 = "Escena 2 prompt";

    protected override string GetScenePrompt()
    {
        // Devuelve el prompt específico de la escena 1
        return promptScene2;
    }
    protected override void Start()
    {
        base.Start(); // Llama al método Start de la clase base
    }
}
