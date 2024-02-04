using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    // Script references.
    private GameManager gM;

    // Bools to say what class the player chose on the main menu.
    private bool warriorClass;
    private bool valkyrieClass;
    private bool rangerClass;
    private bool wizardClass;

    // Class specific variables.
    private float playerMoveSpeed;
    private GameObject playerWeapon;
    private float playerAttackSpeed;
    [HideInInspector] public int ghostDamagePlayerTakes;
    private Sprite playerSprite;
    private bool walkIntoDamage; // Determines if the character can walk into something and damage it.
    private int damagePlayerDeals = 1; // Most likely won't change between class, so defining it here.

    //*Each class variables begin.

    // Warrior class variables.
    private float warriorMoveSpeed = 3.5f;
    public GameObject warriorWeapon;
    private float warriorAttackSpeed = 5.5f;
    private int ghostDamageWarriorTakes = 8;
    public Sprite warriorSprite;

    // Valkyrie class variables.
    private float valkyrieMoveSpeed = 3.5f;
    public GameObject valkyrieWeapon;
    private float valkyrieAttackSpeed = 5.5f;
    private int ghostDamageValkyrieTakes = 7;
    public Sprite valkyrieSprite;

    // Ranger class variables.
    private float rangerMoveSpeed = 3.5f;
    public GameObject rangerWeapon;
    private float rangerAttackSpeed = 7.5f;
    private int ghostDamageRangerTakes = 9;
    public Sprite rangerSprite;

    // Wizard class variables.
    private float wizardMoveSpeed = 3.5f;
    public GameObject wizardWeapon;
    private float wizardAttackSpeed = 5.5f;
    private int ghostDamageWizardTakes = 10;
    public Sprite wizardSprite;

    // Each class variables end.*

    // Player variables that remain the same no matter the class.
    private static int maxHealth = 2000;
    public static int playerHealth = maxHealth;
    public static int playerScore = 0;    
    private int playerGradualHealthLoss = 1;
    private float playerAttackCooldown = .7f;
    private float lastAttackTime;
    private int enemyDeadScore = 10;
    public AudioSource pickupSound;
    public AudioSource hitEnemy;
    public AudioSource usePotion;
    public AudioSource attackHit;

    // Item related variables for key, potion, healing item, and chest.
    private int keyInPossession = 0;
    [HideInInspector] public int potionInPossession = 0;
    private int healthPlayerGains = 100;
    private int chestScore = 100;

    // Variables used for weapon behavior.
    private Transform playerTransform;
    private Vector2 lastFacingDirection = Vector2.right;

    private Animator playerAnimator; // Used for displaying player character animations.

    // Variables responsible for making potion attack work.
    private Camera playerCamera; // Assigned in script, so don't need to do it in the inspector.
    public LayerMask enemyLayer; // !! Make sure to assign in the inspector!!

    // Awake function to set the class specific variables to ensure that they are set before anything else occurs. Also, added setting player health to the max here.
    void Awake()
    {
        SetClassSpecificVariables();
    }
    void Start()
    {
        // Assign script references.
        gM = GameObject.Find("GameManager").GetComponent<GameManager>(); // Assign reference to game manager script.

        InvokeRepeating("GradualHealthDepletion", 1f, 1f); // One second after spawning, begin losing 1 health every second.

        // Assign references to player variables.
        playerTransform = transform;
        playerAnimator = GetComponent<Animator>();
        playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        // Display the UI at the start so the player can see the values they are at.
        // For example, show score at 0 so player knows their score when they begin. Before, it only updated once a score related action occurred.
        HealthUIChange();
        ScoreUIChange();
    }

    void Update()
    {
        PlayerMovement();       

        // If the player presses space and they can attack, the SpawnPlayerWeapon function runs.
        if (Input.GetKey(KeyCode.Space) && CanAttack())
        {
            SpawnPlayerWeapon();
        }  

        // If player presses U, run the potion attack function that handles the use of the potion attack.
        if (Input.GetKeyDown(KeyCode.U))
        {
            PotionAttack();
        }

        // If the player hits 0 health or below (below in case of any unexpected situations) run the game over function and stop the invoke that was occurring.
        if (playerHealth <= 0)
        {
            gM.GameOver();
            CancelInvoke();
        }

        Debugging(); // Debugging is stored in one function and run here to avoid clutter.
    }

    // Depending on which class the player chose, the values for some variables differ.
    // The function below sets the variable values based on what class was chosen by the player.
    void SetClassSpecificVariables()
    {
        // Set the player class variables equal to the game manager player class variables since they should be the same value.
        warriorClass = MainMenu.warrior;
        valkyrieClass = MainMenu.valkyrie;
        rangerClass = MainMenu.ranger;
        wizardClass = MainMenu.wizard;

        if (warriorClass)
        {
            // Warrior class, so set warrior variables as the values to be used by the player.
            playerMoveSpeed = warriorMoveSpeed;
            playerWeapon = warriorWeapon;
            playerAttackSpeed = warriorAttackSpeed;
            ghostDamagePlayerTakes = ghostDamageWarriorTakes;
            playerSprite = warriorSprite;
            walkIntoDamage = true;
        }
        else if (valkyrieClass)
        {
            // Valkyrie class, so set valkyrie variables as the values to be used by the player.
            playerMoveSpeed = valkyrieMoveSpeed;
            playerWeapon = valkyrieWeapon;
            playerAttackSpeed = valkyrieAttackSpeed;
            ghostDamagePlayerTakes = ghostDamageValkyrieTakes;
            playerSprite = valkyrieSprite;
            walkIntoDamage = true;
        }
        else if (rangerClass)
        {
            // Ranger class, so set ranger variables as the values to be used by the player.
            playerMoveSpeed = rangerMoveSpeed;
            playerWeapon = rangerWeapon;
            playerAttackSpeed = rangerAttackSpeed;
            ghostDamagePlayerTakes = ghostDamageRangerTakes;
            playerSprite = rangerSprite;
            walkIntoDamage = false;
        }
        else if (wizardClass)
        {
            // Wizard class, so set wizard variables as the values to be used by the player.
            playerMoveSpeed = wizardMoveSpeed;
            playerWeapon = wizardWeapon;
            playerAttackSpeed = wizardAttackSpeed;
            ghostDamagePlayerTakes = ghostDamageWizardTakes;
            playerSprite = wizardSprite;
            walkIntoDamage = false;
        }
    }

    // When the player collides with another object, check the tag of the other object.
    void OnTriggerEnter2D(Collider2D other)
    {
        // switch statement helps to reduce clutter of multiple if and else if statements.
        switch (other.tag)
        {
            case "ExitOne":
                // First exit collided with.
                gM.ChangeScene("LevelTwo");
                CameraBehavior.playerMoving = false;
                GameManager.levelTwo = true;
                break;
            case "ExitTwo":
                // Second exit collided with.
                EndGameScreen.playerWon = true;
                EndGameScreen.playerCharacter = playerSprite;
                gM.ChangeScene("EndGameScreen");
                break;
            case "Chest":
                // Chest item collided with.
                pickupSound.Play(); // Play sound for pickup.
                PlayerScoreChange(chestScore); // Give player score.
                Destroy(other.gameObject); // Destroy chest.
                break;
            case "Key":
                // Key collided with.
                // Play a sound, give player a key, update the UI, and destroy the item.
                pickupSound.Play();
                keyInPossession += 1;
                //Debug.Log($"Keys: {keyInPossession}");
                gM.ChangeKeysUI(keyInPossession);
                Destroy(other.gameObject);
                break;
            case "Potion":
                // Potion attack item collided with.
                // Play a sound, give the player a potion, update the UI, destroy the item.
                pickupSound.Play();
                potionInPossession += 1;
                //Debug.Log($"Potion: {potionInPossession}");
                gM.ChangePotionUI(potionInPossession);
                Destroy(other.gameObject);
                break;
            case "Door":
                // Door has been collided with.
                // If player has at least 1 key, subtract a key, update the UI, and destroy the door (open it).
                if (keyInPossession >= 1)
                {
                    keyInPossession --;
                    //Debug.Log($"Keys: {keyInPossession}");
                    gM.ChangeKeysUI(keyInPossession);
                    Destroy(other.gameObject);
                }
                break;
            case "GhostSpawner":
                // If the player can walk into something and damge it, deal damage to the spawner and play sound.
                if (walkIntoDamage)
                {
                    PlayAttackHit();
                    other.GetComponent<SpawnerBehavior>().TookDamage(damagePlayerDeals);
                }
                break;
        }
    }

    void PlayerMovement()
    {
        // Since 2D game, use Vector2 which is only x and y movement.
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Enabling the normalize 2 lines down makes sure that diagonal movement is not quicker.
        // Have it commented out because original game has quicker diagonal movement, and we are maintaining that mechanic.
        //movement.Normalize();
        
        // Player moves based on Vector2 movement above and the speed given to them.
        transform.Translate(movement * Time.deltaTime * playerMoveSpeed);
    
        // Run the update sprite animation function so the sprite animation changes based on the movement direction.
        UpdateSpriteAnimation();

        // Variables used for the calculation of the last facing direction used in SpawnPlayerWeapon.
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        // Update the last facing direction whenever movement occurs.
        if (Mathf.Abs(horizontalInput) > 0.05f || Mathf.Abs(verticalInput) > 0.05f)
        {
            lastFacingDirection = new Vector2(horizontalInput, verticalInput).normalized;
        }
    }

    void SpawnPlayerWeapon()
    {
        // Spawn the attack at the player's position and give it a variable name.
        GameObject playerAttack = Instantiate(playerWeapon, playerTransform.position, Quaternion.identity);
        // Get the rigidbody of the player's attack.
        Rigidbody2D playerAttackRb = playerAttack.GetComponent<Rigidbody2D>();
        
        // As long as the playerAttack's rigidbody exits (does not equal null), run code below.
        if (playerAttackRb != null)
        {
            // Set the direction the attack moves in the direction the player is facing.
            playerAttackRb.velocity = lastFacingDirection * playerAttackSpeed;

            // Calculate the angle based on the movement direction of the player.
            // Once calculated, set the player's attack to that rotation.
            float angle = Mathf.Atan2(lastFacingDirection.x, -lastFacingDirection.y) * Mathf.Rad2Deg;
            playerAttack.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            Debug.LogWarning("Rigidbody2D not found on player attack."); // Debug here in case issue occurs.
        }

        lastAttackTime = Time.time; // Begin attack cooldown.       
    }

    // Does as the name suggests by subtracting the depletion from the player's health. This is repeated every second and is mentioned in the start function. Also, updates UI.
    void GradualHealthDepletion()
    {
        playerHealth -= playerGradualHealthLoss;
        HealthUIChange();
    }

    // Changes score and updates UI. Often called by other scripts when a score change occurs.
    public void PlayerScoreChange(int scoreAmount)
    {
        playerScore += scoreAmount;
        ScoreUIChange();
    }

    // Changes the players health and updates UI. Often called by other scripts when a health change occurs.
    public void PlayerHealthChange(int healthAmount)
    {
        playerHealth -= healthAmount;
        HealthUIChange();
    }

    // Made this a function so the code looked a bit prettier.
    public void ScoreUIChange()
    {
        gM.ChangeScoreText(playerScore);
    }

    // Made this a function so the code looked a bit prettier.
    public void HealthUIChange()
    {
        gM.ChangeHealthText(playerHealth);
    }
    
    // Bool function that figures out if the lastAttackTime is less than or greater than the player attack cooldown.
    // If less, player can't attack. If greater, CanAttack is true and the player can attack again.
    bool CanAttack()
    {
        return Time.time - lastAttackTime >= playerAttackCooldown;
    }


    // Updates the sprite animation based on the direction the player is moving in.
    void UpdateSpriteAnimation()
    {
       float horizontalInput = Input.GetAxis("Horizontal");
       float verticalInput = Input.GetAxis("Vertical");
       float inputAmount = 0.02f;

       // Based on the direction of movement, set the sprite for that direction.
       if (horizontalInput < -inputAmount)
       {
           // Moving left, set movingLeft to true and others to false.
           SetAnimatorBools(true, false, false, false);
       }
       else if (horizontalInput > inputAmount)
       {
           // Moving right, set movingRight to true and others to false.
           SetAnimatorBools(false, true, false, false);
       }
       else if (verticalInput > inputAmount)
       {
           // Moving up, set movingUp to true and others to false.
           SetAnimatorBools(false, false, true, false);                       
       }
       else if (verticalInput < -inputAmount)
       {
           // Moving down, set movingDown to true and others to false.
           SetAnimatorBools(false, false, false, true);
       }
    }

    // Updates the Animator bools. Makes code look a lot better as the alternative was listing what is in this function in each if statement above.
    void SetAnimatorBools(bool movingLeft, bool movingRight, bool movingUp, bool movingDown)
    {
        playerAnimator.SetBool("moveLeft", movingLeft);
        playerAnimator.SetBool("moveRight", movingRight);
        playerAnimator.SetBool("moveUp", movingUp);
        playerAnimator.SetBool("moveDown", movingDown);
    }

    
    // This function is used for various debugging items. Found it easier to store them in one place, so put it here.
    void Debugging()
    {
        // When player presses P, add potion and update the UI.
        if (Input.GetKeyDown(KeyCode.P))
        {
            potionInPossession++;
            gM.ChangePotionUI(potionInPossession);

            // Make sure I can't add too many potions.
            if (potionInPossession >= 5)
            {
                potionInPossession = 5;
            }

            // Put it in console just to be sure of number I have.
            Debug.Log("Potions: " + potionInPossession);
        }
        // When O is pressed, subtract 1 potion and update UI. Allows for both up and down to test the UI.
        else if (Input.GetKeyDown(KeyCode.O))
        {
            potionInPossession--;
            gM.ChangePotionUI(potionInPossession);

            // Make sure I can't subtract too many potions.
            if (potionInPossession <= 0)
            {
                potionInPossession = 0;
            }

            // Put it in console just to be sure of number I have.
            Debug.Log("Potions: " + potionInPossession);
        }

        // When K is pressed, increase keys by 1 and update UI.
        if (Input.GetKeyDown(KeyCode.K))
        {
            keyInPossession++;
            gM.ChangeKeysUI(keyInPossession);

            // Make sure I can't add too many keys.
            if (keyInPossession >= 6)
            {
                keyInPossession = 6;
            }

            // Put it in console just to be sure of number I have.
            Debug.Log("Keys: " + keyInPossession);
        }
        // When J is pressed, decrease key by 1 and update UI. Allows for both up and down to test the UI.
        else if (Input.GetKeyDown(KeyCode.J))
        {
            keyInPossession--;
            gM.ChangeKeysUI(keyInPossession);

            // Make sure I can't subtract too many keys.
            if (keyInPossession < 0)
            {
                keyInPossession = 0;
            }

            // Put it in console just to be sure of number I have.
            Debug.Log("Keys: " + keyInPossession);
        }

        // When H is pressed, subtract playerHealth and update the UI. This is used to quickly test game over functionality when hitting zero health.
        if (Input.GetKeyDown(KeyCode.H))
        {
            playerHealth -= 100;
            HealthUIChange();
            
            // Make sure I can't go into negative health.
            if (playerHealth <= 0)
            {
                playerHealth = 0;
                HealthUIChange();
            }
        }   
        
        // Lil sneaky increase player speed so I can get through the level and test things quicker.
        if (Input.GetKeyDown(KeyCode.F))
        {
            playerMoveSpeed++;
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            // Guess I'm adding a way to slow down as well.
            playerMoveSpeed--;
        }
    }    

    // Potion attack function that destroys all enemies on the screen and awards the player with score.
    // For this function to work, make sure the enemies are on a specific layer (found near the top right of the inspector by the tag drop down list.)
    // Then, assign that layer to the player script in the inspector.
    void PotionAttack()
    {
        // If the player has more than 0 potions (at least 1), run the code below.
        if (potionInPossession > 0)
        {
            // Playe use potion sound.
            usePotion.Play();

            // Create a collider array that is called enemiesInView. This array checks to see if there are any colliders within the view of the camera at the enemyLayer and assigns those to the array.
            Collider2D[] enemiesInView = Physics2D.OverlapCircleAll(playerCamera.transform.position, playerCamera.orthographicSize, enemyLayer);

            // For each collider found above, destroy it and update the player's score.
            foreach (Collider2D enemyCollider in enemiesInView)
            {
                if (enemyCollider.tag == "Enemy")
                {
                    enemyCollider.GetComponent<EnemyBehavior>().EnemyTakeDamage(damagePlayerDeals);
                }
                else if (enemyCollider.tag == "GhostSpawner")
                {
                    enemyCollider.GetComponent<SpawnerBehavior>().TookDamage(damagePlayerDeals);
                }
                PlayerScoreChange(enemyDeadScore);
            }

            // Once the above items are completed, subtract a potion from the player and update the potion UI. Also, give the player health and update the UI.
            potionInPossession--;
            gM.ChangePotionUI(potionInPossession);

            //When adding to the player's health, make sure it doesn't go over the maxHealth(2000).
            playerHealth += healthPlayerGains;
            if (playerHealth >= maxHealth)
            {
                playerHealth = maxHealth;
            }
            HealthUIChange();

            //Debug.Log("Potion used. Remaining potions: " + potionInPossession);
        }
        //else
        //{
        //    Debug.Log("No potions available!"); // Here in case something goes wrong.
        //}
    }    

    // Function that plays the hit enemy sound. Called in attack script so only have to assign audio on player.
    public void PlayHitEnemy()
    {
        hitEnemy.Play();
    }

    public void PlayAttackHit()
    {
        attackHit.Play();
    }
}