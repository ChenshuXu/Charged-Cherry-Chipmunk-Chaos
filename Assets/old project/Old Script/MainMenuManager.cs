using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly
using UnityEngine.EventSystems; // include EventSystems namespace so can set initial input for controller support
using UnityEngine.SceneManagement; // include so we can load new scenes

public class MainMenuManager : MonoBehaviour {

    public GameObject _MainMenu;
    public GameObject _LevelSelect;
    public GameObject _Controls;
    public GameObject _Credits;

    public GameObject MainDefault;
    public GameObject LevelSelectDefault;

    // Use this for initialization
    void Start () {
        MenuSwitch("MainMenu");
	}
	
	public void MenuSwitch(string menu)
    {
        // turn all menus off
        _MainMenu.SetActive(false);

        // turn on desired menu and set default selected button for controller input
        switch (menu)
        {
            case "MainMenu":
                _MainMenu.SetActive(true);
                _Controls.SetActive(false);
                _LevelSelect.SetActive(false);
                _Credits.SetActive(false);
                EventSystem.current.SetSelectedGameObject(MainDefault);
                break;
            case "Controls":
                _Controls.SetActive(true);
                _MainMenu.SetActive(false);
                _LevelSelect.SetActive(false);
                _Credits.SetActive(false);
                break;
            case "LevelSelect":
                _Controls.SetActive(false);
                _MainMenu.SetActive(false);
                _LevelSelect.SetActive(true);
                _Credits.SetActive(false);
                EventSystem.current.SetSelectedGameObject(LevelSelectDefault);
                break;
            case "Credits":
                _Controls.SetActive(false);
                _MainMenu.SetActive(false);
                _LevelSelect.SetActive(false);
                _Credits.SetActive(true);
                break;
        }
    }

    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
