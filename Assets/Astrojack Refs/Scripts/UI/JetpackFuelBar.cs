using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JetpackFuelBar : MonoBehaviour
{

	public Image JetpackBar;

	public void UpdateFuelBar(float amount)
	{
		JetpackBar.fillAmount = amount;
	}

}
