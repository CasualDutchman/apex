using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum Screens { Begin, Hud, Settings, Alpha, Pack }

[System.Serializable]
public class SkillHolder {
    public Image skillRadial;
    public TextMeshProUGUI skillName;
}

public class UIManager : MonoBehaviour { 

    Screens currentScreen = Screens.Begin;

    public static UIManager instance;

    public UIComponents components;
    public EventSystem eventSystem;

    bool useIconUI;

    WolfManager wolfManager;
    Settingsmanager settings;
    SkillManager skillManager;
    SeasonManager seasonManager;
    PackManager packManager;

    Dictionary<string, LocalizedItem> textRegistry = new Dictionary<string, LocalizedItem>();

    SkillHolder[] skillHolder = new SkillHolder[5];

    [Header("Adio")]
    UnityEngine.Audio.AudioMixer mixer;

    [Header("HUD")]
    public string unlocalizedExperience;
    public string unlocalizedLevel;
    string unlocalizedLevelIcon;
    public string unlocalizedAlpha;

    [Header("Begin")]
    public string unlocalizedBeginTitle;
    public string unlocalizedBeginExplain;

    [Header("Alpha")]
    public string unlocalizedAlphaTitle;

    [Header("Pack")]
    public string unlocalizedPackTitle;
    public string unlocalizedPackDetailTitle;
    public string unlocalizedPackPackTitle;

    [Header("Settings")]
    public string unlocalizedSettingsTitle;
    public string unlocalizedLanguage;
    public string unlocalizedGraphics;
    public string unlocalizedGraphicsLow;
    public string unlocalizedGraphicsHigh;
    public string unlocalizedIcons;
    public string unlocalizedAudio;
    public string unlocalizedAudioOn;
    public string unlocalizedAudioOff;
    public Sprite toggleOnSprite, toggleOffSprite;

    void Awake() {
        instance = this;

        unlocalizedLevelIcon = unlocalizedLevel + "i";
    }

    void Start() {
        settings = Settingsmanager.instance;
        skillManager = GetComponent<SkillManager>();
        seasonManager = GetComponent<SeasonManager>();
        wolfManager = GetComponent<WolfManager>();
        packManager = GetComponent<PackManager>();
        RegisterTexts();
        LoadSettings();

        if (PlayerPrefs.HasKey("IconUI")) {
            useIconUI = PlayerPrefs.GetInt("IconUI") == 1;
        } else {
            useIconUI = Random.value < 0.5f;
        }

        Analyzer.instance.SetIconUI(useIconUI);

        if (useIconUI) {
            components.iconParent.SetActive(true);
            components.barParent.SetActive(false);
        } else {
            components.iconParent.SetActive(false);
            components.barParent.SetActive(true);
        }

        ColorUI();
    }

    public Image[] ignoreImages;
    public TextMeshProUGUI[] secondaryTexts;

