using UnityEngine;
using System.Collections;

public class IRReflective : MonoBehaviour
{
    [Range(0.1f, 5f)]
    public float reflectivity = 1.5f;
    private Material material;
    private Color originalColor;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        material = renderer.material;
        originalColor = material.GetColor("_BaseColor"); // Используем _BaseColor вместо _Color
        material.SetFloat("_IRReflection", reflectivity);
        material.SetFloat("_BlinkStrength", 0); // Убираем мигание в начале
    }

    public void StartBlinking(Color targetColor, float intensity)
    {
        if (material == null) return;
        StopAllCoroutines();
        material.SetColor("_BlinkColor", targetColor);
        StartCoroutine(BlinkEffect(intensity));
    }

    public void StopBlinking()
    {
        if (material == null) return;
        StopAllCoroutines();
        material.SetFloat("_BlinkStrength", 0); // Выключаем мигание
        material.color = originalColor;
    }

    private IEnumerator BlinkEffect(float intensity)
    {
        material.SetFloat("_BlinkStrength", 1);
        while (true)
        {
            float t = Mathf.Sin(Time.time * intensity) * 0.5f + 0.5f;
            material.SetFloat("_IRReflection", Mathf.Lerp(reflectivity, reflectivity * 2, t));
            yield return null;
        }
    }
}
