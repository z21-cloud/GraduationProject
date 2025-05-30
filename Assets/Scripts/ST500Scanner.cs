using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(ST500Piranya))]
public class ST500Scanner : MonoBehaviour
{
    [SerializeField] private float scanRadius = 5f;
    [SerializeField] private float scanInterval = 1f;
    [SerializeField] private bool isScanningActive = false;
    public bool IsScanningActive => isScanningActive;

    private ST500Piranya piranya;
    private SphereCollider scanArea;
    private Coroutine scanCoroutine;
    private HashSet<ItemContext> nearbyItems = new HashSet<ItemContext>();
    
    private ScanMode currentScanMode = ScanMode.Frequency;
    [SerializeField] private float irScanRadius = 3f;

    public static event System.Action<bool> OnScanningStateChanged;

    private void Awake()
    {
        piranya = GetComponent<ST500Piranya>();
        scanArea = GetComponent<SphereCollider>();
        scanArea.isTrigger = true;
        scanArea.radius = scanRadius;
    }

    private void OnValidate()
    {
        if (scanArea != null)
            scanArea.radius = scanRadius;
    }

    private void Update()
    {
        // Проверяем нажатие клавиши "2" только если устройство включено
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (piranya.IsDeviceActive)
            {
                ToggleScanning();
            }
            else
            {
                Debug.Log("Устройство выключено, сканирование неактивно");
            }
        }
    }

    public void ToggleScanning()
    {
        if (!piranya.IsDeviceActive)
        {
            Debug.Log("Устройство выключено, сканирование неактивно");
            return;
        }

        isScanningActive = !isScanningActive;
        OnScanningStateChanged?.Invoke(isScanningActive);
        Debug.Log($"Сканирование ST-500: {(isScanningActive ? "включено" : "выключено")}");
        if (isScanningActive)
        {
            scanCoroutine = StartCoroutine(ScanRoutine());
        }
        else if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
            scanCoroutine = null;
        }
    }

    public void SetScanMode(ScanMode mode)
    {
        currentScanMode = mode;
        scanArea.radius = mode == ScanMode.IR ? irScanRadius : scanRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemContext item = other.GetComponent<ItemContext>();
        if (item != null)
        {
            nearbyItems.Add(item);
            Debug.Log($"Объект добавлен: ID={item.ID}, Type={item.Type}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ItemContext item = other.GetComponent<ItemContext>();
        if (item != null && item.gameObject != gameObject)
        {
            nearbyItems.Remove(item);
        }
    }

    private IEnumerator ScanRoutine()
    {
        while (true)
        {
            if (piranya.IsDeviceActive && isScanningActive)
            {
                float currentRadius = scanArea.radius;
                foreach (ItemContext item in new List<ItemContext>(nearbyItems))
                {
                    if (item != null && !item.IsDestroyed)
                    {
                        float distance = Vector3.Distance(transform.position, item.transform.position);
                        bool isVisible = item.IsIRDevice
                            ? item.StateType == ItemStateType.Active || item.StateType == ItemStateType.Periodic
                            : item.StateType == ItemStateType.Active || item.StateType == ItemStateType.Periodic;

                        if (distance <= currentRadius && isVisible)
                        {
                            TransmitScanningData(item, distance);
                        }
                    }
                    else
                    {
                        nearbyItems.Remove(item);
                    }
                }
            }

            yield return new WaitForSeconds(scanInterval);
        }
    }

    private void TransmitScanningData(ItemContext item, float distance)
    {
        ItemData data = new ItemData
        {
            ID = item.ID,
            Type = item.Type,
            Frequency = item.IsIRDevice ? 0f : item.Frequency,
            Interactable = item.Interactable,
            Color = item.Color,
            State = item.StateType.ToString(),
            Distance = distance,
            IsIRDevice = item.Type == ItemType.IRDevice
        };

        // Для периодических IR-устройств передаем состояние активности
        if (item.Type == ItemType.IRDevice && item.StateType == ItemStateType.Periodic)
        {
            data.IsActive = (item.CurrentStrategy as IRPeriodicStateStrategy)?.IsActive ?? true;
        }

        item.CurrentStrategy.TransmitData(data);
    }

    public List<ItemData> GetDetectedItems()
    {
        List<ItemData> detectedItems = new List<ItemData>();
        var scannerPos = transform.position;

        Debug.Log($"Проверка объектов: всего {nearbyItems.Count}");

        foreach (ItemContext item in nearbyItems)
        {
            if (item != null && !item.IsDestroyed)
            {
                float distance = Vector3.Distance(scannerPos, item.transform.position);

                // Для IRDevice используем специальную проверку состояния
                bool isVisible = item.IsIRDevice
                    ? item.StateType == ItemStateType.Active || item.StateType == ItemStateType.Periodic
                    : item.StateType == ItemStateType.Active || item.StateType == ItemStateType.Periodic;

                if (distance <= scanArea.radius &&
                    (item.IsIRDevice ||
                    item.StateType == ItemStateType.Active ||
                    item.StateType == ItemStateType.Periodic))
                    {
                    //Debug.Log($"Добавлен в обнаруженные: ID={item.ID}, Type={item.Type}, Dist={distance:F2}м");

                    detectedItems.Add(new ItemData
                    {
                        ID = item.ID,
                        Type = item.Type,
                        Frequency = item.IsIRDevice ? 0f : item.Frequency,
                        Interactable = item.Interactable,
                        Color = item.Color,
                        State = item.StateType.ToString(),
                        Distance = distance
                    });
                }
            }
        }
        return detectedItems;
    }

    private void OnDrawGizmos()
    {
        if (piranya == null)
        {
            Debug.LogWarning("Устройство ST-500 не найдено!");
            return;
        }

        Color gizmoColor;
        if (!piranya.IsDeviceActive)
            gizmoColor = Color.gray;
        else if (isScanningActive)
            gizmoColor = Color.green;
        else
            gizmoColor = Color.red;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, scanRadius);
    }
}