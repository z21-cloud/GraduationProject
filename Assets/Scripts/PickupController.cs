using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionRange = 2f;

    [Header("ќтладка луча")]
    [SerializeField] private bool drawDebugRay = true;
    [SerializeField] private float debugRayLength = 100f;
    [SerializeField] private Color rayColorHit = Color.green;
    [SerializeField] private Color rayColorMiss = Color.red;

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

        // ”ничтожение по нажатию E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (piranya != null && !piranya.IsDeviceActive)
            {
                Debug.Log("ST-500 выключено. —читывание данных невозможно.");
                return;
            }

            currentInteractable.DestroyItem();
            currentInteractable = null;
            OnItemOutOfRange?.Invoke();
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
                float distance = Vector3.Distance(transform.position, item.transform.position);

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

                    OnItemInRange?.Invoke(currentInteractableData);
                    return;
                }
            }
        }

        currentInteractable = null;
        OnItemOutOfRange?.Invoke();
    }
}
