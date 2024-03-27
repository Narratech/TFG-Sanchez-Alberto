using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilizarParaguas : MonoBehaviour, IUtilizable
{
    TraductionLogic logicTraduction;
    public void Utilizar()
    {
        Debug.Log("Has utilizado el paraguas.");
        GameObject obj = GameObject.Find("Traduction");
        if (obj != null)
        {
            logicTraduction = obj.GetComponent<TraductionLogic>();
            logicTraduction.ActivateParaguas();
        }
    }
}
