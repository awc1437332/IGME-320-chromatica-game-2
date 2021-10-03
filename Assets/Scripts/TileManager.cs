using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TileManager : MonoBehaviour
{
    /// <summary>
    /// Number of collectibles that spawn on a lane at once.
    /// </summary>
    [SerializeField]
    public static int collectiblesPerLane = 5;

    /// <summary>
    /// Array holding references to each Tile GameObject.
    /// </summary>
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

    /// <summary>
    /// [PLACEHOLDER] Template prefab representing Obstacle objects.
    /// </summary>
    [SerializeField]
    private GameObject obstacleTemplate;

    [SerializeField]
    private GameObject collectibleTemplate;

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

        // Populate a tile with Obstacle objects whenever TileReset is invoked.
        Tile.TileReset.AddListener(FillTile);
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

    /// <summary>
    /// Uses RNG to determine whether to place an Obstacle at a given lane on a
    /// specified Tiles.
    /// </summary>
    /// <param name="resettingTileIndex">
    /// Index of tile being reset.
    /// </param>
    /// <param name="lane">
    /// Lane to potentially place an Obstacle.
    /// </param>
    /// <returns>
    /// A reference to the newly-created Obstacle or null, based on RNG output.
    /// </returns>
    private GameObject PlaceObstacle(int resettingTileIndex, int lane)
    {
        // 50/50 chance of obstacle generation. Can be modifed to alter
        // game difficulty.
        if (Random.Range(0, 2) == 1)
        {
            // Set coordinates of Obstacle to be placed.
            // x = take x of resetting tile and move Obstacle into the correct
            // lane by shifting it the appropriate number of widths.
            // lane - 1 to prevent spawning off-tile.
            float x = (lane - 1) * obstacleTemplate.transform.localScale.x
                        + tiles[resettingTileIndex].transform.position.x;

            // +1 to raise it above the tile.
            float y = tiles[resettingTileIndex].transform.position.y + 1;

            float z = tiles[resettingTileIndex].transform.position.z;

            // Use overload that accounts for position and rotation
            GameObject o = Instantiate(
                obstacleTemplate,
                new Vector3(x, y, z),
                tiles[resettingTileIndex].transform.rotation);

            // Make the Obstacle a child of the Tile so they move together.
            o.transform.parent = tiles[resettingTileIndex].transform;

            return o;
        }
        
        // No Obstacle created.
        return null;
    }

    private void PlaceCollectibles(string mode, int resettingTileIndex, int lane)
    {
        float x = (lane - 1) * obstacleTemplate.transform.localScale.x
                    + tiles[resettingTileIndex].transform.position.x;

        // +1 to raise it above the tile.
        float y = tiles[resettingTileIndex].transform.position.y + 1;

        float z = tiles[resettingTileIndex].transform.position.z;

        float distBetweenSpawns = tiles[0].transform.localScale.z / collectiblesPerLane;

        switch (mode)
        {
            case "vacant":
                for (int i = 0; i < collectiblesPerLane; i++)
                {
                    GameObject c = Instantiate(
                        collectibleTemplate,
                        new Vector3(x, y, z - tiles[0].transform.localScale.z / 2 + distBetweenSpawns / 2 + distBetweenSpawns * (i)),
                        tiles[resettingTileIndex].transform.rotation);

                    // Make the Collectible a child of the Tile so they
                    // move together.
                    c.transform.parent = tiles[resettingTileIndex].transform;

                    laneScripts[lane].collectibles[lane, i] = c;
                }
                break;
            case "occupied":
                break;
        }
    }

    /// <summary>
    /// Populates a resetting tile with Obstacles and Collectibles.
    /// </summary>
    public void FillTile()
    {
        int resettingTileIndex = LocateResettingTile();
        
        // Break early if an invalid index is found.
        if (resettingTileIndex == -1) return;

        FillObstacles(resettingTileIndex);
        FillCollectibles(resettingTileIndex);

        // Reset procedure complete.
        tileScripts[resettingTileIndex].resetting = false;
    }

    private void FillObstacles(int resettingTileIndex)
    {
        laneScripts[resettingTileIndex].ClearObstacles();

        // Look at the tile before the resetting one to determine obstacle
        // placement. Leverage modulus to create a "circular" general-form
        // expression that works for any index in the array.
        int prevTileIndex = (resettingTileIndex + (tileScripts.Length - 1))
                        % tileScripts.Length;

        // Look at each lane on the tile before.
        for (int i = 0; i < laneScripts[prevTileIndex].obstacles.Length; i++)
        {
            // If that lane has an obstacle, the one following it cannot
            // also be occupied.
            if (laneScripts[prevTileIndex].obstacles[i]) continue;
            // Otherwise, use RNG to determine whether or not to place an
            // obstacle at that lane.
            else
            {
                laneScripts[resettingTileIndex].obstacles[i] = PlaceObstacle(resettingTileIndex, i);
            }
        }
    }

    private void FillCollectibles(int resettingTileIndex)
    {
        // RNG determines whether Collectibles should be placed on the
        // resetting tile.
        if (Random.Range(0, 2) == 1)
        {
            // Spawn collectibles on a random lane.
            int lane = Random.Range(0, 3);

            // Current lane is occupied by an Obstacle. Spawn in an arc.
            if (laneScripts[resettingTileIndex].obstacles[lane])
            {
                return; // skip for now
            }
            // Current lane is empty. Spawn in a straight line.
            else PlaceCollectibles("vacant", resettingTileIndex, lane);
        }
    }
}
