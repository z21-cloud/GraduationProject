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
        return $"ID: {ID}, ���: {Type}, ����: {Color}, �������: {IsActive}, ����������: {Distance:F2}�";
    }
}