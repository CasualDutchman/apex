using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour, IAttackable {

    public float maxSpeed;

    public float maxHealth;
    float health;
    bool dead = false;

    public float attackTime;
    public float attackDamage;

    void Start() {
        health = maxHealth;
    }

    public void Damage(float f) {
        health -= f;
        if (health <= 0) {
            OnDeath();
        }
    }

    void OnDeath() {

    }

    public bool IsAlive() {
        return !dead;
    }
}
