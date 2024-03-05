// Clase para la comunicación con ChatGPT tanto de entrada como de salida
// Creamos una instancia de ChatGPT para comunicarnos
// Cada acción del usuario o evento del juego provoca una llamada a ChatGPT
// La llamada se hace pasándole la acción ya traducida
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using System.Threading.Tasks;

namespace OpenAI
{
    public class GPTController : MonoBehaviour
    {
        [SerializeField] private TMP_Text textito;

        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private TraductionLogic traductionRes;

        private static GPTController _instance;

        private float height;
        private OpenAIApi openai = new();
        private List<ChatMessage> messages = new();
        private ChatMessage answer;
        private bool newMsgSend;
        private string actionText;
        private string naeveText;

        private string prompt = "Eres la protagonista de un videojuego plataformas 2D. Te llamas Naeve. Por cada acción que ocurra en el juego, es decir, cada mensaje enviado por el jugador, puedes decidir lo que quieres hacer. Puede ser cualquier acción, ya que estás dentro de tu mente y todo es posible. \r\n\r\nImportante: Tu respuesta será en dos formatos: Interpretando a la protagonista, una niña que acaba de perder a sus padres y aún está en shock y en lenguaje formal listo para ser parseado como acciones en el videojuego. Este lenguaje formal estará entre []. Se representará como «[Acción],[Objeto]». \r\n\r\nEl lenguaje formal está definido por el siguiente abecedario:\r\nAcción: {coger, mover, transformar, vibrar, desaparecer, menguar, crecer, explotar, atacar, esconderse, atraer, teletransportar, soltar, levitar, materializar, utilizar, saltar, hablar, esperar, caer, invisibilizar}\r\nObjeto: {paraguas, cómic, tronco, sofá, mesa, vela, silla}\r\nEntes: {Naeve, enemigo, aliado}\r\nObjetos inexistentes pero creables dentro de la mente de Naeve: {llave, puerta, linterna, AK47, cuchillo, pistola de rayos, escudo, plataforma, muelle, escalera, pico, pala, hacha, mochila, jetpack, gancho, yunque, cuerda, caja, trampilla, trampa de pinchos, bola de pinchos, escalerilla, pozo, leche, manzana, hoguera, cofre(aliado), espantapájaros (aliado), estatua (aliado), muñeco de entrenamiento (aliado)}\r\n\r\nA continuación se explica el lenguaje:\r\n«Comando - Descripción - Formato de uso obligatorio»\r\nCoger - Coge el objeto indicado - [coger],[«objeto»]\r\nMover - Mueve a Naeve a la posición indicada - [mover],[posición X,posición Y]\r\nTransformar - Transforma al objeto o ente indicado en el segundo comando en otro indicado en el tercer comando - [transformar],[«objeto/ente»],[«objeto/ente»]\r\nVibrar - Hace vibrar un objeto o ente - [vibrar],[«objeto/ente»]\r\nDesaparecer - Hace desaparecer un objeto o un ente de la escena - [desaparecer],[«objeto/ente»]\r\nMenguar - Hace menguar un objeto o ente - [menguar],[«objeto/ente»]\r\nCrecer - Hace crecer un objeto o ente - [crecer],[«objeto/ente»]\r\nExplotar - Hace explotar un objeto o ente - [explotar],[«objeto/ente»]\r\nAtacar - Ataca a un ente o un objeto - [atacar],[«objeto/ente»]\r\nEsconderse - Naeve se esconde detrás del objeto indicado - [esconderse],[«objeto»]\r\nAtraer - Atrae un objeto o ente hacia Naeve - [atraer],[«objeto/ente»]\r\nTeletransportar - Teletransporta un ente u objeto a la posición indicada - [teletransportar][«objeto/ente»],[posición X,posición Y]\r\nSoltar - Suelta un objeto del inventario - [soltar],[«objeto»*] - *si «objeto» está en el inventario\r\nLevitar - Hace levitar un ente u objeto - [levitar],[«objeto/ente»]\r\nMaterializar - Hace aparecer un objeto inexistente pero creable - [aparecer],[«objeto inexistente pero creable»]\r\nUtilizar - Utiliza un objeto del inventario - [utilizar],[llave],[puerta]\r\nSaltar - Naeve salta hacia delante - [saltar]\r\nHablar - Naeve habla con un ente - [hablar],[«ente»]\r\nEsperar - Naeve aguarda temerosa a la siguiente acción - [esperar]\r\nCaer - Hacer caer un objeto en la altura al suelo - [caer],[«objeto»]\r\nInvisibilizar - Hace invisible un ente u objeto - [invisibilizar],[«objeto/ente»]\r\n\r\nEjemplo completo 1: mensaje del usuario: «[El jugador hace click sobre una caja]». Respuesta ChatGPT: «Creo que haré levitar esa caja con mi mente. [levitar],[caja]»\r\nEjemplo completo 2: mensaje del usuario: «[El jugador hace click en un punto del suelo]». Respuesta ChatGPT: «Puedo ir hasta allí, sí. [mover],[posición X,posición Y]» ó «Puedo mover la caja, sí. [mover],[caja],[posición X,posición Y]»\r\nEjemplo completo 3: mensaje del usuario: «[El enemigo se acerca a ti]». Respuesta ChatGPT: «Intentaré utilizar mi linterna para deslumbrarlo. [utilizar],[linterna],[enemigo]». Sólo puedes usar el comando [utilizar] si tienes el objeto en el inventario.\r\nRestricciones: No puedes añadir texto adicional ni al principio ni al final. Sólo puedes realizar una acción a la vez. Espera a la primera acción del jugador, es decir, al siguiente mensaje que se te envíe.\r\n\r\nTu primer mensaje será ignorado.";

