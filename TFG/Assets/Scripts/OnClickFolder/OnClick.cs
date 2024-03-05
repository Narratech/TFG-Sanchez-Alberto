using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class OnClick : MonoBehaviour
{
    private RaycastHit2D hit;
        public RaycastHit2D GetPositionRay(Vector3 mousePosition)
    {
        Vector2 rayOrigin = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 rayDirection = Vector2.zero;

        hit = Physics2D.Raycast(rayOrigin, rayDirection);

        // Puede devolverlo no inicializado
        return hit;
    }
}
