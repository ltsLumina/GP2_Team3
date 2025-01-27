using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float speed = 7.0f;

    float moveDir;
    
    CharacterController characterController;
    InputManager inputs;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputs = GetComponentInChildren<InputManager>();
    }

    void Update()
    {
        Vector3 moveVector = default;
        if (!inputs) // backwards compatibility after I reworked this script
        {
            moveVector.x = Input.GetAxis("Horizontal");
            moveVector.z = Input.GetAxis("Vertical");
        }
        else
        {
            moveVector = new (inputs.MoveInput.x, 0, inputs.MoveInput.y);
        }

        if (moveVector.magnitude > 1) moveVector.Normalize();

        var player = GetComponent<Player>();
        characterController.Move(moveVector * (speed * (player.PlayerStats[Player.Stats.MovementSpeed] / 100f) * Time.deltaTime));
    }
}

