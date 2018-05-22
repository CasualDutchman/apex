using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { Idle, FindSpot, Chase, Flee }

public class EnemyMovement : MonoBehaviour, IMoveable {

    Animator anim;
    Enemy enemy;

    float maxSpeed;
    float currentMaxSpeed;
    Vector3 currentVelocity;

    Transform closest;
    float stateTimer;
    float stateChangeTime;
    float randomMultiplier;

    float fleeTimer;
    float fleeTime;

    Transform chaseTarget;
    float chaseTimer;
    float chaseTime;

    float attackTimer;
    float attackTime;
    bool isAttacking;
    float attackDamage;

    EnemyState enemyState = EnemyState.Idle;

    Vector3 target;

    List<Transform> lookList = new List<Transform>();

    void Start () {
        enemy = GetComponent<Enemy>();
        anim = GetComponent<Animator>();

        gameObject.layer = LayerMask.NameToLayer(enemy.enemyType == EnemyType.Predator ? "Predator" : "Prey");

        stateChangeTime = Random.Range(1f, 17f);
        randomMultiplier = Random.Range(5f, 8f);

        maxSpeed = enemy.maxSpeed;
        currentMaxSpeed = maxSpeed;

        attackTime = enemy.attackTime;
        attackDamage = enemy.attackDamage + SeasonManager.instance.seasonAttribute.extraEnemyDamage;
    }
	
    public bool OnMove() {
        return enemyState != EnemyState.Idle;
    }

