using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notifier : MonoBehaviour {

    public static Notifier instance;

    public UIComponents components;

    public AnimationCurve curve;

    RectTransform notifyBox;

    List<string> notifyList = new List<string>();

    bool isNotifying, doneNotifying;

    void Awake() {
        instance = this;
    }

    void Start () {
        notifyBox = components.notifyBox.GetComponent<RectTransform>();
    }
	
	void Update () {
		
	}

    [ContextMenu("Add")]
    public void Add() {
        AddNotification("pluspack");
    }

    public void AddNotification(string unlocalizedKey) {
        notifyList.Add(LocalizationManager.instance.GetLocalizedValue(unlocalizedKey));
        if (!isNotifying) {
            StartCoroutine(Notify());
        }
    }

    IEnumerator Notify() {
        float timer = 0;

        float slideTime = 1;
        float waitTime = 4;

        components.notifyText.GetComponent<TextMeshProUGUI>().text = notifyList[0];

        isNotifying = true;

        while (isNotifying) {
            timer += Time.deltaTime;

            if(timer < slideTime) {//sliding into view
                notifyBox.anchoredPosition = Vector3.Lerp(new Vector3(-190, 0, 0), new Vector3(190, 0, 0), curve.Evaluate(timer));
            }
            else if(timer > slideTime + waitTime) {//sliding out of view
                float newTimer = timer - (waitTime + slideTime);
                notifyBox.anchoredPosition = Vector3.Lerp(new Vector3(190, 0, 0), new Vector3(-190, 0, 0), curve.Evaluate(newTimer));
            } 
            else {

            }

            if (timer > slideTime + slideTime + waitTime) {
                notifyList.RemoveAt(0);
                if (notifyList.Count > 0) {
                    components.notifyText.GetComponent<TextMeshProUGUI>().text = notifyList[0];
                    timer = 0;
                } else {
                    isNotifying = false;
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
