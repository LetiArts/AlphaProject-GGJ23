using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;
    public bool isProduction = false;

	private int _curGem;
    //adding to _curGem from droppableItem
    public int curGem {
        get { return _curGem; }
        set { 
            _curGem = value;
            SetGem();
        }
    }
    
    private int _curMoney;
    //adding to _curMoney from droppableItem
    public int curMoney {
        get { return _curMoney; }
        set { 
            _curMoney = value;
            SetMoney();
        }
    }

    private void Awake() {
        instance = this;
        if (!isProduction)
        {
            curMoney += 900;
        }
    }

    public void SetGem()
    {
        //i'm not using gem now
        // GemText.text = _curGem.ToString();
    }

    public void SetMoney()
    {
        // if (UIController.instance != null)
        // {
        //     UIController.instance.MoneyText.text = _curMoney.ToString();
        // }
    }
}
