using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    private PlayerControls controls = null;
    private Rigidbody2D rigidbody = null;
    private Animator animator = null;
    private Vector2 moveVector = Vector2.zero; 
    [SerializeField] static private float defaultMoveSpeed = 5.0f;
    [SerializeField] private float moveSpeed = defaultMoveSpeed;
    [SerializeField] private float moveMultiplier = 2.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float dropForce = -10.0f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private int maxDrops = 1;

    [SerializeField] private AudioClip runningSound;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip gameoverSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private LevelManager levelManager;

    private void Awake()
    {
        controls = new PlayerControls();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        animator.SetBool("isIdling", true);
    }

    private void FixedUpdate()
    {
        // Horizontal movement
        Vector3 newScale = transform.localScale;
        newScale.x = Math.Abs(moveVector.x) > 0.9f ? moveVector.x : newScale.x;
        transform.localScale = newScale;
        rigidbody.velocity = new Vector2(moveVector.x * moveSpeed, rigidbody.velocity.y);
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Movement.performed += OnMovementPerformed;
        controls.Player.Movement.canceled += OnMovementCanceled;
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Sprint.performed += OnSprintPerformed;
        controls.Player.Sprint.canceled += OnSprintCanceled;
        controls.Player.Drop.performed += OnDropPerformed;
        controls.Player.Attack.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Movement.performed -= OnMovementPerformed;
        controls.Player.Movement.canceled -= OnMovementCanceled;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Sprint.performed -= OnSprintPerformed;
        controls.Player.Sprint.canceled -= OnSprintCanceled;
        controls.Player.Drop.performed -= OnDropPerformed;
        controls.Player.Attack.performed -= OnAttackPerformed;

    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isIdling", false);
        animator.SetBool("isAttacking1", false);
        animator.SetBool("isAttacking2", false);
        animator.SetBool("isAttacking3", false);
        moveVector = value.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext value)
    {
        animator.SetBool("isIdling", true);
        animator.SetBool("isWalking", false);
        moveVector = Vector2.zero;
    }


    private void OnJumpPerformed(InputAction.CallbackContext value)
    {
        if (maxJumps > 0)
        {
            animator.SetBool("isJumping", true);
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0.0f);
            rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            maxJumps--;
        }
    }

    public void BraceForImpact()
    {
        animator.SetBool("isBracing", true);
        animator.SetBool("isJumping", false);
    }

    IEnumerator WaitAndDie(int i)
    {
        yield return new WaitForSeconds(i);
        levelManager.LoadGameOver();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        animator.SetBool("isBracing", false);
        if (other.gameObject.CompareTag("Enemies"))
        {
            // disable rigidbody
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false;
            }
            rigidbody.simulated = false;
            AudioSource.PlayClipAtPoint(gameoverSound, Camera.main.transform.position);
            animator.SetBool("isDying", true);
            controls.Disable();
            StartCoroutine(WaitAndDie(2));
        }
        if (other.gameObject.CompareTag("Coin"))
        {
            AudioSource.PlayClipAtPoint(coinSound, Camera.main.transform.position);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("GOAL"))
        {
            AudioSource.PlayClipAtPoint(victorySound, Camera.main.transform.position);
            StartCoroutine(WaitAndDie(6));
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Map"))
        {
            animator.SetBool("isJumping", false);
            maxJumps = 2;
            maxDrops = 1;
            AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (currentStateInfo.IsName("PlayerBracing"))
            {
                animator.SetBool("isBracing", false);
            }
        }
    }

    private void OnSprintPerformed(InputAction.CallbackContext value)
    {
        animator.SetBool("isRunning", true);
        moveSpeed = defaultMoveSpeed * moveMultiplier;
    }

    private void OnSprintCanceled(InputAction.CallbackContext value)
    {
        animator.SetBool("isRunning", false);
        moveSpeed = defaultMoveSpeed;
    }

    private void OnDropPerformed(InputAction.CallbackContext value)
    {
        if (maxDrops > 0)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, dropForce);
            maxDrops--;
        }
    }

    private void OnAttackPerformed(InputAction.CallbackContext value)
    {
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (currentStateInfo.IsName("PlayerIdle"))
        {
            animator.SetBool("isAttacking1", true);
        }
        else if (currentStateInfo.IsName("PlayerAttack1"))
        {
            animator.SetBool("isAttacking2", true);
        }
        else if (currentStateInfo.IsName("PlayerAttack2"))
        {
            animator.SetBool("isAttacking3", true);
        }
    }
}
