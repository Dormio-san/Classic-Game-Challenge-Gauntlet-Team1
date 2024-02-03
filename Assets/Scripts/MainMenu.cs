using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public static bool warrior; // Bool for warrior class.
    public static bool valkyrie; // Bool for valkyrie class.
    public static bool ranger; // Bool for ranger class.
    public static bool wizard; // Bool for wizard class.

    // Variables responsible for which UI is displayed on the screen.
    private bool titleScreen = true;
    private bool mainMenu;
    public GameObject titleScreenUI;
    public GameObject mainMenuUI;

    // Player class text that displays at the top right when they choose their class.
    public GameObject warriorText;
    public GameObject valkyrieText;
    public GameObject rangerText;
    public GameObject wizardText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        healthText.text = "Health: " + 2000;
        scoreText.text = "Score: " + 0;
    }

    void Update()
    {
        // If escape key is pressed, exit the game.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // The Unity Editor line allows for testing in the non built version as it exits play mode in the editor.
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
            Debug.Log("Quitting game!");
        }

        // If the title screen is true (title screen is being displayed), run the title screen function.
        if (titleScreen)
        {
            TitleScreen();  
        }
        // Else if the main menu is true (main menu is being displayed), run the main menu check key function.
        else if (mainMenu)
        {
            MainMenuCheckKey();
        }
    }

    // This function sets each of the class variables. Made a function rather than listing each variable in each if statement above.
    void SetClassVariable(bool warriorSelected, bool valkyrieSelected, bool rangerSelected, bool wizardSelected)
    {
        warrior = warriorSelected;
        valkyrie = valkyrieSelected;
        ranger = rangerSelected;
        wizard = wizardSelected;

        warriorText.SetActive(warrior);
        valkyrieText.SetActive(valkyrie);
        rangerText.SetActive(ranger);
        wizardText.SetActive(wizard);
    }

    // Functon to check which key is pressed by the user.
    void MainMenuCheckKey()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float inputAmount = 0.05f;

        if (verticalInput > inputAmount)
        {
            // If W or up arrow key is pressed (up direction), set warrior to true and other class variables to false.
            SetClassVariable(true, false, false, false);
        }
        else if (horizontalInput < -inputAmount)
        {
            // If A or left arrow is pressed (left direction), set valkyrie to true and other class variables to false.
            SetClassVariable(false, true, false, false);
        }
        else if (verticalInput < -inputAmount)
        {
            // If S or down arrow key is pressed (down direction), set ranger to true and other class variables to false.
            SetClassVariable(false, false, true, false);
        }
        else if (horizontalInput > inputAmount)
        {
            // If D or right arrow is pressed (right direction), set wizard to true and other class variables to false.
            SetClassVariable(false, false, false, true);
        }

        // If the attack button is pressed and warrior or valkyrie or ranger or wizard are true, load Level One. 
        // As long as one of the classes has been selected when the player presses the attack button, Level One will load.
        if (Input.GetKeyDown(KeyCode.Space) && (warrior || valkyrie || ranger || wizard))
        {
            GameManager.levelTwo = false;
            EndGameScreen.playerWon = false;
            EndGameScreen.playerLost = false;
            PlayerBehavior.playerScore = 0;
            SceneManager.LoadSceneAsync("LevelOne");
        }
    }

    // Checks to see if the user presses any button. Once they do, turn off the title screen UI and show the main menu UI, allowing the user to choose their class.
    void TitleScreen()
    {
        if (Input.anyKeyDown)
        {
            titleScreen = false;
            titleScreenUI.SetActive(false);
            mainMenuUI.SetActive(true);
            mainMenu = true;
        }
    }
}