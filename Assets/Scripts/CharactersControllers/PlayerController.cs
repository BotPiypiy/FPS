using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : EntityController
{
    public static Transform playerTransform;        //player transform for enemies

    [SerializeField]
    private Transform camera;       //main camera

    [SerializeField]
    private float changeDelay;      //time needed for changing weapon

    [SerializeField]
    [Header("Weapons")]
    private GameObject[] weapons;       //all player's weapons

    private int weaponIndex;        //index of current choosen weapon

    private float verticalVelocity;

    public void Start()
    {
        Initializate();
        NetInitializate();
    }

    [Client]
    private void NetInitializate()
    {
        if (isClient && isLocalPlayer)
        {
            InputManager.Instance.SetPlayer(this);
            InputManager.Instance.SetCamera(camera.gameObject.GetComponent<CameraController>());
        }
    }

    [Client]
    protected override void Initializate()
    {
        if (isClient && isLocalPlayer)
        {
            verticalVelocity = 0;

            weaponIndex = 0;
            //ActivateChoosenWeapon();

            playerTransform = gameObject.GetComponent<Transform>();
            DefaultCamera.Instance.SwitchCamera();
            UIManager.Instance.SwitchSpawnFrame();
        }
        else if(isClient)
        {
            camera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        CmdFall();
    }

    [ClientRpc]
    private void RpcTranslate(Vector3 pos)
    {
        if(isClientOnly)
        {
            transform.position = pos;
        }
    }

    [Command]
    public void CmdMove(Vector3 step)
    {
        transform.position += step.normalized * moveSpeed * Time.deltaTime;
        RpcTranslate(transform.position);
    }

    [Command]
    public void CmdRotate(float angle)
    {
        transform.Rotate(Vector3.up * angle);
        RpcRotate(angle);
    }

    [ClientRpc]
    public void RpcRotate(float angle)
    {
        if (isClientOnly)
        {
            transform.Rotate(Vector3.up * angle);
        }
    }

    [Command]
    private void CmdFall()
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
            Vector3 rotation = new Vector3(camera.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
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