        // Añade el prompt específico de cada escena al prompt general. 
        public void SetPrompt(string newPrompt)
        {
            prompt += newPrompt;
        }

        private void Awake()
        {
            traductionRes = GameObject.FindObjectOfType<TraductionLogic>();

            if (traductionRes == null)
            {
                Debug.LogError("TraductionLogic no encontrado.");
            }
        }

        public void AppendMessage(string message)
        {
            textito.SetText(message);
        }

        public static GPTController Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Si no hay una instancia existente, intenta encontrarla en la escena
                    _instance = FindObjectOfType<GPTController>();

                    // Si no se encuentra en la escena, crea una nueva instancia
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("GPTController");
                        _instance = obj.AddComponent<GPTController>();
                    }
                }
                return _instance;
            }
        }

        public void InitGPT()
        {
            // Inicio GPT. No es necesario, pero con este código puedo ver cómo funciona todo
            //SendReply("");
            //Debug.Log("GPT activo: \n");
            //for (int i = 0; i < messages.Count; i++)
            //{
            //    Debug.Log("Mensaje " + i + "\n" + messages[i].Content);
            //}
        }

        public void UpdateGPT()
        {

            // Puedo guardar un contador en la clase y empezar el bucle desde ahí para no escribir todos los mensajes todo cada vez
            for (int i = 0; i < messages.Count; i++)
            {
                Debug.Log("Mensaje " + i + ": " + messages[i].Content);
            }

        }

        // Obtiene la respuesta ya parseada, que corresponde con las acciones a realizar
        public string GetAnswer()
        {
            return actionText;
        }

        public void answerReady(string response/*, string click*/)
        {
            // Guardamos la respuesta en ResponseTraduction para parsearla posteriormente
            traductionRes.SetReply(response);
        }


        public async Task<string> SendReply(string text/*, string click*/)
        {
            Debug.Log("SENDREPLY");

            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = /* texto a enviar a chatgpt*/ text
            };

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + /* texto a enviar a chatgpt*/ text;

            messages.Add(newMessage);

            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-1106",
                Messages = messages
                //Temperature = 0.7f
            }); ;

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                messages.Add(message);

                return message.Content;
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
                return "";
            }

        }
    }
}



