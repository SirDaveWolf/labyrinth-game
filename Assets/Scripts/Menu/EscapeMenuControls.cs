using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenuControls : MonoBehaviour
{
    public GameObject MainEscapeMenu;
    public GameObject MenuBackground;

    private CursorLockMode _lastCursorLockMode;

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
        _lastCursorLockMode = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        gameObject.SetActive(true);
        MainEscapeMenu.SetActive(true);
    }

    public void GoBackToGame()
    {
        Cursor.lockState = _lastCursorLockMode;
        gameObject.SetActive(false);
        MainEscapeMenu.SetActive(false);
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
