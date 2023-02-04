using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public List<ItemDetails> ItemLevelDetails;

	public string pickupSoundName = "";
}

[System.Serializable]
public class ItemDetails
{
    public string levelName;
	public int itemPrice;
    public int waveTarget;
    public string description;
}
