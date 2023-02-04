using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    public static ItemDropper instance;
    public List<DropableItemStats> dropableItems;

    private void Awake() {
        instance = this;
    }

    public void DropItem(string name, Vector3 WhereToDrop)
    {
        bool isExist = false;
        string itemToFind = name.ToLower();
        //let's iterate through the available items to see if we have a coin
        foreach(var itemStats in dropableItems)
        {
            //we identify item
            if(itemStats.item.ToString().ToLower() == itemToFind)
            {
                isExist = true;
                //let's get object
                GameObject item = ObjectPooler.SharedInstance.GetPooledObject("Coin");
                item.transform.position = WhereToDrop;
                //let's set it active
                item.SetActive(true);
                //let's disable object 
                ObjectPooler.SharedInstance.TakePooledObject(item, itemStats.destroyTime);
            }
        }

        if(isExist == false)
        {
            Debug.LogError("Item Dropper: Make sure the name being accessed " + name + " is in dropable items list");
        }
    }
}
