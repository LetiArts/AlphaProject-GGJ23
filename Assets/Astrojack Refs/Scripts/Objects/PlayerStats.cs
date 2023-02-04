using System.Collections;
using System.Collections.Generic;
using UnityEngine; 


[System.Serializable]
[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    [Range(0, 200)]
	public float maxHealth = 100;
	public float movementSpeed = 50f;

	public float _curHealth;
	public float curHealth
	{
		get { return _curHealth; }
		set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
	}

	[Range(0, 200)]
	public float maxOxygen = 100;
	public float _curOxygen;
	public float curOxygen
	{
		get { return _curOxygen; }
		set { _curOxygen = Mathf.Clamp(value, 0, maxOxygen); }
	}


	[Header("The rate at which health restores")]
	public float healthRegenRate = 2f;

	[Header("The rate at which Oxygen restores and reduces")]
	public float oxygenRefillMultiplier = 5f;
	public float oxygenReductionMultiplier = 2f;

	[Header("The interval between restore")]
	public float healthRegenInterval = 2f;
	
	[Header("Player's Resistance to damage")]
	public float damageResistance = 1f;

	[Header("Kill player if he falls outside screen boundaries")]
	public int fallBoundary = -20;

	[Header("Jetpack Tweaks")]
	[Tooltip("Jetpack Force")]
	public float thrustForce = 2;
	// private float maxVerticalSpeed = 3f;
	[Tooltip("Fuel in seconds")]
	[SerializeField]
	public float maxJetpackTime = 1f;
	public float fillupRate = 0.1f;

	public string hitSoundName = "Grunt";
	public string deathSoundName = "DeathVoice";
}
