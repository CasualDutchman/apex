using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skills/Skill", order = 1)]
public class Skill : ScriptableObject {
    public string code;
    public string unlocalizedName;
    public string unlocalizedDescription;
    public string unlocalizedRequire;
    public SkillType skillType;
    public AnimalType requiredAnimal;
    public int animalCount;
    public SkillShare skillShare;
    public float skillShareAmount;
}
