using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionRange = 2f;

    /*[Header("Отладка луча")]
    [SerializeField] private bool drawDebugRay = true;
    [SerializeField] private float debugRayLength = 10f;
    [SerializeField] private Color rayColorHit = Color.green;
    [SerializeField] private Color rayColorMiss = Color.red;*/

    [SerializeField] private ST500Piranya piranya;

    public static event Action<ItemData> OnItemInRange;
    public static event Action OnItemOutOfRange;

    private ItemContext currentInteractable;
    private ItemData currentInteractableData;

    private void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    private void HandleInput()
    {
        if (currentInteractable == null) return;

        // Уничтожение по нажатию E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable.Type == ItemType.IRDevice)
            {
                Debug.Log("ИК-устройства не могут быть уничтожены");
                return;
            }

            currentInteractable.DestroyItem();
            currentInteractable = null;
        }
    }

    private void HandleRaycast()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            ItemContext item = hit.collider.GetComponent<ItemContext>();
            if (item != null && !item.IsDestroyed)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance <= interactionRange)
                {
                    currentInteractable = item;
                    currentInteractableData = new ItemData
                    {
                        ID = item.ID,
                        Type = item.Type,
                        Frequency = item.Frequency,
                        Interactable = item.Interactable,
                        Color = item.Color,
                        State = item.StateType.ToString(),
                        Distance = distance
                    };
                    PickupController.OnItemInRange?.Invoke(currentInteractableData);
                    return;
                }
            }
        }

        currentInteractable = null;
        PickupController.OnItemOutOfRange?.Invoke();
    }
}
