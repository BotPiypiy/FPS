using UnityEngine;
using Mirror;

public abstract class EntityController : NetworkBehaviour
{
    [Header("Stats")]
    [SerializeField]
    protected int hp;
    [SyncVar]
    [SerializeField]
    protected float moveSpeed;
    [SyncVar]
    [SerializeField]
    protected float jumpForce;

    [Header("Help Objects")]
    [SerializeField]
    protected Transform groundCheck;
    [SerializeField]
    float groundDistance;
    [SerializeField]
    LayerMask groundMask;

    [SerializeField]
    protected Transform firePoint;      //position of creating bullets

    protected Rigidbody rigidbody;

    protected virtual void Initializate()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    public abstract void Move(Vector3 step);
    public abstract void Jump();
    public abstract void Shoot();
    protected abstract void Dead();

    protected bool IsGround()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    public void Damage(int value)
    {
        hp -= value;
        if(hp<=0)
        {
            Dead();
        }
    }
}
