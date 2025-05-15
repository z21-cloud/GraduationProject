using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionRange = .5f;

    [Header("Отладка луча")]
    [SerializeField] private bool drawDebugRay = true;
    [SerializeField] private float debugRayLength = 100f;
    [SerializeField] private Color rayColorHit = Color.green;
    [SerializeField] private Color rayColorMiss = Color.red;

    [SerializeField] private ST500Piranya piranya;

    private ItemContext currentInteractable;

    private void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    private void HandleInput()
    {
        if (currentInteractable == null) return;

        // ЛКМ - считываем данные (только если устройство включено)
        if (Input.GetMouseButtonDown(0))
        {
            if (piranya != null && !piranya.CanInteract())
            {
                Debug.Log("ST-500 выключено. Считывание данных невозможно.");
                return;
            }

            currentInteractable.TransmitData();
        }

        // E - уничтожаем объект (всегда доступно)
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.DestroyItem();
            currentInteractable = null;
        }
    }

    private void HandleRaycast()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            ItemContext item = hit.collider.GetComponent<ItemContext>();
            if(item != null 
                && Vector3.Distance(transform.position, hit.transform.position) <= interactionRange)
            {
                // Отрисовка луча (если включено)
                if (drawDebugRay)
                    Debug.DrawLine(ray.origin, hit.point, rayColorHit);

                currentInteractable = item;
                return;
            }
        }
        currentInteractable = null;

        if (drawDebugRay)
        {
            Vector3 rayEnd = ray.origin + ray.direction * debugRayLength;
            Debug.DrawLine(ray.origin, rayEnd, rayColorMiss);
        }
    }
}
