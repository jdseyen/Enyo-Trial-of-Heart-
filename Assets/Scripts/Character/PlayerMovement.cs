using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.6f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }


    private void Update()
    {
        if (PauseController.IsGamePaused)
        {
            rb.velocity = Vector2.zero; //Stop movement
            animator.SetBool("isWalking", false);
            StopFootsteps();
            return; 
        }
        rb.velocity = moveInput * moveSpeed;
        animator.SetBool("isWalking", rb.velocity.magnitude > 0);

        //StartFootsteps
        if(rb.velocity.magnitude > 0 && !playingFootsteps)
        {
            StartFootsteps();
        }

        else if(rb.velocity.magnitude == 0)
        {
            StopFootsteps();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

    void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootSteps), 0f, footstepSpeed);
        SoundEffectManager.Play("Footstep");
    }

    void StopFootsteps ()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootSteps));
    }

    void PlayFootSteps()
    {
        SoundEffectManager.Play("FootSteps", true);
    }
}
