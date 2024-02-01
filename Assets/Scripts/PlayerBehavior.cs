using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private GameManager gM;

    // Bools to say what class the player chose on the main menu.
    private bool warriorClass = false;
    private bool valkyrieClass = false;
    private bool rangerClass = true;
    private bool wizardClass = false;

    // Class specific variables.
    private float playerMoveSpeed;
    private GameObject playerWeapon;
    private float playerAttackSpeed;
    [HideInInspector] public int ghostDamagePlayerTakes;

    //Each class variables begin.

    // Warrior class variables.
    private float warriorMoveSpeed = 3.5f;
    public GameObject warriorWeapon;
    private float warriorAttackSpeed = 5.5f;
    private int ghostDamageWarriorTakes = 8;

    // Valkyrie class variables.
    private float valkyrieMoveSpeed = 3.5f;
    public GameObject valkyrieWeapon;
    private float valkyrieAttackSpeed = 5.5f;
    private int ghostDamageValkyrieTakes = 7;

    // Ranger class variables.
    private float rangerMoveSpeed = 3.5f;
    public GameObject rangerWeapon;
    private float rangerAttackSpeed = 7.5f;
    private int ghostDamageRangerTakes = 9;

    // Wizard class variables.
    private float wizardMoveSpeed = 3.5f;
    public GameObject wizardWeapon;
    private float wizardAttackSpeed = 5.5f;
    private int ghostDamageWizardTakes = 10;

    // Each class variables end.

    // Player variables that remain the same no matter the class.
    private int playerHealth = 2000;
    private int playerScore = 0;    
    private int playerGradualHealthLoss = 1;
    private float playerAttackCooldown = .7f;
    private float lastAttackTime;
    private int enemyDeadScore = 10;

    // Item related variables for key, potion, healing item, and chest.
    private int keyInPossession = 0;
    public int potionInPossession = 0;
    private int healthPlayerGains = 100;
    private int chestScore = 100;

    private Transform playerTransform;
    private Vector2 lastFacingDirection = Vector2.right;

    // Variables used for player sprite and animation display.
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    private SpriteRenderer spriteRenderer;
    private Animator playerAnimator;

    // Variables responsible for making potion attack work.
    public Camera playerCamera;
    public LayerMask enemyLayer;

    void Awake()
    {
        SetClassSpecificVariables();
    }
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();

        InvokeRepeating("GradualHealthDepletion", 1f, 1f);

        playerTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

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

        if (playerHealth <= 0)
        {
            gM.GameOver();
            CancelInvoke();
        }

        Debugging();
    }

    // Depending on which class the player chose, the values for some variables differ.
    // The function below sets the variable values based on what class was chosen by the player.
    void SetClassSpecificVariables()
    {
        if (warriorClass)
        {
            // Warrior class, so set warrior variables as the values to be used by the player.
            playerMoveSpeed = warriorMoveSpeed;
            playerWeapon = warriorWeapon;
            playerAttackSpeed = warriorAttackSpeed;
            ghostDamagePlayerTakes = ghostDamageWarriorTakes;
        }
        else if (valkyrieClass)
        {
            // Valkyrie class, so set valkyrie variables as the values to be used by the player.
            playerMoveSpeed = valkyrieMoveSpeed;
            playerWeapon = valkyrieWeapon;
            playerAttackSpeed = valkyrieAttackSpeed;
            ghostDamagePlayerTakes = ghostDamageValkyrieTakes;
        }
        else if (rangerClass)
        {
            // Ranger class, so set ranger variables as the values to be used by the player.
            playerMoveSpeed = rangerMoveSpeed;
            playerWeapon = rangerWeapon;
            playerAttackSpeed = rangerAttackSpeed;
            ghostDamagePlayerTakes = ghostDamageRangerTakes;
        }
        else if (wizardClass)
        {
            // Wizard class, so set wizard variables as the values to be used by the player.
            playerMoveSpeed = wizardMoveSpeed;
            playerWeapon = wizardWeapon;
            playerAttackSpeed = wizardAttackSpeed;
            ghostDamagePlayerTakes = ghostDamageWizardTakes;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "ExitOne":
                // First exit collided with.
                gM.ChangeScene("LevelOnePrototype");
                break;
            case "ExitTwo":
                // Second exit collided with.
                gM.ChangeScene("LevelTwo");
                break;
            case "Chest":
                // Chest item collided with.
                PlayerScoreChange(chestScore);
                Destroy(other.gameObject);
                break;
            case "Healing":
                // Healing item collided with.
                PlayerHealthChange(healthPlayerGains);
                if (playerHealth >= 2000)
                {
                    playerHealth = 2000;
                }
                //HealthUIChange();
                Destroy(other.gameObject);
                break;
            case "Key":
                // Key collided with.
                keyInPossession += 1;
                Debug.Log($"Keys: {keyInPossession}");
                gM.ChangeKeysUI(keyInPossession);
                Destroy(other.gameObject);
                break;
            case "Potion":
                // Potion attack item collided with.
                potionInPossession += 1;
                Debug.Log($"Potion: {potionInPossession}");
                gM.ChangePotionUI(potionInPossession);
                Destroy(other.gameObject);
                break;
            case "Door":
                // Door has been collided with.
                if (keyInPossession >= 1)
                {
                    keyInPossession --;
                    Debug.Log($"Keys: {keyInPossession}");
                    gM.ChangeKeysUI(keyInPossession);
                    Destroy(other.gameObject);
                }
                break;
        }
    }

    void PlayerMovement()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // Enabling the normalize makes sure that diagonal movement is not quicker.
        // Have it commented out because original game has quicker diagonal movement.
        //movement.Normalize();
        transform.Translate(movement * Time.deltaTime * playerMoveSpeed);
    
        // Run the update sprite function so the sprite changes based on the movement direction.
        UpdateSprite();

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
        
        if (playerAttackRb != null)
        {
            // Get the player's movement direction to use for calculations.
            //Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            // Set the direction the attack moves in the direction the player is facing.
            playerAttackRb.velocity = lastFacingDirection * playerAttackSpeed;

            // Calculate the angle based on the movement direction of the player.
            // Once calculated, set the player's attack to that rotation.
            float angle = Mathf.Atan2(lastFacingDirection.x, -lastFacingDirection.y) * Mathf.Rad2Deg;
            playerAttack.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            Debug.LogWarning("Rigidbody2D not found on player attack.");
        }
        lastAttackTime = Time.time;        
    }

    void GradualHealthDepletion()
    {
        playerHealth -= playerGradualHealthLoss;
        Debug.Log(playerHealth);
        HealthUIChange();
    }

    public void PlayerScoreChange(int scoreAmount)
    {
        playerScore += scoreAmount;
        Debug.Log($"Score is {playerScore}");
        ScoreUIChange();
    }

    public void PlayerHealthChange(int healthAmount)
    {
        playerHealth -= healthAmount;
        Debug.Log($"Lost {healthAmount}, at {playerHealth} now.");
        HealthUIChange();
    }

    public void ScoreUIChange()
    {
        gM.ChangeScoreText(playerScore);
    }

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

     void UpdateSprite()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float inputAmount = 0.02f;

        // Based on the direction of movement, set the sprite for that direction.
        if (horizontalInput < -inputAmount)
        {
            // Moving left.
            SetAnimatorBools(true, false, false, false);
            //spriteRenderer.sprite = leftSprite;
        }
        else if (horizontalInput > inputAmount)
        {
            // Moving right.
            SetAnimatorBools(false, true, false, false);
            //spriteRenderer.sprite = rightSprite;
        }
        else if (verticalInput > inputAmount)
        {
            // Moving up.
            SetAnimatorBools(false, false, true, false);
            //spriteRenderer.sprite = upSprite;
                       
        }
        else if (verticalInput < -inputAmount)
        {
            // Moving down.
            SetAnimatorBools(false, false, false, true);
            //spriteRenderer.sprite = downSprite;
        }
    }

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
            if (potionInPossession >= 3)
            {
                potionInPossession = 3;
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
            if (keyInPossession >= 4)
            {
                keyInPossession = 4;
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

        if(Input.GetKeyDown(KeyCode.H))
        {
            playerHealth -= 100;
            HealthUIChange();
            
            if (playerHealth <= 0)
            {
                playerHealth = 0;
                HealthUIChange();
            }
        }        
    }

    public void SetPlayerClass(bool warriorChosen, bool valkyrieChosen, bool rangerChosen, bool wizardChosen)
    {
        warriorClass = warriorChosen;
        valkyrieClass = valkyrieChosen;
        rangerClass = rangerChosen;
        wizardClass = wizardChosen;
    }

    void PotionAttack()
    {
        if (potionInPossession > 0)
        {
            Collider2D[] enemiesInView = Physics2D.OverlapCircleAll(playerCamera.transform.position, playerCamera.orthographicSize, enemyLayer);

            foreach (Collider2D enemyCollider in enemiesInView)
            {
                Destroy(enemyCollider.gameObject);
                PlayerScoreChange(enemyDeadScore);
            }

            potionInPossession--;
            gM.ChangePotionUI(potionInPossession);
            Debug.Log("Potion used. Remaining potions: " + potionInPossession);
        }
        else
        {
            Debug.Log("No potions available!");
        }
    }    
}