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
            return $"ID: {ID}, Тип: {Type}, Активно: {IsActive}, Расстояние: {Distance:F2} м";

        return $"ID: {ID}, Тип: {Type}, Частота: {Frequency:F2} МГц, Расстояние: {Distance:F2} м";
    }
}
