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
        // Guarda la posición inicial del guardia
        startPosition = transform.position;
        // Inicia la animación del guardia
        guardaIdle.SetActive(false);
        guardaMoving.SetActive(true);
    }

    private void Update()
    {
        if (!isReturning)
        {
            Vector3 target = player.transform.position + Vector3.right * 7f + Vector3.up * 4.7f;
            // Si el guardia no está volviendo a su posición inicial, lo mueve hacia la posición del jugador
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
            // Si el guardia está volviendo a su posición inicial, lo mueve hacia allí
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
            transform.localScale = new Vector3(-1f, 1f, 1);
            // Si el guardia ha vuelto a su posición inicial, detiene la animación de caminar
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
        Debug.Log("¡Alarma!");
    }

    internal void Irse()
    {
        // Establece que el guardia está volviendo a su posición inicial y activa la animación de caminar
        isReturning = true;
    }
}
