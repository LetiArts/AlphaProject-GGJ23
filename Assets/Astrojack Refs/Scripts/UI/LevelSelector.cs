using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class LevelSelector : MonoBehaviour {

	GameObject Vulkan;
	GameObject Forest;
	GameObject Space;

	[SerializeField]
	string hoverOverSound = "ButtonHover";

	[SerializeField]
	string PressButonSound = "ButtonPress";

	SoundManager audioManager;

	// Use this for initialization
	void Start () 
	{
		Vulkan = GameObject.Find ("Map_Vulkan");
		Forest = GameObject.Find ("Map_Forest");
		Space = GameObject.Find ("Map_Space");

		audioManager = SoundManager.instance;
		if (audioManager == null) 
		{
			Debug.LogError ("No audio found");
		}
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void ButtonHover (Button button)
	{
		if (button.name == "Vulkan") 
		{
			Vulkan.transform.SetAsLastSibling ();
		}
		else if (button.name == "Forest") 
		{
			Forest.transform.SetAsLastSibling ();
		}
		else if (button.name == "Space") 
		{
			Space.transform.SetAsLastSibling ();
		}
	}

	public void ButtonClick (Button button)
	{
		if (button.name == "Vulkan") 
		{
			audioManager.PlaySFX (PressButonSound);
			SceneManager.LoadScene ("Vulkan");

		}
		else if (button.name == "Forest") 
		{
			audioManager.PlaySFX (PressButonSound);
			SceneManager.LoadScene ("Forest");
		}
		else if (button.name == "Space") 
		{
			audioManager.PlaySFX (PressButonSound);
			SceneManager.LoadScene ("Space");
		}
	}

	public void OnMouseOver()
	{
		audioManager.PlaySFX (hoverOverSound);
	}
}
