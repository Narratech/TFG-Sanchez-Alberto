using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    private List<string> lineList = new List<string>(); // Lista de líneas
    private int index = 0;
    private float textSpeed = 0.02f;
    private bool isWriting = false; // Flag para verificar si se está escribiendo

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Si se está escribiendo, completar la línea actual inmediatamente
            if (isWriting)
            {
                CompleteLineImmediately();
            }
            else
            {
                NextLine(); // Avanzar al siguiente texto si no se está escribiendo
            }
        }
    }

    public void DeleteLastMsg()
    {
        StopAllCoroutines();
    }

    public void StartDialogue(string text)
    {
        index = 0;
        lineList.Clear();
        if (text.Length > 150) 
        {
            lineList = SplitTextIntoLines(text, 150); // Dividir el texto en líneas de máximo 30 caracteres
        }
        else
        {
            lineList.Add(text);
        }
        gameObject.SetActive(true);
        textComponent.text = string.Empty;
        StartCoroutine(TypeLine());
    }

    public IEnumerator TypeLine()
    {
        isWriting = true; // Indicar que se está escribiendo
        foreach (char c in lineList[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed); ; // Esperar un frame antes de mostrar el siguiente carácter
        }
        isWriting = false; // Indicar que se ha completado la escritura
    }

    private void CompleteLineImmediately()
    {
        // Detener la escritura y mostrar la línea completa
        StopAllCoroutines();
        textComponent.text = lineList[index];
        isWriting = false;
    }

    private void NextLine()
    {
        // Avanzar al siguiente texto si hay más líneas disponibles
        if (index < lineList.Count - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private List<string> SplitTextIntoLines(string text, int maxLengthPerLine)
    {
        List<string> lines = new List<string>();
        string[] words = text.Split(' '); // Dividir el texto en palabras

        string currentLine = string.Empty;
        foreach (string word in words)
        {
            if ((currentLine + word).Length <= maxLengthPerLine)
            {
                currentLine += word + " "; // Agregar la palabra a la línea actual
            }
            else
            {
                lines.Add(currentLine); // Agregar la línea actual a la lista
                currentLine = word + " "; // Comenzar una nueva línea con la palabra actual
            }
        }

        if (!string.IsNullOrEmpty(currentLine))
        {
            lines.Add(currentLine); // Agregar la última línea si no está vacía
        }

        return lines;
    }
}
