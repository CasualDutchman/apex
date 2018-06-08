using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WolfManager : MonoBehaviour {

    public static WolfManager instance;

    public UIComponents components;
    public UIManager uiManager;

    public GameObject wolfObj;
    public int amountOfWolves;

    public float maxhealth;
    float allHeath;

    public float maxFood;
    public float food;
    float foodTimer;
    public float foodDepletePS;

    public float experience;
    public int level;


    List<WolfMovement> wolfList = new List<WolfMovement>();
    Vector3 wolfCenter;

    public List<Transform> animalList = new List<Transform>();
    Vector3 animalCenter;

    public Vector3 startingPosition;

    void Awake() {
        instance = this;
    }

    void Start () { 
        
    }

    public void LoadPack(Pack pack) {
        food = pack.food;
        experience = pack.experience;
        level = pack.level;

        for (int i = 0; i < pack.amountOfWolves; i++) {
            WolfMovement m = Instantiate(wolfObj).GetComponent<WolfMovement>();
            Vector2 ran = Random.insideUnitCircle * amountOfWolves;
            m.transform.position = pack.startingPosition + new Vector3(ran.x, 0, ran.y);
            m.transform.eulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
            m.manager = this;

            Wolf wolf = m.GetComponent<Wolf>();
            wolf.wolfManager = this;
            wolf.maxHealth = maxhealth;
            wolf.health = pack.health[i];

            wolfList.Add(m);
            animalList.Add(m.transform);
        }

        //Load();
        LoadBeginScreen();

        StartCoroutine(FirstUpdate());
    }

    void LoadBeginScreen() {
        if (PlayerPrefs.HasKey("SavedTime")) {
            long time = System.Convert.ToInt64(PlayerPrefs.GetString("SavedTime"));
            System.DateTime oldDate = System.DateTime.FromBinary(time);
            System.DateTime newDate = System.DateTime.Now;
            System.TimeSpan differ = newDate.Subtract(oldDate);

            int max = 672;//672 is the amount of 15 min in 7 days
            int min15 = Mathf.Clamp(Mathf.FloorToInt((float)differ.TotalMinutes / 15f), 0, max);
            Debug.Log(differ.TotalMinutes);
            if (min15 > 0) {
                components.screenBegin.SetActive(true);
                components.textBeginTitle.GetComponent<TextMeshProUGUI>().text = LocalizationManager.instance.GetLocalizedValue("whengone");


                bool attacked = Random.Range(0, 3 + (max * ((1 / 1.035f) * min15))) <= 2;
                if (attacked) {
                    for (int i = 0; i < amountOfWolves; i++) {
                        wolfList[i].GetComponent<Wolf>().health = Mathf.Clamp(wolfList[i].GetComponent<Wolf>().health - 10f, 0, maxhealth);
                    }

                    string loc = LocalizationManager.instance.GetLocalizedValue("goneattacked");
                    string value = string.Format(loc, 10.ToString(), min15.ToString());
                    components.textBeginExplain.GetComponent<TextMeshProUGUI>().text = value.Replace("\\n", "\n");
                } else {
                    food = Mathf.Clamp(food - min15, 0, maxFood);

                    string loc = LocalizationManager.instance.GetLocalizedValue("gonenormal");
                    string value = string.Format(loc, min15.ToString());
                    components.textBeginExplain.GetComponent<TextMeshProUGUI>().text = value.Replace("\\n", "\n");

                    for (int i = 0; i < amountOfWolves; i++) {
                        wolfList[i].GetComponent<Wolf>().health = Mathf.Clamp(wolfList[i].GetComponent<Wolf>().health + (min15 * 5), 0, maxhealth);
                    }
                }
            } else {
                uiManager.ChangeScreen(Screens.Hud);
            }
        } else {
            uiManager.ChangeScreen(Screens.Hud);
        }
    }

    void Update() {
        wolfCenter = Vector3.zero;
        foreach (WolfMovement wolf in wolfList) {
            wolfCenter += wolf.transform.position;
        }
        wolfCenter /= wolfList.Count;

        animalCenter = Vector3.zero;
        foreach (Transform animal in animalList) {
            animalCenter += animal.position;
        }
        animalCenter /= animalList.Count;

        foodTimer += Time.deltaTime;
        if (foodTimer >= 1) {
            food -= foodDepletePS;
            UpdateFoodBar();
            foodTimer -= 1;
        }
    }


    public void Load() {
        if (PlayerPrefs.HasKey("WolfPack")) {
            string[] wolfData = PlayerPrefs.GetString("WolfPack").Split('/');
            food = float.Parse(wolfData[0]);
            experience = float.Parse(wolfData[1]);
            level = int.Parse(wolfData[2]);
        }

        if (PlayerPrefs.HasKey("Wolfs")) {
            string[] data = PlayerPrefs.GetString("Wolfs").Split('/');
            for (int i = 0; i < data.Length; i++) {
                wolfList[i].GetComponent<Wolf>().health = float.Parse(data[i]);
            }
        }
    }

    public void Save() {
        string str = "";
        str += food.ToString("F2") + "/";
        str += experience.ToString("F2") + "/";
        str += level.ToString("F0");

        PlayerPrefs.SetString("WolfPack", str);

        string str2 = "";
        for (int i = 0; i < wolfList.Count; i++) {
            str2 += wolfList[i].GetComponent<Wolf>().health.ToString("F2") + (i < wolfList.Count - 1 ? "/" : "");
        }

        PlayerPrefs.SetString("Wolfs", str2);

        PlayerPrefs.SetString("SavedTime", System.DateTime.Now.ToBinary().ToString());
    }

    IEnumerator FirstUpdate() {
        yield return new WaitForEndOfFrame();
        UpdatehealthBar();
        UpdateExperience();
        UpdateFoodBar();
    }

    float GetMaxExperience() {
        return 100 + (Mathf.Floor(level / 4f) * 50);
    }

    public void AddFood(float f) {
        food = Mathf.Clamp(food + f, 0, maxFood);
    }

    public void DepleteFood(float f) {
        food -= f;
        if (food <= 0) {

        }
    }

    public void AddExperience(float f) {
        bool b = SkillManager.instance.IsSkillActive("shareKnowledge");
        float skill = SkillManager.instance.GetSkillShareAmount("shareKnowledge");
        experience += f + (b ? f * skill : 0);
        if (experience >= GetMaxExperience()) {
            experience -= GetMaxExperience();
            level++;
        }
        UpdateExperience();
    }

    public void UpdatehealthBar() {
        allHeath = 0;
        foreach (WolfMovement wolf in wolfList) {
            allHeath += wolf.GetComponent<Wolf>().health;
        }
        allHeath /= wolfList.Count;

        UIManager.instance.UpdateHealthBar(allHeath / maxhealth);
    }

    public void UpdateFoodBar() {
        UIManager.instance.UpdateFoodBar(food / maxFood);
    }

    public void UpdateExperience() {
        UIManager.instance.UpdateExperienceBar(experience, GetMaxExperience());
        UIManager.instance.UpdateLevelText(level);
    }

    public void KillEnemy(Transform c) {
        foreach (WolfMovement wolf in wolfList) {
            wolf.KilledEnemy(c);
        }

        if (c.GetComponent<Enemy>()) {
            AddFood(c.GetComponent<Enemy>().foodGain);
        }
    }

    public Vector3 GetRandomCenter() {
        Vector2 ran = Random.insideUnitCircle * amountOfWolves;
        if(ran.magnitude < 2f)
            ran *= 2;
        return wolfCenter + new Vector3(ran.x, 0, ran.y);
    }

    public Vector3 GetCenter() {
        return wolfCenter;
    }
}
