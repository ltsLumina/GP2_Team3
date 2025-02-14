using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class InteractIndicator : MonoBehaviour
{
    [SerializeField] private Image interactIcon;
    [SerializeField] private TMP_Text interactKeyText;

    private InputAction interactAction;

    private float holdTime;
    private float holdDuration;
    private bool isHolding = false;
    private Transform targetTransform;

    private void Start() => gameObject.SetActive(false);

    private void OnEnable()
    {
        PlayerInput input = FindAnyObjectByType<PlayerInput>();
        if (!input)
        {
            enabled = false;
            return;
        }

        interactAction = input.actions["Interact"];

        interactAction.started += HoldStarted;
        interactAction.canceled += HoldCancelled;
        interactAction.performed += HoldPerformed;

        PlayerInteract.OnHoverEnter += OnHoverEnter;
        PlayerInteract.OnHoverExit += OnHoverExit;

        SetProgress(0f);
        SetInteractKey(interactAction.controls[0].displayName);
    }

    private void OnDisable()
    {
        interactAction.started -= HoldStarted;
        interactAction.canceled -= HoldCancelled;
        interactAction.performed -= HoldPerformed;
    }

    private void OnDestroy()
    {
        PlayerInteract.OnHoverEnter -= OnHoverEnter;
        PlayerInteract.OnHoverExit -= OnHoverExit;
    }

    private void Update()
    {
        if (isHolding)
        {
            holdDuration += Time.deltaTime;
            SetProgress(holdDuration / holdTime);
        }

        if (targetTransform != null)
        {
            UpdatePosition();
        }
    }

    private void HoldStarted(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction hold)
        {
            holdTime = hold.duration > 0 ? hold.duration : InputSystem.settings.defaultHoldTime;
        }

        isHolding = true;
        holdDuration = 0f;
        SetProgress(0f);
    }

    private void HoldPerformed(InputAction.CallbackContext context)
    {
        isHolding = false;
        SetProgress(1f);
    }

    private void HoldCancelled(InputAction.CallbackContext context)
    {
        isHolding = false;
        SetProgress(0f);
    }

    private void OnHoverEnter(IInteractable interactable, GameObject obj)
    {
        targetTransform = obj.transform;
        gameObject.SetActive(true);
        UpdatePosition();

        Debug.Log("enter");
    }

    private void OnHoverExit(IInteractable interactable)
    {
        gameObject.SetActive(false);
        targetTransform = null;

        Debug.Log("exit");
    }

    private void UpdatePosition()
    {
        if (targetTransform == null) return;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        (transform as RectTransform).position = screenPosition;
    }

    private void SetProgress(float progress)
    {
        interactIcon.fillAmount = progress;
    }

    private void SetInteractKey(string key)
    {
        interactKeyText.text = key;
    }
}
