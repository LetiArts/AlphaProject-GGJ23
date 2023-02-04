using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gems : MonoBehaviour {

	public int MoneyDrop = 10;

	public Transform particles; 

	void OnTriggerEnter2D(Collider2D _colInfo) 
	{
		Player _player = _colInfo.GetComponent<Player>();
		if (_player != null)
		{
			GameMaster.instance.Money += MoneyDrop;
			Instantiate(particles, transform.position, Quaternion.identity);
			Destroy (gameObject);
		}
	}
}
