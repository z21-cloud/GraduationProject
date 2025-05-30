using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableScanner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer scannerRenderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ST500UIController uiController;

    [Header("Settings")]
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;
    public AudioClip scanningSound;
    public AudioClip vulnerabilitySound;
    [Range(0.1f, 1f)] public float maxScanDistance = 0.3f;
    [Range(0.5f, 2f)] public float minPitch = 0.8f;
    [Range(0.5f, 2f)] public float maxPitch = 1.2f;

    [Header("Runtime Status")]
    [ReadOnly] public bool isScanningMode = false;
    [ReadOnly] public bool isConnectedToCable = false;
    [ReadOnly] public bool vulnerabilityFound = false;
    [ReadOnly] public Cable currentCable;
    [ReadOnly] public GameObject currentVulnerability;

    private void Start()
    {
        SetScanningMode(false);
    }

    public void SetScanningMode(bool active)
    {
        isScanningMode = active;

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
        if (audioSource != null && scanningSound != null)
        {
            audioSource.clip = scanningSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        if (uiController != null)
        {
            uiController.ShowCableScanStatus("Scanning mode activated");
        }
    }

    private void StopScanning()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        DisconnectFromCable();

        if (uiController != null)
        {
            uiController.ShowCableScanStatus("Scanning mode deactivated");
        }
    }

    private void Update()
    {
        if (isScanningMode && isConnectedToCable && currentVulnerability != null)
        {
            UpdateScanningAudio();
        }
    }

    private void UpdateScanningAudio()
    {
        float distance = Vector3.Distance(transform.position, currentVulnerability.transform.position);
        float volume = 1 - Mathf.Clamp01(distance / 3f);
        float pitch = Mathf.Lerp(minPitch, maxPitch, 1 - volume);

        if (audioSource != null)
        {
            audioSource.volume = volume;
            audioSource.pitch = pitch;
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

        Debug.Log($"Try connect to cable: " +
                  $"Scanner: {transform.position}, " +
                  $"Cable: {cable.worldStart} - {cable.worldEnd}, " +
                  $"Result: {isOnCable}");

        if (isOnCable)
        {
            ConnectToCable(cable);
        }
    }

    private void ConnectToCable(Cable cable)
    {
        currentCable = cable;
        isConnectedToCable = true;

        if (uiController != null)
        {
            uiController.ShowCableScanStatus($"Connected to cable: {cable.name}");
        }

        Debug.Log($"Connected to cable: {cable.name}");
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

        if (audioSource != null)
        {
            audioSource.volume = 1f;
            audioSource.pitch = 1f;
        }

        if (uiController != null)
        {
            uiController.ShowCableScanStatus("Disconnected from cable");
        }
    }

    private void FindVulnerability(GameObject vulnerability)
    {
        vulnerabilityFound = true;
        currentVulnerability = vulnerability;

        if (audioSource != null && vulnerabilitySound != null)
        {
            audioSource.PlayOneShot(vulnerabilitySound);
        }

        if (uiController != null)
        {
            uiController.ShowCableScanStatus("VULNERABILITY FOUND! Press E to remove");
        }

        Debug.Log("Vulnerability found!");
    }

    public void RemoveVulnerability()
    {
        if (vulnerabilityFound && currentCable != null)
        {
            currentCable.RemoveVulnerability();
            vulnerabilityFound = false;
            currentVulnerability = null;

            if (uiController != null)
            {
                uiController.ShowCableScanStatus("Vulnerability removed");
            }

            Debug.Log("Vulnerability removed");
        }
    }
}
