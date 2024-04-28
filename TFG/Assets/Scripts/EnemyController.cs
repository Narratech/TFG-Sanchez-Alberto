using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float baseSpeed = 2f;
    private float targetSpeed = 8f; // Velocidad máxima (cuatriplicada)
    private float currentSpeed;
    private float accelerationTime = 10f; // Tiempo en segundos para acelerar
    private float accelerationTimer = 0f;
    private Vector2 target;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            Move(gameObject);
        }
    }

    void Move(GameObject obj)
    {
        target = new Vector2(-51, 9);
        MoveTowardsTarget(obj);
    }

    private void MoveTowardsTarget(GameObject obj)
    {
        accelerationTimer += Time.deltaTime;

        // Acelera gradualmente durante el tiempo especificado
        currentSpeed = Mathf.Lerp(baseSpeed, targetSpeed, accelerationTimer / accelerationTime);

        float step = currentSpeed * Time.deltaTime;
        obj.transform.position = Vector2.MoveTowards(obj.transform.position, target, step);

        if ((Vector2)obj.transform.position == target)
        {
            gameObject.SetActive(false);
        }
    }
}

