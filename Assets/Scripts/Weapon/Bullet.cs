using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed;

    private int damage;

    private void Update()
    {
        transform.localPosition += transform.forward * bulletSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Touch(other);
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

    private void Touch(Collider other)
    {
        EntityController entity = other.gameObject.GetComponent<EntityController>();
        if (entity)
        {
            entity.Damage(damage);
        }

        TryDestroy();
    }

    private void TryDestroy()
    {
        if (gameObject)
        {
            Destroy(gameObject);
        }
    }
}
