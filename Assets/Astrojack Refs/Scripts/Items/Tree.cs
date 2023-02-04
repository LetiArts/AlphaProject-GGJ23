using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tree : MonoBehaviour
{
    public static Tree instance;

    public TreeStats stats;
    [SerializeField]
	private StatusIndicator statusIndicator;
	public Animator TreeAnimator;
	public Button RepairButton;
	public Image repairFill;
	public ParticleSystem fireParticles;

    public bool isInvincible, canRegenerate;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

	public bool m_Grounded; // Whether or not the Tree is on ground level.
    bool isRegenerating;
	float curHealth;
    float healthRegenRate, healthRegenInterval;
	private float repairSpeed;
	bool inRange = false; //if player is in range
	bool needsRepair = false;
	bool activatedPop = false;
	bool waited = false; //for regen purposes..
	bool justRepaired = false;

    [Space]
    [Range(0,1)]
    public float plantGrowth;

    public float plantFullSize;

	private void Awake() {
		instance = this;

		curHealth = stats.curHealth;
		healthRegenInterval = stats.healthRegenInterval;
		healthRegenRate = stats.healthRegenRate;
		repairSpeed = stats.repairSpeed;
        curHealth = stats.maxHealth;
		needsRepair = false;
		// RepairButton.onClick.AddListener(InitRepair);
	}

	private void Start() {
		CheckGroundStatus();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player" && needsRepair == true)
		{
			TreeAnimator.Play("repair_pop");
			RepairButton.interactable = true;
			activatedPop = true;
			inRange = true;
		}

		if (other.gameObject.tag == "Player" && Player.instance.NeedsOxygen())
		{
			Player.instance.shouldLooseOxygen = false;
			Player.instance.isRestoringOxygen = true;
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.tag == "Player" && activatedPop == true)
		{
			if (!justRepaired)
			{
				TreeAnimator.Play("repair_popout");
			}

			justRepaired = false;

			RepairButton.interactable = false;
			activatedPop = false;
			inRange = false;
			repairFill.fillAmount = 0;
		}
		//if we casually exited the trigger
		else if (other.gameObject.tag == "Player")
		{
			Player.instance.InitDroppingOxygen();
		}
	}

    private void Update()
    {
       PlantGrowth();
    }

    public void CheckGroundStatus()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		Collider2D[] colliders = Physics2D.OverlapBoxAll(m_GroundCheck.position, _groundCheckSize, 0, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				m_Grounded = true;
		}
	}

	public void InitRepair()
	{
		StartCoroutine(BeginRepair());
	}

	IEnumerator BeginRepair()
	{
		bool isFilled = false;
		TreeAnimator.Play("repairing");
		RepairButton.interactable = false;
		while (curHealth < stats.maxHealth && inRange)
		{
			repairFill.fillAmount += repairSpeed * Time.deltaTime;
			
			if (repairFill.fillAmount == 1f && !isFilled)
			{
				curHealth = stats.maxHealth;
				needsRepair = false;
				statusIndicator.SetHealth(curHealth, stats.maxHealth, false);
				fireParticles.Stop();
				justRepaired = true;
				Debug.LogError("Finished repair");
				TreeAnimator.Play("repair_popout");

				yield break;
			}
			
			yield return null;
		}
	}

	public void DamageTree (int damage) {
		curHealth -= damage; 
		Debug.LogWarning("Tree is taking damage");
		//if we're not invincible then let's damage
		if (!isInvincible)
		{
			if (curHealth <= 0) {
				SoundManager.instance.PlaySFX (stats.deathSoundName);
				DestroyTree();
			} 

			statusIndicator.SetHealth(curHealth, stats.maxHealth, true);
			needsRepair = true;

			if (curHealth <= stats.maxHealth / 2)
			{
				fireParticles.Play();
			}
		}

		if(canRegenerate && !isRegenerating)
		{
			//false because we are restoring
			RestoreHealth();
		}
	}

	void RestoreHealth()
	{
		isRegenerating = true;
		StartCoroutine (Regenerate(healthRegenRate, healthRegenInterval));
	}

	IEnumerator Regenerate (float rate, float interval)
	{
		if (curHealth < stats.maxHealth)
		{
			if (waited == false)
			{
				yield return new WaitForSeconds (interval);
				waited = true;
			}
			curHealth += rate;
			statusIndicator.SetHealth(curHealth, curHealth, false); 
			// Debug.LogError($"regenerated with {rate} -- Cur health is now {curHealth}" );
			yield return new WaitForSeconds (interval);
			StartCoroutine (Regenerate(healthRegenRate, healthRegenInterval));
		}
		else{
			isRegenerating = false;
			waited = false;
			needsRepair = false;
			yield break;
		}
	}

	IEnumerator MakeInvincible(float time) 
	{
		isInvincible = true;
		yield return new WaitForSeconds(time);
		isInvincible = false;
	}

    void DestroyTree()
    {

    }
    void PlantGrowth()
    {
        transform.localScale = new Vector3(plantFullSize*plantGrowth, plantFullSize * plantGrowth, plantFullSize * plantGrowth);
    }
}
