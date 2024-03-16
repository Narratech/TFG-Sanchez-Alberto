using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class ResumenController
{
    private GPTController resumenControllerGPT;
    private string resumenReply = "";

    public async Task<string> SendAndHandleReplyResumen(string msg)
    {
        GameObject gptResumenObject = GameObject.Find("Resumen Chat");
        string res = "";

        if (gptResumenObject != null)
        {
            resumenControllerGPT = gptResumenObject.GetComponent<GPTController>();

            if (resumenControllerGPT != null)
            {
                resumenReply = await resumenControllerGPT.SendReply(msg);
                while (string.IsNullOrEmpty(resumenReply)) // Esperar hasta que la respuesta no esté vacía
                {
                    await Task.Delay(10);
                }
                resumenControllerGPT.AppendMessage(resumenReply);
                resumenControllerGPT.UpdateGPT();

                res = resumenReply;
                resumenReply = "";
            }
        }
        return res;
    }
}
