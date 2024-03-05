using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Estas funciones heredarían de OnClick, en la que estaría el contexto

public class OnClickNaeve : MonoBehaviour
{
    public GameObject enemy;
    private bool enemyActivated = false;
    private float activationAxis = 35f; // La coordenada en la que aparecerá el enemigo

    private void Update()
    {
        if (transform.position.x > activationAxis && !enemyActivated)
        {
            // Activa el enemigo
            enemy.SetActive(true);
            enemyActivated = true;
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Naeve alza la cabeza");
        // New objeto chatGPT con el mensaje que genera el click
    }
}
