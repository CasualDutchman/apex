using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackManager : MonoBehaviour {

    public List<Pack> packList = new List<Pack>();

	void Start () {
        Load();
	}

    void LoadFirst() {
        Pack pack = new Pack();
        pack.amountOfWolves = 3;
        pack.health = 90;
        pack.food = 50;
        pack.experience = 0;
        pack.level = 1;
    }

    void Load() {
        if (PlayerPrefs.HasKey("PackCount")) {
            int count = PlayerPrefs.GetInt("PackCount");
            for (int i = 0; i < count; i++) {
                Pack pack = new Pack();
                string[] data = PlayerPrefs.GetString("Pack" + i).Split('/');
                pack.amountOfWolves = int.Parse(data[0]);
                pack.health = float.Parse(data[1]);
                pack.food = float.Parse(data[2]);
                pack.experience = float.Parse(data[3]);
                pack.level = int.Parse(data[4]);
            }
        }else {
            LoadFirst();
        }
    }

    void Save() {
        PlayerPrefs.SetInt("PackCount", packList.Count);
        for (int i = 0; i < packList.Count; i++) {
            string data = "";
            data += packList[i].amountOfWolves.ToString("F0") + "/";
            data += packList[i].health.ToString("F2") + "/";
            data += packList[i].food.ToString("F2") + "/";
            data += packList[i].experience.ToString("F2") + "/";
            data += packList[i].level.ToString("F0") + "/";

            PlayerPrefs.SetString("Pack" + i, data);
        }
    }
}

[System.Serializable]
public class Pack {
    public int amountOfWolves;
    public float health;
    public float food;
    public float experience;
    public int level;

    public string[] skillnames;
    public int[] skillcounts;
    public bool[] skillfinishes;

    public void LoadFromSave() {

    }
}
