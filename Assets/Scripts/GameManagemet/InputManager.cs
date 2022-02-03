using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InputManager : MonoBehaviour
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
        }
        if (camera)
        {
            InputCamera();
        }
    }

    [Client]
    private void InputMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 movePos = player.transform.right * moveX + player.transform.forward * moveZ;

        player.CmdMove(movePos);
        //player.Move(movePos);
    }

    [Client]
    private void InputCamera()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * camera.GetSensevity() * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * camera.GetSensevity() * Time.deltaTime;

            player.CmdRotate(mouseX);
            //player.Rotate(mouseX);

            camera.Rotate(mouseY);
        }
    }
}
