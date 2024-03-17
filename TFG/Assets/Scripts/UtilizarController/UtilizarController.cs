using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilizador
{
    public void Utilizar(GameObject objeto)
    {
        IUtilizable utilizable = objeto.GetComponent<IUtilizable>();
        if (utilizable != null)
        {
            utilizable.Utilizar();
        }
        else
        {
            Debug.Log("El objeto no implementa la interfaz IUtilizable.");
        }
    }
}

