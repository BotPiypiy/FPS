using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DefaultCamera : MonoBehaviour
{
    public static DefaultCamera Instance;

    private void Awake()
    {
        Initializate();
    }

    private void Initializate()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Client]
    public void SwitchCamera()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
