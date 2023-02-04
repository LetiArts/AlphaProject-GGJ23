using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineWPN : MonoBehaviour {

	public GameObject Pistol;
	public GameObject MachineGun;
	public GameObject LaserGun;

	void Start()
	{
		//LaserGun.SetActive (false);
		MachineGun.SetActive (false);
		//Pistol.SetActive (true);
	}

	void OnTriggerEnter2D (Collider2D _colInfo)
	{
		if (_colInfo.tag == "Player") 
		{
			LaserGun.SetActive (false);
			Pistol.SetActive (false);
			MachineGun.SetActive (true);
			Destroy (gameObject, 1f);
		}
	}
}
