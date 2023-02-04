using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class PlayerCompanionStats : ScriptableObject
{
    [Range(0, 200)]
	public int maxHealth = 100;
    [Range(0, 100)]
	public int dealingDamage = 20;
    public float moveSpeed = 300f;
    public string target;

    [Header("Missile Stats")]
	public float fireRate = 0; //seconds between missile
	public float missileSpeed = 5f;
    public float rotateSpeed = 250f;
    public string missileTarget;


    [Header("Camera Shake")]
	public float shakeAmt = 0.1f;
	public float shakeLength = 0.1f;

	public string deathSoundName = "Explosion";
}
