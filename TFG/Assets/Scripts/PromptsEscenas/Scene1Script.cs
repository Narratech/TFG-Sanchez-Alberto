using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1Script : SceneScript
{
    private string promptScene1 = "Escena: «Bosque oscuro. Hay una tormenta. Apareces en la posición (0,0). Hay un paraguas en la posición (36.35,-1.3). Hay un cómic en la posición (15.2,16). Hay una rama en la posición (-7.27,-1.3). Hay un sofá en la posición (23.9,0.27). Hay una mesa en la posición (83.5,0.13). Hay una vela en la posición (83.62.99,). Hay una silla en la posición (78.08,-0.77). Hay un obstáculo en la posición (53.14,-0.5). Si la posición del objeto no está dada, significa que lo tienes en el inventario a tu disposición.»";

    protected override string GetScenePrompt()
    {
        // Devuelve el prompt específico de la escena 1
        return promptScene1;
    }
    protected override void Start()
    {
        base.Start(); // Llama al método Start de la clase base
    }

}