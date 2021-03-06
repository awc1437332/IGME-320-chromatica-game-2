using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    public bool isActive = false;
    public bool resetting = false;

    [SerializeField]
    float tileSpeed = 10.0f;
    public int tileOrder = 0;

    // Track ResetTile() calls in TileManager by invoking an event to avoid
    // iterating through an array.
    public static UnityEvent tileReset = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        //Sets the starting position for this tile
        transform.position = new Vector3(0f, 0f, 20.0f * tileOrder);

        //Starts the tile as active for debugging purposes
        //isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Tiles only move if the game allows them to be active
        if (isActive)
        {
            //When the tile passes behind the camera
            if (transform.position.z <= -20.0f)
            {
                //Move the tile back
                ResetTile();
            }

            MoveTile();
        }
    }

    //Moves the tile towards the camera
    void MoveTile()
    {
        transform.Translate(0f, 0f, -tileSpeed * Time.deltaTime);
    }

    //Translates the tile back to the beginning of the loop and determines its
    //new mesh. Also invokes the TileReset event and marks this Tile as 
    // "resetting" so the TileManager can spawn Obstacles on it appropriately.
    void ResetTile()
    {
        //Generate a new tile with the manager that will call ChangeMesh()

        transform.Translate(0f, 0f, 80.0f);
        resetting = true;
        tileReset.Invoke();
    }

    //CALLED BY MANAGER CLASS
    //Changes the mesh of the object to the specified index
    public void ChangeMesh(int index)
    {
        this.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("Tile " + index);
    }
}
