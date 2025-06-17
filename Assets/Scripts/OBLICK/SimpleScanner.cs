using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SimpleScanner : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text statusText;
    public TMP_Text remainingCountText;
    public TMP_Text destroyedCountText;

    [Header("Scan Settings")]
    public float scanRange = 10f;
    public float scanRadius = 1.5f;
    public float pulseSpeed = 2f;
    public Color scanColor = Color.red;
    public LayerMask scanLayer;
    public Camera playerCamera;

    [SerializeField] private bool _isBinocularEquipped;
    public bool isBinocularEquipped => _isBinocularEquipped;

    private HashSet<GameObject> totalCameras = new HashSet<GameObject>();
    private HashSet<GameObject> destroyedCameras = new HashSet<GameObject>();
    private bool isScanning;
    private Coroutine statusMessageCoroutine;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        // Подсчет всех камер в сцене
        GameObject[] allCameras = GameObject.FindGameObjectsWithTag("Scannable");
        totalCameras = new HashSet<GameObject>(allCameras);
        UpdateUI();
    }

    void Update()
    {
        if (!isBinocularEquipped) return;

        HandleInput();
        if (isScanning) ScanArea();
        UpdateUI();
    }

    public void SetActive(bool active)
    {
        _isBinocularEquipped = active;
        if (!active)
        {
            isScanning = false;
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isScanning = !isScanning;
        }

        if (isScanning && Input.GetMouseButtonDown(0))
        {
            TryDestroyCamera();
        }
    }

    void ScanArea()
    {
        Vector3 start = Camera.main.transform.position;
        Vector3 end = start + Camera.main.transform.forward * scanRange;

        Collider[] hits = Physics.OverlapCapsule(start, end, scanRadius, scanLayer);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Scannable"))
            {
                GameObject cameraObj = hit.gameObject;

                // Проверка на уничтожение
                if (destroyedCameras.Contains(cameraObj)) continue;

                // Запуск мигания
                cameraObj.GetComponent<IRReflective>()?.StartBlinking(scanColor, pulseSpeed);

                // Временное сообщение об обнаружении
                ShowStatusMessage("Обнаружена камера!", 5f);
            }
        }
    }

    void TryDestroyCamera()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, scanRange, scanLayer))
        {
            if (hit.collider.CompareTag("Scannable"))
            {
                GameObject cameraObj = hit.collider.gameObject;

                // Уничтожение
                Destroy(cameraObj);
                destroyedCameras.Add(cameraObj);

                // Обновление UI
                ShowStatusMessage("Камера уничтожена!", 1f);
            }
        }
    }

    void ShowStatusMessage(string message, float duration)
    {
        if (statusMessageCoroutine != null)
            StopCoroutine(statusMessageCoroutine);

        statusMessageCoroutine = StartCoroutine(StatusMessageCoroutine(message, duration));
    }

    IEnumerator StatusMessageCoroutine(string message, float duration)
    {
        string previousStatus = statusText.text;
        statusText.text = message;
        yield return new WaitForSeconds(duration);
        statusText.text = previousStatus;
    }

    public void UpdateUI()
    {
        // Камер осталось
        int remaining = totalCameras.Count - destroyedCameras.Count;
        remainingCountText.text = $"Камер осталось: {remaining}";
        destroyedCountText.text = $"Уничтожено: {destroyedCameras.Count}";
    }

    public void EquipBinocular()
    {
        _isBinocularEquipped = true;
        UpdateUI();
    }

    public void UnequipBinocular()
    {
        _isBinocularEquipped = false;
        isScanning = false;
        UpdateUI();
    }

    void OnDrawGizmos()
    {
        if (!isScanning || !isBinocularEquipped) return;

        Gizmos.color = Color.blue;
        Vector3 start = Camera.main.transform.position;
        Vector3 end = start + Camera.main.transform.forward * scanRange;
        Gizmos.DrawWireSphere(start, scanRadius);
        Gizmos.DrawWireSphere(end, scanRadius);
        Gizmos.DrawLine(start, end);
    }
}