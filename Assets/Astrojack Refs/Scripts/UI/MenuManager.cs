using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
	
	[SerializeField]
	string hoverOverSound = "ButtonHover";

	[SerializeField]
	string PressButonSound = "ButtonPress";

	// SoundManager audioManager;

	void Start()
	{
		// audioManager = SoundManager.instance;
		// if (audioManager == null) 
		// {
		// 	Debug.LogError ("No audio found");
		// }
	}
	// Use this for initialization
	public void StartGame () 
	{
		// audioManager.PlaySFX (PressButonSound);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
	}

	public void QuitGame()
	{
		// audioManager.PlaySFX (PressButonSound);
		Debug.Log ("QUIT");
		Application.Quit ();
	}

	public void OnMouseOver()
	{
		// audioManager.PlaySFX (hoverOverSound);
	}
}