    void ColorUI() {
        Image[] im = components.transform.GetComponentsInChildren<Image>(true);
        for (int i = 0; i < im.Length; i++) {
            bool ignore = false;
            for (int j = 0; j < ignoreImages.Length; j++) {
                if (im[i] == ignoreImages[j]) {
                    ignore = true;
                    break;
                }
            }
            if (ignore)
                continue;

            im[i].color = seasonManager.seasonAttribute.backgroundColor;
        }

        TextMeshProUGUI[] tm = components.transform.GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < tm.Length; i++) {
            bool second = false;
            for (int j = 0; j < secondaryTexts.Length; j++) {
                if (tm[i] == secondaryTexts[j]) {
                    secondaryTexts[j].color = seasonManager.seasonAttribute.secondaryTextColor;
                    second = true;
                    break;
                }
            }

            if(!second)
                tm[i].color = seasonManager.seasonAttribute.textColor;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (currentScreen != Screens.Hud) {
                ChangeScreen(Screens.Hud);
            } else {
                Save();
                Application.Quit();
            }
        }
    }

    void OnApplicationPause(bool pause) {
        if (pause) {
            Save();
        }
    }

    void Save() {
        PlayerPrefs.SetInt("IconUI", useIconUI ? 1 : 0);

        skillManager.SaveSkills();
        seasonManager.OnQuit();
        wolfManager.Save();
        packManager.Save();
        Analyzer.instance.SendData();
        PlayerPrefs.Save();
    }

    void RegisterTexts() {
        //HUD
        RegisterText(components.textExperience, unlocalizedExperience);
        RegisterText(components.textLevelBar, unlocalizedLevel);
        RegisterText(components.textLevelIcon, unlocalizedLevelIcon);
        RegisterButton(components.buttonSettings, () => ChangeScreen(Screens.Settings));
        RegisterButton(components.buttonAlpha, () => ChangeScreen(Screens.Alpha));
        //RegisterText(components.buttonAlpha.GetChild(0), unlocalizedAlpha);

        //Begin
        RegisterButton(components.buttonBackBegin, () => ChangeScreen(Screens.Hud));

        //Alpha
        RegisterSkill(components.skillA1, 0);
        RegisterSkill(components.skillA2, 1);
        RegisterSkill(components.skillB1, 2);
        RegisterSkill(components.skillB2, 3);
        RegisterSkill(components.skillB3, 4);
        RegisterButton(components.skillDescBack, () => HideSkillDesc());
        RegisterButton(components.buttonBackAlpha, () => ChangeScreen(Screens.Hud));
        RegisterButton(components.buttonToPack, () => ChangeScreen(Screens.Pack));

        //Pack
        RegisterText(components.textPackTitle, unlocalizedPackTitle);
        RegisterText(components.textPackDescriptionTitle, unlocalizedPackDetailTitle);
        RegisterText(components.textPackList, unlocalizedPackPackTitle);
        RegisterButton(components.buttonPackSwitch, () => Switch());
        RegisterButton(components.buttonBackPack, () => ChangeScreen(Screens.Hud));
        RegisterButton(components.buttonToAlpha, () => ChangeScreen(Screens.Alpha));

        //Settings
        RegisterButton(components.buttonBackSettings, () => ChangeScreen(Screens.Hud));
        RegisterText(components.textSettingsTitle, unlocalizedSettingsTitle);
        RegisterText(components.textLanguage, unlocalizedLanguage);
        RegisterText(components.textGraphical, unlocalizedGraphics);
        RegisterText(components.textGraphicalLow, unlocalizedGraphicsLow);
        RegisterText(components.textGraphicalHigh, unlocalizedGraphicsHigh);
        RegisterText(components.textAudio, unlocalizedAudio);
        RegisterText(components.textAudioOn, unlocalizedAudioOn);
        RegisterText(components.textAudioOff, unlocalizedAudioOff);
        RegisterText(components.textIcons, unlocalizedIcons);
        RegisterToggle(components.buttonAudio, (b) => { OnToggleAudio(b); });
        RegisterToggle(components.buttonIcons, (b) => { OnIcon(b); });
        RegisterButton(components.buttonEnglish, () => OnChangeLanguage("EN_us"));
        RegisterButton(components.buttonDutch, () => OnChangeLanguage("NL_nl"));
        RegisterButton(components.buttonGraphicsHigh, () => OnToggleGraphical(true));
        RegisterButton(components.buttonGraphicsLow, () => OnToggleGraphical(false));
        
    }

    public bool IsHittingUI() {
        bool b = false;
#if UNITY_EDITOR
        b = eventSystem.IsPointerOverGameObject();
#endif
#if UNITY_ANDROID
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            b = eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif
        return b;
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
            case Screens.Begin: return components.screenBegin;
            case Screens.Pack: return components.screenPack;
        }
    }

    public void ChangeScreen(Screens screen) {
        if (screen == Screens.Alpha) {
            OnAlphaOpened();
        }else if (screen == Screens.Pack) {
            selected = -1;
            UpdatePackList();
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
        //Debug.Log("ShowSkill");
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
        if (useIconUI) {
            GetImage(components.imageHealthFillIcon).fillAmount = fill;
        } else {
            GetImage(components.imageHealthFillBar).fillAmount = fill;
        }
    }

    public void UpdateFoodBar(float fill) {
        if (useIconUI) {
            GetImage(components.imageFoodBarFillIcon).fillAmount = fill;
        } else {
            GetImage(components.imageFoodBarFillBar).fillAmount = fill;
        }
    }

    public void UpdateExperienceBar(float currentXP, float maxXP) {
        if (useIconUI) {
            GetImage(components.imageExperienceBarFillIcon).fillAmount = currentXP / maxXP;
        } else {
            GetImage(components.imageExperienceBarFillBar).fillAmount = currentXP / maxXP;
            UpdateText(unlocalizedExperience, currentXP.ToString("F0"), maxXP.ToString("F0"));
        }        
    }

    public void UpdateLevelText(int i) {
        if (useIconUI) {
            UpdateText(unlocalizedLevelIcon, i, "");
        } else {
            UpdateText(unlocalizedLevel, i, "");
        }
    }
    #endregion

    #region Pack
    int selected = 0;

    void Select(int i) {
        selected = i;

        components.buttonPackSwitch.gameObject.SetActive(selected != packManager.packIndex);
    }

    void Switch() {

    }

    public void UpdatePackList() {
        selected = packManager.packIndex;
        components.buttonPackSwitch.gameObject.SetActive(selected != packManager.packIndex);

        for (int i = 1; i < components.textPackListParent.childCount; i++) {
            Destroy(components.textPackListParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < packManager.packList.Count; i++) {
            Pack pack = packManager.packList[i];
            int index = i;

            GameObject go = Instantiate(components.textPackTemplate.gameObject, components.textPackListParent);
            go.GetComponent<Button>().onClick.AddListener(() => Select(index));
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Pack " + i;
            go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Level " + pack.level;
            go.transform.GetChild(2).GetComponent<Image>().color = packManager.packIndex == i ? Color.green : Color.white;
            go.SetActive(true);
        }
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

        components.textAudioOn.gameObject.SetActive(settings.audioSettings == OnOff.On);
        components.textAudioOff.gameObject.SetActive(settings.audioSettings == OnOff.Off);
        components.buttonAudio.GetComponent<Image>().sprite = settings.audioSettings == OnOff.On ? toggleOnSprite : toggleOffSprite;

        ChangeIcon(useIconUI);

        components.textIconOn.gameObject.SetActive(useIconUI);
        components.textIconOff.gameObject.SetActive(!useIconUI);
        components.buttonIcons.GetComponent<Image>().sprite = useIconUI ? toggleOnSprite : toggleOffSprite;

        components.buttonGraphicsHigh.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings != Graphical.High;
        components.buttonGraphicsLow.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings != Graphical.Low;

        settings.EnableSettings();
    }

    void ChangeIcon(bool b) {
        components.iconParent.SetActive(b);
        components.barParent.SetActive(!b);
    }

    public void OnIcon(bool b) {
        useIconUI = b;

        ChangeIcon(b);
        Analyzer.instance.SwitchIconUI(b);

        components.textIconOn.gameObject.SetActive(b);
        components.textIconOff.gameObject.SetActive(!b);
        components.buttonIcons.GetComponent<Image>().sprite = b ? toggleOnSprite : toggleOffSprite;
    }

    public void OnToggleAudio(bool b) {
        settings.OnToggleAudio(b, unlocalizedAudio);

        components.textAudioOn.gameObject.SetActive(b);
        components.textAudioOff.gameObject.SetActive(!b);
        components.buttonAudio.GetComponent<Image>().sprite = b ? toggleOnSprite : toggleOffSprite;
    }

    public void OnToggleGraphical(bool b) {
        if (settings.graphicalSettings == Graphical.High && b)
            return;
        if (settings.graphicalSettings == Graphical.Low && !b)
            return;

        settings.OnToggleGraphical(b, unlocalizedGraphics);

        if (!b) {
            components.buttonGraphicsHigh.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings != Graphical.High;
            components.buttonGraphicsLow.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings != Graphical.Low;
        } 
        else {
            components.buttonGraphicsHigh.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings != Graphical.High;
            components.buttonGraphicsLow.GetChild(0).GetComponent<Image>().enabled = settings.graphicalSettings != Graphical.Low;
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


