using System.Collections;
using System.Collections.Generic;
using UnityEngine; 


[System.Serializable]
[CreateAssetMenu]
public class TreeStats : ScriptableObject
{
    [Range(0, 200)]
	public float maxHealth = 100;
	public float _curHealth;
	[Range(0, 1)]
	public float repairSpeed = 0.1f;
	public float curHealth
	{
		get { return _curHealth; }
		set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
	}

	[Header("The rate at which health restores")]
	public float healthRegenRate = 2f;

	[Header("The interval between restore")]
	public float healthRegenInterval = 2f;

	public string deathSoundName = "Explosion";
}
