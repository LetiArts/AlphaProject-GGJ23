using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour 
{
	public static CameraShake instance;

	public Camera mainCam;

	void Awake()
	{
		instance = this;
		
		if (mainCam == null)
			mainCam = Camera.main;
	}

	public void Shake(float intensity, float duration, float frequency, float smoothTime = 0.5f)
	{
		StartCoroutine(ShakeCoroutine(intensity, duration, frequency, smoothTime));
	}

	IEnumerator ShakeCoroutine(float intensity, float duration, float frequency, float smoothTime)
	{
		Vector3 originalPos = mainCam.transform.localPosition;
		float elapsed = 0.0f;
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;          

			float x = originalPos.x + Random.Range(-1f, 1f) * intensity;
			float y = originalPos.y + Random.Range(-1f, 1f) * intensity;

			Vector3 newPos = new Vector3(x, y, originalPos.z);
			mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, newPos, 1 - Mathf.Exp(-smoothTime * Time.deltaTime));

			intensity = Mathf.Lerp(intensity, 0, elapsed / duration);
			frequency = Mathf.Lerp(frequency, 0, elapsed / duration);
			yield return new WaitForSeconds(1 / frequency);
		}
		mainCam.transform.localPosition = originalPos;
	}
}
