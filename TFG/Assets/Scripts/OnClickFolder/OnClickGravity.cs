using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// Estas funciones heredarían de OnClick, en la que estaría el contexto

public class OnClickGravity : MonoBehaviour
{
    //private TraductionLogic traductionRes = new();
    //private OnClick onClick = new();
    //public Transform playerTransform;

    //private Vector2 target;
    //private float speed = 10f;
    //private bool moving;
    //private string reply;
    //private string answer;

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0) && !moving)
    //    {
    //        // llamamos a una función auxiliar pasándole la posición en la que se ha clickado para saber si se ha hecho click en el objeto en concreto
    //        RaycastHit2D hit = onClick.GetPositionRay(Input.mousePosition);

    //        if (hit.collider != null && hit.collider.CompareTag(tag))
    //        {
    //            // Si clickas a un objeto, su cuerpo2D pasa a estar despierto, es decir, le añadimos gravedad
    //            //gameObject.AddComponent<Rigidbody2D>();
    //            if (gameObject.GetComponent<Rigidbody2D>().IsSleeping())
    //            {
    //                gameObject.GetComponent<Rigidbody2D>().WakeUp();
    //            }

    //            Debug.Log("Collider detectado: " + hit.collider.gameObject.name);
    //            Debug.Log("Punto de colisión: " + hit.point);
    //            StartCoroutine(SendAndHandleReply(hit));                
    //            //capturar el error del límite de gpt y lanzar la animación o mensaje iddle u otra de error
    //            // CAMBIAR EL PROMPT EN FUNCIÓN DEL LUGAR EXACTO EN EL QUE SE CLIQUE

    //        }
            
    //    }

    //    if (moving && (Vector2)playerTransform.position != target)
    //    {
    //        MovePlayerTowardsTarget();
    //    }
    //    else if (moving)
    //    {
    //        Debug.Log("movin false");
    //        moving = false;
    //    }
    //}

    //private void MovePlayerTowardsTarget()
    //{
    //    float step = speed * Time.deltaTime;
    //    playerTransform.position = Vector2.MoveTowards(playerTransform.position, target, step);
    //}

    //private IEnumerator SendAndHandleReply(RaycastHit2D hit)
    //{
    //    //GPTController.Instance.SendReply("[el jugador hace click en el cómic, que cae al suelo]", "gravity");


    //    while (string.IsNullOrEmpty(reply)) // Esperar hasta que la respuesta no esté vacía
    //    {
    //        yield return null;
    //    }

    //    // Aquí puedes usar la respuesta obtenida
    //    Debug.Log("get answer: " + reply);
    //    traductionRes.InterpretString(reply);
    //    answer = traductionRes.Traduce(reply ?? ""); // Llama a traduce para traducir el mensaje a una acción.
    //    if (answer == "move")
    //    {
    //        moving = true;
    //        target = (Vector2)hit.point;
    //        target.y = playerTransform.position.y;
    //        MovePlayerTowardsTarget();
    //    }
    //    GPTController.Instance.UpdateGPT();

    //    // Resetea reply para futuras solicitudes
    //    reply = "";
    //}

    //// Este método se llama desde GPTController cuando la respuesta está lista
    //public void SetReply(string response)
    //{
    //    reply = response;
    //}
}
