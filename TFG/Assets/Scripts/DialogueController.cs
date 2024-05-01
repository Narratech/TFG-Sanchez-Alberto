using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    private List<string> lineList = new List<string>(); // Lista de l�neas
    private int index = 0;
    private float textSpeed = 0.02f;
    private bool isWriting = false; // Flag para verificar si se est� escribiendo

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Si se est� escribiendo, completar la l�nea actual inmediatamente
            if (isWriting)
            {
                CompleteLineImmediately();
            }
            else
            {
                NextLine(); // Avanzar al siguiente texto si no se est� escribiendo
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
            lineList = SplitTextIntoLines(text, 150); // Dividir el texto en l�neas de m�ximo 30 caracteres
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
        isWriting = true; // Indicar que se est� escribiendo
        foreach (char c in lineList[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed); ; // Esperar un frame antes de mostrar el siguiente car�cter
        }
        isWriting = false; // Indicar que se ha completado la escritura
    }

    private void CompleteLineImmediately()
    {
        // Detener la escritura y mostrar la l�nea completa
        StopAllCoroutines();
        textComponent.text = lineList[index];
        isWriting = false;
    }

    private void NextLine()
    {
        // Avanzar al siguiente texto si hay m�s l�neas disponibles
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
                currentLine += word + " "; // Agregar la palabra a la l�nea actual
            }
            else
            {
                lines.Add(currentLine); // Agregar la l�nea actual a la lista
                currentLine = word + " "; // Comenzar una nueva l�nea con la palabra actual
            }
        }

        if (!string.IsNullOrEmpty(currentLine))
        {
            lines.Add(currentLine); // Agregar la �ltima l�nea si no est� vac�a
        }

        return lines;
    }
}
