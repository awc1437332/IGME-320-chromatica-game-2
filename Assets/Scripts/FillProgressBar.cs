using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FillProgressBar : MonoBehaviour
{
    public static UnityEvent levelUp = new UnityEvent();
    
    /// <summary>
    /// Reference to the "progress bar" slider.
    /// </summary>
    [SerializeField]
    private Slider slider;

    /// <summary>
    /// Reference to text on the progress bar.
    /// </summary>
    [SerializeField]
    private Text displayText;

    /// <summary>
    /// Value to increment progress by per candy collected.
    /// </summary>
    [SerializeField]
    private float collectibleIncrementFactor; // 0.0043

    /// <summary>
    /// Value to increment progress by per jump completed.
    /// </summary>
    [SerializeField]
    private float jumpIncrementFactor;

    /// <summary>
    /// Value to decrement progress by each frame.
    /// </summary>
    [SerializeField]
    private float decrementProgressFactor;

    /// <summary>
    /// Rate at which to increase progress decrements each time the player levels up.
    /// </summary>
    [SerializeField]
    private float progressDecrementScaleFactor;
    

    private float currentValue;

    public float CurrentValue
    {
        get { return currentValue; }

        // Updates currentValue, slider.value, and displayText.text together.
        set
        {
            currentValue = value;
            slider.value = currentValue;
            displayText.text = (slider.value).ToString("P2"); // 2 dp

            // Progress >= 100%. Player levels up.
            if (currentValue >= 1)
                levelUp.Invoke();
        }
    }

    public void IncrementCollectible()
    {
        // Use the set property, since it updates the slider as well.
        // Multiply by scaleDecrementFactor for balance against increased
        // decrement rate.
        CurrentValue += collectibleIncrementFactor * (progressDecrementScaleFactor * 1.75f);
    }

    public void IncrementJump()
    {
        // Use the set property, since it updates the slider as well.
        // Multiply by scaleDecrementFactor for balance against increased
        // decrement rate.
        CurrentValue += jumpIncrementFactor * (progressDecrementScaleFactor * 1.75f);
    }

    /// <summary>
    /// Resets the progress bar.
    /// </summary>
    private void Reset()
    {
        CurrentValue = 0f;
        //Debug.Log("Level Up!");
    }

    /// <summary>
    /// Increases the difficulty by speeding the game up.
    /// </summary>
    private void SpeedUp()
    {
        Time.timeScale += 0.05f;
    }

    private void ScaleDecrement()
    {
        decrementProgressFactor *= progressDecrementScaleFactor * Time.deltaTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentValue = 0f;

        // Reset the progress bar when the player levels up.
        levelUp.AddListener(Reset);
        levelUp.AddListener(SpeedUp);
        levelUp.AddListener(ScaleDecrement);
    }

    // Update is called once per frame
    void Update()
    {
        //// Testing functionality.
        //CurrentValue += 0.0043f;

        CurrentValue -= decrementProgressFactor;
    }
}
