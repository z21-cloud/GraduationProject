using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableScanner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer scannerRenderer;

    [Header("Settings")]
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;
    [Range(0.1f, 1f)] public float maxScanDistance = 0.3f;

    [Header("Runtime Status")]
    [ReadOnly] public bool isScanningMode = false;
    [ReadOnly] public bool isConnectedToCable = false;
    [ReadOnly] public bool vulnerabilityFound = false;
    [ReadOnly] public Cable currentCable;
    [ReadOnly] public GameObject currentVulnerability;

    private ST500Piranya piranya;

    // События для UI контроллера
    public event Action<string> OnStatusChanged;
    public event Action<bool> OnVulnerabilityStateChanged;
    public event Action<float> OnProgressUpdated;

    private void Start()
    {
        SetScanningMode(false);

        piranya = GetComponent<ST500Piranya>();
        SetScanningMode(false);
    }

    public void SetScanningMode(bool active)
    {
        isScanningMode = active;

        if (isScanningMode && piranya != null && !piranya.IsDeviceActive)
        {
            Debug.Log("Устройство выключено, сканирование невозможно");
            return;
        }

        // Изменяем цвет сканера
        if (scannerRenderer != null)
        {
            scannerRenderer.material.color = active ? activeColor : inactiveColor;
        }

        if (active)
        {
            StartScanning();
        }
        else
        {
            StopScanning();
        }
    }

    private void StartScanning()
    {
        // Останавливаем любой предыдущий звук сканирования
        AudioManager.Instance.StopLoopingSound();

        // Запускаем звук сканирования
        AudioManager.Instance.PlayCableScanningSound();

        OnStatusChanged?.Invoke("Scanning mode activated");
    }

    private void StopScanning()
    {
        if (AudioManager.Instance.IsLoopingSoundPlaying() &&
        AudioManager.Instance.GetLoopingClip() == AudioManager.Instance.scanningSound)
        {
            AudioManager.Instance.StopLoopingSound();
        }
        // Останавливаем звук сканирования
        AudioManager.Instance.StopLoopingSound();

        // Отключаемся от кабеля
        DisconnectFromCable();

        // Уведомляем UI контроллер
        OnStatusChanged?.Invoke("Scanning mode deactivated");
    }

    private void Update()
    {
        if (isScanningMode && isConnectedToCable && currentCable != null)
        {
            // Рассчитываем прогресс сканирования
            Vector3 cableDirection = currentCable.worldEnd - currentCable.worldStart;
            Vector3 scannerPosition = transform.position - currentCable.worldStart;
            float progress = Vector3.Dot(scannerPosition, cableDirection.normalized) / cableDirection.magnitude;

            // Отправляем прогресс в UI контроллер
            OnProgressUpdated?.Invoke(Mathf.Clamp01(progress));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isScanningMode) return;

        // Проверка подключения к кабелю
        if (!isConnectedToCable)
        {
            Cable cable = other.GetComponent<Cable>();
            if (cable != null)
            {
                TryConnectToCable(cable);
            }
        }

        // Проверка обнаружения уязвимости
        if (isConnectedToCable && other.CompareTag("Vulnerability"))
        {
            if (!vulnerabilityFound)
            {
                FindVulnerability(other.gameObject);
            }
        }
    }

    private void TryConnectToCable(Cable cable)
    {
        bool isOnCable = cable.IsPointOnCable(transform.position, maxScanDistance);

        if (isOnCable)
        {
            ConnectToCable(cable);
        }
    }

    private void ConnectToCable(Cable cable)
    {
        currentCable = cable;
        isConnectedToCable = true;

        // Уведомляем UI контроллер
        OnStatusChanged?.Invoke($"Подключен к кабелю: {cable.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Cable>() == currentCable)
        {
            DisconnectFromCable();
        }
    }

    private void DisconnectFromCable()
    {
        isConnectedToCable = false;
        vulnerabilityFound = false;
        currentCable = null;
        currentVulnerability = null;

        // Уведомляем UI контроллер
        OnStatusChanged?.Invoke("Отключен от кабеля");
        OnVulnerabilityStateChanged?.Invoke(false);
    }

    private void FindVulnerability(GameObject vulnerability)
    {
        if (vulnerability == null) return;

        vulnerabilityFound = true;
        currentVulnerability = vulnerability;

        // Активируем визуальные эффекты
        VulnerabilityMarker marker = vulnerability.GetComponent<VulnerabilityMarker>();
        if (marker != null)
        {
            marker.Highlight();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVulnerabilityFoundSound();
        }

        // Уведомляем UI контроллер
        OnStatusChanged?.Invoke("Обнаружена уязвимость. Нажимет 'Е', чтобы ее уничтожить.");
        OnVulnerabilityStateChanged?.Invoke(true);
    }

    public void RemoveVulnerability()
    {
        if (vulnerabilityFound && currentCable != null)
        {
            // Активируем эффект уничтожения
            VulnerabilityMarker marker = currentVulnerability.GetComponent<VulnerabilityMarker>();
            if (marker != null)
            {
                marker.Remove();
            }
            else
            {
                Destroy(currentVulnerability);
            }
            currentCable.RemoveVulnerability();
            vulnerabilityFound = false;
            currentVulnerability = null;

            AudioManager.Instance.PlayVulnerabilityDestroyedSound();

            // Уведомляем UI контроллер
            OnStatusChanged?.Invoke("Уязвимость устранена");
            OnVulnerabilityStateChanged?.Invoke(false);
        }
    }
}
