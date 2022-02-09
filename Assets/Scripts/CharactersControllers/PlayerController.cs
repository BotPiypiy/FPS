using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : EntityController
{
    [SerializeField]
    private CameraController camera;       //main camera

    [SerializeField]
    private float changeDelay;      //time needed for changing weapon

    [SerializeField]
    [Header("Weapons")]
    private GameObject[] weapons;       //all player's weapons

    private int weaponIndex;        //index of current choosen weapon

    private Vector3 velocity;

    public void Start()
    {
        Initializate();
    }

    [Client]
    protected override void Initializate()
    {
        if (isClient && isLocalPlayer)
        {
            InputManager.Instance.SetPlayer(this);

            velocity = Vector3.zero;

            weaponIndex = 0;
            //ActivateChoosenWeapon();

            DefaultCamera.Instance.SwitchCamera();
            UIManager.Instance.SwitchSpawnFrame();
        }
        else
        {
            camera.gameObject.GetComponent<Camera>().enabled = false;
            camera.gameObject.GetComponent<AudioListener>().enabled = false;
        }
    }

    private void Update()
    {
        Fall();
        PhysicsMove();
        if (isServer)
        {
            IgnoreRigidbody();
            RpcTranslate(transform.position);
        }
    }

    #region Ignore Rgigidbody
    [Server]
    private void IgnoreRigidbody()
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        RpcIgnoreRigidbody();
    }

    [ClientRpc]
    private void RpcIgnoreRigidbody()
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    #endregion

    #region Rpc Sync
    [ClientRpc]
    private void RpcTranslate(Vector3 pos)
    {
        if (false)                          //если слишком большая дистанция ??? (надо придумать как вычислять дистанцию
                                            //или задать константу)
        {
            transform.position = pos;
        }
    }

    [ClientRpc]
    private void RpcRotate(Quaternion rot)
    {
        gameObject.transform.rotation = rot;
    }
    #endregion

    #region Move
    [Command]
    public void CmdMove(Vector3 step)
    {
        transform.position += step.normalized * moveSpeed * Time.deltaTime;
        RpcMove(transform.position);
    }

    [ClientRpc]
    public void RpcMove(Vector3 pos)
    {
        if (!isLocalPlayer)
        {
            transform.position = pos;
        }
    }

    [Client]
    public void Move(Vector3 step)
    {
        if (isClientOnly)
        {
            transform.position += step.normalized * moveSpeed * Time.deltaTime;
        }

        CmdMove(step);
    }
    #endregion

    #region Rotate
    [Command]
    public void CmdRotateX(float angle)
    {
        transform.Rotate(Vector3.up * angle * camera.gameObject.GetComponent<CameraController>().GetSensevity());
        RpcRotateX(transform.rotation);
    }

    [ClientRpc]
    public void RpcRotateX(Quaternion rot)
    {
        if (!isLocalPlayer)
        {
            transform.rotation = rot;
        }
    }

    [Client]
    public void RotateX(float angle)
    {
        if (isClientOnly)
        {
            transform.Rotate(Vector3.up * angle * camera.gameObject.GetComponent<CameraController>().GetSensevity());
        }

        CmdRotateX(angle);
    }

    [Command]
    public void CmdRotateY(float angle)
    {
        camera.Rotate(angle);
        RpcRotateY(camera.transform.rotation);
    }

    [ClientRpc]
    private void RpcRotateY(Quaternion rot)
    {
        if (!isLocalPlayer)
        {
            camera.gameObject.transform.rotation = rot;
        }
    }

    [Client]
    public void RotateY(float angle)
    {
        if (isClientOnly)
        {
            camera.Rotate(angle);
        }

        CmdRotateY(angle);
    }
    #endregion

    #region Fall
    private void Fall()
    {
        if (IsGround() && velocity.y < 0)
        {
            velocity.y = 0;
        }
        else if (!IsGround())
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }
    }
    #endregion

    #region Jump
    [Command]
    private void CmdJump()
    {
        velocity.y += Mathf.Sqrt((jumpHeight + jumpHeight) * (-Physics.gravity.y));
        RpcJump(velocity.y);
    }

    [ClientRpc]
    private void RpcJump(float y)
    {
        if (!isLocalPlayer)
        {
            velocity.y = y;
        }
    }

    [Client]
    public void Jump()
    {
        if (IsGround())
        {
            if (isClientOnly)
            {
                velocity.y += Mathf.Sqrt((jumpHeight + jumpHeight) * (-Physics.gravity.y));
            }
            CmdJump();
        }
    }
    #endregion

    #region Force and velocity
    private void PhysicsMove()
    {
        transform.position += velocity * Time.deltaTime;
    }
    #endregion

    [Client]
    public void SwitchWeapon()
    {
        int currentWeapon = weaponIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponIndex = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponIndex = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            weaponIndex = 2;
        }

        if(weaponIndex != currentWeapon)
        {
            StartCoroutine(ChangeWeapon(weaponIndex));
            weaponIndex = weapons.Length;
            ActivateChoosenWeapon();
        }
    }

    [Client]
    private void ActivateChoosenWeapon()
    {
        if(weaponIndex == weapons.Length)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (i == weaponIndex)
                {
                    weapons[i].SetActive(true);
                }
                else
                {
                    weapons[i].SetActive(false);
                }    
            }
        }
    }

    [Client]
    private IEnumerator ChangeWeapon(int indexWeapon)
    {
        yield return new WaitForSeconds(changeDelay);

        weaponIndex = indexWeapon;

        ActivateChoosenWeapon();
    }

    public override void Shoot()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 rotation = camera.transform.localEulerAngles;
            weapons[weaponIndex].GetComponent<Weapon>().Shoot(firePoint.position, rotation);
        }
    }

    [Client]
    protected override void Dead()
    {
        GameController.ReloadScene();
    }

    [Client]
    private void OnDestroy()
    {
        if (isClient && isLocalPlayer)
        {
            InputManager.Instance.SetPlayer(null);
        }
    }
}
