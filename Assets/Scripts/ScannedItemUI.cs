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
    [SerializeField] private Image colorIndicator;

    private Color defaultColor;
    private Color highLightColor = Color.red;

    public void Initialize(ItemData data)
    {
        idText.text = $"{data.ID}";
        typeText.text = $"{data.Type}";
        defaultColor = idText.color;

        if (data.IsIRDevice)
        {
            frequencyText.text = "» -сигнал";
            distanceText.text = $"{data.Distance:F2} м";
            colorIndicator.color = new Color(data.Color.r, data.Color.g, data.Color.b, 0.8f);
        }
        else
        {
            frequencyText.text = $"{data.Frequency:F2} ћ√ц";
            distanceText.text = $"{data.Distance:F2} м";
            float alpha = Mathf.Clamp01(1 - data.Distance / 10f);
            colorIndicator.color = new Color(data.Color.r, data.Color.g, data.Color.b, alpha);
        }

        /*float maxDistance = 10f;
        float alpha = Mathf.InverseLerp(maxDistance, 0f, data.Distance);
        alpha = Mathf.Clamp01(alpha);

        Color colorWithAlpha = data.Color;
        colorWithAlpha.a = alpha;
        colorIndicator.color = colorWithAlpha*/;
    }

    public void SetHighlight(bool isHighlighted)
    {
        idText.color = isHighlighted ? highLightColor : defaultColor;
        typeText.color = isHighlighted ? highLightColor : defaultColor;
    }
}
