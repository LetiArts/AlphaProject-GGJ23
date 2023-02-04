using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

	public GameObject Pistol;
	public GameObject MachineGun;
	public GameObject LaserGun;

	void Start()
	{
		LaserGun.SetActive (false);
		//MachineGun.SetActive (false);
		//Pistol.SetActive (true);
	}

	void OnTriggerEnter2D (Collider2D _colInfo)
	{
		if (_colInfo.tag == "Player") 
		{
			Pistol.SetActive (false);
			MachineGun.SetActive (false);
			LaserGun.SetActive (true);
			Destroy (gameObject, 1f);

		}
			
	}
}
