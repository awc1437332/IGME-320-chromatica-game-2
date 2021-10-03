using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] tiles;

    /// <summary>
    /// Array holding references to each Tile's Tile script.
    /// </summary>
    private Tile[] tileScripts;

    /// <summary>
    /// Array holding references to each Tile's Lanes script.
    /// </summary>
    private Lanes[] laneScripts;

    [SerializeField]
    private GameObject obstacleTemplate;

    // Start is called before the first frame update
    void Start()
    {
        // Initialise and populate tileScripts and laneScripts.
        tileScripts = new Tile[tiles.Length];
        laneScripts = new Lanes[tiles.Length];

        for (int i = 0; i < tileScripts.Length; i++)
        {
            tileScripts[i] = tiles[i].GetComponent<Tile>();
            laneScripts[i] = tiles[i].GetComponent<Lanes>();
        }

        Tile.TileReset.AddListener(FillTile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Finds the tile marked as "resetting" in tileScripts. Called when the
    /// TileReset event is invoked.
    /// </summary>
    /// <returns>
    /// Index of resetting tile, or -1 if not found.
    /// </returns>
    private int LocateResettingTile()
    {
        for (int i = 0; i < tiles.Length; i++)
            if (tileScripts[i].resetting) return i;

        return -1;
    }

    private GameObject PlaceObstacle(int resettingTileIndex, int lane)
    {
        if (Random.Range(0, 2) == 0)
        {
            // Use overload that accounts for position and rotation
            GameObject r = Instantiate(
                obstacleTemplate,
                new Vector3(
                    tiles[resettingTileIndex].transform.position.x + lane * obstacleTemplate.transform.localScale.x,
                    tiles[resettingTileIndex].transform.position.y,
                    tiles[resettingTileIndex].transform.position.z),
                tiles[resettingTileIndex].transform.rotation);
            r.transform.parent = tiles[resettingTileIndex].transform;
        }
        
        return null;
    }

    public void FillTile()
    {
        int resettingTileIndex = LocateResettingTile();

        // Break early if an invalid index is found.
        if (resettingTileIndex == -1) return;

        // Look at the tile before the resetting one to determine obstacle
        // placement. Leverage modulus to create a "circular" general-form
        // expression that works for any index in the array.
        int prevTileIndex = (resettingTileIndex + (tileScripts.Length - 1)) 
                        % tileScripts.Length;

        // Look at each lane on the tile before.
        for (int i = 0; i < laneScripts[prevTileIndex].lanes.Length; i++) 
        {
            // If that lane has an obstacle, the one following it cannot
            // also be occupied.
            if (laneScripts[prevTileIndex].lanes[i]) continue;
            // Otherwise, use RNG to determine whether or not to place an
            // obstacle at that lane.
            else
            {
                laneScripts[resettingTileIndex].lanes[i] = PlaceObstacle(resettingTileIndex, i);
                //tiles[resettingTileIndex].
            }
        }
    }
}
