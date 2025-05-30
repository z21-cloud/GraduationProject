using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemData
{
    public int ID;
    public ItemType Type;
    public float Frequency;
    public bool Interactable;
    public Color Color;
    public string State;
    public float Distance;

    public bool IsIRDevice;
    public bool IsActive;

    public override string ToString()
    {
        if (IsIRDevice)
            return $"ID: {ID}, ���: {Type}, �������: {IsActive}, ����������: {Distance:F2} �";

        return $"ID: {ID}, ���: {Type}, �������: {Frequency:F2} ���, ����������: {Distance:F2} �";
    }
}
