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
        }
    }

    [Client]
    protected override void Initializate()
    {
        if (isClient && isLocalPlayer)
        {
            base.Initializate();

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
    public override void Move(Vector3 step)
    {
        if (isClientOnly)
        {
            transform.position += step.normalized * moveSpeed * Time.deltaTime;
        }

        CmdMove(step);
    }

    [Command]
    public void CmdRotateX(float angle)
    {
        transform.Rotate(Vector3.up * angle * camera.gameObject.GetComponent<CameraController>().GetSensevity() * Time.deltaTime);
        RpcRotateX(transform.rotation);
    }

    [ClientRpc]
    public void RpcRotateX(Quaternion rot)
    {
        if(!isLocalPlayer)
        {
            transform.rotation = rot;
        }
    }

    [Client]
    public void RotateX(float angle)
    {
        if (isClientOnly)
        {
            transform.Rotate(Vector3.up * angle * camera.gameObject.GetComponent<CameraController>().GetSensevity() * Time.deltaTime);
        }

        CmdRotateX(angle);
    }

    [Command]
    public void CmdRotateY(float angle)
    {
        camera.gameObject.GetComponent<CameraController>().Rotate(angle);
        RpcRotateY(camera.rotation);
    }

    [ClientRpc]
    public void RpcRotateY(Quaternion rot)
    {
        if (!isLocalPlayer)
        {
            camera.rotation = rot;
        }
    }

    [Client]
    public void RotateY(float angle)
    {
        if (isClientOnly)
        {
            camera.gameObject.GetComponent<CameraController>().Rotate(angle);
        }

        CmdRotateY(angle);
    }

    [Client]
    public override void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(IsGround())
            {
                rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }
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
        }
    }
}
