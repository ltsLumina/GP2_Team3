using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float interactableDistance = 5f;
    [SerializeField] private float enemyInteractableDistance = 10f;

    private HashSet<IInteractable> previousInteractableObjects = new();
    private HashSet<IInteractable> previousEnemyObjects = new();

    private void Update()
    {
        SearchInteractableObjects();
        SearchEnemyObjects();

        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, interactableLayer))
            {
                if (hit.collider.TryGetComponent<IInteractable>(out var interactable) && previousInteractableObjects.Contains(interactable))
                {
                    interactable.OnInteract();
                }
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
            {
                if (hit.collider.TryGetComponent<IInteractable>(out var enemy) && previousEnemyObjects.Contains(enemy))
                {
                    enemy.OnInteract();
                }
            }
        }
    }

    private void SearchInteractableObjects()
    {
        foreach (var interactable in previousInteractableObjects)
        {
            interactable.OnHoverExit();
        }

        var interactableObjects = new HashSet<IInteractable>();
        var colliders = Physics.OverlapSphere(transform.position, interactableDistance, interactableLayer);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactableObjects.Add(interactable);
                interactable.OnHoverEnter();
            }
        }

        previousInteractableObjects = interactableObjects;
    }

    private void SearchEnemyObjects()
    {
        foreach (var enemy in previousEnemyObjects)
        {
            enemy.OnHoverExit();
        }

        var enemyObjects = new HashSet<IInteractable>();
        var colliders = Physics.OverlapSphere(transform.position, enemyInteractableDistance, enemyLayer);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<IInteractable>(out var enemy))
            {
                enemyObjects.Add(enemy);
                enemy.OnHoverEnter();
            }
        }

        previousEnemyObjects = enemyObjects;
    }
}
