/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using V_AnimationSystem;
using CodeMonkey.Utils;

/*
 * Enemy
 * */
public class Enemy : MonoBehaviour {

    private static List<Enemy> enemyList;

    public static Enemy Create(Vector3 position) {
        Transform enemyTransform = Instantiate(GameAssets.i.pfEnemy, position, Quaternion.identity);

        Enemy enemy = enemyTransform.GetComponent<Enemy>();

        if (enemyList == null) enemyList = new List<Enemy>();
        enemyList.Add(enemy);

        return enemy;
    }

    public static Enemy GetClosestEnemy(Vector3 position, float range) {
        if (enemyList == null) return null; // No enemies!

        Enemy closestEnemy = null;

        for (int i = 0; i < enemyList.Count; i++) {
            Enemy testEnemy = enemyList[i];
            if (Vector3.Distance(position, testEnemy.GetPosition()) > range) {
                // Enemy too far, skip
                continue;
            }

            if (closestEnemy == null) {
                // No closest enemy
                closestEnemy = testEnemy;
            } else {
                // Already has a closest enemy, get which is closer
                if (Vector3.Distance(position, testEnemy.GetPosition()) < Vector3.Distance(position, closestEnemy.GetPosition())) {
                    // Test Enemy is closer than Closest Enemy
                    closestEnemy = testEnemy;
                }
            }
        }

        return closestEnemy;
    }


    private const float SPEED = 30f;

    private HealthSystem healthSystem;
    private Character_Base characterBase;
    private State state;
    private Vector3 lastMoveDir;
    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    private float pathfindingTimer;

    private UnitAnimType idleUnitAnim;
    private UnitAnimType walkUnitAnim;
    private UnitAnimType hitUnitAnim;
    private UnitAnimType attackUnitAnim;

    private enum State {
        Normal,
        Attacking,
        Busy,
    }

    private void Awake() {
        characterBase = gameObject.GetComponent<Character_Base>();
        healthSystem = new HealthSystem(100);
        SetStateNormal();
    }

    private void Start() {
        /*
        HealthBar healthBar = HealthBar.Create(transform, new Vector3(0, 9), new Vector3(8, 1), Color.red, UtilsClass.GetColorFromString("4F4F4F"), new HealthBar.Border { color = Color.black, thickness = .4f });
        healthSystem.OnHealthChanged += (object sender, EventArgs e) => {
            healthBar.SetSize(healthSystem.GetHealthPercent());
        };
        */
    }

    private void Update() {
        pathfindingTimer -= Time.deltaTime;

        switch (state) {
        case State.Normal:
            HandleMovement();
            FindTarget();
            break;
        case State.Attacking:
            break;
        case State.Busy:
            break;
        }
    }

    private void FindTarget() {
        float targetRange = 200f;
        float attackRange = 11f;
        CharacterController2D player = CharacterController2D.Instance;
        if (player != null) {
            if (Vector3.Distance(player.GetPosition(), GetPosition()) < attackRange) {
                StopMoving();
                state = State.Attacking;
                Vector3 attackDir = (player.GetPosition() - GetPosition()).normalized;
                characterBase.PlayPunchSlowAnimation(attackDir, (Vector3 hitPosition) => {
                    //player.Damage(this);
                }, SetStateNormal);
            } else {
                if (Vector3.Distance(player.GetPosition(), GetPosition()) < targetRange) {
                    if (pathfindingTimer <= 0f) {
                        pathfindingTimer = .3f;
                        SetTargetPosition(player.GetPosition());
                    }
                }
            }
        }
    }

    public bool IsDead() {
        return healthSystem.IsDead();
    }
    
    private void SetStateNormal() {
        state = State.Normal;
    }

    private void SetStateAttacking() {
        state = State.Attacking;
    }

    public void Damage(Vector3 attackerPosition) {
        Vector3 dirFromAttacker = (transform.position - attackerPosition).normalized;
        Blood_Handler.SpawnBlood(GetPosition(), dirFromAttacker);

        float knockbackDistance = 5f;
        transform.position += dirFromAttacker * knockbackDistance;
        int damageAmount = 30;

        DamagePopup.Create(GetPosition(), damageAmount, false);

        healthSystem.Damage(damageAmount);
        if (IsDead()) {
            Die();
        }
    }

    private void Die() {
        Vector3 flyingBodyDir = UtilsClass.GetRandomDir();
        FlyingBody.Create(GameAssets.i.pfEnemyFlyingBody, GetPosition(), flyingBodyDir);
        Destroy(gameObject);

        enemyList.Remove(this);
    }

    private void HandleMovement() {
        if (pathVectorList != null) {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 1f) {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                characterBase.PlayMoveAnim(moveDir);
                transform.position = transform.position + moveDir * SPEED * Time.deltaTime;
            } else {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count) {
                    StopMoving();
                    characterBase.PlayIdleAnim();
                }
            }
        } else {
            characterBase.PlayIdleAnim();
        }
    }

    private void StopMoving() {
        pathVectorList = null;
    }

    public void SetTargetPosition(Vector3 targetPosition) {
        currentPathIndex = 0;
        //pathVectorList = GridPathfinding.instance.GetPathRouteWithShortcuts(GetPosition(), targetPosition).pathVectorList;
        pathVectorList = new List<Vector3> { targetPosition };
        if (pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
        
}
