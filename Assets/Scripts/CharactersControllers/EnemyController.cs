using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityController
{
    [SerializeField]
    private float attackDelay;          //time between enemy try shooting player
    private float attackTime;           //time of last shoot

    [SerializeField]
    [Header("Weapons")]
    private GameObject[] weapons;       //all avaible weapons

    private Weapon weapon;          //randomly choosen weapon

    private void Awake()
    {
        Initializate();
    }

    private void Update()
    {
        //Move();
        //Fall();
        //Jump();
        //transform.LookAt(PlayerController.playerTransform);
        //Shoot();
    }

    protected override void Initializate()
    {
        WeaponInit();

        attackTime = Time.time + attackDelay;
    }

    private void WeaponInit()       //randomly choosing weapon and destroying others
    {
        int rnd = Random.Range(0, weapons.Length);
        weapon = weapons[rnd].GetComponent<Weapon>();

        for(int i = 0; i<weapons.Length; i++)
        {
            if(i != rnd)
            {
                Destroy(weapons[i]);
            }
        }
    }

    public override void Shoot()
    {
        if (Time.time > attackTime + attackDelay)
        {
            weapon.Shoot(firePoint.position, transform.localEulerAngles);
        }
    }

    protected override void Dead()
    {
        Destroy(gameObject);
    }
}
