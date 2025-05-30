using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Cable : MonoBehaviour
{
    [Header("Cable Points")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Cable Settings")]
    [Tooltip("Толщина кабеля (радиус коллайдера)")]
    public float cableThickness = 0.1f;

    [Header("Vulnerability")]
    public GameObject vulnerabilityPrefab;
    [Tooltip("Randomize vulnerability position")]
    public bool useRandomPosition = true;

    [Tooltip("Manual position (0-1) if not randomized")]
    [Range(0f, 1f)] public float vulnerabilityPosition = 0.5f;

    [Header("Debug")]
    public Vector3 worldStart;
    public Vector3 worldEnd;
    [SerializeField] private GameObject vulnerability;
    // Показывает фактическое положение уязвимости
    [ReadOnly] public float actualVulnerabilityPosition;

    private LineRenderer debugLine;

    private void Start()
    {
        InitializeCable();
    }

    public void InitializeCable()
    {
        // Проверяем точки
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("StartPoint or EndPoint not assigned!");
            return;
        }

        // Обновляем мировые координаты
        worldStart = startPoint.position;
        worldEnd = endPoint.position;

        // Создаем уязвимость
        GenerateVulnerability();

        // Создаем визуализацию для отладки
        CreateDebugVisual();
    }

    private void GenerateVulnerability()
    {
        if (vulnerabilityPrefab == null)
        {
            Debug.LogWarning("Vulnerability prefab is not assigned!");
            return;
        }

        // Определяем позицию уязвимости
        float positionFactor = useRandomPosition
            ? Random.Range(0f, 1f)
            : Mathf.Clamp01(vulnerabilityPosition);

        // Позиция уязвимости вдоль кабеля
        Vector3 position = Vector3.Lerp(worldStart, worldEnd, positionFactor);

        // Создаем уязвимость
        vulnerability = Instantiate(vulnerabilityPrefab, position, Quaternion.identity);
        vulnerability.name = "Vulnerability_" + gameObject.name;

        // Сохраняем фактическую позицию для отладки
        actualVulnerabilityPosition = positionFactor;
    }

    private void CreateDebugVisual()
    {
        // Создаем LineRenderer только для отладки
        debugLine = gameObject.AddComponent<LineRenderer>();
        debugLine.positionCount = 2;
        debugLine.SetPosition(0, worldStart);
        debugLine.SetPosition(1, worldEnd);
        debugLine.startWidth = cableThickness;
        debugLine.endWidth = cableThickness;
        debugLine.material = new Material(Shader.Find("Standard")) { color = Color.blue };
    }

    public bool IsPointOnCable(Vector3 point, float maxDistance = 0.5f)
    {
        // Проекция точки на линию кабеля
        Vector3 lineDirection = worldEnd - worldStart;
        Vector3 pointDirection = point - worldStart;
        float t = Mathf.Clamp01(Vector3.Dot(pointDirection, lineDirection) / lineDirection.sqrMagnitude);
        Vector3 projectedPoint = worldStart + t * lineDirection;

        return Vector3.Distance(point, projectedPoint) < maxDistance;
    }

    public void RemoveVulnerability()
    {
        if (vulnerability != null)
        {
            Destroy(vulnerability);
            vulnerability = null;
        }
    }

    // Обновляем визуализацию при изменении в редакторе
    private void OnValidate()
    {
        if (startPoint != null && endPoint != null && !Application.isPlaying)
        {
            worldStart = startPoint.position;
            worldEnd = endPoint.position;

            // Обновляем только если не в случайном режиме
            if (!useRandomPosition && vulnerability != null)
            {
                Vector3 newPosition = Vector3.Lerp(worldStart, worldEnd, vulnerabilityPosition);
                vulnerability.transform.position = newPosition;
            }

            if (debugLine != null)
            {
                debugLine.SetPosition(0, worldStart);
                debugLine.SetPosition(1, worldEnd);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (startPoint == null || endPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(worldStart, cableThickness / 2);
        Gizmos.DrawWireSphere(worldEnd, cableThickness / 2);

        if (vulnerability != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(vulnerability.transform.position, 0.15f);
        }
    }
}
