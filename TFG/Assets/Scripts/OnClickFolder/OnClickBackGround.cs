using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

// Estas funciones heredar�an de OnClick, en la que estar�a el contexto

public class OnClickBackGround : MonoBehaviour
{
    //private ChatMessage reply;
    //private string answer;

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Vector2 rayOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector2 rayDirection = Vector2.zero;

    //        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection);

    //        if (hit.collider != null && hit.collider.CompareTag("GravityObject"))
    //        {
    //            Debug.Log("Collider detectado: " + hit.collider.gameObject.name);
    //            Debug.Log("Punto de colisi�n: " + hit.point);

    //            // El clic fue en el suelo, mueve al personaje a la posici�n de la colisi�n
    //            //target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //            target = (Vector2)hit.point;
    //            target.y = playerTransform.position.y;
    //            // capturar el error del l�mite de gpt y lanzar la animaci�n o mensaje iddle u otra de error
    //            // CAMBIAR EL PROMPT EN FUNCI�N DEL LUGAR EXACTO EN EL QUE SE CLIQUE
    //            GPTController.Instance.SendReply("el jugador hace click en un objeto, que cae en el suelo");
    //            reply = GPTController.Instance.GetAnswer();
    //            Debug.Log("esta es la info inicial: " + reply.Content);
    //            answer = traductionRes.Traduce(reply.Content ?? ""); // Llama a traduce para traducir el mensaje a una acci�n.
    //            if (answer == "move")
    //            {
    //                moving = true;
    //            }
    //            GPTController.Instance.UpdateGPT();
    //        }

    //    }

    //    if (moving && (Vector2)playerTransform.position != target)
    //    {
    //        float step = speed * Time.deltaTime;
    //        playerTransform.position = Vector2.MoveTowards(playerTransform.position, target, step);
    //    }
    //    else
    //    {
    //        moving = false;
    //    }
    //}
}