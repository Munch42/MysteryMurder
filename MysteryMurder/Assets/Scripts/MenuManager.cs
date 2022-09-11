using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // This combined with the Awake assignment of this allows us to access this specific instance of the menu manager on the gameobject in unity in another script without needing a reference to this game object. 
    // Since static means that it is assigned to the script not the object.
    // We can use it like: MenuManager.Instance.OpenMenu("title");
    public static MenuManager Instance;

    [SerializeField]
    Menu[] menus;

    private void Awake()
    {
        Instance = this;
    }

    // Good for scripts that need to open and close menus since they can just use a string "key"
    public void OpenMenu(string menuName)
    {
        foreach(Menu menu in menus){
            if (menu.menuName == menuName)
            {
                OpenMenu(menu);
            }
        }
    }

    // Good for buttons where you can drag in the menu
    public void OpenMenu(Menu menu)
    {
        closeOtherMenus();
        menu.Open();
    }

    public void CloseMenu(string menuName)
    {
        foreach (Menu menu in menus)
        {
            if (menu.menuName == menuName)
            {
                CloseMenu(menu);
            }
        }
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    private void closeOtherMenus()
    {
        foreach (Menu menu in menus)
        {
            // If it is not the menu we are looking for, but it is currently open, then we want to close it.
            // Here, we just close all the menus and then after this function is called, we will open whatever menu we want.
            menu.Close();
        }
    }

}
