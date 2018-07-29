using UnityEngine;
using System.Collections;
[System.Flags]
public enum EquipSlots
{
    None = 0,
    RightHand = 1 << 0,
    LeftHand = 1 << 1,
    Head = 1 << 2,
    UpperBody = 1 << 3,
    LowerBody = 1 << 4,
    RightFoot = 1 << 5,
    LeftFoot = 1 << 6
}