    void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(target, 0.2f);
    }

	void Update () {
        if (enemy.IsAlive()) {
            if (lookList.Count > 0) {
                enemyState = enemy.enemyType == EnemyType.Predator ? EnemyState.Chase : EnemyState.Flee;
            }

            Vector3 velocity = Vector3.zero;

            if (enemyState == EnemyState.FindSpot) {
                Vector3 dif = target - transform.position;
                velocity = dif.magnitude > 1 ? dif.normalized : dif;
                if (Vector3.Distance(transform.position, target) < 0.3f) {
                    if (Random.Range(0, 3) == 0) {
                        randomMultiplier = Random.Range(5f, 8f);
                        Vector2 ran = new Vector2(transform.position.x, transform.position.z) + (new Vector2(dif.x, dif.z).normalized * randomMultiplier);
                        if (ran.magnitude < 2)
                            ran *= 2;
                        target = new Vector3(ran.x, 0, ran.y);
                    } else {
                        enemyState = EnemyState.Idle;
                    }
                }
            } else if (enemyState == EnemyState.Chase) {
                chaseTime += Time.deltaTime;
                currentMaxSpeed = maxSpeed + (chaseTime < 2 ? 1.5f : (chaseTime > 5 ? -0.5f : 0));

                if (lookList.Count <= 0) {
                    target = chaseTarget.position;
                    Vector3 dif = chaseTarget.position - transform.position;
                    velocity = dif.magnitude > 1 ? dif.normalized : dif;

                    chaseTimer -= Time.deltaTime;
                    if (chaseTimer <= 0) {
                        chaseTarget = null;
                        enemyState = EnemyState.Idle;
                    }
                } else {
                    IMoveable moveable = closest.GetComponent<IMoveable>();
                    bool moving = moveable.OnMove();
                    if (moving) {
                        Vector3 dif = target - transform.position;
                        velocity = dif.magnitude > 1 ? dif.normalized : dif;
                    }

                    if (Vector3.Distance(transform.position, closest.position) < (moving ? 1.3f : 2f)) {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target - transform.position), Time.deltaTime * (currentMaxSpeed + 2));
                        attackTimer += Time.deltaTime;
                        if (attackTimer >= attackTime) {
                            StartCoroutine(DoAttack());
                            attackTimer = 0;
                        }
                    } else if (!moving && Vector3.Distance(transform.position, closest.position) < 5f) {
                        Vector3 dif = target - transform.position;
                        velocity = dif.magnitude > 1 ? dif.normalized : dif;
                        velocity *= 0.4f;
                    }
                }
            } else if (enemyState == EnemyState.Flee) {
                fleeTime += Time.deltaTime;
                currentMaxSpeed = maxSpeed + (fleeTime < 4 ? 1 : 0);
                currentMaxSpeed *= enemy.GetHealth() <= enemy.maxHealth * 0.5f ? 0.5f : 1;

                if (lookList.Count <= 0) {
                    fleeTimer += Time.deltaTime;
                    if (fleeTimer >= 10) {
                        currentMaxSpeed = maxSpeed;
                        enemyState = EnemyState.Idle;
                    }
                }

                Vector3 dif = transform.position - target;
                dif *= 30;
                velocity = dif.magnitude > 1 ? dif.normalized : dif;
            }

            ChangeState();

            currentVelocity = Vector3.Lerp(currentVelocity, velocity * currentMaxSpeed, Time.deltaTime * 2);
            currentVelocity.y = 0;

            if (enemy.IsAlive() && !isAttacking) {
                if (currentVelocity != Vector3.zero && currentVelocity.magnitude > float.Epsilon) {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentVelocity), Time.deltaTime * (currentMaxSpeed + 2));
                }
                transform.Translate(transform.forward * currentVelocity.magnitude * Time.deltaTime, Space.World);
            }

            anim.SetFloat("Blend", currentVelocity.magnitude / currentMaxSpeed);


            Ray ray = new Ray(transform.position + Vector3.up * 30, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50, LayerMask.GetMask("Ground"))) {
                Vector3 pos = transform.position;
                pos.y = hit.point.y;
                transform.position = pos;
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
                attackable.Damage(10);

                if (!attackable.IsAlive()) {
                    lookList.Remove(closest);
                    chaseTarget = null;

                    enemyState = EnemyState.FindSpot;
                    Vector2 ran = Random.insideUnitCircle * randomMultiplier;
                    if (ran.magnitude < 2)
                        ran *= 2;
                    target = transform.position + new Vector3(ran.x, 0, ran.y);
                }

                didAttack = true;
                anim.SetBool("Attack", false);
                isAttacking = false;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return 0;
    }

    void ChangeState() {
        if (enemyState != EnemyState.Idle) {
            stateTimer = 0;
            return;
        }

        stateTimer += Time.deltaTime;
        if (stateTimer >= stateChangeTime) {
            if (enemyState == EnemyState.Idle) {
                enemyState = Random.Range(0, 3) == 0 ? EnemyState.Idle : EnemyState.FindSpot;
                if (enemyState == EnemyState.FindSpot) {
                    stateTimer = 0;
                    stateChangeTime = Random.Range(7f, 17f);
                    randomMultiplier = Random.Range(5f, 8f);
                    Vector2 ran = Random.insideUnitCircle * randomMultiplier;
                    if (ran.magnitude < 2)
                        ran *= 2;
                    target = transform.position + new Vector3(ran.x, 0, ran.y);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wolf") || other.gameObject.layer == LayerMask.NameToLayer(enemy.enemyType == EnemyType.Predator ? "Prey" : "Predator"))
            if (!lookList.Contains(other.transform)) {
                lookList.Add(other.transform);
                if(lookList.Count > 0) {
                    enemyState = enemy.enemyType == EnemyType.Predator ? EnemyState.Chase : EnemyState.Flee;
                    stateTimer = 0;
                }
            }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wolf") || other.gameObject.layer == LayerMask.NameToLayer(enemy.enemyType == EnemyType.Predator ? "Prey" : "Predator"))
            if (lookList.Contains(other.transform)) {
                lookList.Remove(other.transform);

                if(lookList.Count <= 0) {
                    if (enemy.enemyType == EnemyType.Predator) {
                        chaseTarget = other.transform;
                        chaseTimer = Random.Range(3f, 8f);
                    } else {
                        target = other.transform.position;
                    }

                    closest = null;
                }
            }
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wolf") || other.gameObject.layer == LayerMask.NameToLayer(enemy.enemyType == EnemyType.Predator ? "Prey" : "Predator")) {
            foreach (Transform trans in lookList) {
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

            target = closest.position;
        }
    }
}
