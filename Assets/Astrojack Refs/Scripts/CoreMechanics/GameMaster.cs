using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {

	public static GameMaster instance; 

	[SerializeField] 
	private int maxLives = 3;
	private static int _remainingLives;
	public static int RemainingLives
	{
		get { return _remainingLives; }
	}

	[SerializeField]
	private int startingMoney;
	public int Money;
	public ParticleSystem toxicFumesFx;

	void Awake () {
		instance = this;

		if (toxicFumesFx != null)
		{
			toxicFumesFx.Play();
		}
	}


	public Transform playerPrefab;
	public Transform spawnPoint;  
	public float spawnDelay = 3;
	public Transform ParticleSpawnPrefab; 
	public string respawnCountdownSound = "RespawnCountdown";
	public string spawnSound = "Spawn";

	[SerializeField]
	private GameObject gameOverUI;

	//cache
	private SoundManager audioManager;

	void Start()
	{
		ScoreMaster.instance.ResetScore ();
		_remainingLives = maxLives;

		Money = startingMoney;

		//caching
		audioManager = SoundManager.instance;
		if (audioManager == null)
		{
			audioManager.PlayDefualtBG();
		}
	}

	public void EndGame ()
	{
		Debug.Log("GAME OVER");
		gameOverUI.SetActive(true);
	}

	public IEnumerator _RespawnPlayer () {
		audioManager.PlaySFX (respawnCountdownSound);
		yield return new WaitForSeconds (spawnDelay);

		audioManager.PlaySFX (spawnSound);
		Instantiate (playerPrefab, spawnPoint.position, spawnPoint.rotation);
		Instantiate (ParticleSpawnPrefab, spawnPoint.position, spawnPoint.rotation); 
	}
}