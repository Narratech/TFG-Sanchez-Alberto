using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SIN USO AHORA MISMO
public class MousePosition : MonoBehaviour
{
    private Vector2 mousePosition;

    public void SetMousePosition(Vector2 mousePosition)
    {
        this.mousePosition = mousePosition;
        Debug.Log(this.mousePosition);
    }

    public Vector2 GetMousePosition()
    {
        return mousePosition;
    }
}
