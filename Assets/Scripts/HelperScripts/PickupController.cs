using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupController : MonoBehaviour
{
    [SerializeField] private float interactionRange = 2f;

    /*[Header("Отладка луча")]
    [SerializeField] private bool drawDebugRay = true;
    [SerializeField] private float debugRayLength = 10f;
    [SerializeField] private Color rayColorHit = Color.green;
    [SerializeField] private Color rayColorMiss = Color.red;*/

    [Header("Devices")]
    [SerializeField] private GameObject st500Device;
    [SerializeField] private GameObject oblik2Device;
    [SerializeField] private ST500Piranya piranya;
    [SerializeField] private Camera playerCamera;

    [Header("UI")]
    [SerializeField] private GameObject oblickUI;

    public static event Action<ItemData> OnItemInRange;
    public static event Action OnItemOutOfRange;

    private ItemContext currentInteractable;
    private ItemData currentInteractableData;

    private enum ActiveDevice { None, ST500, Oblik2 }
    private ActiveDevice currentDevice = ActiveDevice.ST500;
    private SimpleScanner oblikScanner;

    private void Start()
    {
        if (oblik2Device != null)
        {
            oblikScanner = oblik2Device.GetComponent<SimpleScanner>();
            if (oblikScanner != null)
            {
                oblikScanner.playerCamera = playerCamera;
            }
        }

        oblickUI.SetActive(false);
        SetDeviceActive(ActiveDevice.ST500, true);
        SetDeviceActive(ActiveDevice.Oblik2, false);
    }

    private void Update()
    {
        HandleRaycast();
        HandleInput();
        HandleDeviceSwitch();

        if (currentDevice == ActiveDevice.Oblik2 && oblikScanner != null)
        {
            oblikScanner.UpdateUI();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Reloaded");
            ReloadScene();
        }
    }

    private void HandleDeviceSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchDevices();
        }
    }

    private void SwitchDevices()
    {
        // Переключаем устройства
        if (currentDevice == ActiveDevice.ST500)
        {
            SetDeviceActive(ActiveDevice.ST500, false);
            SetDeviceActive(ActiveDevice.Oblik2, true);
            currentDevice = ActiveDevice.Oblik2;
            Debug.Log("Переключено на ОБЛИК-2");
        }
        else
        {
            SetDeviceActive(ActiveDevice.Oblik2, false);
            SetDeviceActive(ActiveDevice.ST500, true);
            currentDevice = ActiveDevice.ST500;
            Debug.Log("Переключено на ST500");
        }
    }

    private void SetDeviceActive(ActiveDevice device, bool active)
    {
        switch (device)
        {
            case ActiveDevice.ST500:
                if (st500Device != null)
                {
                    st500Device.SetActive(active);
                    
                    // Отключаем сканирование при выключении
                    if (!active)
                    {
                        var scanner = st500Device.GetComponent<ST500Scanner>();
                        if (scanner != null && scanner.IsScanningActive)
                        {
                            scanner.ToggleScanning();
                        }

                        var cableScanner = st500Device.GetComponent<CableScanner>();
                        if (cableScanner != null && cableScanner.isScanningMode)
                        {
                            cableScanner.SetScanningMode(false);
                        }
                    }
                }
                break;

            case ActiveDevice.Oblik2:
                if (oblik2Device != null)
                {
                    oblik2Device.SetActive(active);
                    oblickUI.SetActive(active);

                    if (oblikScanner != null)
                    {
                        // Активируем/деактивируем сканер
                        oblikScanner.SetActive(active);

                        if (active)
                        {
                            // Инициализируем при активации
                            oblikScanner.Initialize();
                        }
                        else
                        {
                            // Деактивируем при выключении
                            oblikScanner.UnequipBinocular();
                        }
                    }
                }
                break;
        }
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

    private void ReloadScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
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
