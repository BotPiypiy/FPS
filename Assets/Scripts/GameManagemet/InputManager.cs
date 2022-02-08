using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InputManager : MonoBehaviour
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
            InputCamera();
        }
    }

    [Client]
    private void InputMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 movePos = player.transform.right * moveX + player.transform.forward * moveZ;
    
        player.Move(movePos);
    }

    [Client]
    private void InputCamera()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");

            player.RotateX(mouseX);
            player.RotateY(mouseY);
        }
    }
}
