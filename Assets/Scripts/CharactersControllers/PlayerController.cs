using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Runtime.Serialization.Formatters.Binary;

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

    Coroutine weaponChange;

    public void Start()
    {
        Initializate();
    }

    protected override void Initializate()
    {
        if (isClient && isLocalPlayer)
        {
            InputManager.Instance.SetPlayer(this);

            velocity = Vector3.zero;

            weaponIndex = 1;
            SwitchWeapon(0);

            DefaultCamera.Instance.SwitchCamera();
            UIManager.Instance.SwitchSpawnFrame();
        }
        else if(isClient && !isLocalPlayer)
        {
            SwitchWeapon(weapons.Length + 1);
            camera.gameObject.GetComponent<Camera>().enabled = false;
            camera.gameObject.GetComponent<AudioListener>().enabled = false;
        }
    }

    private void Update()
    {
        Fall();
        IgnoreRigidbody();
        PhysicsMove();
        if (isServer)
        {
            RpcTranslate(transform.position);
        }
    }

    #region Ignore Rgigidbody
    private void IgnoreRigidbody()
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    #endregion

    #region Rpc Sync
    [ClientRpc]
    private void RpcTranslate(Vector3 pos)
    {
        if (Vector3.Distance(transform.position, pos) > moveSpeed)
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
    private void CmdMove(Vector3 pos)
    {
        if (Vector3.Distance(transform.position, pos) > moveSpeed)
        {
            TRpcMove(transform.position);
        }
        else
        {
            transform.position = pos;
            RpcMove(transform.position);
        }
    }
   
    [ClientRpc]
    private void RpcMove(Vector3 pos)
    {
        if (!isLocalPlayer)
        {
            transform.position = pos;
        }
    }
    
    [TargetRpc]
    private void TRpcMove(Vector3 pos)
    {
        transform.position = pos;
    }

    [Client]
    public void Move(Vector3 step)
    {
        transform.position += step.normalized * moveSpeed * Time.deltaTime;
        CmdMove(transform.position);
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

    #region Weapon
    [Command]
    private void CmdSwitchWeapon(int index)
    {
        if (weaponIndex > weapons.Length)
        {
            RpcSwitchWeapon(weaponIndex);
        }
        else
        {
            if (weaponIndex != index)
            {
                if (weaponIndex == weapons.Length)
                {
                    StopCoroutine(weaponChange);
                }
                weaponChange = StartCoroutine(SChangeWeapon(index));
                weaponIndex = weapons.Length;
            }
            RpcSwitchWeapon(weaponIndex);
            ActivateChoosenWeapon();
        }
    }

    [ClientRpc]
    private void RpcSwitchWeapon(int index)
    {
        if(!isLocalPlayer)
        {
            weaponIndex = index;
        }

        ActivateChoosenWeapon();
    }

    [Client]
    public void SwitchWeapon(int index)
    {
        CmdSwitchWeapon(index);

        if (isClientOnly)
        {
            if (weaponIndex != index)
            {
                if (weaponIndex == weapons.Length)
                {
                    StopCoroutine(weaponChange);
                }
                weaponChange = StartCoroutine(ChangeWeapon(index));
                weaponIndex = weapons.Length;
            }
            ActivateChoosenWeapon();
        }
    }

    private void ActivateChoosenWeapon()
    {
        if (weaponIndex == weapons.Length)
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

    [Server]
    private IEnumerator SChangeWeapon(int indexWeapon)
    {
        yield return new WaitForSeconds(changeDelay);

        weaponIndex = indexWeapon;

        RpcSwitchWeapon(weaponIndex);
        ActivateChoosenWeapon();
    }

    [Client]
    private IEnumerator ChangeWeapon(int indexWeapon)
    {
        yield return new WaitForSeconds(changeDelay);

        weaponIndex = indexWeapon;

        ActivateChoosenWeapon();
    }
    #endregion

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
        if (isLocalPlayer)
        {
            //NetworkClient.Disconnect();
        } 
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
