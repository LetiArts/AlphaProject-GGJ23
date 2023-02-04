using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar, OxygenBarFill;
    public Color normalHealthColor, lowHealthColor;
	public PlayerStats stats;
    public float maxHealth;
    public float currentHealth;
    public float regenerationRate;
    public float smoothTime = 0.1f;
    public float lowHealthThreshold = 20f;
    [Range(0, 1)]
    public float lowOxygenThreshold = 0.2f;
    public Animator lowHealthAnim, oxygenFillAnimator;
    private float healthVelocity;
    private bool lowHealth;
    //just to ensure we aren't stuck playing the idle anim.. system would work well without this
    //but i prefer to be more optimal
    bool isPlayingIdleOxygenIdleAnim = false;

    private void Start() {
        currentHealth = stats.curHealth;
        maxHealth = stats.maxHealth;
    }

    void Update()
    {
        healthBar.fillAmount = Mathf.SmoothDamp(healthBar.fillAmount, stats.curHealth / stats.maxHealth, ref healthVelocity, smoothTime);
        
        if(Player.instance.canRegenerate && Player.instance.NeedsHealth())
		{
            Regenerate(Player.instance.healthRegenRate);
            Debug.LogError("Regeninig");
        }
        HandleLowHealth();
    }

    public void TakeDamage(float dmg)
    {
        stats.curHealth -= dmg;
        // animator.SetBool("TakeDamage");
    }

    public void HealDamage(float restore)
    {
        if (stats.curHealth < stats.maxHealth)
        {
            stats.curHealth += restore;
        }
        else{
            stats.curHealth = stats.maxHealth;
        }
        // animator.SetBool("TakeDamage");
    }

    public void Regenerate(float regenRate)
    {
        stats.curHealth += Time.deltaTime * regenRate;
        // animator.SetBool("Regenerate");
        if (stats.curHealth > stats.maxHealth)
        {
            stats.curHealth = stats.maxHealth;
        }
    }

    public void HandleLowHealth()
    {
        if (stats.curHealth <= lowHealthThreshold && !lowHealth)
        {
            lowHealth = true;
            healthBar.color = lowHealthColor;
            lowHealthAnim.Play("low_health_indicator");
        }
        else if (stats.curHealth > lowHealthThreshold && lowHealth)
        {
            lowHealth = false;
            healthBar.color = normalHealthColor;
            lowHealthAnim.Play("idle_low_health_indicator");
        }
    }

    public void SetOxygenLevel(float _curOxygen)
	{
		OxygenBarFill.fillAmount = _curOxygen * 0.01f;
        if (OxygenBarFill.fillAmount <= lowOxygenThreshold)
        {
            oxygenFillAnimator.Play("low_oxygen_indicator");
            isPlayingIdleOxygenIdleAnim = false;
        }
        else if (OxygenBarFill.fillAmount > lowOxygenThreshold && !isPlayingIdleOxygenIdleAnim)
        {
            oxygenFillAnimator.Play("normal_oxygen_indicator");
            isPlayingIdleOxygenIdleAnim = true;
        }
	}
}
