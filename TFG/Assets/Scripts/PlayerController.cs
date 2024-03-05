using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    //private float speed = 5f;
    //private Vector3 positionToMove;

    //public void Move(Vector3 mousePosition)
    //{
    //    positionToMove = Camera.main.ScreenToWorldPoint(mousePosition);
    //    positionToMove.z = transform.position.z;
    //    positionToMove.y = transform.position.y;
    //    transform.position = Vector3.MoveTowards(transform.position, positionToMove, speed * Time.deltaTime);
    //}

    //private ResponseTraduction tradutionRes = new ResponseTraduction();
    //private OpenAIApi openai = new OpenAIApi();
    //private List<ChatMessage> messages = new List<ChatMessage>();
    //private string answer;
    //private bool moveAllowed = false;
    //private Vector3 target;

    //private string prompt = "Rol y objetivo: Soy \"Naeve IA\", la inteligencia artificial de Naeve, una niña de 11 años en un videojuego en 2D. Respondo a las acciones del jugador con las decisiones de Naeve, sin tener conciencia de estar en un juego. Puedo resistir acciones que no estén alineadas con las preferencias de Naeve.\r\nContexto: Los padres de Naeve han sido asesinados por un hombre siniestro con sombrero. Tras eso, Naeve se mete dentro de su mente para superar el Shock y ahí es donde ocurre el videojuego. El escenario es oscuro y tormentoso. Los objetos más queridos de Naeve son un cómic que le regaló su padre y un paraguas rojo de su madre.\r\nRestricciones: No continúo la historia ni creo detalles del escenario. Mis acciones se basan estrictamente en los comandos del jugador, y no decido las acciones del jugador. Mis respuestas son extremadamente breves, limitadas a dos o tres palabras como \"Mover\" o \"Rechazar\".\r\nDirectrices: Actúo en base a los comandos del jugador, manteniendo consistencia con el personaje de Naeve, quien está en un estado de miedo. Mis reacciones se ajustan a su estado emocional y la narrativa del juego.\r\nAclaración: Ante comandos poco claros, pido aclaración para asegurar acciones apropiadas y contextualizadas.\r\nPersonalización: Mi tono es temeroso, acorde al estado emocional de Naeve. Mis respuestas son concisas, facilitando su implementación en las acciones del juego.";

    //public async void SendReply(string text /*, Vector3 mousePosition*/)
    //{
    //    var newMessage = new ChatMessage()
    //    {
    //        Role = "user",
    //        Content = /* texto a enviar a chatgpt*/ text
    //    };

    //    if (messages.Count == 0) newMessage.Content = prompt + "\n" + /* texto a enviar a chatgpt*/ text;

    //    messages.Add(newMessage);

    //    // Complete the instruction
    //    var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
    //    {
    //        Model = "gpt-3.5-turbo-0613",
    //        Messages = messages
    //    });

    //    if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
    //    {
    //        var message = completionResponse.Choices[0].Message;
    //        message.Content = message.Content.Trim();

    //        Traduce(message /*, mousePosition*/); // Llama a traduce para traducir el mensaje a una acción.

    //        messages.Add(message);
    //    }
    //    else
    //    {

    //        Debug.LogWarning("No text was generated from this prompt.");
    //    }

    //}

    //public void Traduce(ChatMessage message/*, Vector3 mousePosition*/)
    //{
    //    // Guardamos el contenido del mensaje de ChatGPT
    //    string content = message.Content.ToLower();
    //    Debug.Log("GPT: " + content);

    //    // Si el mensaje es "mover", movemos a Naeve a la posición donde ha sido el click
    //    // Tendríamos que obtener la posición del click del ratón, habiéndola guardado previamente
    //    if (content.Contains("mover") || content.Contains("mueve"))
    //    {
    //        // Movemos a Naeve
    //        Debug.Log("Naeve se moveria");
    //        moveAllowed = true;
    //        //Move(mousePosition);
    //        // PlayerController.Move(mousePosition);
    //        // Devolvemos true si lo que quería era moverse
    //    }
    //    else if (content.Contains("esconder") || content.Contains("esconderse"))
    //    {
    //        // Habría que saber donde se esconde Naeve, dependiendo esto de la escena.
    //        // También podría darse la posibilidad de que Naeve no pueda esconderse
    //    }
    //    else
    //    {
    //        Debug.Log("levanta la cabeza, confusa");
    //        // Si no se cumple ninguna, Naeve solo levantaría la cabeza
    //    }

    //}

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetMouseButtonDown(0))
        //{
        //    target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    target.z = transform.position.z;
        //    target.y = transform.position.y;

        //    //SendReply("el jugador hace click a la derecha de Naeve");
        //}
        ////transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        ////transform.position = target;

        //while (moveAllowed)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        //    if (transform.position == target)
        //    {
        //        moveAllowed = false;
        //    }
        //}


        //if (Input.GetKey(KeyCode.A))
        //{
        //    gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-8000f * Time.deltaTime, 0));
        //}

        //if (Input.GetKey(KeyCode.D))
        //{
        //    gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(8000f * Time.deltaTime, 0));
        //}

        //if (Input.GetKey(KeyCode.S))
        //{
        //    // Agacharse
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 4000f));
        //}
    }
    //private void OnMouseDown()
    //{
    //    Debug.Log(Input.mousePosition);
    //    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    pos.z = gameObject.GetComponent<Transform>().position.z;
    //    pos.y = gameObject.GetComponent<Transform>().position.y;
    //    gameObject.GetComponent<Transform>().position = pos;
    //    Debug.Log("adios");
    //}

}
