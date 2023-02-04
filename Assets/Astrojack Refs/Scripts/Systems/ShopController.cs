using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopController : MonoBehaviour
{
    public static ShopController instance;

    public GameObject shopMenu, shopButton, closeButton; //public declarations
    public Transform itemsContainer; //the container to keep all the shop items
    public GameObject itemButton; //prefab of item to be bought

    public ShopItem[] shopItems;
    
    // bool openedShop = false;

    private void Awake() {
        instance = this;
    }
    
    private void Start() {
        shopButton.GetComponent<Button>().onClick.AddListener(OpenShop);
        closeButton.GetComponent<Button>().onClick.AddListener(Close);
        // shopButton.SetActive(false);
    }

    void Close ()
    {
        //if the menu is already open, let's close it
        if (shopMenu.activeSelf)
        {
            shopMenu.SetActive(false);
            Time.timeScale = 1;
        }
        foreach (Transform obj in itemsContainer)
        {
            ObjectPooler.SharedInstance.TakePooledObject(obj.gameObject, 0.5f);
        }
    }

    public void OpenShop()
    {
        foreach(var obj in shopItems)
        {
            //create a new button
            GameObject shopItem = ObjectPooler.SharedInstance.GetPooledObject("ShopBtn");

            //get component from button
            ShopItemButton itemRef = shopItem.GetComponent<ShopItemButton>(); //get reference to the button
            
            //let's set appropriate items
            itemRef.itemName.text = obj.itemName;
            itemRef.itemImage.sprite = obj.itemImage;
            itemRef.itemPrice.text = obj.ItemLevelDetails[itemRef.curLevel].itemPrice.ToString();
            itemRef.waveTarget = obj.ItemLevelDetails[itemRef.curLevel].waveTarget;
            itemRef.itemDesc.text = obj.ItemLevelDetails[itemRef.curLevel].description;

            //if this item isn't ready for this wave let's deactivate it
            if (WaveSpawner.instance.currentWave >= itemRef.waveTarget)
            {
                shopItem.GetComponent<Button>().interactable = true;
            }else{
                shopItem.GetComponent<Button>().interactable = false;
                itemRef.itemPrice.text = "Unlock at wave " + itemRef.waveTarget;
            }
            Debug.Log(shopItem.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text);

            //set shop item active because we are pooling it 
            if (!shopItem.activeSelf)
            {
                shopItem.SetActive(true);
            }
        } 

        //let's set shop active
        shopMenu.SetActive (true);

        //freeze time
        Time.timeScale = 0;
    }

    //let's update the properties of a bought item
    public void UpdateItemInfo(ShopItemButton itemToUpdate)
    {
        foreach(var item in shopItems)
        {
            if (item.itemName == itemToUpdate.itemName.text)
            {
                //let's set appropriate items
                itemToUpdate.itemPrice.text = item.ItemLevelDetails[itemToUpdate.curLevel].itemPrice.ToString();
                itemToUpdate.itemDesc.text = item.ItemLevelDetails[itemToUpdate.curLevel].description;
                itemToUpdate.waveTarget = item.ItemLevelDetails[itemToUpdate.curLevel].waveTarget;

                //if this item isn't ready for this wave let's deactivate it
                if (WaveSpawner.instance.currentWave >= itemToUpdate.waveTarget)
                {
                    itemToUpdate.GetComponent<Button>().interactable = true;
                }else{
                    itemToUpdate.GetComponent<Button>().interactable = false;
                    itemToUpdate.itemPrice.text = "Unlock at wave " + itemToUpdate.waveTarget;
                }
            }
        }
    }

    public void CloseShop()
    {
        shopMenu.SetActive (false);
    }

}
