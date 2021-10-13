using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PooledObjects
{
    Obstacle,
    Collectible
}

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject obstacleTemplate;
    [SerializeField]
    private GameObject collectibleTemplate;
    [SerializeField]
    private int obstaclePoolSize;
    [SerializeField]
    private int collectiblePoolSize;
    private static ObjectPool instance;
    private GameObject[] obstaclePool;
    private GameObject[] collectiblePool;

    public static ObjectPool Instance
    {
        get
        {
            // FindObjectOfType returns the first active loaded object of a
            // specified type. It is used here to get the first ObjectPool
            // instance in the scene.
            if (!instance)
                instance = FindObjectOfType(typeof(ObjectPool)) as ObjectPool;

            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialise obstacle and collectible pools with the appropriate
        // number of instances.
        obstaclePool = new GameObject[obstaclePoolSize];
        collectiblePool = new GameObject[collectiblePoolSize];
        GameObject temp;

        for (int i = 0; i < obstaclePoolSize; i++)
        {
            temp = Instantiate(obstacleTemplate);
            temp.SetActive(false);
            obstaclePool[i] = temp;
        }

        for (int i = 0; i < collectiblePoolSize; i++)
        {
            temp = Instantiate(collectibleTemplate);
            temp.SetActive(false);
            collectiblePool[i] = temp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetPooledObject(PooledObjects pooledObject)
    {
        switch (pooledObject)
        {
            case PooledObjects.Obstacle:
                for (int i = 0; i < obstaclePoolSize; i++)
                    if (!obstaclePool[i].activeInHierarchy) return obstaclePool[i];
                break;
            case PooledObjects.Collectible:
                for (int i = 0; i < collectiblePoolSize; i++)
                    if (!collectiblePool[i].activeInHierarchy) return collectiblePool[i];
                break;
        }

        return null;
    }
}
