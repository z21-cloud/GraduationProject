using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST500Antenna : MonoBehaviour
{
    [Header("��������� ������������")]
    [SerializeField] private Renderer antennaRenderer;
    [SerializeField] private float rotationSpeed = 30f; // �������� �������� � �������� � �������
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // ��� �������� (������ Y)

    [Header("����� ���������")]
    [SerializeField] private Color deviceOffColor = Color.gray;
    [SerializeField] private Color scanningInactiveColor = Color.red;
    [SerializeField] private Color scanningActiveColor = Color.green;

    [Header("������")]
    [SerializeField] private GameObject parentObject;

    private ST500Piranya piranya;
    private ST500Scanner scanner;
    private Coroutine rotationCoroutine;

    private void Awake()
    {
        piranya = GetComponentInParent<ST500Piranya>();
        scanner = GetComponentInParent<ST500Scanner>();

        if (piranya == null || scanner == null)
        {
            Debug.LogError("�� ������� ���������� ST500Piranya ��� ST500Scanner");
        }
    }

    private void OnEnable()
    {
        ST500Piranya.OnDeviceStateChanged += HandleDeviceStateChange;
        ST500Scanner.OnScanningStateChanged += HandleScanningStateChange;
    }

    private void OnDisable()
    {
        ST500Piranya.OnDeviceStateChanged -= HandleDeviceStateChange;
        ST500Scanner.OnScanningStateChanged -= HandleScanningStateChange;
    }

    private void Start()
    {
        UpdateAntennaVisuals();
    }

    private void HandleDeviceStateChange(bool isActive)
    {
        UpdateAntennaVisuals();
    }

    private void HandleScanningStateChange(bool isActive)
    {
        UpdateAntennaVisuals();
    }

    private void UpdateAntennaVisuals()
    {
        if (antennaRenderer == null) return;

        // ��������� ����
        if (!piranya.IsDeviceActive)
        {
            antennaRenderer.material.color = deviceOffColor;
            StopRotation();
        }
        else if (!scanner.IsScanningActive)
        {
            antennaRenderer.material.color = scanningInactiveColor;
            StopRotation();
        }
        else if(scanner.IsScanningActive && piranya.IsDeviceActive)
        {
            antennaRenderer.material.color = scanningActiveColor;
            StartRotation();
        }
    }

    private void StartRotation()
    {
        if (rotationCoroutine != null) return;
        rotationCoroutine = StartCoroutine(RotateAntenna());
    }

    private void StopRotation()
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }
    }

    private IEnumerator RotateAntenna()
    {
        while (true)
        {
            parentObject.transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // ��� ������������ � ���������
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        if (piranya == null || scanner == null)
        {
            Debug.LogWarning("��������� ������� � ��������� �������� ������ � ����");
        }
    }
}
