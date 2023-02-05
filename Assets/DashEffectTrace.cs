using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffectTrace : MonoBehaviour
{
    public SpriteRenderer currentSprite;
    public GameObject instantiateObject;
    public GameObject[] traceObject;

    public float spacing;
    public float smoothSpeed;
    public Transform targetDirection;
    private SpriteRenderer[] allSprite;
    private Vector3[] refVelocity;
    // Start is called before the first frame update
    void Start()
    {
        refVelocity = new Vector3[traceObject.Length];
        //StartCoroutine(TraceEffect());
        //instantiateObject = GameObject.FindGameObjectWithTag("TraceObject");

        for (int i=0; i < traceObject.Length; i++)
        {
           
           traceObject[i] = Instantiate(instantiateObject, transform.position, Quaternion.identity);
           SpriteRenderer singleSprite = traceObject[i].GetComponent<SpriteRenderer>();
           // allSprite[i] = singleSprite;
           singleSprite.sprite = currentSprite.sprite;
            singleSprite.sortingLayerName = "Player";                
             
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        FlipDirection();
        traceObject[0].transform.position = transform.position;
        for (int i = 1; i < traceObject.Length; i++)
        {
            //traceObject[i + 1].transform.position = (Vector3.left * spacing) + traceObject[i].transform.position;
            traceObject[i].transform.position = Vector3.SmoothDamp(traceObject[i].transform.position , traceObject[i-1].transform.position, ref refVelocity[i], smoothSpeed);

            
        }
        
    }

    IEnumerator TraceEffect()
    {
        traceObject[0].transform.position = (Vector3.left * spacing) + transform.position;
        for (int i=0; i < traceObject.Length; i++)
        {
            traceObject[i+1].transform.position = (Vector3.left * spacing) + traceObject[i].transform.position;

        }
        yield return null;
    }
    void FlipDirection()
    {
        float direction = Input.GetAxisRaw("Horizontal");
        if (direction<0)
        {
            for(int i= 0; i< traceObject.Length; i++)
            {
                traceObject[i].transform.localScale = new Vector3(-2.2f, 2.2f, 2.2f);
            }
        }
        else if(direction > 0)
        {
            for (int i = 0; i < traceObject.Length; i++)
            {
                traceObject[i].transform.localScale = new Vector3(2.2f,2.2f,2.2f);
            }
        }

    }
}
