using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StatsManager : NetworkBehaviour
{
    public static StatsManager Instance;

    [Header("Player stats")]
    [SerializeField]
    private float playerSpeed;
    [SyncVar]
    private float _playerSpeed;
    [SerializeField]
    private float jumpForce;
    [SyncVar]
    private float _jumpForce;

    public float PlayerSpeed
    {
        get { return _playerSpeed; }
    }

    public float JumpForce
    {
        get { return _jumpForce; }
    }

    private void Awake()
    {
        Initializate();
    }

    private void Initializate()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StatsInit()
    {
        if (isServer)
        {
            _playerSpeed = playerSpeed;
            _jumpForce = jumpForce;
        }
    }
}
