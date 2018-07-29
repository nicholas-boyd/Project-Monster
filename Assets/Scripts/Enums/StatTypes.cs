using UnityEngine;
using System.Collections;
public enum StatTypes
{
	HP,  // Hit Points
	MHP, // Max Hit Points
	MP,  // Mana
	MMP, // Max Mana
	CON, // Constitution, determines MHP and hp recovery
	STR, // Strength, determines attack power
	CHA, // Charisma, gives map bonuses
	DEX, // Dexterity, determines speed
    WIS, // Wisdom, determines MMP and mana recovery
    SPD, // Speed
	Count//Count, always last stattype. Never set, used to get number of stattypes when making array in stats
}