using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    [HideInInspector]
    public bool open;

    private void Start()
    {
        // This sets the open variable to match whatever we set in the inspector since if it is enabled, the open bool is always false which means that it will try opening this menu, not closing it even when it is open. 
        // Thus, this sets the open state of the menu in this script to match the active state we set in the editor.
        open = gameObject.activeSelf;
    }

    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }
}
