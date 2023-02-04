using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour {

	[SerializeField]
	string hoverOverSound = "ButtonHover";

	[SerializeField]
	string PressButonSound = "ButtonPress";

	SoundManager audioManager;

	void OnEnable()
	{
		audioManager = SoundManager.instance;
		if (audioManager == null) 
		{
			Debug.LogError ("No audio found");
		}
	}

	public void Quit ()
	{
		audioManager.PlaySFX (PressButonSound);
		Debug.Log("APPLICATION QUIT!");
		Application.Quit();
	}

	public void Retry ()
	{
		audioManager.PlaySFX (PressButonSound);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LevelSelect()
	{
		audioManager.PlaySFX (PressButonSound);
		SceneManager.LoadScene ("Level Select");
	}

	public void MainMenu()
	{
		audioManager.PlaySFX (PressButonSound);
		SceneManager.LoadScene ("MainMenu");
	}

	public void OnMouseOver()
	{
		audioManager.PlaySFX (hoverOverSound);
	}
}
