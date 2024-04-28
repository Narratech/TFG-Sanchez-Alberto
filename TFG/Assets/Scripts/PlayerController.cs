using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 12f;
    private float jumpingPower = 18f;
    private bool isFacingRight = false;

    public static bool playerControl = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator naeveAnimator;
    private bool isJumping;

    void Update()
    {
        if (PauseMenu.gameIsPaused) // Asumiendo que isGamePaused es una variable estática o accesible globalmente
        {
            return; // Ignora el resto del código en Update si el juego está pausado
        }
        if (playerControl)
        {
            horizontal = Input.GetAxisRaw("Horizontal");

            if (!IsGrounded())
            {
                naeveAnimator.SetBool("isJump", true);
                isJumping = true;
            }

            // Activar la animación de salto cuando se detecta un salto y el personaje está en el suelo
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                naeveAnimator.SetBool("isJump", true);
                isJumping = true;
            }

            // Activar la animación de correr cuando el personaje se está moviendo horizontalmente

            naeveAnimator.SetBool("isRun", (Mathf.Abs(horizontal) > 0f && !isJumping));

            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }

            if (isJumping)
            {
                // Introduzco un pequeño retraso para que le de tiempo a comenzar a saltar antes de comprobar si Naeve no está en el suelo. Ya que, de otro modo, se desactiva inmediatamente la animación
                Invoke("CheckGroundedAfterJump", 0.1f);
            }

            Flip();
        }
        
    }

    private void FixedUpdate()
    {
        if (playerControl)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void CheckGroundedAfterJump()
    {
        if (IsGrounded())
        {
            naeveAnimator.SetBool("isJump", false);
            isJumping = false;
        }
    }


    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
