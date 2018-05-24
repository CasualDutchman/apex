using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum Screens { Hud, Settings, Alpha }

[System.Serializable]
public class SkillHolder {
    public Image skillRadial;
    public TextMeshProUGUI skillName;
}

public class UIManager : MonoBehaviour { 

    Screens currentScreen = Screens.Hud;

    public static UIManager instance;

    public UIComponents components;
    public EventSystem eventSystem;

    Settingsmanager settings;
    SkillManager skillManager;
    SeasonManager seasonManager;

    Dictionary<string, LocalizedItem> textRegistry = new Dictionary<string, LocalizedItem>();

    SkillHolder[] skillHolder = new SkillHolder[5];

    [Header("Adio")]
    UnityEngine.Audio.AudioMixer mixer;

    [Header("HUD")]
    public string unlocalizedExperience;
    public string unlocalizedLevel;
    public string unlocalizedAlpha;

    [Header("Alpha")]
    public string unlocalizedAlphaTitle;

    [Header("Settings")]
    public string unlocalizedSettingsTitle;
    public string unlocalizedLanguage;
    public string unlocalizedTutorial;
    public string unlocalizedTutorialStart;
    public string unlocalizedGraphics;
    public string unlocalizedGraphicsLow;
    public string unlocalizedGraphicsMed;
    public string unlocalizedGraphicsHigh;
    public string unlocalizedAudio;
    public string unlocalizedAudioOnOff;

    void Awake() {
        instance = this;
    }

