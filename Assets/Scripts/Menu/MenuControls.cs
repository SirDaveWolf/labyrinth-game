using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControls : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public GameObject PlayMenu;

    public Dropdown ThemeDropdown;
    

    private void Start()
    {
        ShowMainMenu();

        var themes = System.Enum.GetNames(typeof(GameTheme));
        foreach (var theme in themes)
            ThemeDropdown.options.Add(new Dropdown.OptionData(theme));

        ThemeDropdown.RefreshShownValue();

        GameRules.GameTheme = GameTheme.Default;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void ShowMainMenu()
    {
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
        PlayMenu.SetActive(false);
    }

    public void ShowOptionsMenu()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
        PlayMenu.SetActive(false);
    }

    public void ShowPlayMenu()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        PlayMenu.SetActive(true);
    }

    public void ChangeTheme(int value)
    {
        GameRules.GameTheme = (GameTheme)value;
    }

    public void ChangePlayers(int value)
    {
        GameRules.PlayerCount = value + 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
