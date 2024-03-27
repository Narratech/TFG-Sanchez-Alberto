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

        //private static GPTController _instance;

        private float height;
        private OpenAIApi openai = new();
        private List<ChatMessage> messages = new();
        private ChatMessage answer;
        private bool newMsgSend;
        private string actionText;
        private string naeveText;

        private string prompt = "";

        // Añade el prompt específico de cada escena al prompt general. 
        public void SetPrompt(string newPrompt)
        {
            prompt += newPrompt;
        }

        public void AppendMessage(string message)
        {
            textito.SetText(message);
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
            if (this.name == "Naeve GPT" || this.name ==  "Action GPT")
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    Debug.Log(this.name + ". Mensaje " + i + ": " + messages[i].Content);
                }
            }
        }

        // Obtiene la respuesta ya parseada, que corresponde con las acciones a realizar
        public string GetAnswer()
        {
            return actionText;
        }

        public async Task<string> SendReply(string text/*, string click*/)
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = /* texto a enviar a chatgpt*/ text
            };

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + /* texto a enviar a chatgpt*/ text;

            messages.Add(newMessage);

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



