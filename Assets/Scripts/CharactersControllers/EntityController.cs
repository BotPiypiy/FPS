using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class EntityController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField]
    protected int hp;
    [SerializeField]
    protected float moveSpeed;
    [SerializeField]
    protected float jumpHeight;

    [Header("Help Objects")]
    [SerializeField]
    protected Transform groundCheck;
    [SerializeField]
    float groundDistance;
    [SerializeField]
    LayerMask groundMask;

    [SerializeField]
    protected Transform firePoint;      //position of creating bullets

    protected CharacterController characterController;
    protected Vector3 velocity;


    protected virtual void Initializate()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        velocity = Vector3.zero;
    }

    protected abstract void Move();
    protected abstract void Jump();
    protected abstract void Shoot();
    protected abstract void Dead();

    protected void Landing()
    {
        if (IsGround() && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    protected void Fall()
    {
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

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
