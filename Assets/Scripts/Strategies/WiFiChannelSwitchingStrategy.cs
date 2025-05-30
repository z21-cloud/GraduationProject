using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiFiChannelSwitchingStrategy : IFrequencyStrategy
{
    private static readonly float[] channels = new float[]
    {
        2412f, 2417f, 2422f, 2427f, 2432f, 2437f, 2442f,
        2447f, 2452f, 2457f, 2462f, 2467f, 2472f, 2484f // 14 channels
    };

    private float currentFrequency = channels[0];

    public float GetCurrentFrequency()
    {
        return currentFrequency;
    }

    public void UpdateFrequency()
    {
        int randomIndex = Random.Range(0, channels.Length);
        currentFrequency = channels[randomIndex];
        //Debug.Log($"[WiFi] Канал сменился на {currentFrequency:F2} МГц");
    }
}
