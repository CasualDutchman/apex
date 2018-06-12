using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SeasonalAttributes", menuName = "Attribute/Season", order = 1)]
public class SeasonalAttributes : ScriptableObject {

    [Header("World")]
    public Material worldMaterial;
    public Color wolfOutline;
    public Color preyOutline;
    public Color predatorOutline;

    [Header("Wolves")]
    public float extraMobility;
    public float extraDamage;
    public float extraEnemyDamage;
    public float foodConsumptionPerMinute;
    public float healthRegenOnResting;

    [Header("UI")]
    public Color backgroundColor;
    public Color textColor, secondaryTextColor;
    public Color xpBar, healthBar, hungerBar;
}
