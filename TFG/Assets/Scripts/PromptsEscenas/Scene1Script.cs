using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1Script : SceneScript
{
    private string promptScene1 = "Escena: �Bosque oscuro. Hay una tormenta. Apareces en la posici�n (0,0). Hay un paraguas en la posici�n (36.35,-1.3). Hay un c�mic en la posici�n (15.2,16). Hay una rama en la posici�n (-7.27,-1.3). Hay un sof� en la posici�n (23.9,0.27). Hay una mesa en la posici�n (83.5,0.13). Hay una vela en la posici�n (83.62.99,). Hay una silla en la posici�n (78.08,-0.77). Hay un obst�culo en la posici�n (53.14,-0.5). Si la posici�n del objeto no est� dada, significa que lo tienes en el inventario a tu disposici�n.�";

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