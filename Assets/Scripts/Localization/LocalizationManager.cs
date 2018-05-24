using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LocalizationManager : MonoBehaviour {

    public static LocalizationManager instance;

    UIManager uiManager;

    Dictionary<string, string> dictionary = new Dictionary<string, string>();//1. key, code to actual language // 2. value, display language

    public string loadedlanguage = "EN_us";
    public TextAsset att;

    public bool debugAll = false;

    void Awake() {
        instance = this;
        uiManager = GetComponent<UIManager>();

        if (PlayerPrefs.HasKey("Language")) {
            loadedlanguage = PlayerPrefs.GetString("Language");
        }
        StartCoroutine(LoadDictionary(loadedlanguage));
    }

    /// <summary>
    /// When the language is updated
    /// </summary>
    /// <param name="lang">language index string</param>
    public void OnChangeLanguagePref(string lang) {
        PlayerPrefs.SetString("Language", lang);
        PlayerPrefs.Save();
        loadedlanguage = lang;
        StartCoroutine(LoadDictionary(lang));

        uiManager.OnChangeLanguagePref();
    }

    /// <summary>
    /// Return the localized value based on the key
    /// </summary>
    /// <param name="key">unlocalized key</param>
    /// <returns>Localized value</returns>
    public string GetLocalizedValue(string key) {
        key = key.ToLower();
        string result = key + " Missing";

        if (dictionary.ContainsKey(key)) {
            result = dictionary[key];
        }

        return result;
    }

    /// <summary>
    /// Load the Dictionary based on the language index string
    /// </summary>
    /// <param name="lang">language index string</param>
    IEnumerator LoadDictionary(string lang) {
        dictionary = new Dictionary<string, string>();

        TextAsset textass = Resources.Load("Languages/" + lang) as TextAsset;

        if(textass != null) {
            string content = textass.text;
            if (content == "")
                content = System.Text.Encoding.Default.GetString(textass.bytes);

            string[] dataArray = content.Split("\n"[0]);

            if (debugAll) {
                Debug.Log(textass.text);
                Debug.Log(dataArray.Length);
                Debug.Log(dataArray[0]);
            }

            foreach (string data in dataArray) {
                if (data.StartsWith("/") || string.IsNullOrEmpty(data) || char.IsControl(data[0]))
                        continue;

                string[] keyValue = data.Split('=');
                dictionary.Add(keyValue[0], keyValue[1]);
                if (debugAll) {
                    Debug.Log(string.Format("{0} || {1} , {2}", data, keyValue[0], keyValue[1]));
                }
            }

            Debug.Log(lang +" content loaded. Dictionary contains " + dictionary.Count + " entries");
        } else {
            Debug.LogError(lang + ".txt does not excist in the Resource folder.");
        }

        yield return 0; 
    }
}
