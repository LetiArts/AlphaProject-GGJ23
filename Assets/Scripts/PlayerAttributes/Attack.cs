using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	public static Attack instance;

	public float dmgValue = 4;
	public float dashDmgValue = 20f;
	public GameObject throwableObject;
	public Transform attackCheck;
	public Transform abilityShotPoint;
	public float attackRange = 0.9f;
	public float nextAttackTime = 2f;
	public float abilityChargeTime = 0.5f;
	private Rigidbody2D m_Rigidbody2D;
	public Animator animator;
	public bool canAttack = true;
	//we check if we can recieve attack input
	public bool canRecieveInput = true;
	//checks if attack input has been recieved
	public bool InputRecieved;

	[Header("Camera Shake")]
	public float shakeAmt = 0.5f;
	public float shakeLenght = 1f;
	public float shakeFrequency = 0.5f;

	private void Awake()
	{
		instance = this;
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update()
    {

		if (Input.GetKeyDown(KeyCode.W))
		{
			CheckAttack();
		}

		// if (Input.GetKeyDown(KeyCode.E) && canAttack)
		// {
		// 	canAttack = false;
		
		// 	StartCoroutine(UseAbility());
		// 	StartCoroutine(AttackCooldown());
		// }
	}

	IEnumerator UseAbility()
	{
		animator.SetTrigger("PowerShot");
		
		yield return new WaitForSeconds(abilityChargeTime);
		GameObject throwableWeapon = Instantiate(throwableObject, abilityShotPoint.position, Quaternion.identity) as GameObject; 
		Vector2 direction = new Vector2(transform.localScale.x, 0);
		throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction; 
		throwableWeapon.name = "ThrowableWeapon";
	}

	IEnumerator AttackCooldown()
	{
		yield return new WaitForSeconds(nextAttackTime);
		canAttack = true;
	}

	//we check if we can attack and stop player movement in IdleBehavior script
	public void CheckAttack()
	{
		if (canRecieveInput)
		{
			//attack input recieved
			InputRecieved = true;
			//do attack damage
			// DoAttack();
			//let's tell the system we cannot recieve further input
			canRecieveInput = false;
		}else{
			return;
		}
	}

	public void InputMgr()
	{
		if(!canRecieveInput)
		{
			canRecieveInput = true;
		}else{
			canRecieveInput = false;
		}
	}

	void DoAttack()
	{
		dmgValue = Mathf.Abs(dmgValue);
		Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackRange);

		for (int i = 0; i < collidersEnemies.Length; i++)
		{
			if (collidersEnemies[i].gameObject.tag == "Enemy")
			{
				Enemy enemy = collidersEnemies[i].gameObject.GetComponent<Enemy>();
				EnemyAI enemyAI = collidersEnemies[i].gameObject.GetComponent<EnemyAI>();
				if (enemy != null)
				{
					CameraShake.instance.Shake(shakeAmt, shakeLenght, shakeFrequency);
					enemy.DamageEnemy (dmgValue);
				}

				if (enemyAI != null)
				{
					enemyAI.DamageEnemy (dmgValue);
				}
			}
		}
	}

	public void DoDashAttack()
	{
		dashDmgValue = Mathf.Abs(dashDmgValue);
		Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(transform.position, 4);

		for (int i = 0; i < collidersEnemies.Length; i++)
		{
			if (collidersEnemies[i].gameObject.tag == "Enemy")
			{
				Enemy enemy = collidersEnemies[i].gameObject.GetComponent<Enemy>();
				EnemyAI enemyAI = collidersEnemies[i].gameObject.GetComponent<EnemyAI>();
				if (enemy != null)
				{
					CameraShake.instance.Shake(shakeAmt, shakeLenght, shakeFrequency);
					enemy.DamageEnemy (dashDmgValue);
					Debug.Log($"Dashed Enemy and did {dashDmgValue} damage");
				}

				if (enemyAI != null)
				{
					enemyAI.DamageEnemy (dashDmgValue);
				}
			}
		}
	}


	private void OnDrawGizmosSelected() {
		if (attackCheck == null)
			return;

        Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackCheck.position, attackRange);
	}
}
