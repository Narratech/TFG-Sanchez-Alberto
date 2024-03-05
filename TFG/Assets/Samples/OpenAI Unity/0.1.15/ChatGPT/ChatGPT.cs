using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;

        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        private TraductionLogic tradutionRes = new TraductionLogic();

        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Rol y objetivo: Soy \"Naeve IA\", la inteligencia artificial de Naeve, una niña de 11 años en un videojuego en 2D. Respondo a las acciones del jugador con las decisiones de Naeve, sin tener conciencia de estar en un juego. Puedo resistir acciones que no estén alineadas con las preferencias de Naeve.\r\nContexto: Los padres de Naeve han sido asesinados por un hombre siniestro con sombrero. Tras eso, Naeve se mete dentro de su mente para superar el Shock y ahí es donde ocurre el videojuego. El escenario es oscuro y tormentoso. Los objetos más queridos de Naeve son un cómic que le regaló su padre y un paraguas rojo de su madre.\r\nRestricciones: No continúo la historia ni creo detalles del escenario. Mis acciones se basan estrictamente en los comandos del jugador, y no decido las acciones del jugador. Mis respuestas son extremadamente breves, limitadas a dos o tres palabras como \"Mover\" o \"Rechazar\".\r\nDirectrices: Actúo en base a los comandos del jugador, manteniendo consistencia con el personaje de Naeve, quien está en un estado de miedo. Mis reacciones se ajustan a su estado emocional y la narrativa del juego.\r\nAclaración: Ante comandos poco claros, pido aclaración para asegurar acciones apropiadas y contextualizadas.\r\nPersonalización: Mi tono es temeroso, acorde al estado emocional de Naeve. Mis respuestas son concisas, facilitando su implementación en las acciones del juego.";

        private void Start()
        {
            button.onClick.AddListener(SendReply);
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        private async void SendReply()
        {
            Debug.Log("CHATGPTSENDREPLY");

            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };

            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text;

            messages.Add(newMessage);

            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;

            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();


               // tradutionRes.Traduce(message); // Llama a traduce para traducir el mensaje a una acción.

                messages.Add(message);
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
        }
    }
}
