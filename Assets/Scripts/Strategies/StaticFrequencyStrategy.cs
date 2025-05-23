using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticFrequencyStrategy : IFrequencyStrategy
{
    private readonly float fixedFrequency;

    public StaticFrequencyStrategy(float frequency)
    {
        fixedFrequency = frequency;
    }
    public float GetCurrentFrequency() => fixedFrequency;

    public void UpdateFrequency()
    {
        //Nothing to do, cause static
    }
}
