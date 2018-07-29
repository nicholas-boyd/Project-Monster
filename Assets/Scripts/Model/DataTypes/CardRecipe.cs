using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardRecipe : ScriptableObject {

    public string Name;
    public string Description;

    public Sprite Sprite;
    public Sprite AbilityRangeIndicator;
    public ParticleSystem ParticleEffect;
    public ParticleEffectType ParticalEffectType;
    public bool ProjectileEffect = false;
    public ParticleSystem RedHitMarker;
    public ParticleSystem BlueHitMarker;

    public int AbilityManaCost = 0;
    public string AbilityEffectTarget = "DefaultAbilityEffectTarget";
    public string AbilityEffect = "DamageAbilityEffect";
    public string AbilityPower = "PhysicalAbilityPower";
    public int BasePower = 0;
    public string AbilityRange = "ConstantAbilityRange";
    public int AbilityRangeHorizontal = 1;
}