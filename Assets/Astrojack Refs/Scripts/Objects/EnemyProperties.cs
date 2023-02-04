using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class EnemyProperties : ScriptableObject
{
    [Range(0, 500)]
	public int maxHealth = 100;
    [Range(0, 100)]
	public int dealingDamage = 40;
    [Range(0, 1)]
	public float timeToDecideNextMove = 0.5f;
    public float moveSpeed = 5f;
	public float m_DashForce = 25f;
    public string target;
    
    [Header("For close combat enemies")]
    public float meleeDist = 1.5f;
    [Header("For explosive enemies")]
    public float explodeDist = 1.5f;
    public int explosionDelay = 10;
    [Header("For ranged enemies")]
    public float rangeDist = 5f;

    [Header("Camera Shake")]
	public float shakeAmt = 0.1f;
	public float shakeLength = 0.1f;

    [Header("For Range enemies")]
    public bool canShoot = false;

    [Header("For Exploding enemies")]
    public bool movingMine = false;
        
    [Header("For enemies that jump")]
    public bool canJump = false;

    [Header("For enemies that dash")]
	public bool canDash = false;
    [HideInInspector] public bool airControl = false;

	public string deathSoundName = "Explosion";
}
