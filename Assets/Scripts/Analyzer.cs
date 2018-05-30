using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Analyzer : MonoBehaviour {

    public static Analyzer instance;

    float playTime;

    float damageToEnemies;
    float damageToWolves;

    int preyKilled, predatorKilled;

    int raccoonKilled, foxKilled, coyoteKilles, jackalKilled, dogKilled, cougarKilled, tigerKilled, bearKilled, grizzlyKilled, reindeerKilled, mooseKilles, bisonKilles, muskoxKilled;

    void Awake() {
        instance = this;
    }
	
	void Update () {
        playTime += Time.deltaTime;
    }

    public void SendData() {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        dict.Add("Playtime", playTime);
        dict.Add("Damage to enemies", damageToEnemies);
        dict.Add("Damage to wolves", damageToWolves);

        AnalyticsResult result = Analytics.CustomEvent("OnEndGame", dict);

        dict = new Dictionary<string, object>();
        dict.Add("prey killed", preyKilled);

        dict.Add("Amount reindeer killed", reindeerKilled);
        dict.Add("Amount moose killed", mooseKilles);
        dict.Add("Amount bison killed", bisonKilles);
        dict.Add("Amount muskox killed", muskoxKilled);
        AnalyticsResult result2 = Analytics.CustomEvent("OnEndGamePrey", dict);

        dict = new Dictionary<string, object>();
        dict.Add("predator killed", predatorKilled);

        dict.Add("Amount raccoon killed", raccoonKilled);
        dict.Add("Amount fox killed", foxKilled);
        dict.Add("Amount coyote killed", coyoteKilles);
        dict.Add("Amount jackal killed", jackalKilled);
        dict.Add("Amount dog killed", dogKilled);
        dict.Add("Amount cougar killed", cougarKilled);
        dict.Add("Amount tiger killed", tigerKilled);
        dict.Add("Amount bear killed", bearKilled);
        dict.Add("Amount grizzly killed", grizzlyKilled);
        AnalyticsResult result3 = Analytics.CustomEvent("OnEndGamePredator", dict);
    }

    public void AddWolfDamage(float amount) {
        damageToWolves += amount;
    }

    public void AddEnemyDamage(float amount) {
        damageToEnemies += amount;
    }

    public void KilledEnemy(AnimalType type, EnemyType etype) {
        switch (type) {
            case AnimalType.Raccoon: raccoonKilled++; break;
            case AnimalType.Fox: foxKilled++; break;
            case AnimalType.Coyote: coyoteKilles++; break;
            case AnimalType.Jackal: jackalKilled++; break;
            case AnimalType.Dog: dogKilled++; break;
            case AnimalType.Cougar: cougarKilled++; break;
            case AnimalType.Tiger: tigerKilled++; break;
            case AnimalType.Bear: bearKilled++; break;
            case AnimalType.Grizzly: grizzlyKilled++; break;
            case AnimalType.Reindeer: reindeerKilled++; break;
            case AnimalType.Moose: mooseKilles++; break;
            case AnimalType.Bison: bisonKilles++; break;
            case AnimalType.Muskox: muskoxKilled++; break;
            default: break;
        }

        switch (etype) {
            case EnemyType.Predator: predatorKilled++; break;
            case EnemyType.Prey: preyKilled++; break;
            default: break;
        }
    }
}
