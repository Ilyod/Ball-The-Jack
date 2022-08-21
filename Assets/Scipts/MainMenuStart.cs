using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuStart : MonoBehaviour
{
    public GameObject MainMenu;

    // Start is called before the first frame update
    void Start()
    {
        // To be sure that it's always the Main Menu that is active when the scene loads
        GameObject[] OtherMenus = GameObject.FindGameObjectsWithTag("Menu2");
        foreach (GameObject menu in OtherMenus)
        {
            menu.SetActive(false);
        }
        MainMenu.SetActive(true);
    }
}
