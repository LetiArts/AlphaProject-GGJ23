using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemButton : MonoBehaviour
{
    public static ShopItemButton instance;

    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemDesc;
    public TMP_Text itemPrice;
    public List<GameObject> starIndicators;
    public int waveTarget;

    [HideInInspector] public int curLevel = 0;

    private void Awake() {
        instance = this;
        this.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        UpdateStars();
    }


    public void OnButtonClick()
    {
        //logic for what happens when we click button
        foreach (var item in ShopController.instance.shopItems)
        {
            if (item.itemName == itemName.text)
            {
                if (ResourceManager.instance.curMoney > item.ItemLevelDetails[curLevel].itemPrice)
                {
                    curLevel++;
                    //logic
                    OnPurchase(item.itemName);
                    //update details of this item in the shop
                    ShopController.instance.UpdateItemInfo(this);
                    //subtract money
                    ResourceManager.instance.curMoney -= item.ItemLevelDetails[curLevel].itemPrice;
                    //indicate by star
                    UpdateStars();
                }else{
                    Debug.LogWarning("You don't have enough money");
                }
            }
        }
    }

    //what happens when we purchase an item
    void OnPurchase(string name)
    {
        if (name == "Jetpack")
        {
            if (curLevel == 1)
            {
                //tell character controller to switch to jetpack
                CharacterController2D.instance.switchedToJetpack = true; 
                //set the jetpack indicator activator
            }
            else if (curLevel == 2)
            {
                //def fill rate is 0.1 so 100% = 1
                CharacterController2D.instance.fillupRate += 0.1f;
            }
            else{
                //def fill rate is 0.1 so 100% = 1
                CharacterController2D.instance.fillupRate += 0.1f;
                //def maxJetpack time is 1 so 100% = 10
                CharacterController2D.instance.maxJetpackTime += 1f;
            }
        }
        else if (name == "Vitality")
        {
            if (curLevel == 1)
            {
                //tell player to regen
                Player.instance.canRegenerate = true;
            }
            else if (curLevel == 2)
            {
                Player.instance.healthRegenInterval -= 0.20f;
            }
            else{
                Player.instance.healthRegenInterval -= 0.5f;
                Player.instance.healthRegenRate += 0.5f;
            }
        }
        else if (name == "Shredder")
        {
            if (curLevel == 1)
            {
                Weapon.instance.Damage += 5;
            }
            else 
            {
                Weapon.instance.Damage += 5;
                Weapon.instance.maxAmmo += 10; 
            }      
        }
        else if (name == "Drone")
        {
            if (curLevel == 1)
            {
                WaveSpawner.instance.SpawnDrone();
            }
            else{
                PlayerCompanion.instance.moveSpeed += 50f;
                PlayerCompanion.instance.dealingDamage += 10;
            }        
        }
    }

    void UpdateStars()
    {
        for (int i = 0; i < curLevel; i++)
        {
            starIndicators[i].SetActive(true);
        }
    }
}
