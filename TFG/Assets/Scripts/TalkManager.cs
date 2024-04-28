using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    // Patrón Singleton.
    public static TalkManager Instance;
    public GameObject portonTalkMenu;
    public GameObject estatuaTalkMenu;
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

}
