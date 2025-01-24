using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    private CharacterController characterController;
    public float speed = 7.0f;
    internal float moveDir;
    internal object controller;

    InputAction moveAction;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        moveAction = InputSystem.actions["Move"];
    }

    private void Update()
    {
        var move = moveAction.ReadValue<Vector2>();
        var moveVector = new Vector3(move.x, 0, move.y);

        if (moveVector.magnitude > 1)
        {
            moveVector.Normalize();
        } 

        characterController.Move(moveVector * speed * Time.deltaTime);
    }
}

