using Mirror;
using System.Collections;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private LayerMask layer;

    private int damage;

    private void Update()
    {
        if (isServer)
        {
            Cast();
        }
        else if (isClient)
        {
            Move();
        }
    }

    [Server]
    private void Cast()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, bulletSpeed * Time.deltaTime, layer,
            QueryTriggerInteraction.Ignore))
        {
            Touch(hit.collider);
            TryDestroy();
        }
    }

    private void Move()
    {
        transform.localPosition += transform.forward * bulletSpeed * Time.deltaTime;
    }

    public void Initializate(int _damage, float _liveTime)
    {
        damage = _damage;

        StartCoroutine(DestroyAfter(_liveTime));
    }

    private IEnumerator DestroyAfter(float liveTIme)
    {
        yield return new WaitForSeconds(liveTIme);

        TryDestroy();
    }

    [Server]
    private void Touch(Collider other)
    {
        EntityController entity = other.gameObject.GetComponent<EntityController>();
        if (entity)
        {
            entity.SDamage(damage);
        }
    }

    [Server]
    private void TryDestroy()
    {
        if (gameObject)
        {
            Destroy(gameObject);
        }
    }
}
