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
    /// <summary>
    /// [PLACEHOLDER] Template prefab representing Obstacle objects.
    /// </summary>
    [SerializeField]
    private GameObject obstacleTemplate;

    /// <summary>
    /// [PLACEHOLDER] Template prefab representing Collectible objects.
    /// </summary>
    [SerializeField]
    private GameObject collectibleTemplate;

    /// <summary>
    /// Initial size of array holding Obstacle instances.
    /// </summary>
    [SerializeField]
    private int obstaclePoolSize;

    /// <summary>
    /// Initial size of array holding Collectible instances.
    /// </summary>
    [SerializeField]
    private int collectiblePoolSize;

    /// <summary>
    /// Holds reference to this Singleton class.
    /// </summary>
    private static ObjectPool instance;

    /// <summary>
    /// Array pooling Obstacle instances.
    /// </summary>
    private GameObject[] obstaclePool;

    /// <summary>
    /// Array pooling Collectible instances.
    /// </summary>
    private GameObject[] collectiblePool;

    /// <summary>
    /// Singleton instance of ObjectPool.
    /// </summary>
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

    /// <summary>
    /// Requests a GameObject of the specified type from the corresponding
    /// array.
    /// </summary>
    /// <param name="requestedObjectType">
    /// Type of GameObject requested.
    /// </param>
    /// <returns>
    /// The GameObject, if one is available, and null otherwise.
    /// </returns>
    public GameObject GetPooledObject(PooledObjects requestedObjectType)
    {
        GameObject returnObject = null;
        
        switch (requestedObjectType)
        {
            case PooledObjects.Obstacle:
                for (int i = 0; i < obstaclePoolSize; i++)
                    if (!obstaclePool[i].activeInHierarchy)
                        returnObject = obstaclePool[i];
                break;
            case PooledObjects.Collectible:
                for (int i = 0; i < collectiblePoolSize; i++)
                    if (!collectiblePool[i].activeInHierarchy)
                        returnObject = collectiblePool[i];
                break;
        }

        // Set the pooled object to active and visible in the
        // Scene before returning it to the caller.
        if (returnObject)
        {
            returnObject.SetActive(true);
            returnObject.GetComponent<Renderer>().enabled = true;
        }

        return returnObject;
    }
}
