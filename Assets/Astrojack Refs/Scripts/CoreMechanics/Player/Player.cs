using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public static Player instance;

	private SoundManager audioManager;
	public PlayerStats stats;
	CharacterController2D characterController;

	[SerializeField]
	private HealthBar healthBar;
	public bool isInvincible;
	[HideInInspector] public float healthRegenInterval;
	[HideInInspector] public float healthRegenRate, reductionMultiplier, refillMultiplier;
	public bool canRegenerate,isRegenerating;
	[HideInInspector] public bool isRestoringOxygen = false;
	[HideInInspector] public bool shouldLooseOxygen = false;
	bool isLoosingOxygenHealth = false;

	private void Awake() {
		instance = this;

		healthRegenInterval = stats.healthRegenInterval;
		healthRegenRate = stats.healthRegenRate;
		reductionMultiplier = stats.oxygenReductionMultiplier;
		refillMultiplier = stats.oxygenRefillMultiplier;
	}

	void Start()
	{
		characterController = CharacterController2D.instance;

		stats.curHealth = stats.maxHealth;
		stats.curOxygen = stats.maxOxygen;

		if (healthBar == null)
		{
			Debug.LogError("No health bar referenced on Player");
		}
		else
		{
			healthBar.SetOxygenLevel(stats.curOxygen);
		}

		audioManager = SoundManager.instance;
		if (audioManager == null)
		{
			Debug.LogError("FREAK OUT! No AudioManager found in the scene.");
		}

		InitDroppingOxygen();
	}

	public void InitDroppingOxygen()
	{
		StartCoroutine(BeginDroppingOxygen());
	}

	IEnumerator BeginDroppingOxygen()
	{
		yield return new WaitForSeconds(2f);
		isRestoringOxygen = false;
		shouldLooseOxygen = true;
	}

	void Update () {
		if (transform.position.y <= stats.fallBoundary)
		{
			DamagePlayer (995f);
		}

		if (Input.GetKeyDown("m")) {
			DamagePlayer(10);
			GameObject pill = ObjectPooler.SharedInstance.GetPooledObject("Pill");
			pill.SetActive(true);
		}

		if (shouldLooseOxygen)
		{
			if (stats.curOxygen > 0 && !isRestoringOxygen)
			{
				stats.curOxygen -= reductionMultiplier * Time.deltaTime;
				healthBar.SetOxygenLevel(stats.curOxygen);
			}
			else if (stats.curOxygen <= 0 && !isRestoringOxygen && !isLoosingOxygenHealth)
			{
				isLoosingOxygenHealth = true;
				InvokeRepeating("HealthReduction", 1.0f, 1f);
			}
		}
		//gaining oxygen
		else 
		{
			if (isRestoringOxygen && NeedsOxygen())
			{
				CancelInvoke("HealthReduction");
				isLoosingOxygenHealth = false;
				stats.curOxygen += 1 * Time.deltaTime * refillMultiplier;
				healthBar.SetOxygenLevel(stats.curOxygen);
			}
		}
		
	}

	void HealthReduction()
	{
		DamagePlayer(5f);
	}

	public bool NeedsOxygen()
	{
		bool needsOxygen = false;
		if (stats.curOxygen < stats.maxOxygen)
		{
			needsOxygen = true;
		}

		return needsOxygen;
	}

	public bool NeedsHealth()
	{
		bool needsHealth = false;
		if (stats.curHealth < stats.maxHealth)
		{
			needsHealth = true;
		}
		return needsHealth;
	}

	public void RestoreHealth(int restoreAmount)
	{
		healthBar.HealDamage(restoreAmount); 
	}

	public void DamagePlayer (float damage) 
	{
		healthBar.TakeDamage(damage -= stats.damageResistance);
		//if we're not invincible then let's damage player
		if (!isInvincible)
		{
			if (stats.curHealth <= 0) {
				audioManager.PlaySFX (stats.deathSoundName);
				characterController.KillPlayer();
			} 
			else 
			{
				audioManager.PlaySFX (stats.hitSoundName);
			}
		}
	}

	IEnumerator MakeInvincible(float time) 
	{
		isInvincible = true;
		yield return new WaitForSeconds(time);
		isInvincible = false;
		yield break;
	}
}
