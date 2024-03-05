using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1Script : SceneScript
{
    private string promptScene1 = "Escena: �Bosque oscuro. Hay una tormenta. Apareces en la posici�n (0,0). Hay un paraguas en la posici�n (22.82,0). Hay un c�mic en la posici�n (10,19.8). Hay un tronco en la posici�n (58.79,0). Hay un sof� en la posici�n (46.34,1.5). Hay una mesa en la posici�n (76.43,0). Hay una vela en la posici�n (80.5,6.6). Hay una silla en la posici�n (69,0). Si la posici�n del objeto no est� dada, significa que lo tienes en el inventario a tu disposici�n.�";

    protected override string GetScenePrompt()
    {
        // Devuelve el prompt espec�fico de la escena 1
        return promptScene1;
    }
    protected override void Start()
    {
        base.Start(); // Llama al m�todo Start de la clase base
    }

}