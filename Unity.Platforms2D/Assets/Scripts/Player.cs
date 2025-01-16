using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(Animator))]
public class Player : MonoBehaviour
{
    #region Editor fields


    [SerializeField] private TextMeshProUGUI text;

    [Header("Movement")]
    [SerializeField] private Vector2 feetOffset;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask jumpableLayer;
    [SerializeField] private float maxFloorDistance;
    [SerializeField] private int maxJumps;
    [SerializeField] private float coyoteTime;

    [Header("Attack")]
    [SerializeField] private Vector2 attackOffset;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackDamage;
    [SerializeField] private LayerMask attackLayer;
    
    #endregion

    #region Private fields

    private Rigidbody2D rb;
    private Animator anim;
    private int jumpsLeft;

    #endregion

    private void Awake()
    {
        // Initialize Colliders
        gameObject.tag = "PlayerHitbox";

        var hitbox = this.EnsureSelfHitBox<CapsuleCollider2D>();
        var sensor = this.EnsureChildSensorBox<CapsuleCollider2D>(hitbox);
        sensor.gameObject.tag = "PlayerSensor";
        var feet = this.EnsureChildSensorBox<BoxCollider2D>(layer: 8);
        if(feet is BoxCollider2D feetbox)
        {
            feetbox.gameObject.transform.position = FeetPosition;
            feetbox.size = new Vector2(0.1f, maxFloorDistance);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jump();
        Attack();
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
        }
    }

    // Executed from animation
    public void AttackHit()
    {
        foreach (var coll in Physics2D.OverlapCircleAll(AttackPosition, attackRadius, attackLayer))
        {
            if (coll.gameObject != gameObject && coll.TryGetComponent<HealthSystem>(out var healthSystem))
                healthSystem.ApplyDamage(attackDamage);
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CanJump())
        {
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
             jumpsLeft--;
        }
    }

    private bool CanJump()
    {
        if (Physics2D.Raycast(FeetPosition, Vector2.down, maxFloorDistance, jumpableLayer))
        {
            //jumpsLeft = maxJumps;
            Debug.Log("Jumps reset!");
        }
        return jumpsLeft > 0;
    }

    private void Movement()
    {
        var inputH = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(inputH * movementSpeed, rb.velocity.y);
        anim.SetBool("Running", inputH != 0);
        if (inputH != 0)
            transform.eulerAngles = new Vector3(0, inputH > 0 ? 0 : 180, 0);
    }

    private void OnDestroy()
    {
        text.text = "You lost! press R to Restart";
    }

    private void OnDrawGizmos()
    {
        var sprite = GetComponent<SpriteRenderer>();
        Util.DrawGizmosCircle(AttackPosition, Vector3.forward, attackRadius);
        Gizmos.DrawLine(FeetPosition + Vector3.left * 0.5f, FeetPosition + Vector3.right * 0.5f);
        Gizmos.DrawLine(FeetPosition, FeetPosition + Vector3.down * maxFloorDistance);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            StopAllCoroutines();
            jumpsLeft = maxJumps;
        }
        else if (collision.CompareTag("Finish"))
        {
            text.text = "You win! press R to Restart";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            if(this.isActiveAndEnabled && this.gameObject.activeInHierarchy)
                StartCoroutine(CoyoteTime());
        }
    }

    private IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(coyoteTime);
        jumpsLeft--;
    }

    private Vector3 AttackPosition => transform.position + (Vector3)(attackOffset * (transform.rotation.y == 0 ? Vector2.one : (Vector2.left + Vector2.up)));
    private Vector3 FeetPosition => transform.position + (Vector3)(feetOffset * (transform.rotation.y == 0 ? Vector2.one : (Vector2.left + Vector2.up)));
}
