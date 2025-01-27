using UnityEngine;

public class DashController : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 15f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;


    InputManager inputs;
    CharacterController controller;
    Vector3 dashVelocity;

    bool canDash = true;
    float dashTimeRemaining;

    void Awake()
    {
        inputs = GetComponentInChildren<InputManager>();
        controller = GetComponent<CharacterController>();
    }

    public void Dash()
    {
        if (canDash) InitiateDash();
        ApplyDash();
    }

    void InitiateDash()
    {
        Vector2 moveInput = inputs.MoveInput.normalized;
        var moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        if (moveDirection == Vector3.zero) moveDirection = transform.forward;

        dashVelocity = moveDirection * dashSpeed;
        dashTimeRemaining = dashDuration;
        canDash = false;

        Invoke(nameof(ResetDash), dashCooldown);
    }
    private void OnTriggerEnter(Collider collision)
    {
        //while dashing collision deal damage
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(5);
            enemy.OnDeath += ResetDash;
        }
    }
    void ApplyDash()
    {
        if (dashTimeRemaining > 0)
        {
            controller.Move(dashVelocity * Time.deltaTime);
            dashTimeRemaining -= Time.deltaTime;
        }
    }

    void ResetDash() => canDash = true;
}