using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]

public class ObjectPoolItem
{
    public GameObject objectToPool;
    public GameObject parent;
    public int amountToPool;
    public bool shouldExpand;
}

public class ObjectPooler : MonoBehaviour
{

    public static ObjectPooler SharedInstance;

    public List<ObjectPoolItem> itemsToPool;

    private List<GameObject> pooledObjects;

    void Awake()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.transform.parent = item.parent.transform;
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
        
    }

    // Use this for initialization
    void Start()
    {
        SharedInstance = this;
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                if (item.shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }

    public GameObject GetPooledObject(int index)
    {
        return pooledObjects[index];
    }

    public int GetSize()
    {
        return itemsToPool.Count;
    }

    public int GetTotalPoolSize()
    {
        return pooledObjects.Count;
    }

    public int GetItemPoolSize(string tag)
    {
        int count = 0;
        foreach(GameObject obj in pooledObjects)
        {
            if (obj.tag.Equals(tag))
            {
                count++;
            }
        }
        return count;
    }

    

}
