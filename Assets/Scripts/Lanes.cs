using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lanes : MonoBehaviour
{
    // ***Script to be attached to a Tile prefab***

    /// <summary>
    /// Number of in-game lanes.
    /// </summary>
    const int laneCount = 3;

    /// <summary>
    /// Tracks occupancy of lanes on this Tile prefab.
    /// </summary>
    public GameObject[] lanes = new GameObject[laneCount];

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
