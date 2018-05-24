using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {

    public static SkillManager instance;

    public string resourceLocation;

    public SkillItem[] skills;
    bool[] completedSkillArray;

    void Awake() {
        instance = this;
    }

    void Start () {
        if (PlayerPrefs.HasKey("Skill0")) {
            LoadSkills();
        } else {
            GetRandomSkills();
        }
	}

    public float GetSkillShareAmount(string s) {
        for (int i = 0; i < skills.Length; i++) {
            if (skills[i].skill.code.ToLower() == s.ToLower()) {
                return skills[i].skill.skillShareAmount;
            }
        }
        return 0;
    }

    public bool IsSkillActive(string s) {
        for (int i = 0; i < skills.Length; i++) {
            if (skills[i].skill.code.ToLower() == s.ToLower()) {
                return completedSkillArray[i];
            }
        }
        return false;
    }

    public void KillAnimal(AnimalType type) {
        for (int i = 0; i < skills.Length; i++) {
            if (skills[i].skill.requiredAnimal == type) {
                skills[i].animalCounter++;
                if (skills[i].animalCounter >= skills[i].skill.animalCount) {
                    completedSkillArray[i] = true;
                }
                break;
            }
        }
    }

    void GetRandomSkills() {
        skills = new SkillItem[5];
        for (int i = 0; i < 5; i++) {
            skills[i] = new SkillItem();
        }
        completedSkillArray = new bool[5];
        for (int i = 0; i < 5; i++) {
            completedSkillArray[i] = false;
        }

        List<Skill> skillList = new List<Skill>(Resources.LoadAll<Skill>(resourceLocation));
        List<Skill> skillListA = new List<Skill>();
        List<Skill> skillListB = new List<Skill>();
        foreach (Skill skill in skillList) {
            if(skill.skillType == SkillType.A) {
                skillListA.Add(skill);
            } else {
                skillListB.Add(skill);
            }
        }

        skills[0].skill = skillListA[Random.Range(0, skillListA.Count)];
        skillListA.Remove(skills[0].skill);
        skills[1].skill = skillListA[Random.Range(0, skillListA.Count)];

        skills[2].skill = skillListB[Random.Range(0, skillListB.Count)];
        skillListB.Remove(skills[2].skill);
        skills[3].skill = skillListB[Random.Range(0, skillListB.Count)];
        skillListB.Remove(skills[3].skill);
        skills[4].skill = skillListB[Random.Range(0, skillListB.Count)];
    }

    void LoadSkills() {
        skills = new SkillItem[5];
        completedSkillArray = new bool[5];

        for (int i = 0; i < 5; i++) {
            skills[i] = new SkillItem();

            string str = PlayerPrefs.GetString("Skill" + i);
            string[] data = str.Split('/');

            skills[i].skill = Resources.Load<Skill>(resourceLocation + "/" + data[0]);
            skills[i].animalCounter = int.Parse(data[1]);
            completedSkillArray[i] = skills[i].animalCounter >= skills[i].skill.animalCount;
        }
    }

    public void SaveSkills() {
        for (int i = 0; i < skills.Length; i++) {
            string str = skills[i].skill.name + "/";
            str += skills[i].animalCounter;
            PlayerPrefs.SetString("Skill" + i, str);
        }
    }
}

[System.Serializable]
public class SkillItem{
    public Skill skill;
    public int animalCounter;
}

public enum SkillType { A, B}

public enum SkillShare { MoreDamageToEnemy, InstantKillEnemy, LessDamageFromEnemy, FasterMovement, AdditinalFood, Additionalhealth, NoDamageFromEnemy,
                         NoRunner, MoreXP, FindResting, NotGetAttack, HearEnemies, HearPrey}