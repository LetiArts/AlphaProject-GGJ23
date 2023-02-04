using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour 
{
	public static Player instance;

	private SoundManager audioManager;
	PlayerMovement playerMovement;
	PlayerAnimator playerAnimator;

	public PlayerStats stats;

	[SerializeField]
	private HealthBar healthBar;
	public bool isInvincible;
	[HideInInspector] public float healthRegenInterval;
	[HideInInspector] public float healthRegenRate, reductionMultiplier, refillMultiplier;
	public bool canRegenerate,isRegenerating;
	[HideInInspector] public bool isRestoringOxygen = false;
	[HideInInspector] public bool shouldLooseOxygen = false;
	bool isLoosingOxygenHealth = false;
	private Rigidbody2D m_Rigidbody2D;


	private void Awake() {
		instance = this;

		healthRegenInterval = stats.healthRegenInterval;
		healthRegenRate = stats.healthRegenRate;
		reductionMultiplier = stats.oxygenReductionMultiplier;
		refillMultiplier = stats.oxygenRefillMultiplier;
		
		playerMovement = PlayerMovement.instance;
		playerAnimator = PlayerAnimator.instance;
	}

	void Start()
	{
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
				KillPlayer();
			} 
			else 
			{
				audioManager.PlaySFX (stats.hitSoundName);
			}
		}
	}

	public void KillPlayer()
	{
		StartCoroutine(WaitToDead());
	}

	IEnumerator WaitToDead()
	{
		playerAnimator.anim.SetBool("IsDead", true);
		yield return new WaitForSeconds(0.4f);
		playerMovement.RB.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
		yield return new WaitForSeconds(1.1f);
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
	}


	IEnumerator MakeInvincible(float time) 
	{
		isInvincible = true;
		yield return new WaitForSeconds(time);
		isInvincible = false;
		yield break;
	}
}
