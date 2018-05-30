using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Season { Spring, Summer, Autumn, Winter }

public class SeasonManager : MonoBehaviour {

    public static SeasonManager instance;

    public Material wolfOutline, preyOutline, predatorOutline;
    public SeasonalAttributes spring, summer, autumn, winter;

    public bool overrideSeason;
    public Season currentSeason;
    public Season season { get { return currentSeason; } }

    Material currentSeasonMaterial;
    public Material seasonMaterial { get { return currentSeasonMaterial; } }

    SeasonalAttributes currentSeasonAttributes;
    public SeasonalAttributes seasonAttribute { get { return currentSeasonAttributes; } }

    bool saveTime;

    void Awake() {
        instance = this;
        LoadSeason();
    }

    void LoadSeason() {
        if (!overrideSeason) {
            if (PlayerPrefs.HasKey("SeasonTime")) {
                long time = System.Convert.ToInt64(PlayerPrefs.GetString("SeasonTime"));
                System.DateTime oldDate = System.DateTime.FromBinary(time);
                System.DateTime newDate = System.DateTime.Now;
                System.TimeSpan difer = newDate.Subtract(oldDate);

                #if UNITY_EDITOR
                Debug.Log(difer.TotalHours + " / " + difer.Days + "d / " + difer.Hours + "h / " + difer.Minutes + "m / " + difer.Seconds + "s");
                #endif

                int seasonCode = PlayerPrefs.GetInt("Season");

                if (difer.TotalHours >= 24) {
                    int dif = Mathf.Abs(Mathf.FloorToInt((float)difer.TotalHours / 24f));
                    if (dif >= 4)
                        dif = dif % 4;

                    seasonCode += dif;
                    if (seasonCode >= 4)
                        seasonCode = seasonCode % 4;

                    saveTime = true;
                } else {
                    saveTime = false;
                }

                currentSeason = seasonCode == 0 ? Season.Spring : (seasonCode == 1 ? Season.Summer : (seasonCode == 2 ? Season.Autumn : Season.Winter));
            } else {
                currentSeason = Season.Spring;
                saveTime = true;
            }
        }

        currentSeasonAttributes = currentSeason == Season.Spring ? spring : (currentSeason == Season.Summer ? summer : (currentSeason == Season.Autumn ? autumn : winter));

        wolfOutline.SetColor("_OutlineColor", currentSeasonAttributes.wolfOutline);
        preyOutline.SetColor("_OutlineColor", currentSeasonAttributes.preyOutline);
        predatorOutline.SetColor("_OutlineColor", currentSeasonAttributes.predatorOutline);

        currentSeasonMaterial = currentSeasonAttributes.worldMaterial;
    }

    public void OnQuit() {
        if (saveTime)
            PlayerPrefs.SetString("SeasonTime", System.DateTime.Now.ToBinary().ToString());

        PlayerPrefs.SetInt("Season", currentSeason == Season.Spring ? 0 : (currentSeason == Season.Summer ? 1 : (currentSeason == Season.Autumn ? 2 : 3)));
    }

    void Update () {
		
	}
}
