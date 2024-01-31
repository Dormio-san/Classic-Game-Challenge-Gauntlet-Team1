using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private GameManager gM;
    private float playerMoveSpeed = 3;
    private int playerHealth = 2000;
    private int playerScore = 0;
    public GameObject playerWeapon;
    private int playerGradualHealthLoss = 1;
    private float playerAttackCooldown = .8f;
    private float lastAttackTime;
    [HideInInspector] public float playerAttackSpeed;
    private int keyInPossession = 0;
    private int potionInPossession = 0;
    private int healthPlayerGains = 100;
    private int chestScore = 100;
    private Transform playerTransform;
    private Vector2 lastFacingDirection = Vector2.right;
    // Sprites for the different directions the player is moving.
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    private SpriteRenderer spriteRenderer;
    private Animator playerAnimator;

    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        SetClassSpecificVariables();
        InvokeRepeating("GradualHealthDepletion", 1f, 1f);
        playerTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        PlayerMovement();        

        if (Input.GetKey(KeyCode.Space) && CanAttack())
        {
            SpawnPlayerWeapon();
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
        // Elf
        playerAttackSpeed = 6.5f;
        //ghostDamagePlayerTakes = 7;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "ExitOne":
                // First exit collided with.
                gM.ChangeScene("LevelOne");
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
                //gM.ChangeHealthText(playerHealth);
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
        gM.ChangeHealthText(playerHealth);
    }

    public void PlayerScoreChange(int scoreAmount)
    {
        playerScore += scoreAmount;
        Debug.Log($"Score is {playerScore}");
        gM.ChangeScoreText(playerScore);
    }

    public void PlayerHealthChange(int healthAmount)
    {
        playerHealth += healthAmount;
        Debug.Log($"Lost {healthAmount}, at {playerHealth} now.");
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

        
    }
}