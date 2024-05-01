using UnityEngine;

public class GuardaController : MonoBehaviour
{
    public GameObject guardaMoving;
    public GameObject guardaIdle;
    public GameObject player;
    private Vector3 startPosition;
    private bool isReturning = false;
    private float speed = 5f;

    private void Start()
    {
        // Guarda la posici�n inicial del guardia
        startPosition = transform.position;
        // Inicia la animaci�n del guardia
        guardaIdle.SetActive(false);
        guardaMoving.SetActive(true);
    }

    private void Update()
    {
        if (!isReturning)
        {
            Vector3 target = player.transform.position + Vector3.right * 7f + Vector3.up * 4.7f;
            // Si el guardia no est� volviendo a su posici�n inicial, lo mueve hacia la posici�n del jugador
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (target.x > transform.position.x)
            {
                transform.localScale = new Vector3(-1f, 1f, 1);
            }
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1);
            }

            if (transform.position == target)
            {
                guardaIdle.SetActive(true);
                guardaMoving.SetActive(false);
            }
            else
            {
                guardaIdle.SetActive(false);
                guardaMoving.SetActive(true);
            }
        }
        else
        {
            // Si el guardia est� volviendo a su posici�n inicial, lo mueve hacia all�
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
            transform.localScale = new Vector3(-1f, 1f, 1);
            // Si el guardia ha vuelto a su posici�n inicial, detiene la animaci�n de caminar
            if (transform.position == startPosition)
            {
                guardaIdle.SetActive(true);
                guardaMoving.SetActive(false);
            }
            else
            {
                guardaIdle.SetActive(false);
                guardaMoving.SetActive(true);
            }
        }
    }

    internal void Alarma()
    {
        // Coloca un mensaje en la consola de Unity
        Debug.Log("�Alarma!");
    }

    internal void Irse()
    {
        // Establece que el guardia est� volviendo a su posici�n inicial y activa la animaci�n de caminar
        isReturning = true;
    }
}
