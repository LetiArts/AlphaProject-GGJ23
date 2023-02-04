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
	// public Animator TreeAnimator;
	// public Button RepairButton;
	public Animator treeAnim;

    public bool isInvincible, canRegenerate;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

	public bool m_Grounded; // Whether or not the Tree is on ground level.
    bool isRegenerating;
	float curHealth;
    float healthRegenRate, healthRegenInterval;
	private float repairSpeed;
	bool waited = false; //for regen purposes..


    [Space]
	public bool shouldGrow = false;
    [Range(0,1)]
    public float plantGrowth;
	public Transform treeTransform;
    public float startGrowthSize;
	public float growthMultiplier;
    public float plantFullSize;
    private float startNendsize;

	private void Awake() {
		instance = this;

		curHealth = stats.curHealth;
		healthRegenInterval = stats.healthRegenInterval;
		healthRegenRate = stats.healthRegenRate;
		repairSpeed = stats.repairSpeed;
        curHealth = stats.maxHealth;
		// RepairButton.onClick.AddListener(InitRepair);
	}

	private void Start() {
		CheckGroundStatus();
        startNendsize = plantFullSize + startGrowthSize;
	}


    private void Update()
    {
		if (shouldGrow)
		{
			PlantGrowth();
		}
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

			if (curHealth <= stats.maxHealth / 2)
			{
				treeAnim.Play("low_health_indicator");
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
		StartCoroutine(DeactivateObj());
		treeAnim.Play("treeDeath");
    }

	IEnumerator DeactivateObj() 
	{
		yield return new WaitForSeconds(2);
		this.gameObject.SetActive(false);
		yield break;
	}

    void PlantGrowth()
    {
        plantGrowth += plantGrowth < 1? (Time.deltaTime * growthMultiplier)/100 : 0;
        plantGrowth = Mathf.Clamp(plantGrowth, 0,1);
        float sizeGrowthTotal = startGrowthSize+(plantFullSize * (plantFullSize * plantGrowth));


        treeTransform.localScale = new Vector3(sizeGrowthTotal,sizeGrowthTotal,sizeGrowthTotal);
    }
}
