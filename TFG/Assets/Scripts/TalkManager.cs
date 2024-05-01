using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    // Patr�n Singleton.
    public static TalkManager Instance;
    public GameObject portonTalkMenu;
    public GameObject estatuaTalkMenu;
    public GameObject quimeraTalkMenu;
    public GameObject loboTalkMenu;
    public GameObject guardaTalkMenu;

    public GameObject portonDialogueBox;
    public GameObject estatuaDialogueBox;

    public void Awake()
    {
        Instance = this;
    }

    public void WakeUpPortonMenu()
    {
        if (portonTalkMenu != null) 
        {
            PauseMenu pauseMenu = gameObject.GetComponent<PauseMenu>();
            if (pauseMenu != null )
            {
                pauseMenu.Pause(portonTalkMenu);
            }
        }
    }

    public void WakeUpEstatuaMenu()
    {
        if (estatuaTalkMenu != null)
        {
            PauseMenu pauseMenu = gameObject.GetComponent<PauseMenu>();
            if (pauseMenu != null)
            {
                pauseMenu.Pause(estatuaTalkMenu);
            }
        }
    }

    internal void WakeUpQuimeraMenu()
    {
        if (quimeraTalkMenu != null)
        {
            PauseMenu pauseMenu = gameObject.GetComponent<PauseMenu>();
            if (pauseMenu != null)
            {
                pauseMenu.Pause(quimeraTalkMenu);
            }
        }
    }

    internal void WakeUpLoboMenu()
    {
        if (loboTalkMenu != null)
        {
            PauseMenu pauseMenu = gameObject.GetComponent<PauseMenu>();
            if (pauseMenu != null)
            {
                pauseMenu.Pause(loboTalkMenu);
            }
        }
    }

    internal void WakeUpGuardaMenu()
    {
        if (guardaTalkMenu != null)
        {
            PauseMenu pauseMenu = gameObject.GetComponent<PauseMenu>();
            if (pauseMenu != null)
            {
                pauseMenu.Pause(guardaTalkMenu);
            }
        }
    }
}