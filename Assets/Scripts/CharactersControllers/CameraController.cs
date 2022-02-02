using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField][Header("Mouse sensevity")]
    private float sensevity;

    [SerializeField]
    private Transform player;

    private float cameraRotation;       //current camera rotation

    private void Awake()
    {
        Initializate();
    }

    void Update()
    {
        Look();
    }

    private void Initializate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraRotation = 0f;
    }

    private void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensevity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensevity * Time.deltaTime;

        player.transform.Rotate(Vector3.up * mouseX);

        cameraRotation -= mouseY;
        cameraRotation = Mathf.Clamp(cameraRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(cameraRotation, 0, 0);
    }
}
