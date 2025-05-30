using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluetoothFrequencyHoppingStrategy : IFrequencyStrategy
{
    private float currentFrequency;
    private readonly float minFreq = 2402f;
    private readonly float maxFreq = 2480f;

    public BluetoothFrequencyHoppingStrategy(float initialFrequency)
    {
        currentFrequency = initialFrequency;
    }

    public float GetCurrentFrequency()
    {
        return currentFrequency;
    }

    public void UpdateFrequency()
    {
        currentFrequency = Random.Range(minFreq, maxFreq);
        //Debug.Log($"[Bluetooth] „астота сменилась на {currentFrequency:F2} ћ√ц");
    }
}
