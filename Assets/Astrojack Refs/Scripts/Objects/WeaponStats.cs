using System.Collections;
using System.Collections.Generic;
using UnityEngine; 


[System.Serializable]
[CreateAssetMenu]
public class WeaponStats : ScriptableObject 
{
	public string weaponName;
	public Sprite weaponImage;
	public int Damage = 10;
	public int maxAmmo = 10; 
	public float fireRate = 0;
	public float effectSpawnRate = 10;
	public float reloadTime = 1f;
	//SHAKE
	public float camShakeAmt = 0.05f;
	public float camShakeLenght = 0.1f;
	public string weaponShootSound = "DefaultShot";
}
