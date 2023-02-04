using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolWPN : MonoBehaviour {

	public GameObject Pistol; 
	public GameObject MachineGun;

	void Start()
	{
		Pistol.SetActive (true);
	}

	void OnTriggerEnter2D (Collider2D _colInfo)
	{
		if (_colInfo.tag == "Player") 
		{
			Pistol.SetActive (true);
			MachineGun.SetActive (false);
		}

		Destroy (gameObject);
	}
}

