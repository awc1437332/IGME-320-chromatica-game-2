using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField]
    public GameObject shopScreen;
    public Text timerText;

    private Player player;
    private TileManager tileManager;

    private bool isOpen = false;

    private float timer = 0;

    [SerializeField]
    float startingTime = 4;

    // Start is called before the first frame update
    void Start()
    {
        //Assigns the player and tileManager scripts to references
        player = GameObject.Find("PlayerModel").GetComponent<Player>();
        tileManager = GameObject.Find("Tile Manager").GetComponent<TileManager>();

        //Disables the shop screen
        shopScreen.SetActive(false);

        //Sets the timer
        timer = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            //Updates the text on the UI based on deltaTime
            timerText.text = Mathf.Floor(timer).ToString();
            timer -= Time.deltaTime;

            //If the timer exceeds the maximum amount of time, disable the shop
            if (timer <= 0)
            {
                ToggleShop(false);
                print("Time is up");
            }
        }
    }

    //Enables the shop window
    public void ToggleShop(bool value)
    {
        //Enables the shop UI and toggles its isOpen
        isOpen = value;
        shopScreen.SetActive(value);

        //If the shop is being toggled open
        if (value)
        {
            //Disable the player and tiles
            player.TogglePlayer(false);
            tileManager.ToggleTiles(false);

            //Enable the shop screen
            shopScreen.SetActive(true);
        }
        else
        {
            //Enable the player and tiles
            player.TogglePlayer(true);
            tileManager.ToggleTiles(true);

            //Disable the shop screen
            shopScreen.SetActive(false);

            //Reset the timer
            timer = 0.0f;

            //Disable the shop object
            gameObject.SetActive(false);
        }
    }

    //Simply a dummy function for testing Shop UI controls
    public void DummyGivePowerup()
    {
        ToggleShop(false);
        print("Powerup Given");
    }
}
