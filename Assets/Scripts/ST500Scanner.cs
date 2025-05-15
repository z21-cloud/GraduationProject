using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(ST500Piranya))]
public class ST500Scanner : MonoBehaviour
{
    [SerializeField] private float scanRadius = 5f;
    [SerializeField] private float scanInterval = 1f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private bool isScanningActive = false;
    public bool IsScanningActive => isScanningActive;

    private ST500Piranya piranya;
    private SphereCollider scanArea;
    private Coroutine scanCoroutine;
    private HashSet<ItemContext> nearbyItems = new HashSet<ItemContext>();

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
        isScanningActive = !isScanningActive;
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

    private void OnTriggerEnter(Collider other)
    {
        ItemContext item = other.GetComponent<ItemContext>();
        if (item != null)
        {
            nearbyItems.Add(item);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ItemContext item = other.GetComponent<ItemContext>();
        if (item != null)
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
                foreach (ItemContext item in new List<ItemContext>(nearbyItems))
                {
                    if (item != null && !item.IsDestroyed)
                    {
                        float distance = Vector3.Distance(transform.position, item.transform.position);

                        if (distance <= scanRadius &&
                            (item.StateType == ItemStateType.Active || item.StateType == ItemStateType.Periodic))
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
            Frequency = item.Frequency,
            Interactable = item.Interactable,
            Color = item.Color,
            State = item.StateType.ToString()
        };

        item.CurrentStrategy.TransmitData(data);
        Debug.Log($"[Сканирование] Расстояние до предмета {item.ID}: {distance:F2} м");
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