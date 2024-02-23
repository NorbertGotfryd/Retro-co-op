using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerController currentAttacker;
    private List<GameObject> currentStandingGameObjects = new List<GameObject>();

    public int currentHp;
    public int maxHp;
    public int score;
    public int maxJumps;
    public float moveSpeed;
    public float jumpForce;

    private int jumpsAvilable;
    private float currentMoveInput;

    [Header("Components")]
    public Rigidbody2D rig;
    public Animator anim;
    public Transform muzzle;
    public PlayerContainerUi containerUi;

    [Header("Combat")]
    public float attackRate;
    public float projectileSpeed;
    public GameObject projectilePrefab;

    private float lastAttackTime;

    public void Respawn(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        currentHp = maxHp;
        containerUi.UpdateHealthBar(currentHp, maxHp);
        currentAttacker = null;
    }

    private void FixedUpdate()
    {
        Move();

        //annimation update
        if(currentMoveInput == 0.0f && IsGrounded())
        {
            anim.SetBool("Moving", false);
            anim.SetBool("Jumping", false);
        }
        else if (currentMoveInput != 0.0f && IsGrounded())
        {
            anim.SetBool("Moving", true);
            anim.SetBool("Jumping", false);
        } else if (!IsGrounded())
        {
            anim.SetBool("Moving", false);
            anim.SetBool("Jumping", true);
        }

        //flip the player base on movement
        if(currentMoveInput != 0.0f)
            transform.localScale = new Vector3(currentMoveInput > 0 ? 1 : -1, 1, 1);
    }

    private void Update()
    {
        if (transform.position.y < -10)
            PlayerManager.instance.OnPlayerDeath(this, currentAttacker);
    }

    private void Move()
    {
        rig.velocity = new Vector2(currentMoveInput * moveSpeed, rig.velocity.y);
    }

    private void Jump()
    {
        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        currentMoveInput = context.ReadValue<float>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (jumpsAvilable > 0)
            {
                jumpsAvilable--;
                Jump();
            }
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * projectileSpeed, 0);
        projectile.GetComponent<Projectile>().owner = this;
    }

    public void TakeDamage(int damage, PlayerController attacker)
    {
        currentAttacker = attacker;
        currentHp -= damage;

        if (currentHp <= 0)
            PlayerManager.instance.OnPlayerDeath(this, currentAttacker);

        //update players hp bar
        containerUi.UpdateHealthBar(currentHp, maxHp);
    }

    private bool IsGrounded()
    {
        return currentStandingGameObjects.Count > 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].point.y < transform.position.y)
        {
            jumpsAvilable = maxJumps;

            if (!currentStandingGameObjects.Contains(collision.gameObject))
                currentStandingGameObjects.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (currentStandingGameObjects.Contains(collision.gameObject))
            currentStandingGameObjects.Remove(collision.gameObject);
    }
}
