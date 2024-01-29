using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private GameManager gM;
    float playerMoveSpeed = 3;
    int playerHealth = 2000;
    int playerScore = 0;
    public GameObject playerWeapon;
    int playerGradualHealthLoss = 1;
    float playerAttackCooldown = .8f;
    float lastAttackTime;
    [HideInInspector] public float playerAttackSpeed;
    int keyInPossession = 0;
    int potionInPossession = 0;
    int healthPlayerGains = 100;
    int chestScore = 100;
    Transform playerTransform;
    Vector2 lastFacingDirection = Vector2.right;
    // Sprites for the different directions the player is moving.
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    private SpriteRenderer spriteRenderer;

    void Start()
    {        
        GameObject gameManagerObject = GameObject.Find("GameManager");
        gM = gameManagerObject.GetComponent<GameManager>();
        SetClassSpecificVariables();
        InvokeRepeating("GradualHealthDepletion", 1f, 1f);
        playerTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        PlayerMovement();

        

        if (Input.GetKeyDown(KeyCode.Space) && CanAttack())
        {
            SpawnPlayerWeapon();
        }

        
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
                //gM.ChangeKeysUI(keyInPossession);
                Destroy(other.gameObject);
                break;
            case "Potion":
                // Potion attack item collided with.
                potionInPossession += 1;
                Debug.Log($"Potion: {potionInPossession}");
                //gM.ChangePotionUI(potionInPossession);
                Destroy(other.gameObject);
                break;
            case "Door":
                // Door has been collided with.
                if (keyInPossession >= 1)
                {
                    keyInPossession --;
                    Debug.Log($"Keys: {keyInPossession}");
                    //gM.ChangeKeysUI(keyInPossession);
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
        //gM.ChangeHealthText(playerHealth);
    }

    public void PlayerScoreChange(int scoreAmount)
    {
        playerScore += scoreAmount;
        Debug.Log($"Score is {playerScore}");
        //gM.ChangeScoreText(playerScore);
    }

    public void PlayerHealthChange(int healthAmount)
    {
        playerHealth += healthAmount;
        Debug.Log($"Lost {healthAmount}, at {playerHealth} now.");
        //gM.ChangeHealthText(playerHealth);
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

        // Based on the direction of movement, set the sprite for that direction.
        if (horizontalInput > 0.15f)
        {
            spriteRenderer.flipX = horizontalInput > 0.15f;
        }
        else if (horizontalInput < -0.15f)
        {
            spriteRenderer.flipX = horizontalInput < -0.15f;
        }
        else if (verticalInput > 0.15f)
        {
            spriteRenderer.sprite = upSprite;
        }
        else if (verticalInput < -0.15f)
        {
            spriteRenderer.sprite = downSprite;
        }
    }
}
