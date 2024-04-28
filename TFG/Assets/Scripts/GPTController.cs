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
        [SerializeField] public TMP_Text textito;
        [SerializeField] public RectTransform sent;
        [SerializeField] public RectTransform received;
        [SerializeField] public TMP_InputField inputField;
        [SerializeField] public Button button;
        public GameObject dialogueBox;
        public GameObject portonChat;
        public GameObject estatuaChat;

        //private static GPTController _instance;

        private float height;
        private OpenAIApi openai = new();
        private List<ChatMessage> messages = new();
        private ChatMessage answer;
        private bool newMsgSend;

        private string prompt = "";

        //private void Start()
        //{
        //    // Utilizo un función anónima para pasar el argumento al momento de añadir al listener
        //    button.onClick.AddListener(async () => await SendReply(reply));
        //}

        // Añade el prompt específico de cada escena al prompt general. 
        public void SetPrompt(string newPrompt)
        {
            prompt += newPrompt;
        }

        public string GetInputField()
        {
            return inputField.text;
        }

        public void AppendMessage(string message)
        {
            // Ignoro el primer mensaje
            if (messages.Count > 2)
            {
                if (this.name == "Naeve GPT")
                {
                    dialogueBox.SetActive(true);
                    DialogueController dialogueController = dialogueBox.GetComponent<DialogueController>();
                    if (dialogueController != null)
                    {
                        Debug.Log("Hablamos con Naeve");
                        dialogueController.DeleteLastMsg();
                        dialogueController.StartDialogue(message);
                    }
                }
                else if (this.name == "NPC Chat")
                {
                    portonChat.SetActive(true);
                    DialogueController dialogueController = portonChat.GetComponent<DialogueController>();
                    if (dialogueController != null)
                    {
                        dialogueController.DeleteLastMsg();
                        dialogueController.StartDialogue(message);
                    }

                }
                else if (this.name == "Estatua Chat")
                {
                    estatuaChat.SetActive(true);
                    DialogueController dialogueController = estatuaChat.GetComponent<DialogueController>();
                    if (dialogueController != null)
                    {
                        dialogueController.DeleteLastMsg();
                        dialogueController.StartDialogue(message);
                    }
                }

                else
                {
                    textito.SetText(message);
                }
            }
        }

        // Función para devolver toda la conversación. La usaré con el GPT de Naeve para mandárselo al GPT resumidor. Comienzo en el mensaje 2 ya que no me interesa volver a mandarle todo el contexto ni la primera respuesta de GPT.
        public string GetAllPrompts()
        {
            string allPrompts = "";
            for (int i = 2; i < messages.Count; i++)
            {
                string part = "Mensaje " + i + ": " + messages[i].Content + "\n";
                allPrompts += part;
            }
            return allPrompts;
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

            //Puedo guardar un contador en la clase y empezar el bucle desde ahí para no escribir todos los mensajes todo cada vez
            //if (this.name == "Naeve GPT" || this.name == "Action GPT")
            //{
            //for (int i = 0; i < messages.Count; i++)
            //{
            //    Debug.Log(this.name + ". Mensaje " + i + ": " + messages[i].Content);
            //}
            //}
        }

        // Obtiene la respuesta ya parseada, que corresponde con las acciones a realizar

        public async Task<string> SendReply(string text/*, string click*/)
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = /* texto a enviar a chatgpt*/ text
            };

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + /* texto a enviar a chatgpt*/ text;

            messages.Add(newMessage);

            // Si tenemos inputField y botón en este GPT, los desactivamos y reseteamos el texto para que no se puedan utilizar durante la comunicación
            if (inputField != null && button != null)
            {
                button.enabled = false;
                inputField.text = "";
                inputField.enabled = false;
            }

            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0125",
                Messages = messages
                //Temperature = 0.7f
            }); ;

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                messages.Add(message);

                // Volvemos a poner disponibles el input field y el botón
                if (inputField != null && button != null)
                {
                    button.enabled = true;
                    inputField.enabled = true;
                }

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



