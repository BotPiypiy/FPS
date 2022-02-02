using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    [Header("Bullet prefab")]
    GameObject bullet;

    [Header("Weapon stats")]
    [SerializeField]
    private int damage;
    [SerializeField]
    private float fireDelay;            //time between shoots
    private float shootTime;            //last shoot time
    [SerializeField]
    private float spray;                //spray
    [SerializeField]
    private float sprayDelay;           //spray time
    [SerializeField]
    private float shootDistance;        //living time of bullets (in sec)
    [SerializeField]
    private float reloadDelay;          //time for reload
    [SerializeField]
    private int maxAmmo;                //max ammo
    private int ammo;                   //current ammo

    private Coroutine reloadCoroutine;      //coroutine of reloading, needed to stop reloading, if player switching another weapon

    private void Awake()
    {
        Initializate();
    }

    private void Initializate()
    {
        shootTime = Time.time;
        ammo = maxAmmo;
    }

    public void Shoot(Vector3 position, Vector3 rotation)
    {
        if (ammo > 0)
        {
            if (Time.time > shootTime + fireDelay + sprayDelay)         //shooting without spray
            {
                ShootBullet(position, Quaternion.Euler(rotation));
            }
            else if (Time.time > shootTime + fireDelay)                 //shooting with spray
            {
                Quaternion sprayRotation = Quaternion.Euler(rotation.x + Random.Range(-spray, spray), rotation.y + Random.Range(-spray, spray),
                    rotation.z + Random.Range(-spray, spray));

                ShootBullet(position, sprayRotation);
            }
        }
        else
        {
            reloadCoroutine = StartCoroutine(Reload());
        }
    }

    private void ShootBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bulletObject = Instantiate(bullet, position, rotation);
        bulletObject.GetComponent<Bullet>().Initializate(damage, shootDistance);

        shootTime = Time.time;

        ammo--;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadDelay);
        ammo = maxAmmo;
    }

    private void OnDisable()
    {
        StopCoroutine(reloadCoroutine);
    }
}
