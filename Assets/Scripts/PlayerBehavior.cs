using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameObject gM;
    float playerMoveSpeed = 3;

    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager");       
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.name)
        {
            case "ExitOne":
                gM.GetComponent<GameManager>().ChangeScene("LevelOne");
                break;
            case "ExitTwo":
                gM.GetComponent<GameManager>().ChangeScene("LevelTwo");
                break;
        }
    }

    void PlayerMovement()
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * playerMoveSpeed);
    }
}
