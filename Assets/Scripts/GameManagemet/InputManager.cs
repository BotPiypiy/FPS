using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InputManager : NetworkBehaviour
{
    private PlayerController player;
    private CameraController camera;

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

    [Client]
    public void SetCamera(CameraController cameraController)
    {
        camera = cameraController;
    }

    private void Update()
    {
        if (player)
        {
            InputMove();
            InputJump();
            InputView();
        }

        InputCursor();
    }

    [Client]
    private void InputMove()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            Vector3 movePos = player.transform.right * moveX + player.transform.forward * moveZ;

            player.CmdMove(movePos);
        }
    }

    [Client]
    private void InputJump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            player.CmdJump();
        }
    }

    [Client]
    private void InputView()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * camera.GetSensevity() * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * camera.GetSensevity() * Time.deltaTime;

            player.CmdRotateX(mouseX);
            player.CmdRotateY(mouseY);
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
            return;
        }
    }
}
