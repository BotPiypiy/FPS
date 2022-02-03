using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public static MyNetworkManager Instance;

    public override void Awake()
    {
        base.Awake();
        Initializate();
    }

    private void Initializate()
    {
        if(!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        UIManager.Instance.SwitchSpawnFrame();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Cursor.lockState = CursorLockMode.None;
    }
}
