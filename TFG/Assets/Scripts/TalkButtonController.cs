using UnityEngine;

public class TalkButtonController : MonoBehaviour
{
    public GameObject talkButton;
    public GameObject player;
    public float activationDistance = 25f; // Distancia m�nima para activar el bot�n

    void Update()
    {
        // Calcula la distancia entre este objeto y el talkButton
        float distance = Vector2.Distance(gameObject.transform.position, player.transform.position);
        // Activa el bot�n si la distancia es menor que la distancia de activaci�n
        talkButton.SetActive(distance <= activationDistance);
    }

    public void Deactive()
    {
        talkButton.SetActive(false);
    }
}
