using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject GameOverScreen;
    public GameObject WinScreen;
    public Button RetryButton, MainMenuButton;

    private void Start() {
        RetryButton.onClick.AddListener( RetryGame );
        MainMenuButton.onClick.AddListener( GoToMainMenu );

        GameOverScreen.SetActive(false);
        WinScreen.SetActive(false);
    }

    void ActivateGameOver()
    {
        GameOverScreen.SetActive(true);
    }

    void ActivateWinScreen()
    {
        WinScreen.SetActive(true);
    }

    void RetryGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
