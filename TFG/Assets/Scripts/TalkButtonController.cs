using UnityEngine;

public class TalkButtonController : MonoBehaviour
{
    public GameObject talkButton;
    public GameObject player;
    private float activationDistance = 20f; // Distancia m�nima para activar el bot�n

    void Update()
    {
        // Calcula la distancia entre este objeto y el talkButton
        float distance = Vector2.Distance(gameObject.transform.position, player.transform.position);

        // Activa el bot�n si la distancia es menor que la distancia de activaci�n
        if (distance <= activationDistance)
        {
            talkButton.SetActive(true);
        }
        else
        {
            talkButton.SetActive(false);
        }
    }
}
