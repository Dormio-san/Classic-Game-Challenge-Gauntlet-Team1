using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

//Debugs used in script are for testing unless otherwise specified.
public class GameManager : MonoBehaviour
{
    public GameObject playerSpawnAnimation;
    float animationWaitTime = 1.5f;
    public GameObject playerAvatar;
    // Not sure if spawning will occur in this script or will be preset through Unity.
    // public GameObject enemyGhostSpawner;
    // public GameObject enemyGhost;
    // public GameObject chestItem;
    // public GameObject keyItem;
    // public GameObject potionItem;
    // public GameObject healingItem;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerScoreText;    
    public GameObject keyImageUI;
    public GameObject potionImageUI;
    bool isGameOver = false;


    void Start()
    {
        StartCoroutine("SpawnLevelOne");
    }

    void Update()
    {
        //If escape key is pressed, exit the game.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //The Unity Editor line allows for testing in the non built version as it exits play mode in the editor.
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
            Debug.Log("Quitting game!");
        }        
    }

    IEnumerator SpawnLevelOne()
    {
        Instantiate(playerSpawnAnimation, new Vector3(0, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(animationWaitTime);
        Instantiate(playerAvatar, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void ChangeHealthText(int currentHealth)
    {
        //playerHealthText.text = currentHealth;
    }

    public void ChangeScoreText(int currentScore)
    {
        //playerScoreText.text = currentScore;
    }

    public void ChangeKeysUI(int numberOfKeys)
    {
        if (numberOfKeys == 1)
        {
            keyImageUI.SetActive(true);
        }
        else if (numberOfKeys <= 0)
        {
            keyImageUI.SetActive(false);
        }
    }

    public void ChangePotionUI(int numberOfPotions)
    {
        if (numberOfPotions <= 0)
        {
            potionImageUI.SetActive(false);
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
