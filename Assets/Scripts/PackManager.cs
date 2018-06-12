using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackManager : MonoBehaviour {

    public WolfManager wolfManager;
    public SkillManager skillManager;

    public List<Pack> packList = new List<Pack>();

    public int packIndex = 0;
    Pack currentPack;

	void Start () {
        Load();
	}

    void LoadFirst() {
        Pack pack = new Pack {
            amountOfWolves = 3,
            food = 50,
            experience = 0,
            level = 1
        };
        pack.health = new float[pack.amountOfWolves];
        for (int i = 0; i < pack.amountOfWolves; i++) {
            pack.health[i] = 80;
        }

        pack.skills = skillManager.GetRandomSkillSaves();

        packList.Add(pack);
        currentPack = pack;
    }

    void Load() {
        if (PlayerPrefs.HasKey("PackCount")) {
            int count = PlayerPrefs.GetInt("PackCount");
            packIndex = PlayerPrefs.GetInt("PackIndex");
            for (int i = 0; i < count; i++) {
                Pack pack = new Pack();
                string[] data = PlayerPrefs.GetString("Pack" + i).Split('/');
                pack.amountOfWolves = int.Parse(data[0]);
                pack.food = float.Parse(data[1]);
                pack.experience = float.Parse(data[2]);
                pack.level = int.Parse(data[3]);

                pack.health = new float[pack.amountOfWolves];
                for (int k = 0; k < pack.amountOfWolves; k++) {
                    pack.health[k] = float.Parse(data[4 + k]);
                }
                int newbegin = 4 + pack.amountOfWolves;

                pack.startingPosition = new Vector3(float.Parse(data[newbegin]), 0, float.Parse(data[newbegin + 1]));

                pack.skills = new SkillSave[5];
                for (int p = 0; p < 5; p++) {
                    pack.skills[p] = new SkillSave();
                    pack.skills[p].skillName = data[newbegin + 2 + (3 * p) + 0];
                    pack.skills[p].skillCount = int.Parse(data[newbegin + 2 + (3 * p) + 1]);
                    pack.skills[p].skillFinish = int.Parse(data[newbegin + 2 + (3 * p) + 2]) == 1;
                }
                newbegin = newbegin + 2 + (3 * 5);

                packList.Add(pack);
            }
            currentPack = packList[packIndex];
        }else {
            LoadFirst();
        }

        wolfManager.LoadPack(currentPack);
        skillManager.LoadPack(currentPack);
    }

    public void SwitchPack(int newIndex) {

    }

    public void SaveCurrentPack() {
        wolfManager.SavePack(packList[packIndex]);
    }

    public void Save() {
        SaveCurrentPack();

        PlayerPrefs.SetInt("PackCount", packList.Count);
        PlayerPrefs.SetInt("PackIndex", packIndex);
        for (int i = 0; i < packList.Count; i++) {
            string data = "";
            data += packList[i].amountOfWolves.ToString("F0") + "/";
            data += packList[i].food.ToString("F2") + "/";
            data += packList[i].experience.ToString("F2") + "/";
            data += packList[i].level.ToString("F0") + "/";
            for (int k = 0; k < packList[i].amountOfWolves; k++) {
                data += packList[i].health[k].ToString("F2") + "/";
            }
            data += packList[i].startingPosition.x.ToString("F2") + "/";
            data += packList[i].startingPosition.z.ToString("F2") + "/";

            for (int k = 0; k < packList[i].skills.Length; k++) {
                data += packList[i].skills[k].skillName + "/";
                data += packList[i].skills[k].skillCount.ToString("F0") + "/";
                data += (packList[i].skills[k].skillFinish ? 1 : 0).ToString("F0") + "/";
            }
            PlayerPrefs.SetString("Pack" + i, data);
        }
    }
}

[System.Serializable]
public class Pack {
    public int amountOfWolves;
    public float food;
    public float experience;
    public int level;
    public Vector3 startingPosition;

    public float[] health;

    public SkillSave[] skills;
}

public class SkillSave {
    public string skillName;
    public int skillCount;
    public bool skillFinish;
}
