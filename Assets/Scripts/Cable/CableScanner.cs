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

    // ������� ��� UI �����������
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
            Debug.Log("���������� ���������, ������������ ����������");
            return;
        }

        // �������� ���� �������
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
        // ������������� ����� ���������� ���� ������������
        AudioManager.Instance.StopLoopingSound();

        // ��������� ���� ������������
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
        // ������������� ���� ������������
        AudioManager.Instance.StopLoopingSound();

        // ����������� �� ������
        DisconnectFromCable();

        // ���������� UI ����������
        OnStatusChanged?.Invoke("Scanning mode deactivated");
    }

    private void Update()
    {
        if (isScanningMode && isConnectedToCable && currentCable != null)
        {
            // ������������ �������� ������������
            Vector3 cableDirection = currentCable.worldEnd - currentCable.worldStart;
            Vector3 scannerPosition = transform.position - currentCable.worldStart;
            float progress = Vector3.Dot(scannerPosition, cableDirection.normalized) / cableDirection.magnitude;

            // ���������� �������� � UI ����������
            OnProgressUpdated?.Invoke(Mathf.Clamp01(progress));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isScanningMode) return;

        // �������� ����������� � ������
        if (!isConnectedToCable)
        {
            Cable cable = other.GetComponent<Cable>();
            if (cable != null)
            {
                TryConnectToCable(cable);
            }
        }

        // �������� ����������� ����������
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

        // ���������� UI ����������
        OnStatusChanged?.Invoke($"��������� � ������: {cable.name}");
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

        // ���������� UI ����������
        OnStatusChanged?.Invoke("�������� �� ������");
        OnVulnerabilityStateChanged?.Invoke(false);
    }

    private void FindVulnerability(GameObject vulnerability)
    {
        if (vulnerability == null) return;

        vulnerabilityFound = true;
        currentVulnerability = vulnerability;

        // ���������� ���������� �������
        VulnerabilityMarker marker = vulnerability.GetComponent<VulnerabilityMarker>();
        if (marker != null)
        {
            marker.Highlight();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVulnerabilityFoundSound();
        }

        // ���������� UI ����������
        OnStatusChanged?.Invoke("���������� ����������. ������� '�', ����� �� ����������.");
        OnVulnerabilityStateChanged?.Invoke(true);
    }

    public void RemoveVulnerability()
    {
        if (vulnerabilityFound && currentCable != null)
        {
            // ���������� ������ �����������
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

            // ���������� UI ����������
            OnStatusChanged?.Invoke("���������� ���������");
            OnVulnerabilityStateChanged?.Invoke(false);
        }
    }
}
