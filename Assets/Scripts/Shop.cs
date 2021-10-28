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

    //Pricing fields
    [SerializeField]
    private const int doubleJumpPrice = 30;

    [SerializeField]
    private const int extraLifePrice = 100;

    [SerializeField]
    private const int doubleCollectiblesPrice = 15;

    [SerializeField]
    float startingTime = 4;

    // Start is called before the first frame update
    void Start()
    {
        //Assigns the player and tileManager scripts to references
        player = GameObject.Find("Player").GetComponent<Player>();
        tileManager = GameObject.Find("Tile Manager").GetComponent<TileManager>();

        //Disables the shop screen
        shopScreen.SetActive(false);

        //Starts the shop as inactive
        gameObject.SetActive(false);

        //Sets the timer
        timer = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            //Updates the text on the UI based on deltaTime
            timerText.text = "Time Left: " + Mathf.Floor(timer);
            timer -= Time.deltaTime;

            //If the timer exceeds the maximum amount of time, disable the shop
            if (timer <= 0)
            {
                ToggleShop(false);
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
            timer = startingTime;

            //Disable the shop object
            gameObject.SetActive(false);
        }
    }

    //Gives the player the specified powerup
    public void GivePowerup(string power)
    {
        //Determine which power to give to the player
        switch (power)
        {
            case "Double Jump":
                //Voids the transaction if the player does not have enough points
                if (player.Score <= doubleJumpPrice)
                {
                    break;
                }

                //Gives the player the powerup
                player.ActivateDoubleJump(doubleJumpPrice);

                //Disables the shop
                ToggleShop(false);
                break;

            case "Extra Life":
                //Voids the transaction if the player does not have enough points
                if (player.Score <= extraLifePrice)
                {
                    break;
                }

                //Gives the player the powerup
                player.ActivateExtraLife(extraLifePrice);

                //Disables the shop
                ToggleShop(false);
                break;

            case "Double Collectibles":
                //Voids the transaction if the player does not have enough points
                if (player.Score <= doubleCollectiblesPrice)
                {
                    break;
                }

                //Gives the player the powerup
                player.ActivateDoubleCollectibles(doubleCollectiblesPrice);

                //Disables the shop
                ToggleShop(false);
                break;
        }
    }
}
