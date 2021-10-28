using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    Start,
    Gameplay,
    Pause,
    End
}

public class StateManager : MonoBehaviour
{
    public GameState currentState = GameState.Start;

    public GameObject tileManagerObject;
    private TileManager tileManager;

    [SerializeField]
    private GameObject startScreen;

    [SerializeField]
    private GameObject pauseScreen;

    [SerializeField]
    private GameObject endScreen;

    [SerializeField]
    private Text scoreText;

    //This field is a reference to the player object
    [SerializeField]
    GameObject playerObject;
    
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        //Creates a reference to the tile manager
        tileManager = tileManagerObject.GetComponent<TileManager>();

        //Sets the player script
        player = playerObject.GetComponent<Player>();

        //Disables the player and tiles to start
        player.TogglePlayer(false);

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

        //Enables the player
        player.TogglePlayer(true);

        tileManager.ToggleTiles(true);

        //Disables the start screen
        startScreen.SetActive(false);
    }

    //Resumes the current game
    public void ResumeGame()
    {
        ChangeState(GameState.Gameplay);

        tileManager.ToggleTiles(true);

        //Disables the player
        player.TogglePlayer(true);

        //Disables the pause and end screens
        pauseScreen.SetActive(false);
        endScreen.SetActive(false);
    }

    //Pauses the game and disables tiles
    public void PauseGame()
    {
        ChangeState(GameState.Pause);

        tileManager.ToggleTiles(false);

        //Disables the player
        player.TogglePlayer(false);

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
            scoreText.text = "Score: " + player.Score.ToString();
            endScreen.SetActive(true);
        }
        else
        {
            //Disables the pause screen and enables the end screen
            pauseScreen.SetActive(false);
            endScreen.SetActive(true);
        }
    }

    public void RestartGame()
    {
        //Resets the scene entirely
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
