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

    bool iconUI;
    int switchedtoIcon, switchedtoBar;

    public float distanceWalked;

    void Awake() {
        instance = this;
    }
	
	void Update () {
        playTime += Time.deltaTime;
    }

    public void SendData() {
        Dictionary<string, object> dict = new Dictionary<string, object> {
            { "Playtime", playTime },
            { "Damage to enemies", damageToEnemies },
            { "Damage to wolves", damageToWolves },
            { "Distance walked", distanceWalked }
        };

        AnalyticsResult result = Analytics.CustomEvent("OnEndGame", dict);

        dict = new Dictionary<string, object> {
            { "prey killed", preyKilled },

            { "Amount reindeer killed", reindeerKilled },
            { "Amount moose killed", mooseKilles },
            { "Amount bison killed", bisonKilles },
            { "Amount muskox killed", muskoxKilled }
        };
        AnalyticsResult result2 = Analytics.CustomEvent("OnEndGamePrey", dict);

        dict = new Dictionary<string, object> {
            { "predator killed", predatorKilled },

            { "Amount raccoon killed", raccoonKilled },
            { "Amount fox killed", foxKilled },
            { "Amount coyote killed", coyoteKilles },
            { "Amount jackal killed", jackalKilled },
            { "Amount dog killed", dogKilled },
            { "Amount cougar killed", cougarKilled },
            { "Amount tiger killed", tigerKilled },
            { "Amount bear killed", bearKilled },
            { "Amount grizzly killed", grizzlyKilled }
        };
        AnalyticsResult result3 = Analytics.CustomEvent("OnEndGamePredator", dict);

        dict = new Dictionary<string, object> {
            { "Use Icons", iconUI },
            { "Use Bars", !iconUI },
            { "UI to Bar", switchedtoBar },
            { "UI to Icon", switchedtoIcon }
        };
        AnalyticsResult result4 = Analytics.CustomEvent("OnEndGameUI", dict);
    }

    public void SetIconUI(bool b) {
        iconUI = b;
    }

    public void SwitchIconUI(bool b) {
        if (b == iconUI)
            return;

        if (b) {
            switchedtoIcon++;
        } else {
            switchedtoBar++;
        }
        iconUI = b;
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
