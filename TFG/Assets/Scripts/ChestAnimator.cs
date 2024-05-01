using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAnimator : MonoBehaviour
{
    public Animator chestAnimation;

    internal void OpenChest()
    {
        chestAnimation.SetBool("IsOpened", true);
    }


}
