﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Prey, Predator }

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : MonoBehaviour, IAttackable {

    [HideInInspector]
    public EnemyManager manager;

    Animator anim;

    public Vector3 homePos;

    public EnemyType enemyType;

    public float maxSpeed;

    public float maxHealth;
    float health;

    bool dead = false;
    float deadTimer;

    public float attackTime;
    public float attackDamage;

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

    public void Damage(float f) {
        health -= f;
        if (health <= 0) {
            OnDeath();
        }
    }

    [ContextMenu("Die")]
    void OnDeath() {
        dead = true;
        anim.SetBool("Dead", true);

        Destroy(GetComponent<Collider>());

        deathRenderer = transform.GetChild(1).GetComponent<Renderer>();
        deathColor = deathRenderer.material.GetColor("_OutlineColor");
    }

    public bool IsAlive() {
        return !dead;
    }

    public float GetHealth() {
        return health;
    }
}
