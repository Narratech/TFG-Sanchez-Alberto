using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Estas funciones heredar�an de OnClick, en la que estar�a el contexto

public class OnClickGround : MonoBehaviour
{
    ////private GPTController gpt = new GPTController();
    //// Puede meter todas estas funciones auxiliares que uso en una clase utilities para no tener que crear tantos objetos
    //private OnClick onClick = new();
    //private TraductionLogic traductionRes = new();

    //private void Start()
    //{
    //    GPTController.Instance.InitGPT();
    //}

    //public Transform playerTransform;

    //private Vector2 target;
    //private float speed = 10f;
    //private bool moving;
    //private string reply = "";
    //private string answer;

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0) && !moving)
    //    {
    //        RaycastHit2D hit = onClick.GetPositionRay(Input.mousePosition);

    //        if (hit.collider != null && hit.collider.CompareTag(tag))
    //        {
    //            Debug.Log("Collider detectado: " + hit.collider.gameObject.name);
    //            Debug.Log("Punto de colisi�n: " + hit.point);
    //            StartCoroutine(SendAndHandleReply(hit));
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
    //    //GPTController.Instance.SendReply("[el jugador hace click en un punto en el suelo para que naeve se mueva]", "ground");
        

    //    while (string.IsNullOrEmpty(reply)) // Esperar hasta que la respuesta no est� vac�a
    //    {
    //        yield return null;
    //    }

    //    // Aqu� puedes usar la respuesta obtenida
    //    Debug.Log("get answer: " + reply);
        
    //    answer = traductionRes.Traduce(reply ?? ""); // Llama a traduce para traducir el mensaje a una acci�n.
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

    //// Este m�todo se llama desde GPTController cuando la respuesta est� lista
    //public void SetReply(string response)
    //{
    //    reply = response;
    //}

}
