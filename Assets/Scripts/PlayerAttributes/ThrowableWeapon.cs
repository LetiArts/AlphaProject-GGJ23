using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableWeapon : MonoBehaviour
{
	public Vector2 direction;
	public bool hasHit = false;
	PlayerData playerData;

    // Update is called once per frame
    void FixedUpdate()
    {
		if ( !hasHit)
		GetComponent<Rigidbody2D>().velocity = direction * playerData.throwableSpeed;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Enemy")
		{
			collision.gameObject.SendMessage("ApplyDamage", Mathf.Sign(direction.x) * 2f);
			hasHit = true;
		}
		Destroy(gameObject);
	}
}
