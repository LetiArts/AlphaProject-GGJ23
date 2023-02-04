using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jetpack : MonoBehaviour
{

	private Rigidbody2D rb;

	[SerializeField]
	private Vector2 force = new Vector2(0f, 25f);
	[Tooltip("Max jetpack force")]
	[SerializeField]
	private float maxVerticalSpeed = 3f;
	[Tooltip("Fuel in seconds")]
	[SerializeField]
	private float maxJetpackTime = 1.5f;
	private float jetpackTimeCounter = 0f;

	public JetpackFuelBar JetpackBar;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Z))
		{
			if (jetpackTimeCounter < maxJetpackTime)
			{
				jetpackTimeCounter += Time.deltaTime;
				if (rb.velocity.y < maxVerticalSpeed)
					rb.velocity += force * Time.deltaTime;
				else
					rb.velocity = new Vector2(0f, maxVerticalSpeed);
			}
		}
		else
		{
			if (jetpackTimeCounter > 0f)
			{
				jetpackTimeCounter -= Time.deltaTime;
			}
			else
				jetpackTimeCounter = 0f;
		}

		if (JetpackBar)
			JetpackBar.UpdateFuelBar(1f - jetpackTimeCounter / maxJetpackTime);

	}

}
