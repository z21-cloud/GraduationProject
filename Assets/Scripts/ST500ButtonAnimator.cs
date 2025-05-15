using UnityEngine;
using System.Collections;

public class ST500ButtonAnimator : MonoBehaviour
{
    [SerializeField] private Transform buttonTransform; // Кнопка
    [SerializeField] private Renderer buttonRenderer; // Материал кнопки
    [SerializeField] private float moveSpeed = 0.1f; // Скорость анимации
    [SerializeField] private float moveDistance = 1f; // Расстояние движения

    private bool isDeviceActive;
    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = buttonTransform.localPosition;
    }

    public void UpdateButtonState(bool isActive)
    {
        if (isDeviceActive == isActive) return;

        isDeviceActive = isActive;
        StartCoroutine(AnimateButton());
    }

    private IEnumerator AnimateButton()
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = isDeviceActive
            ? originalPosition + Vector3.down * moveDistance
            : originalPosition;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime);

            buttonTransform.localPosition = Vector3.Lerp(
                buttonTransform.localPosition,
                targetPosition,
                t * moveSpeed
            );

            yield return null;
        }

        buttonTransform.localPosition = targetPosition;
        buttonRenderer.material.color = isDeviceActive ? Color.green : Color.red;
    }
}