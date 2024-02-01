using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public bool warrior = false; // Bool for warrior class.
    public bool valkyrie = false; // Bool for valkyrie class.
    public bool ranger = false; // Bool for ranger class.
    public bool wizard = false; // Bool for wizard class.

    private PlayerBehavior pB; // Variable to reference player script. Plan is to have blank object with player script on main menu scene so this script can access it.

    void Start()
    {
        // Set variable reference to player script.
        pB = GameObject.FindWithTag("Player").GetComponent<PlayerBehavior>(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // If W is pressed, set warrior to true and other class variables to false.
            SetClassVariable(true, false, false, false);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            // If A is pressed, set valkyrie to true and other class variables to false.
            SetClassVariable(false, true, false, false);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // If S is pressed, set ranger to true and other class variables to false.
            SetClassVariable(false, false, true, false);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            // If D is pressed, set wizard to true and other class variables to false.
            SetClassVariable(false, false, false, true);
        }

        // If Z is pressed and warrior or valkyrie or ranger or wizard are true, load Level One. 
        // As long as one of the classes has been selected when the player presses the attack button, Level One will load.
        if (Input.GetKeyDown(KeyCode.Z) && (warrior || valkyrie || ranger || wizard))
        {
            pB.SetPlayerClass(warrior, valkyrie, ranger, wizard);
            SceneManager.LoadScene("LevelOne");
        }
    }

    // This function sets each of the class variables. Made a function rather than listing each variable in each if statement above.
    void SetClassVariable(bool warriorSelected, bool valkyrieSelected, bool rangerSelected, bool wizardSelected)
    {
        warrior = warriorSelected;
        valkyrie = valkyrieSelected;
        ranger = rangerSelected;
        wizard = wizardSelected;
    }
}