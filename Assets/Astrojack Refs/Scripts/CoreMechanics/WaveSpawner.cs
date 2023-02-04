using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour {
	public static WaveSpawner instance;

	public enum SpawnState { SPAWNING, WAITING, COUNTING };

	[System.Serializable]
	public class Wave
	{
		public string name;
		public Transform enemy;
		public int count;
		public float rate;
	}

	// public Wave[] waves;
	public EnemyWaves enemyWaves;
	private int nextWave = 0;
	public int NextWave
	{
		get { return nextWave + 1; }
	}
	public int currentWave = 1;


	public Transform[] randomSpawnPoints;
	public Transform[] groundLevelSpawnPoints;
	public WaveUI waveUI;

	public float timeBetweenWaves = 5f;
	private float waveCountdown;
	public float WaveCountdown
	{
		get { return waveCountdown; }
	}

	private float searchCountdown = 1f;

	private SpawnState state = SpawnState.COUNTING;
	public SpawnState State
	{
		get { return state; }
	}


	[Header("Dropbox logic")]
	public Transform[] DropSpawnPositions;
	[HideInInspector]
	public GameObject itemBeingDropped;
	bool spawnedDrone = false;

	private void Awake() {
		instance = this;
	}

	void Start()
	{
		if (randomSpawnPoints.Length == 0)
		{
			Debug.LogError("No spawn points referenced.");
		}

		waveCountdown = timeBetweenWaves;
	}

	void Update()
	{
		if (state == SpawnState.WAITING)
		{
			if (!EnemyIsAlive())
			{
				WaveCompleted();
			}
			else
			{
				return;
			}
		}

		if (waveCountdown <= 0)
		{
			if (state != SpawnState.SPAWNING)
			{
				// ShopController.instance.shopButton.SetActive(false);

				if(spawnedDrone)
				{
					PlayerCompanion.instance.Shoot();
					PlayerCompanion.instance.canShoot = true;
					PlayerCompanion.instance.waveCompleted = false;
				}

				StartCoroutine( SpawnWave ( enemyWaves.Waves[nextWave] ) );
			}
		}
		else
		{
			waveCountdown -= Time.deltaTime;
		}
	}

	void WaveCompleted()
	{
		Debug.Log("Wave Completed!");
		// ShopController.instance.shopButton.SetActive(true);

		PlayerCompanion.instance.canShoot = false;
		PlayerCompanion.instance.waveCompleted = true;

		state = SpawnState.COUNTING;
		waveCountdown = timeBetweenWaves;

		if (nextWave + 1 > enemyWaves.Waves.Count - 1)
		{
			nextWave = 0;
			Debug.Log("ALL WAVES COMPLETE! Looping...");
		}
		else
		{
			nextWave++;
			//just for shop purposes.. don't rely on this
			currentWave++;
		}

	}

	bool EnemyIsAlive()
	{
		searchCountdown -= Time.deltaTime;
		if (searchCountdown <= 0f)
		{
			searchCountdown = 1f;
			if (GameObject.FindGameObjectWithTag("Enemy") == null)
			{
				return false;
			}
		}
		return true;
	}

	IEnumerator SpawnWave(EnemyWaves.Wave _wave)
	{
		bool isActivatedBoss = false;
		//let's get the total number of types
		int enemyTypes = _wave.enemyGroups.Count;
		
		Debug.Log("Spawning Wave: " + _wave.name);

		//let's change our state to spawning 
		state = SpawnState.SPAWNING;
		//let's loop through given enemy types
		for (int i = 0; i < enemyTypes; i++)
		{
			//if we have more than one group of enemy
			//let's loop through the group and get number of enemies in group
			for (int j = 0; j < _wave.enemyGroups[i].count; j++)
			{
				//if we have a boss
				if (_wave.enemyGroups[i].isBossEnemy == true && !isActivatedBoss)
				{
					waveUI.UpdateBossUI();
					// Debug.LogError($"Wave has boss");
					yield return new WaitForSeconds(1f);
					isActivatedBoss = true;
				}
				//spawn enemies/enemy from group - i 
				if (_wave.enemyGroups[i].isBossEnemy == false && _wave.enemyGroups[i].isGroundTreeEnemy == false)
				{
					SpawnEnemy(_wave.enemyGroups[i].enemy, true); // spawn at random position
				}
				else {
					SpawnEnemy(_wave.enemyGroups[i].enemy, false); //spawn at boss points
				}
				//time between each enemy in a group
				yield return new WaitForSeconds( 1f/_wave.enemyGroups[i].spawnrate );
			}
			//time between the groups of enemies
			yield return new WaitForSeconds( 1f/_wave.timeBetweenGroups );
		}

		state = SpawnState.WAITING;

		yield break;
	}

	void SpawnEnemy(GameObject _enemy, bool randomSpot)
	{
		// Debug.Log("Spawning Enemy: " + _enemy.name);
		Transform _sp = null;
		if (randomSpot)
		{
			_sp = randomSpawnPoints[Random.Range (0, randomSpawnPoints.Length)];
		}else{
			_sp = groundLevelSpawnPoints[Random.Range (0, groundLevelSpawnPoints.Length)];
		}

		Instantiate(_enemy, _sp.position, _sp.rotation);
	}

	public void SpawnDrone()
	{
		GameObject drone = ObjectPooler.SharedInstance.GetPooledObject("Companion");
		drone.transform.position = DropSpawnPositions[0].position;
		drone.SetActive(true);
		spawnedDrone = true;
	}
}
