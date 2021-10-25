using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileManager : MonoBehaviour
{
    #region fields
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
    /// [PLACEHOLDER] Template prefab representing Tile objects.
    /// </summary>
    [SerializeField]
    private GameObject tileTemplate;

    /// <summary>
    /// Distance to spawn Collectibles from an Obstacle. Added to Obstacle's
    /// length when spawning Collectibles in an arc around it to give "padding".
    /// </summary>
    [SerializeField]
    private float distanceFromObstacle;

    /// <summary>
    /// Array holding references to each Tile's Tile script.
    /// </summary>
    private Tile[] tileScripts;

    /// <summary>
    /// Array holding references to each Tile's Lanes script.
    /// </summary>
    private Lanes[] laneScripts;

    /// <summary>
    /// States that a lane can be in concerning Obstacle occupancy.
    /// </summary>
    private enum LaneStates
    {
        Vacant,
        Occupied
    }
    #endregion

    #region methods
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
        Tile.tileReset.AddListener(FillTile);
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
    /// Uses RNG to determine whether to place an Obstacle in a given lane on a
    /// specified Tile.
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

            // Request obstacle instance from the object pool.
            GameObject obstacle = ObjectPool.Instance.GetPooledObject(PooledObjects.Obstacle);

            // If the request is successful, update the obstacle instance's
            // position and rotation accordingly.
            if (obstacle)
            {
                obstacle.transform.position = new Vector3(x, y, z);
                obstacle.transform.rotation = tiles[resettingTileIndex].transform.rotation;
            }

            // Make the Obstacle a child of the Tile so they move together.
            obstacle.transform.parent = tiles[resettingTileIndex].transform;

            return obstacle;
        }
        
        // No Obstacle created.
        return null;
    }

    /// <summary>
    /// Places a set of Collectibles in a given lane on a specified Tile.
    /// </summary>
    /// <param name="occupied">
    /// Whether an Obstacle is occupying the given lane.
    /// </param>
    /// <param name="resettingTileIndex">
    /// Index of the Tile that is being reset.
    /// </param>
    /// <param name="lane">
    /// The lane to spawn Collectibles on.
    /// </param>
    private void PlaceCollectibles(bool occupied, int resettingTileIndex, int lane)
    {
        // Set coordinates of Obstacle to be placed.
        // x = take x of resetting tile and move Obstacle into the correct
        // lane by shifting it the appropriate number of widths.
        // lane - 1 to prevent spawning off-tile.
        float x = (lane - 1) * obstacleTemplate.transform.localScale.x
                    + tiles[resettingTileIndex].transform.position.x;

        // Calculate the distance between each Collectible.
        float distBetweenSpawns = tileTemplate.transform.localScale.z 
                                    / collectiblesPerLane;

        // Current tile has an obstacle on it. Spawn Collectibles in an arc.
        if (occupied)
        {
            // Evenly distribute Collectibles about an central spawn origin.
            float theta = Mathf.PI / collectiblesPerLane;
            float angle;

            for (int i = 0; i < collectiblesPerLane; i++)
            {
                // Spawn from centre of obstacle occupying current lane.
                // Do it this way to ensure dynamic placement regardless
                // of obstacle size.
                Vector3 spawnOrigin = laneScripts[resettingTileIndex].obstacles[lane].transform.position;

                // Calculate angles about a semicircle forming the spawn arc.
                angle = Mathf.PI - (theta * (i + 1)) + theta / 2;

                // Unit horizontal and vertical distance from spawnOrigin 
                // to place the next collectible.
                float zUnit = Mathf.Cos(angle);
                float yUnit = Mathf.Sin(angle);

                // Calculate radius about Obstacle by adding half of (global)
                // Obstacle length with distanceFromObstacle.
                float radius = 
                    laneScripts[resettingTileIndex].obstacles[lane].transform.lossyScale.z / 2 
                    + distanceFromObstacle;

                // Request collectible instance from the object pool.
                GameObject collectible = ObjectPool.Instance.GetPooledObject(PooledObjects.Collectible);

                // If the request is successful, update the collectible instance's
                // position and rotation accordingly.
                if (collectible)
                {
                    collectible.transform.position = new Vector3(
                        x,
                        spawnOrigin.y + yUnit * radius + 1, // +1 to raise it above the tile.
                        spawnOrigin.z + zUnit * radius);
                    collectible.transform.rotation = tiles[resettingTileIndex].transform.rotation;
                }

                // Make the Collectible a child of the Tile so they
                // move together.
                collectible.transform.parent = tiles[resettingTileIndex].transform;

                // Add to the Collectible array within the appropriate lane of
                // the Tile's Lanes script.
                laneScripts[resettingTileIndex].collectibles[lane, i] = collectible;
            }
        }
        // Current tile is vacant. Spawn Collectibles in a straight line.
        else
        {
            // +1 to raise it above the tile.
            float y = tiles[resettingTileIndex].transform.position.y + 1;

            float z;

            for (int i = 0; i < collectiblesPerLane; i++)
            {
                // 1) Get centre of resetting Tile.
                // 2) Subtract half of Tile's length to get location of end
                // of Tile.
                // 3) Set Collectibles distBetweenSpawns units away from each
                // other + 0.5(distBetweenSpawns) to centre them on the Tile.
                z = tiles[resettingTileIndex].transform.position.z // 1)
                    - tileTemplate.transform.localScale.z / 2      // 2)
                    + distBetweenSpawns * (0.5f + i);              // 3)

                // Request collectible instance from the object pool.
                GameObject collectible = ObjectPool.Instance.GetPooledObject(PooledObjects.Collectible);

                // If the request is successful, update the collectible instance's
                // position and rotation accordingly.
                if (collectible)
                {
                    collectible.transform.position = new Vector3(x, y, z);
                    collectible.transform.rotation = tiles[resettingTileIndex].transform.rotation;
                }

                // Make the Collectible a child of the Tile so they
                // move together.
                collectible.transform.parent = tiles[resettingTileIndex].transform;

                // Add to the Collectible array within the appropriate lane of
                // the Tile's Lanes script.
                laneScripts[resettingTileIndex].collectibles[lane, i] = collectible;
            }
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

    /// <summary>
    /// Populates a resetting tile with Obstacles.
    /// </summary>
    /// <param name="resettingTileIndex">
    /// Index of resetting tile.
    /// </param>
    private void FillObstacles(int resettingTileIndex)
    {
        laneScripts[resettingTileIndex].ClearLanes();

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

    /// <summary>
    /// Populates a resetting tile with Collectibles.
    /// </summary>
    /// <param name="resettingTileIndex">
    /// Index of resetting tile.
    /// </param>
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
                PlaceCollectibles(true, resettingTileIndex, lane);
            // Current lane is empty. Spawn in a straight line.
            else PlaceCollectibles(false, resettingTileIndex, lane);
        }
    }

    /// <summary>
    /// Stops all tiles from moving or updating.
    /// </summary>
    public void ToggleTiles(bool value)
    {
        foreach(Tile tile in tileScripts)
        {
            tile.isActive = value;
        }
    }
    #endregion
}