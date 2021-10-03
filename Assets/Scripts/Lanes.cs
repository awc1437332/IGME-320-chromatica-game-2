using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lanes : MonoBehaviour
{
    // ***Lanes script to be attached to a Tile prefab***

    const int laneCount = 3;
    public GameObject[] lanes = new GameObject[laneCount];

    private void Start()
    {
        // Set Obstacles to be cleared when the TileReset event is invoked.
        Tile.TileReset.AddListener(ClearObstacles);
    }

    /// <summary>
    /// Loops through and clears all obstacles on the Tile this script is 
    /// attached to. Called when a Tile's position is reset.
    /// </summary>
    public void ClearObstacles()
    {
        foreach (GameObject obstacle in lanes)
            if (obstacle) Destroy(obstacle);
    }
}
