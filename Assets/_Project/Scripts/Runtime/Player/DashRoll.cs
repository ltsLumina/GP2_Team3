using UnityEngine;

public class DashController : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private CharacterController controller;
    private Vector3 dashVelocity;
    private bool canDash = true;
    private float dashTimeRemaining;

    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            InitiateDash();
        }
        ApplyDash();
    }

    void InitiateDash()
    {
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (moveDirection == Vector3.zero)
        {
            moveDirection = transform.forward;
        }

        dashVelocity = moveDirection * dashSpeed;
        dashTimeRemaining = dashDuration;
        canDash = false;
        Invoke("ResetDash", dashCooldown);
    }

    void ApplyDash()
    {
        if (dashTimeRemaining > 0)
        {
            controller.Move(dashVelocity * Time.deltaTime);
            dashTimeRemaining -= Time.deltaTime;
        }
    }

    void ResetDash()
    {
        canDash = true;
    }
}