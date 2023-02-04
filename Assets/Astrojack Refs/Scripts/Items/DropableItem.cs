using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropableItem : MonoBehaviour
{
    // public Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
    // [SerializeField] private LayerMask _groundLayer;
    // [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    // bool isFrozen;
    private new Rigidbody2D rigidbody;
    public DropableItemStats itemStats;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player")
        {
            if(itemStats.coinValue > 0)
            {
                this.gameObject.SetActive(false);
                ResourceManager.instance.curMoney += itemStats.coinValue;
            }
            else if (itemStats.healthRestore > 0)
            {
                this.gameObject.SetActive(false);
                Player.instance.RestoreHealth(itemStats.healthRestore);
            }else if (itemStats.gemValue > 0)
            {
                this.gameObject.SetActive(false);
                ResourceManager.instance.curGem += itemStats.gemValue;
            }
            //TODO
            //Invincibilty etc
        }
    }
}
