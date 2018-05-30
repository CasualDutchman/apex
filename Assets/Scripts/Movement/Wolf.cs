using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour, IAttackable {

    public WolfManager wolfManager;

    public float maxSpeed;

    public float maxHealth;
    public float health;
    bool dead = false;

    public float attackTime;
    public float attackDamage;

    void Start() {
        health = maxHealth;
    }

    public void Damage(float f, bool isWolf) {
        health -= f;
        Analyzer.instance.AddWolfDamage(f);
        wolfManager.UpdatehealthBar();
        if (health <= 0) {
            OnDeath();
        }
    }

    public void AddHealth(float f) {
        health = Mathf.Clamp(health + f, 0, maxHealth);
        wolfManager.UpdatehealthBar();
    }

    void OnDeath() {

    }

    public bool IsAlive() {
        return !dead;
    }
}
