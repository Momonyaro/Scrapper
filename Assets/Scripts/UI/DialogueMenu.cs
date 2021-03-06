using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMenu : MonoBehaviour
{
    public bool showMenu = false;
    public Animator animator;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            showMenu = !showMenu;
            animator.SetBool("SHOW_MENU", showMenu);
        }
    }
}
