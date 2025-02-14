using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactableDistance = 5f;

    private IInteractable currentInteractable;

    public static event Action<IInteractable, GameObject> OnHoverEnter;
    public static event Action<IInteractable> OnHoverExit;

    private InputAction interactAction;

    private void OnEnable()
    {
        PlayerInput input = FindAnyObjectByType<PlayerInput>();
        if (!input)
        {
            enabled = false;
            return;
        }

        interactAction = input.actions["Interact"];
        interactAction.performed += InteractPerformed;
    }

    private void OnDisable()
    {
        interactAction.performed -= InteractPerformed;

        OnHoverEnter = null;
        OnHoverExit = null;
    }

    private void InteractPerformed(InputAction.CallbackContext context)
    {
        currentInteractable?.OnInteract();
    }

    private void Update()
    {
        SearchInteractableObjects();
    }

    private void SearchInteractableObjects()
    {
        IInteractable closest = null;
        GameObject closestObject = null;
        float closestDistance = float.MaxValue;

        var colliders = Physics.OverlapSphere(transform.position, interactableDistance, interactableLayer);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<IInteractable>(out var interactable))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    if(interactable is Door) { }
                        distance -= 1f;

                    closestDistance = distance;
                    closest = interactable;
                    closestObject = collider.gameObject;
                }
            }
        }

        if (currentInteractable != closest)
        {
            if (currentInteractable != null)
            {
                OnHoverExit?.Invoke(currentInteractable);
                currentInteractable.OnHoverExit();
            }

            currentInteractable = closest;

            if (currentInteractable != null)
            {
                OnHoverEnter?.Invoke(currentInteractable, closestObject);
                currentInteractable.OnHoverEnter();
            }
        }
    }
}