    void Start() {
        settings = Settingsmanager.instance;
        skillManager = GetComponent<SkillManager>();
        seasonManager = GetComponent<SeasonManager>();
        RegisterTexts();
        LoadSettings();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (currentScreen != Screens.Hud) {
                ChangeScreen(Screens.Hud);
            } else {
                skillManager.SaveSkills();
                seasonManager.OnQuit();
                //wolfPack.Save();
                PlayerPrefs.Save();
                Application.Quit();
            }
        }
    }

    void OnApplicationPause(bool pause) {
        if (pause) {
            skillManager.SaveSkills();
            //wolfPack.Save();
            PlayerPrefs.Save();
            seasonManager.OnQuit();
        }
    }

    void RegisterTexts() {
        //HUD
        RegisterText(components.textExperience, unlocalizedExperience);
        RegisterText(components.textLevel, unlocalizedLevel);
        RegisterButton(components.buttonSettings, () => ChangeScreen(Screens.Settings));
        RegisterButton(components.buttonAlpha, () => ChangeScreen(Screens.Alpha));
        //RegisterText(components.buttonAlpha.GetChild(0), unlocalizedAlpha);

        //Alpha
        RegisterSkill(components.skillA1, 0);
        RegisterSkill(components.skillA2, 1);
        RegisterSkill(components.skillB1, 2);
        RegisterSkill(components.skillB2, 3);
        RegisterSkill(components.skillB3, 4);
        RegisterButton(components.skillDescBack, () => HideSkillDesc());
        RegisterButton(components.buttonBackAlpha, () => ChangeScreen(Screens.Hud));

        //Settings
        RegisterButton(components.buttonBackSettings, () => ChangeScreen(Screens.Hud));
        RegisterText(components.textSettingsTitle, unlocalizedSettingsTitle);
        RegisterText(components.textTutorial, unlocalizedTutorial);
        RegisterText(components.textTutorialStart, unlocalizedTutorialStart);
        RegisterText(components.textLanguage, unlocalizedLanguage);
        RegisterText(components.textGraphical, unlocalizedGraphics);
        RegisterText(components.textGraphicalLow, unlocalizedGraphicsLow);
        RegisterText(components.textGraphicalMed, unlocalizedGraphicsMed);
        RegisterText(components.textGraphicalHigh, unlocalizedGraphicsHigh);
        RegisterText(components.textAudio, unlocalizedAudio);
        RegisterText(components.textAudioOnOff, unlocalizedAudioOnOff);
        RegisterButton(components.buttonTutorial, () => OnTutorial());
        RegisterToggle(components.buttonAudio, (b) => { OnToggleAudio(b); });
        RegisterButton(components.buttonEnglish, () => OnChangeLanguage("EN_us"));
        RegisterButton(components.buttonDutch, () => OnChangeLanguage("NL_nl"));
        RegisterButton(components.buttonGraphicsHigh, () => OnToggleGraphical(true));
        RegisterButton(components.buttonGraphicsLow, () => OnToggleGraphical(false));
    }

    public bool IsHittingUI() {
        return eventSystem.IsPointerOverGameObject();
    }

    public bool IsHUD() {
        return currentScreen == Screens.Hud;
    }

    #region Registry
    void RegisterButton(Transform t, UnityEngine.Events.UnityAction action) {
        Button b = t.GetComponent<Button>();
        b.onClick.AddListener(action);
    }

    void RegisterToggle(Transform t, UnityEngine.Events.UnityAction<bool> action) {
        Toggle tog = t.GetComponent<Toggle>();
        tog.onValueChanged.AddListener(action);
    }

    void RegisterSkill(Transform t, int i) {
        skillHolder[i] = new SkillHolder() {
            skillRadial = t.GetChild(0).GetComponent<Image>(),
            skillName = t.GetChild(1).GetComponent<TextMeshProUGUI>()
        };
        RegisterButton(t, () => ShowSkillDesc(i));
    }

    void RegisterText(Transform t, string key) {
        LocalizedItem item = t.gameObject.AddComponent<LocalizedItem>();
        item.Register(key.ToLower());
        item.Change();
        textRegistry.Add(key, item);
    }

    void UpdateText(string key, params object[] par) {
        //textRegistry[unlocalizedExperience].UpdateItem(par);
        textRegistry[key].Change();
        textRegistry[key].UpdateItem(par);
    }

    public void OnChangeLanguagePref() {
        foreach (KeyValuePair<string, LocalizedItem> item in textRegistry) {
            item.Value.Change();
            //item.Value.UpdateItem("");
        }
        //UpdateText(unlocalizedExperience);
    }

    public Image GetImage(Transform obj) {
        return obj.GetComponent<Image>();
    }
    #endregion

    #region Screens
    GameObject GetScreen(Screens screen) {
        switch (screen) {
            default: case Screens.Hud: return components.screenHUD;
            case Screens.Alpha: return components.screenAlpha;
            case Screens.Settings: return components.screenSettings;
        }
    }

    public void ChangeScreen(Screens screen) {
        if (screen == Screens.Alpha) {
            OnAlphaOpened();
        }
        StartCoroutine(IEChangeScreen(screen));
    }

    IEnumerator IEChangeScreen(Screens screen) {
        GetScreen(currentScreen).SetActive(false);
        currentScreen = screen;
        GetScreen(currentScreen).SetActive(true);
        yield return 0;
    }
    #endregion

    #region Alpha
    void OnAlphaOpened() {
        for (int i = 0; i < 5; i++) {
            skillHolder[i].skillName.text = LocalizationManager.instance.GetLocalizedValue(skillManager.skills[i].skill.unlocalizedName);
            if (skillManager.skills[i].skill.skillType == SkillType.A) {
                skillHolder[i].skillRadial.fillAmount = (float)skillManager.skills[i].animalCounter / (float)skillManager.skills[i].skill.animalCount;
            } else {
                skillHolder[i].skillRadial.fillAmount = 1;
            }
        }

        components.skillDescriptionBox.gameObject.SetActive(false);
    }

    public void HideSkillDesc() {
        components.skillDescriptionBox.gameObject.SetActive(false);
    }

    public void ShowSkillDesc(int i) {
        Debug.Log("ShowSkill");
        components.skillDescTitle.GetComponent<TextMeshProUGUI>().text = LocalizationManager.instance.GetLocalizedValue(skillManager.skills[i].skill.unlocalizedName);
        components.skillDescDesc.GetComponent<TextMeshProUGUI>().text = LocalizationManager.instance.GetLocalizedValue(skillManager.skills[i].skill.unlocalizedDescription);

        if (!string.IsNullOrEmpty(skillManager.skills[i].skill.unlocalizedRequire)) {
            string reqMessage = LocalizationManager.instance.GetLocalizedValue(skillManager.skills[i].skill.unlocalizedRequire);
            string animalName = LocalizationManager.instance.GetLocalizedValue(skillManager.skills[i].skill.requiredAnimal.ToString() + (skillManager.skills[i].skill.animalCount > 1 ? "multi" : string.Empty));
            reqMessage = string.Format(reqMessage, skillManager.skills[i].animalCounter, skillManager.skills[i].skill.animalCount, animalName);

            components.skillDescReq.GetComponent<TextMeshProUGUI>().text = reqMessage;
        } else {
            components.skillDescReq.GetComponent<TextMeshProUGUI>().text = "";
        }
        components.skillDescriptionBox.gameObject.SetActive(true);
    }
    #endregion

    #region Hud
    public void UpdateHealthBar(float fill) {
        GetImage(components.imageHealthFill).fillAmount = fill;
    }

    public void UpdateFoodBar(float fill) {
        GetImage(components.imageFoodBarFill).fillAmount = fill;
    }

    public void UpdateExperienceBar(float currentXP, float maxXP) {
        GetImage(components.imageExperienceBarFill).fillAmount = currentXP / maxXP;
        UpdateText(unlocalizedExperience, currentXP.ToString("F0"), maxXP.ToString("F0"));
    }

    public void UpdateLevelText(int i) {
        UpdateText(unlocalizedLevel, i, "");
    }
    #endregion

    #region Settings
    void LoadSettings() {
        settings.graphicalSettings = PlayerPrefs.GetInt(unlocalizedGraphics) == 0 ? Graphical.High : Graphical.Low;
        settings.audioSettings = PlayerPrefs.GetInt(unlocalizedAudio) == 0 ? OnOff.On : OnOff.Off;

        settings.SetGraphical();
        settings.SetAudio();

        //components.buttonGraphics.GetComponent<Toggle>().isOn = settings.graphicalSettings == Graphical.High;
        components.buttonAudio.GetComponent<Toggle>().isOn = settings.audioSettings == OnOff.On;

        components.buttonGraphicsHigh.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings == Graphical.High;
        components.buttonGraphicsLow.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings == Graphical.Low;

        settings.EnableSettings();
    }

    public void OnTutorial() {

    }

    public void OnToggleAudio(bool b) {
        settings.OnToggleAudio(b, unlocalizedAudio);
    }

    public void OnToggleGraphical(bool b) {
        if (settings.graphicalSettings == Graphical.High && b)
            return;
        if (settings.graphicalSettings == Graphical.Low && !b)
            return;

        settings.OnToggleGraphical(b, unlocalizedGraphics);

        if (!b) {
            components.buttonGraphicsHigh.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings == Graphical.High;
            components.buttonGraphicsLow.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings == Graphical.Low;
        } 
        else {
            components.buttonGraphicsHigh.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings == Graphical.High;
            components.buttonGraphicsLow.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings == Graphical.Low;
        }
    }

    public void OnChangeLanguage(string lang) {
        if (!LocalizationManager.instance.loadedlanguage.Equals(lang)) {
            LocalizationManager.instance.OnChangeLanguagePref(lang);
            OnChangeLanguagePref();
        }
    }

    public void DeleteAndQuit() {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
    #endregion

}


