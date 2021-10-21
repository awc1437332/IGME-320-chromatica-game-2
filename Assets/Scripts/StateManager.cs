using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    GameObject startScreen;
    GameObject pauseScreen;
    GameObject endScreen;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = tileManagerObject.GetComponent<TileManager>();

        //Sets the current state to Gameplay for MVI purposes
        currentState = GameState.Start;

        //Adds a listener to change the state when the player dies
        Player.PlayerDeath.AddListener(delegate { EndGame(true); });

        //Sets the variables for the gamestate screens
        startScreen = GameObject.Find("UI/StartScreen");
        pauseScreen = GameObject.Find("UI/PauseScreen");
        endScreen = GameObject.Find("UI/EndScreen");

        //Disables the pause and end screens
        pauseScreen.SetActive(false);
        endScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == GameState.Gameplay)
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
    }

    //Changes the current gamestate to the specified state (Intended for external use)
    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    //Starts a new game
    public void StartGame()
    {
        ChangeState(GameState.Gameplay);

        //NOTE: ADD CODE TO RESET TILES ETC. LATER

        tileManager.ToggleTiles(true);

        //Disables the start screen
        startScreen.SetActive(false);
    }

    //Resumes the current game
    public void ResumeGame()
    {
        ChangeState(GameState.Gameplay);

        tileManager.ToggleTiles(true);

        //Disables the pause and end screens
        pauseScreen.SetActive(false);
        endScreen.SetActive(false);
    }

    //Pauses the game and disables tiles
    public void PauseGame()
    {
        ChangeState(GameState.Pause);

        tileManager.ToggleTiles(false);

        //Makes the pause screen visible and interactible
        pauseScreen.SetActive(true);
    }

    //Changes the game state and disables all tiles from moving
    private void EndGame(bool displayScore)
    {
        ChangeState(GameState.End);

        tileManager.ToggleTiles(false);

        if (displayScore)
        {
            //Setup for score display
        }
        else
        {
            //Disables the pause screen and enables the end screen
            pauseScreen.SetActive(false);
            endScreen.SetActive(true);
        }
    }
}
