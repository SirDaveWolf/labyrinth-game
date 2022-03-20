using Assets.Scripts.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenuControls : MonoBehaviour
{
    public GameObject MainEscapeMenu;
    public GameObject MenuBackground;
    public GameObject OptionsMenu;

    private CursorLockMode _lastCursorLockMode;

    private Options _options;

    void Awake()
    {
        _options = new Options();
        _options.Load();
        _options.SetGlobals();
    }

    // Start is called before the first frame update
    void Start()
    {
        GoBackToGame();
    }

    public bool IsActive()
    {
        return gameObject.activeSelf || MainEscapeMenu.activeSelf;
    }

    public void ShowEscapeMenu()
    {
        _options.GetFromGlobals();
        _options.Save();

        _lastCursorLockMode = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        gameObject.SetActive(true);
        MainEscapeMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

    public void ShowOptionsMenu()
    {
        MainEscapeMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }

    public void GoBackToGame()
    {
        Cursor.lockState = _lastCursorLockMode;
        gameObject.SetActive(false);
        MainEscapeMenu.SetActive(false);
        OptionsMenu.SetActive(false);
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
