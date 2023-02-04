using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    
    public TMP_Text MoneyText, KillsText;
    private int kills;
    public int curKills
	{
		get { return kills++; }
		set { KillsText.text = kills.ToString(); }
	}

    public GameObject GameOverScreen;
    public GameObject WinScreen;
    public Button RetryButton, MainMenuButton;


    private void Awake() {
        instance = this;
    }
    
    private void Start() {
        if(RetryButton != null && MainMenuButton != null)
        {
            RetryButton.onClick.AddListener( RetryGame );
            MainMenuButton.onClick.AddListener( GoToMainMenu );
        }

        if (GameOverScreen != null && WinScreen != null)
        {
            GameOverScreen.SetActive(false);
            WinScreen.SetActive(false);
        }
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
