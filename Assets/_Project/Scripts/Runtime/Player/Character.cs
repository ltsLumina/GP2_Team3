using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float speed = 7.0f;

    float moveDir;
    float verticalVelocity;
    const float GRAVITY = -90.81f;

    CharacterController characterController;
    InputManager inputs;
    Player player;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputs = GetComponentInChildren<InputManager>();
        player = GameManager.Instance.Player;
    }

    void Update()
    {
        Vector3 moveVector = default;

        if (!inputs) // backwards compatibility after I reworked this script
        {
            moveVector.x = Input.GetAxis("Horizontal");
            moveVector.z = Input.GetAxis("Vertical");
        }
        else { moveVector = new Vector3(inputs.MoveInput.x, 0, inputs.MoveInput.y); }

        if (moveVector.magnitude > 1) moveVector.Normalize();

        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity = 0; // Reset vertical velocity when grounded
        }
        else
        {
            verticalVelocity += GRAVITY * Time.deltaTime; // Apply gravity
        }

        moveVector.y = verticalVelocity; // Include vertical velocity in movement

        characterController.Move(moveVector * (speed * (player.PlayerStats[Player.Stats.MovementSpeed] / 100f) * Time.deltaTime));
    }
}

