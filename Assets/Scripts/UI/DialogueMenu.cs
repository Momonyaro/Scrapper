using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMenu : MonoBehaviour
{
    public bool showMenu = false;
    public Animator animator;
    private bool _lastState = false;

    // Update is called once per frame
    void Update()
    {
        if (_lastState != showMenu)
        {
            animator.SetBool("SHOW_MENU", showMenu);
            _lastState = showMenu;
        }
    }
}
