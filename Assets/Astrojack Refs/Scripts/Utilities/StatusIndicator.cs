using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour {

	public static StatusIndicator instance;

	public Image healthBarFill;
	bool isSetHealth = false;

	private void Awake() {
		instance = this;
	}


	public void SetHealth(float _cur, float _max, bool isDamaging)
	{
		StartCoroutine (DisplayHealth(_cur, _max, isDamaging));
	}

	IEnumerator DisplayHealth(float _cur, float _max, bool isDamaging)
	{
		//let's cache _cur and _max for manipulation
		float target = _cur;
		float maxReach = _max;
		float cacheHealth = 0;
		bool isInflicted = false;

		//let's loop until health is set
		while (!isSetHealth)
		{
			//if we're damaging player
			if (isDamaging)
			{
				//let's reduce player health
				maxReach--;
				isInflicted = true;
			}else{
				//let's refill health
				maxReach++;
			}

			//let's recheck if we're damaging player or not
			if(isDamaging)
			{
				if (maxReach <= target)
				{
					//let's end the loop
					isSetHealth = true;
				}
			}
			//if we are refilling health 
			else{
				if (maxReach >= target)
				{
					//let's end the loop
					// Debug.LogError($"target {target} is equal to {maxReach}");
					isSetHealth = true;
				}
			}

			cacheHealth = maxReach*.01f;

			// Debug.Log("we set image fill to " + cacheHealth*.1f);
			//let's set health bar
			if(healthBarFill != null)
			{
				while (isDamaging && cacheHealth < healthBarFill.fillAmount)
				{
					healthBarFill.fillAmount -= 2f * Time.deltaTime;
					yield return null;
				}

				if(Player.instance.canRegenerate && Player.instance.isRegenerating && !isInflicted)
				{
					healthBarFill.fillAmount = cacheHealth;
				}
				else if (!isDamaging){
					healthBarFill.fillAmount = cacheHealth;
				}
			}
			
			yield return null;
		}

		isSetHealth = false;
	}
}
