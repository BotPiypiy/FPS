using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InputManager : NetworkBehaviour
{
    private PlayerController player;

    public static InputManager Instance;


    private void Start()
    {
        Initializate();
    }

    private void Initializate()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Client]
    public void SetPlayer(PlayerController playerController)
    {
        player = playerController;
    }

    private void Update()
    {
        if (player)
        {
            InputMove();
            InputJump();
            InputView();
            InputCursor();
        }
    }

    [Client]
    private void InputMove()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            Vector3 movePos = player.transform.right * moveX + player.transform.forward * moveZ;

            player.Move(movePos);
        }
    }

    [Client]
    private void InputJump()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.Jump();
            }
        }
    }

    [Client]
    private void InputView()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime;

            player.RotateX(mouseX);
            player.RotateY(mouseY);
        }
    }

    [Client]
    private void InputCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
