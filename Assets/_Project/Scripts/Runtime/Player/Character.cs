using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float speed = 7.0f;

    public Animator animator;

    float moveDir;
    float verticalVelocity;
    const float GRAVITY = -90.81f;

    CharacterController characterController;
    InputManager inputs;
    Player player;

    private Vector3 lastGroundedPosition;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputs = GetComponentInChildren<InputManager>();
    }

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    public void UpdateCharacter()
    {
        if (!CheckIsOnGround())
            return;

        transform.position = lastGroundedPosition;

        Vector3 moveVector = inputs
            ? new Vector3(inputs.MoveInput.x, 0, inputs.MoveInput.y)
            : new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (moveVector.magnitude > 1)
            moveVector.Normalize();

        moveVector = Quaternion.Euler(0, -45, 0) * moveVector;

        verticalVelocity = characterController.isGrounded ? 0 : verticalVelocity + GRAVITY * Time.deltaTime;
        moveVector.y = verticalVelocity;

        if (CheckMovement(moveVector))
        {
            characterController.Move(moveVector * (speed * player[Player.Stats.MovementSpeed] * Time.deltaTime));
        }

        Vector3 playerForward = transform.forward;
        Vector3 moveDirection = new Vector3(inputs.MoveInput.x, 0, inputs.MoveInput.y).normalized;

        float directionMultiplier = Vector3.Dot(playerForward, moveDirection) >= 0 ? 1f : -1f;

        animator.SetFloat("currentMoveSpeed", Mathf.Clamp01(inputs.MoveInput.magnitude) * directionMultiplier);
        animator.SetBool("isMoving", inputs.IsMoving);
    }


    private bool CheckIsOnGround()
    {
        Ray ray = new(transform.position + new Vector3(0, 5, 0), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            lastGroundedPosition = hitInfo.point;
            return true;
        }

        transform.position = lastGroundedPosition;
        return false;
    }

    /// <summary>
    /// Check if the player can move to the next position.
    /// </summary>
    /// <returns>Returns true if next position is on ground</returns>
    private bool CheckMovement(Vector3 moveVector)
    {

        if (!inputs.IsMoving)
            return false;

        var nextPos = transform.position + new Vector3(moveVector.x, 0, moveVector.z) * (speed * player[Player.Stats.MovementSpeed] * Time.deltaTime);
        nextPos.y += 0.5f; //Just to make sure it isn't somehow under the ground

        return Physics.Raycast(nextPos, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Ground"));
    }
}
