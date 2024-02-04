using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGameScreen : MonoBehaviour
{
    // Variables related to displaying the correct UI based on whether the player won or lost.
    public static bool playerWon;
    public static bool playerLost;
    public GameObject playerWonUI;
    public GameObject playerLostUI;
    public static Sprite playerCharacter;
    public TextMeshProUGUI yourScoreTextWin;
    public TextMeshProUGUI yourScoreTextLose;

    void Start()
    {
        if (playerWon)
        {
            PlayerWonGame();
        }
        else if (playerLost)
        {
            PlayerLostGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckKeyInput();    
    }

    // Function that displays the UI items related to winning the game.
    void PlayerWonGame()
    {
        Image playerCharacterObject = GameObject.Find("PlayerCharacter").GetComponent<Image>();
        playerCharacterObject.sprite = playerCharacter;
        yourScoreTextWin.text = "Your Score: " + PlayerBehavior.playerScore;
    }

    // Function that displays the UI items related to losing the game.
    void PlayerLostGame()
    {
        playerWonUI.SetActive(false);
        playerLostUI.SetActive(true);
        yourScoreTextLose.text = "Your Score: " + PlayerBehavior.playerScore;
    }

    void CheckKeyInput()
    {
        // If the R key is pressed, restart the game. Restarting the game will send the player to the main menu.
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }

        // If escape key is pressed, exit the game.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // The Unity Editor line allows for testing in the non built version as it exits play mode in the editor.
            //UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
            //Debug.Log("Quitting game!");
        }
    }
}
