using TMPro;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    private bool wolfFollowing = false;
    public Transform playerTransform; // Referencia al transform del jugador
    private float followDistance = 5f; // Distancia a la que el lobo seguirá al jugador

    private Vector2 direction; // Dirección hacia la que debe moverse el lobo

    private void Update()
    {
        if (wolfFollowing)
        {
            // Comprueba si la referencia al jugador está configurada
            if (playerTransform != null)
            {
                Vector3 targetPosition;
                // Gira el lobo para que mire hacia el jugador
                if (playerTransform.localScale.x > 0) // Si el jugador está a la derecha
                {
                    transform.localScale = new Vector3(-0.2f, 0.2f, 1); // Girar hacia la izquierda
                                                                        // Calcula la posición objetivo del lobo (posición del jugador + distancia de seguimiento)
                    targetPosition = playerTransform.position + Vector3.left * followDistance + Vector3.up * 2.5f;
                }
                else // Si el jugador está a la izquierda
                {
                    transform.localScale = new Vector3(0.2f, 0.2f, 1); // Girar hacia la derecha
                    targetPosition = playerTransform.position + Vector3.right * followDistance + Vector3.up * 2.5f;
                }
                // Asigna la posición objetivo al lobo
                transform.position = targetPosition;
            }
        }
    }

    public void SetFollow(bool follow)
    {
        wolfFollowing = follow;
    }

    public bool GetFollow()
    {
        return wolfFollowing;
    }

    private void MoveWolf()
    {
        // Mover el lobo en la dirección calculada
        transform.Translate(direction * Time.deltaTime);

        // Girar el lobo para que mire hacia el jugador
        if (direction.x > 0) // Si el jugador está a la derecha
        {
            transform.localScale = new Vector3(-0.2f, 0.2f, 1); // Girar hacia la izquierda
        }
        else // Si el jugador está a la izquierda
        {
            transform.localScale = new Vector3(0.2f, 0.2f, 1); // Girar hacia la derecha
        }
    }
}
