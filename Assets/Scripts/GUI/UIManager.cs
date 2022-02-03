using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    GameObject spawnFrame;

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
    public void SwitchSpawnFrame()
    {
        spawnFrame.SetActive(!spawnFrame.activeSelf);
    }
}
