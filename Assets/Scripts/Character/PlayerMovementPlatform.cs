using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementPlatform : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;

    Animator animator;

    float horizontalMovement;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);

        animator.SetFloat("Blend", horizontalMovement);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void PlayFootstep()
    {
        SoundEffectManager.Play("FootSteps", true);
    }
}