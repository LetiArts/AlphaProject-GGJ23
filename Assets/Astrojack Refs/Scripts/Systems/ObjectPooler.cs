using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]

public class ObjectPoolItem {
  public GameObject objectToPool;
  public Transform Container;
  public int amountToPool;
  public bool shouldExpand;
}

public class ObjectPooler : MonoBehaviour 
{

  public static ObjectPooler SharedInstance;
  public List<ObjectPoolItem> itemsToPool;
  public List<GameObject> pooledObjects;

  void Awake() {
    SharedInstance = this;
  }

  // Use this for initialization
  void Start () {
    pooledObjects = new List<GameObject>();
    foreach (ObjectPoolItem item in itemsToPool) {
      for (int i = 0; i < item.amountToPool; i++) {
        GameObject obj = (GameObject)Instantiate(item.objectToPool, item.Container);
        obj.SetActive(false);
        pooledObjects.Add(obj);
      }
    }
  }

  //call this in another script to get an object from pool
  //so instead of using GameObject = instantiate(Gameobject), use GameObject = ObjectPooler.SharedInstance.GetPooledObject("tag of Object") as GameObject
  public GameObject GetPooledObject(string tag) {
    for (int i = 0; i < pooledObjects.Count; i++) {
      if (!pooledObjects[i].activeSelf && pooledObjects[i].tag == tag) 
      {
        return pooledObjects[i];
      }
    }
    foreach (ObjectPoolItem item in itemsToPool) {
      if (item.objectToPool.tag == tag) {
        if (item.shouldExpand) {
          GameObject obj = (GameObject)Instantiate(item.objectToPool, item.Container);
          obj.SetActive(false);
          pooledObjects.Add(obj);
          return obj;
        }
      }else{
        Debug.LogWarning("Could not get object with tag:" + tag);;
      }
    }
    return null;
  }

  //Call this when you want to destroy object 
  //so instead of calling Destroy(gameObject, time), call ObjectPooler.SharedInstance.TakePooledObject(gameObject, time)
  public void TakePooledObject(GameObject obj, float time) {
    StartCoroutine(DeactivateObj(obj, time));    
  }

  IEnumerator DeactivateObj(GameObject gameObject, float time)
  {
    if (gameObject != null && gameObject.activeSelf)
    {
      yield return new WaitForSeconds(time);
      gameObject.SetActive(false);
    }
  }

  // Update is called once per frame
  void Update () {

  }
}
