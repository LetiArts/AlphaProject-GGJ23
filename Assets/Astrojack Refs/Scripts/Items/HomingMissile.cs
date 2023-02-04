using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : MonoBehaviour 
{
	private Transform target;
	private bool searchingForEnemy = false;
	private Rigidbody2D rb;


	// Use this for initialization
	public void ActivateMissile()
	{
		rb = GetComponent<Rigidbody2D>();

		if (GameObject.FindGameObjectWithTag(PlayerCompanion.instance.companionProperties.missileTarget) != null)
		{
			target = GameObject.FindGameObjectWithTag(PlayerCompanion.instance.companionProperties.missileTarget).transform;
			if (target == null) 
			{
				if (!searchingForEnemy)
				{
					searchingForEnemy = true;
					StartCoroutine (searchForEnemy());
				}
				return;
			}
		}
	}
	
	IEnumerator searchForEnemy()
	{
		GameObject sResult = GameObject.FindGameObjectWithTag(PlayerCompanion.instance.companionProperties.missileTarget);
		if (sResult == null) 
		{
			yield return new WaitForSeconds (0.5f);
			StartCoroutine (searchForEnemy ());
		}
		else
		{
			target = sResult.transform;
			searchingForEnemy = false;
			yield return false;
		}
	}

	void FixedUpdate () {
		if (target == null) 
		{
			searchingForEnemy = false;
				if (!searchingForEnemy) 
				{
					searchingForEnemy = true;
					StartCoroutine (searchForEnemy ());
				}
				return;
			}

		Vector2 direction = (Vector2)target.position - rb.position;

		direction.Normalize();

		float rotateAmount = Vector3.Cross(direction, transform.up).z;

		rb.angularVelocity = -rotateAmount * PlayerCompanion.instance.companionProperties.rotateSpeed;

		rb.velocity = transform.up * PlayerCompanion.instance.missileSpeed;
	}

	void OnCollisionEnter2D(Collision2D _colInfo)
	{
		Enemy enemy = _colInfo.collider.GetComponent<Enemy>();
		EnemyAI enemyAI = _colInfo.collider.GetComponent<EnemyAI>();

		if (enemy != null)
		{
			enemy.DamageEnemy (PlayerCompanion.instance.dealingDamage);
		}
		else if (enemyAI != null)
		{
			enemyAI.DamageEnemy (PlayerCompanion.instance.dealingDamage);
		}

		this.gameObject.GetComponent<Animator>().Play("Explode");
		ObjectPooler.SharedInstance.TakePooledObject(this.gameObject, 0.3f);
		// Put a particle effect here
	}
}
