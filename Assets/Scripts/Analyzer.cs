using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Analyzer : MonoBehaviour {

    public static Analyzer instance;

    float playTime;

    float damageToEnemies;
    float damageToWolves;

    void Awake() {
        instance = this;
    }
	
	void Update () {
        playTime += Time.deltaTime;
    }

    private void OnDestroy() {
        SendDate();
    }

    public void SendDate() {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        dict.Add("Playtime", playTime);
        dict.Add("Damage to enemies", damageToEnemies);
        dict.Add("Damage to wolves", damageToWolves);

        Analytics.CustomEvent("OnEndGame", dict);
    }

    public void AddWolfDamage(float amount) {
        damageToWolves += amount;
    }

    public void AddEnemyDamage(float amount) {
        damageToEnemies += amount;
    }
}
