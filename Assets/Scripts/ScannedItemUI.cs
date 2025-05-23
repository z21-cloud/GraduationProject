using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScannedItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI idText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI frequencyText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private float transparency = 0.5f;
    [SerializeField] private Image colorIndicator;

    public void Initialize(ItemData data)
    {
        idText.text = $"{data.ID}";
        typeText.text = $"{data.Type}";
        frequencyText.text = $"{data.Frequency:F2} ÌÃö";
        distanceText.text = $"{data.Distance:F2} ì";
        float maxDistance = 10f;
        float alpha = Mathf.InverseLerp(maxDistance, 0f, data.Distance);
        alpha = Mathf.Clamp01(alpha);

        Color colorWithAlpha = data.Color;
        colorWithAlpha.a = alpha;
        colorIndicator.color = colorWithAlpha;

    }
}
