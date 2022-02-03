using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraController : MonoBehaviour
{
    [SerializeField][Header("Mouse sensevity")]
    private float sensevity;

    [SerializeField]
    private PlayerController player;

    private float cameraRotation;       //current camera rotation

    private void Start()
    {
        Initializate();
    }

    void Update()
    {
        Look();
    }

    [Client]
    private void Initializate()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public float GetSensevity()
    {
        return sensevity;
    }

    [Client]
    private void Look()
    {

        if(Input.GetKeyDown(KeyCode.Escape))
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

    [Client]
    public void Rotate(float angle)
    {
        cameraRotation -= angle;
        cameraRotation = Mathf.Clamp(cameraRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(cameraRotation, 0, 0);
    }
}
