using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
	
	[SerializeField]
	string hoverOverSound = "ButtonHover";

	[SerializeField]
	string PressButonSound = "ButtonPress";

    public void PlayFXSound(string SoundName)
    {
        if (SoundManager.instance.IsSoundFXMuted() == false)
        {
            SoundManager.instance.PlaySFX(SoundName);
        }
    }

    public void BeginGame()
    {
        CheckingFirstTime();
    }


    public void CheckingFirstTime()
    {
        if (PlayerPrefs.GetInt("first_time_opening", 1) == 1)
        {
            PlayerPrefs.SetInt("first_time_opening", 0);
        	
			SceneManager.LoadScene("DialogScene");
        }
		else{
        	SceneManager.LoadScene("Bunker");
		}
    }

	public void QuitGame()
	{
		Debug.Log ("QUIT");
		Application.Quit ();
	}
}
