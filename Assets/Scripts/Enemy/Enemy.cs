using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Prey, Predator }

public enum AnimalType { Raccoon, Fox, Coyote, Jackal, Dog, Cougar, Tiger, Bear, Grizzly, Reindeer, Moose, Bison, Muskox }


[RequireComponent(typeof(EnemyMovement))]
public class Enemy : MonoBehaviour, IAttackable {

    [HideInInspector]
    public EnemyManager manager;

    Animator anim;

    public Vector3 homePos;

    public EnemyType enemyType;
    public AnimalType animalType = AnimalType.Fox;

    public float maxSpeed;

    public float maxHealth;
    float health;

    public float experienceGain;
    public float foodGain;

    bool dead = false;
    float deadTimer;

    public float attackTime;
    public float attackDamage;

    public AudioClip damageClip;

    Color deathColor;
    Renderer deathRenderer;

    void Start() {
        anim = GetComponent<Animator>();
        health = maxHealth;
    }

    void Update() {
        if (dead) {
            deadTimer += Time.deltaTime;
            if (deadTimer >= 5.5f) {
                transform.position = Vector3.Lerp(transform.position, transform.position - Vector3.up * 0.2f, Time.deltaTime);
                deathColor.a -= Time.deltaTime;
                deathRenderer.material.SetColor("_OutlineColor", deathColor);
            }
            if (deadTimer >= 10) {
                EnemyManager.instance.RemoveEnemy(homePos);
            }
        }

        if (manager.OutOfReach(transform.position)) {
            EnemyManager.instance.RemoveEnemy(homePos);
        }
    }

    public void Damage(float f, bool isWolf) {
        bool b = SkillManager.instance.IsSkillActive("hunterInstinct");
        float ff = SkillManager.instance.GetSkillShareAmount("hunterInstinct");
        float d = (b ? f + ff : f);
        health -= d;
        Analyzer.instance.AddEnemyDamage(d);
        if (health <= 0) {
            OnDeath(isWolf);
        }else {
            if (enemyType == EnemyType.Prey) {
                Settingsmanager.instance.PlaySound(damageClip);
            }
        }
    }

    public void AddHealth(float f, float food) {
        health = Mathf.Clamp(health + f, 0, maxHealth);
    }

    [ContextMenu("Die")]
    void OnDeath(bool isWolf) {
        dead = true;
        anim.SetBool("Dead", true);

        Destroy(GetComponent<Collider>());

        deathRenderer = transform.GetChild(1).GetComponent<Renderer>();
        deathColor = deathRenderer.material.GetColor("_OutlineColor");

        if(isWolf)
            WolfManager.instance.AddExperience(experienceGain);

        Analyzer.instance.KilledEnemy(animalType, enemyType);
        SkillManager.instance.KillAnimal(animalType);
    }

    public bool IsAlive() {
        return !dead;
    }

    public float GetHealth() {
        return health;
    }
}
