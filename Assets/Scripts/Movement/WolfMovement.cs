using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WolfState { Idle, Input, FindSpot, Attack }

public class WolfMovement : MonoBehaviour, IMoveable {

    Wolf wolf;
    public WolfManager manager;
    InputManager inputManager;
    Animator anim;

    public WolfState wolfState;
    float stateTimer;
    float stateChangeTime;

    Vector3 target;

    float maxSpeed;
    Vector3 currentVelocity;

    float attackTimer;
    float attackTime;
    bool isAttacking;
    float attackDamage;

    List<Transform> neighborList = new List<Transform>();
    List<Transform> lookList = new List<Transform>();
    Transform closest;

	void Start () {
        wolf = GetComponent<Wolf>();
        anim = GetComponent<Animator>();
        inputManager = GameObject.FindObjectOfType<InputManager>();
        stateChangeTime = Random.Range(1f, 17f);

        attackTime = GetComponent<Wolf>().attackTime;

        maxSpeed = wolf.maxSpeed + SeasonManager.instance.seasonAttribute.extraMobility;
        attackDamage = wolf.attackDamage + SeasonManager.instance.seasonAttribute.extraDamage;
    }
	
	void Update () {
        Vector3 velocity = Vector3.zero;
        Vector3 input = inputManager.GetInputVector();

        if(input.magnitude > 0) {
            velocity = input;
            if(wolfState == WolfState.Attack) {
                OnAttack();
            }else {
                wolfState = WolfState.Input;
            }
        } else {
            if (wolfState == WolfState.Input) {
                stateChangeTime = Random.Range(1f, 17f);
                wolfState = WolfState.Idle;
            }

            if (wolfState == WolfState.FindSpot) {
                Vector3 dif = target - transform.position;
                velocity = dif.magnitude > 1 ? dif.normalized : dif;
                if (Vector3.Distance(transform.position, target) < 0.2f) {
                    wolfState = WolfState.Idle;
                }
            }
            else if (wolfState == WolfState.Attack) {
                OnAttack();
            }
        }

        ChangeState();

        currentVelocity = Vector3.Lerp(currentVelocity, velocity * maxSpeed, Time.deltaTime * 2);
        currentVelocity.y = 0;

        if (currentVelocity != Vector3.zero && currentVelocity.magnitude > float.Epsilon) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentVelocity), Time.deltaTime * (maxSpeed + 1));
        }
        transform.Translate(transform.forward * currentVelocity.magnitude * Time.deltaTime, Space.World);

        anim.SetFloat("Blend", currentVelocity.magnitude / maxSpeed);


        Ray ray = new Ray(transform.position + Vector3.up * 30, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50, LayerMask.GetMask("Ground"))) {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
	}

    public bool OnMove() {
        return wolfState == WolfState.FindSpot || wolfState == WolfState.Input;
    }

    void OnAttack() {
        if (lookList.Count > 0) {
            IMoveable moveable = closest.GetComponent<IMoveable>();
            bool moving = moveable.OnMove();
            if(moving) {
                if (Vector3.Distance(transform.position, closest.position) < 3f) {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target - transform.position), Time.deltaTime * maxSpeed);
                    attackTimer += Time.deltaTime;
                    if (attackTimer >= attackTime) {
                        StartCoroutine(DoAttack());
                        attackTimer = 0;
                    }
                }
            }
        }
    }

    IEnumerator DoAttack() {
        float attackAnimTimer = 0;
        bool didAttack = false;

        isAttacking = true;
        anim.SetBool("Attack", true);

        while (isAttacking) {
            attackAnimTimer += Time.deltaTime;
            if (attackAnimTimer >= 0.2f && !didAttack) {
                IAttackable attackable = closest.GetComponent<IAttackable>();
                attackable.Damage(attackDamage);

                if (!attackable.IsAlive()) {
                    manager.KillEnemy(closest);
                }

                didAttack = true;
                anim.SetBool("Attack", false);
                isAttacking = false;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return 0;
    }

    public void KilledEnemy(Transform c) {
        if(lookList.Contains(c))
            lookList.Remove(closest);

        if (manager.animalList.Contains(c))
            manager.animalList.Remove(c);

        closest = null;

        if(lookList.Count <= 0)
            wolfState = WolfState.Idle;
    }

    void ChangeState() {
        if (wolfState == WolfState.Input || wolfState == WolfState.Attack || wolfState == WolfState.FindSpot) {
            stateTimer = 0;
            return;
        }

        stateTimer += Time.deltaTime;
        if (stateTimer >= stateChangeTime) {
            if (wolfState == WolfState.Idle) {
                wolfState = Random.Range(0, 3) == 0 ? WolfState.Idle : WolfState.FindSpot;
                if (wolfState == WolfState.FindSpot) {
                    stateTimer = 0;
                    stateChangeTime = Random.Range(7f, 17f);
                    target = manager.GetRandomCenter();
                }
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wolf")) {
            neighborList.Add(other.transform);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Prey") || other.gameObject.layer == LayerMask.NameToLayer("Predator")) {
            if (!lookList.Contains(other.transform)) {
                lookList.Add(other.transform);
                if (lookList.Count > 0) {
                    wolfState = WolfState.Attack;
                    stateTimer = 0;
                }
            }
            if (!manager.animalList.Contains(other.transform)) {
                manager.animalList.Add(other.transform);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wolf")) {
            neighborList.Remove(other.transform);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Prey") || other.gameObject.layer == LayerMask.NameToLayer("Predator")) {
            if (lookList.Contains(other.transform)) {
                lookList.Remove(other.transform);

                if (lookList.Count <= 0) {
                    wolfState = WolfState.Idle;
                    stateTimer = 0;
                }
            }

            if (manager.animalList.Contains(other.transform)) {
                manager.animalList.Remove(other.transform);
            }
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Prey") || other.gameObject.layer == LayerMask.NameToLayer("Predator")) {
            foreach (Transform trans in lookList) {
                if (trans == null)
                    continue;

                if (closest == null) {
                    closest = trans;
                } else {
                    float dis1 = Vector3.Distance(transform.position, trans.position);
                    float dis2 = Vector3.Distance(transform.position, closest.position);
                    if (dis1 < dis2) {
                        closest = trans;
                    }
                }
            }
            if(closest != null)
                target = closest.position;
        }
    }
}
