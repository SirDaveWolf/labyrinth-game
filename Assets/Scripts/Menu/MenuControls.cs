using Assets.Scripts;
using Assets.Scripts.Menu;
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
    public GameObject ControlsMenu;
    public GameObject RulesMenu;
    public GameObject CreditsMenu;

    public Dropdown ThemeDropdown;
    public Dropdown CardsPerPlayerDropdown;

    public Text RuleText;

    private Options _options;

    private List<string> RuleTexts;
    private int CurrentText;

    private void Start()
    {
        RuleTexts = new List<string>()
        {
            @"Search the Labyrinth for your magical objects by carefully moving through the constantly moving maze.

The first player to find all of their objects and then return to the starting square is the winner.",
            @"On your turn, look at your card by clicking on it without showing it to the other players. You now have to try and get to the square showing the same picture as on your card. To do this, first insert a maze tile and then move your playing piece.",
            @"Along the edge of the board are 12 arrows. When it is your turn you must choose one of the arrows and then insert a maze tile, which will push out a maze tile at the opposite end.

The tile that is pushed out remains on the edge of the board until it is inserted elsewhere during the next player's turn. A maze tile cannot be pushed back at the same place as the previous player pushed it out.",
            @"After shifting the maze, move your playing character in first person as far as you like along the open pathway.

If you are unable to reach your goal directly get into the best possible starting position for your next turn. However, moving is not compulsory so the character also may be left where it is. You do not have to move if you don't want to.",
            @"The first player to find all of his collectables and get his character back onto its starting position is the winner."
        };

        CurrentText = 0;

        _options = new Options();
        _options.Load();
        _options.SetGlobals();

        ShowMainMenu();

        var themes = System.Enum.GetNames(typeof(GameTheme));
        foreach (var theme in themes)
            ThemeDropdown.options.Add(new Dropdown.OptionData(theme));

        ThemeDropdown.RefreshShownValue();

        GameRules.GameTheme = GameTheme.Default;

        CardsPerPlayerDropdown.value = GameRules.MaxCardsPerPlayer;
        CardsPerPlayerDropdown.RefreshShownValue();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void ShowMainMenu()
    {
        _options.GetFromGlobals();
        _options.Save();

        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
        PlayMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        RulesMenu.SetActive(false);
        CreditsMenu.SetActive(false);
    }

    public void ShowOptionsMenu()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
        PlayMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        RulesMenu.SetActive(false);
        CreditsMenu.SetActive(false);
    }

    public void ShowPlayMenu()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        PlayMenu.SetActive(true);
        ControlsMenu.SetActive(false);
        RulesMenu.SetActive(false);
        CreditsMenu.SetActive(false);
    }

    public void ShowControlsMenu()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        PlayMenu.SetActive(false);
        ControlsMenu.SetActive(true);
        RulesMenu.SetActive(false);
        CreditsMenu.SetActive(false);
    }

    public void ShowRulesMenu()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        PlayMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        RulesMenu.SetActive(true);
        CreditsMenu.SetActive(false);
    }

    public void ShowCreditsMenu()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        PlayMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        RulesMenu.SetActive(false);
        CreditsMenu.SetActive(true);
    }

    public void RulesNextText()
    {
        if (CurrentText < RuleTexts.Count - 1)
            CurrentText++;

        RuleText.text = RuleTexts[CurrentText];
    }

    public void RulesPreviousText()
    {
        if (CurrentText > 0)
            CurrentText--;

        RuleText.text = RuleTexts[CurrentText];
    }

    public void ChangeTheme(int value)
    {
        GameRules.GameTheme = (GameTheme)value;
    }

    public void ChangeCardsPerPlayer(int value)
    {
        GameRules.MaxCardsPerPlayer = value + 1;
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
