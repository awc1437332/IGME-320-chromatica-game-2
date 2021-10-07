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
    /// Tracks occupancy of lanes by obstacles on this Tile prefab.
    /// </summary>
    public GameObject[] obstacles = new GameObject[laneCount];

    /// <summary>
    /// 2D array tracking occupancy of lanes by collectibles on this Tile prefab.
    /// Index 0: lane and Index 1: array of collectibles.
    /// </summary>
    public GameObject[,] collectibles = new GameObject[
        laneCount, 
        TileManager.collectiblesPerLane];

    /// <summary>
    /// Loops through and clears all obstacles and collectibles occupying the 
    /// Tile this script is attached to. Called when a Tile's position is reset.
    /// </summary>
    public void ClearObstacles()
    {
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i])
            {
                Destroy(obstacles[i]);
                obstacles[i] = null;
            }
        }

        for (int i = 0; i < collectibles.GetLength(0); i++)
        {
            for (int j = 0; j < collectibles.GetLength(1); j++)
            {
                if (collectibles[i, j])
                {
                    //Debug.Log(string.Format("destroyed collectible {0} at lane {1}", j, i));
                    Destroy(collectibles[i, j]);
                    collectibles[i, j] = null;
                }
            }
        }
    }
}
