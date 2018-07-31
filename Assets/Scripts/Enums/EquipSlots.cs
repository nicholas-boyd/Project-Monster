using UnityEngine;
using System.Collections;
[System.Flags]
public enum EquipSlots
{
    None = 0,
    Hands = 1 << 0,
    Head = 1 << 1,
    UpperBody = 1 << 2,
    LowerBody = 1 << 3,
    Feet = 1 << 4
}