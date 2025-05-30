using UnityEngine;

[System.Serializable]
public struct IRDeviceData
{
    public int ID;
    public ItemType Type;
    public Color Color;
    public bool IsActive;
    public float Distance;

    public override string ToString()
    {
        return $"ID: {ID}, Тип: {Type}, Цвет: {Color}, Активно: {IsActive}, Расстояние: {Distance:F2}м";
    }
}