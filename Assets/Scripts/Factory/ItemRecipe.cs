using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemRecipe : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public EquipSlots defaultSlots;
    public EquipSlots secondarySlots;
    public List<Feature> features;
}