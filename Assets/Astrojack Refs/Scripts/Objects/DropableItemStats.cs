using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu]
public class DropableItemStats : ScriptableObject
{
	public ItemTypes item;
    [Range(0, 15)]
    public float destroyTime = 2f;
	[Range(0, 100)]
	public int coinValue;
	[Range(0, 100)]
	public int gemValue;
	public int healthRestore;
	public bool makePlayerInvincible = false;
	public string pickupSoundName = "";


	public enum ItemTypes
	{
		coin,
		pill,
		gem,
		invincibility
	}
}
