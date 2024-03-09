using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Jobs;

public class LogicController
{
    private List<Transform> bodyParts = new List<Transform>();

    public void SetBodyParts(Transform transform) 
    {
        foreach (Transform child in transform)
        {
            bodyParts.Add(child);
            SetBodyParts(child);
        }
    }

    public void ChangeLayer(Transform transform, int layer, string spriteLayer)
    {
        SpriteRenderer spriteRenderer;
        transform.gameObject.layer = layer;

        List<int> originalOrder = new List<int>();
        // Recorro cada parte del cuerpo de Naeve y guardo el orden de cada parte dentro de la capa y cambio la capa
        foreach (Transform part in bodyParts)
        {
            // Obtengo el SpriteRenderer y cambio la capa
            spriteRenderer = part.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                originalOrder.Add(spriteRenderer.sortingOrder);
                part.gameObject.layer = layer;
                spriteRenderer.sortingLayerName = spriteLayer;
            }          
        }

        int i = 0;
        // Recorro de nuevo cada parte del cuerpo de Naeve para devolver el orden relativo de cada parte dentro de la capa. Utilizo un contador para saltarme las partes del cuero que no tienen spriteRenderer y así no saltarno ninguna de las partes del cuerpo que sí lo tienen.
        foreach (Transform part in bodyParts)
        {
            // Obtengo el SpriteRenderer y cambio la capa
            bodyParts[i].SetSiblingIndex(originalOrder[i]);
            spriteRenderer = bodyParts[i].GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = originalOrder[i];
                i++;
            }
        }

        // Restaurar el orden original
        //for (int i = 0; i < originalOrder.Count; i++)
        //{
        //    bodyParts[i].SetSiblingIndex(originalOrder[i]);
        //    spriteRenderer = bodyParts[i].GetComponent<SpriteRenderer>();
        //    if (spriteRenderer != null)
        //    {
        //        spriteRenderer.sortingOrder = originalOrder[i];
        //    }
        //}
    }

    public List<Transform> getBodyParts()
    {
        return bodyParts;
    }
}
