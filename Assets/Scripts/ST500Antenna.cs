using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST500Antenna : MonoBehaviour
{
    [Header("Настройки визуализации")]
    [SerializeField] private Renderer antennaRenderer;
    [SerializeField] private float rotationSpeed = 30f; // Скорость вращения в градусах в секунду
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // Ось вращения (обычно Y)

    [Header("Цвета состояния")]
    [SerializeField] private Color deviceOffColor = Color.gray;
    [SerializeField] private Color scanningInactiveColor = Color.red;
    [SerializeField] private Color scanningActiveColor = Color.green;

    [Header("Префаб")]
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
            Debug.LogError("Не найдены компоненты ST500Piranya или ST500Scanner");
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

        // Обновляем цвет
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

    // Для тестирования в редакторе
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        if (piranya == null || scanner == null)
        {
            Debug.LogWarning("Настройки антенны в редакторе доступны только в игре");
        }
    }
}
