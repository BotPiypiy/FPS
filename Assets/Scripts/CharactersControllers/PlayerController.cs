using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//сдлетаь синхронизирующие функции, ответки не должны воздействовать на игрока, который послал команду

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

    private float verticalVelocity;

    [Header("Network")]
    [SerializeField]
    private float syncDelay;
    private float syncTime;


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
            InputManager.Instance.SetCamera(camera);

            verticalVelocity = 0;

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
        if (isServer)
        {
            Fall();
            CmdIgnoreRigidbody();
        }
        else if(isClient)
        {
            RpcSyncPos(transform.position);
        }
    }

    [Server]
    private void CmdIgnoreRigidbody()
    {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        RpcIgnoreRigidbody();
    }

    [ClientRpc]
    private void RpcIgnoreRigidbody()
    {
        if(isClientOnly)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    [ClientRpc]
    private void RpcTranslate(Vector3 pos)
    {
        transform.position = pos;
    }

    [ClientRpc]
    private void RpcRotate(Quaternion rot)
    {
        if (isClientOnly && !isLocalPlayer)
        {
            gameObject.transform.rotation = rot;
        }
    }

    [TargetRpc]
    private void RpcSyncPos(Vector3 pos)
    {
        transform.position = pos;
    }

    [Command]
    public void CmdMove(Vector3 step)
    {
        transform.position += step.normalized * moveSpeed * Time.deltaTime;
        RpcTranslate(transform.position);
    }

    [Client]
    public void Move(Vector3 step)
    {
        if(isClientOnly)
        {
            transform.position += step.normalized * moveSpeed * Time.deltaTime;
        }
    }

    [Command]
    public void CmdRotateX(float angle)
    {
        transform.Rotate(Vector3.up * angle);
        RpcRotate(transform.rotation);
    }

    [Client]
    public void RotateX(float angle)
    {
        if (isClientOnly)
        {
            transform.Rotate(Vector3.up * angle);
        }
    }

    [Command]
    public void CmdRotateY(float angle)
    {
        camera.Rotate(angle);
        RpcRotateY(camera.transform.localRotation);
    }

    [ClientRpc]
    private void RpcRotateY(Quaternion rot)
    {
        if (isClientOnly && !isLocalPlayer)
        {
            camera.gameObject.transform.localRotation = rot;
        }
    }

    [Client]
    public void RotateY(float angle)
    {
        if (isClientOnly)
        {
            camera.Rotate(angle);
        }
    }

    [Server]
    private void Fall()
    {
        if (IsGround() && verticalVelocity < 0)
        {
            verticalVelocity = 0;
        }
        else if (!IsGround())
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
        RpcTranslate(transform.position);
    }

    [Command]
    public void CmdJump()
    {
        if (IsGround())
        {
            verticalVelocity = Mathf.Sqrt((jumpHeight + jumpHeight) * (-Physics.gravity.y));
            transform.position += Vector3.up * verticalVelocity * Time.deltaTime;
            RpcTranslate(transform.position);
        }
    }

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
            Vector3 rotation = new Vector3(camera.transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
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
            InputManager.Instance.SetCamera(null);
        }
    }
}
