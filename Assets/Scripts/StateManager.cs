using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Start,
    Gameplay,
    Pause,
    End
}

public class StateManager : MonoBehaviour
{
    public GameState currentState;

    public GameObject tileManagerObject;
    private TileManager tileManager;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = tileManagerObject.GetComponent<TileManager>();

        //Sets the current state to Gameplay for MVI purposes
        currentState = GameState.Gameplay;

        //Adds a listener to change the state when the player dies
        Player.PlayerDeath.AddListener(EndGame);
    }

    // Update is called once per frame
    void Update()
    {
        //If the player presses P, toggle the pause/resume function
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentState == GameState.Gameplay)
            {
                PauseGame();
            }
            else if (currentState == GameState.Pause)
            {
                ResumeGame();
            }
        }
    }

    //Changes the current gamestate to the specified state (Intended for external use)
    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    //Starts a new game
    private void StartGame()
    {
        ChangeState(GameState.Gameplay);

        //NOTE: ADD CODE TO RESET TILES ETC. LATER

        tileManager.ToggleTiles(true);
    }

    //Resumes the current game
    private void ResumeGame()
    {
        ChangeState(GameState.Gameplay);

        tileManager.ToggleTiles(true);
    }

    //Pauses the game and disables tiles
    private void PauseGame()
    {
        ChangeState(GameState.Pause);

        tileManager.ToggleTiles(false);
    }

    //Changes the game state and disables all tiles from moving
    private void EndGame()
    {
        ChangeState(GameState.End);

        tileManager.ToggleTiles(false);
    }
}
