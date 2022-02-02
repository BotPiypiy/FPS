using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        Initializate();
    }

    private void Update()
    {
        Move();
        Jump();
        SwitchWeapon();
        Shoot();
    }

    protected override void Initializate()
    {
        base.Initializate();

        weaponIndex = 0;
        ActivateChoosenWeapon();

        playerTransform = gameObject.GetComponent<Transform>();
    }

    protected override void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime;
        Vector3 movePos = transform.right * moveX + transform.forward * moveZ;

        characterController.Move(movePos);
    }

    protected override void Jump()
    {
        Landing();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(IsGround())
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -Physics.gravity.y);
            }
        }

        Fall();
    }

    private void SwitchWeapon()
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

    private IEnumerator ChangeWeapon(int indexWeapon)
    {
        yield return new WaitForSeconds(changeDelay);

        weaponIndex = indexWeapon;

        ActivateChoosenWeapon();
    }

    protected override void Shoot()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 rotation = new Vector3(camera.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
            weapons[weaponIndex].GetComponent<Weapon>().Shoot(firePoint.position, rotation);
        }
    }

    protected override void Dead()
    {
        GameController.ReloadScene();
    }

}
