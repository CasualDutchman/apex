using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedItem : MonoBehaviour {

    TextMeshProUGUI _text;
    public string key;
    string value;

    public object[] pars;

    public void Register(string k) {
        _text = GetComponent<TextMeshProUGUI>();
        key = k;
    }

    public void Change() {
        value = LocalizationManager.instance.GetLocalizedValue(key);

        if (pars != null && pars.Length > 0) {
            _text.text = string.Format(value, pars);
        }else {
            _text.text = value;
        }
    }

    public void UpdateItem(params object[] par) {  
        _text.text = string.Format(value, par);
        pars = par;
    }
}
