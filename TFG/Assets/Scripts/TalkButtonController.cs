using UnityEngine;

public class TalkButtonController : MonoBehaviour
{
    public GameObject talkButton;
    public GameObject player;
    private float activationDistance = 20f; // Distancia mínima para activar el botón

    void Update()
    {
        // Calcula la distancia entre este objeto y el talkButton
        float distance = Vector2.Distance(gameObject.transform.position, player.transform.position);

        // Activa el botón si la distancia es menor que la distancia de activación
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
