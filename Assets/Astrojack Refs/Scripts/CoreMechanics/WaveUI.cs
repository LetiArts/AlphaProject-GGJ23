using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveUI : MonoBehaviour {

	[SerializeField]
	WaveSpawner spawner;

	[SerializeField]
	Animator waveAnimator;

	[SerializeField]
	TMP_Text waveCountdownText;

	[SerializeField]
	TMP_Text waveCountText;
	
	[SerializeField]
	TMP_Text curWaveText;
	public GameObject curWaveHeader;
	bool hasSetHeaderActive = false;
	private WaveSpawner.SpawnState previousState;

	private void Awake() {
		if (WaveSpawner.instance != null)
		{
			spawner = WaveSpawner.instance;
		}
	}

	// Use this for initialization
	void Start () {
		curWaveHeader.SetActive(false);

		if (spawner == null)
		{
			Debug.LogError("No spawner referenced!");
			this.enabled = false;
		}
		if (waveAnimator == null)
		{
			Debug.LogError("No waveAnimator referenced!");
			this.enabled = false;
		}
		if (waveCountdownText == null)
		{
			Debug.LogError("No waveCountdownText referenced!");
			this.enabled = false;
		}
		if (waveCountText == null)
		{
			Debug.LogError("No waveCountText referenced!");
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch (spawner.State)
		{
			case WaveSpawner.SpawnState.COUNTING:
				UpdateCountingUI();
				break;
			case WaveSpawner.SpawnState.SPAWNING:
				UpdateSpawningUI();
				break;
        }

		previousState = spawner.State;
	}

	void UpdateCountingUI ()
	{
		if (previousState != WaveSpawner.SpawnState.COUNTING)
		{
			waveAnimator.SetBool("WaveBoss", false);
			waveAnimator.SetBool("WaveIncoming", false);
			waveAnimator.SetBool("WaveCountdown", true);
			//Debug.Log("COUNTING");
		}
		waveCountdownText.text = ((int)spawner.WaveCountdown).ToString();
	}

	void UpdateSpawningUI()
	{
		if (previousState != WaveSpawner.SpawnState.SPAWNING)
		{
			// waveAnimator.SetBool("WaveBoss", false);
			waveAnimator.SetBool("WaveCountdown", false);
			waveAnimator.SetBool("WaveIncoming", true);

			waveCountText.text = "Wave <color=red>" + spawner.NextWave.ToString() + "</color>";
			curWaveText.text = "Wave " + spawner.NextWave.ToString();

			if (!hasSetHeaderActive)
			{
				StartCoroutine (SetHeaderActive());
			}
		}
	}

	IEnumerator SetHeaderActive()
	{
		yield return new WaitForSeconds(2f);
		curWaveHeader.SetActive(true);
		hasSetHeaderActive = true;
		yield break;
	}

	public void UpdateBossUI()
	{
		waveAnimator.SetBool("WaveBoss", true);
	}
}
