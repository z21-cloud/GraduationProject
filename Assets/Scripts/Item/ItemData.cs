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

    public override string ToString()
    {
        return $"ID: {ID}, Тип: {Type}, Частота: {Frequency:F2} МГц, Расстояние: {Distance:F2} м";
    }
}
