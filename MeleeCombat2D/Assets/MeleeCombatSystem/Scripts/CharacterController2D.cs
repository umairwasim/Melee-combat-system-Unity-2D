/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class CharacterController2D : MonoBehaviour {

    private const float MOVE_SPEED = 60f;

    public static CharacterController2D Instance { get; private set; }

    private enum State {
        Normal,
        Attacking,
    }

    private Character_Base characterBase;
    private Rigidbody2D rigidbody2D;
    private Vector3 moveDir;
    private Vector3 lastMoveDir;
    private State state;

    private void Awake() {
        Instance = this;
        characterBase = GetComponent<Character_Base>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        state = State.Normal;
    }

    private void Update() {
        switch (state) {
        case State.Normal:
            HandleMovement();
            HandleAttack();
            break;
        case State.Attacking:
            HandleAttack();
            break;
        }
    }

    private void FixedUpdate() {
        rigidbody2D.velocity = moveDir * MOVE_SPEED;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    private void HandleMovement() {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S)) {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A)) {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D)) {
            moveX = +1f;
        }

        moveDir = new Vector3(moveX, moveY).normalized;
        if (moveX != 0 || moveY != 0) {
            // Not idle
            lastMoveDir = moveDir;
        }
        characterBase.PlayMoveAnim(moveDir);
    }

    private void HandleAttack() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            Vector3 mouseDir = (mousePosition - transform.position).normalized;

            float attackOffset = 10f;
            Vector3 attackPosition = transform.position + mouseDir * attackOffset;

            Vector3 attackDir = mouseDir;

            float attackRange = 10f;
            Enemy targetEnemy = Enemy.GetClosestEnemy(attackPosition, attackRange);

            if (targetEnemy != null) {
                // Have a valid enemy, attack him!
                targetEnemy.Damage(transform.position);
                attackDir = (targetEnemy.GetPosition() - transform.position).normalized;
            }

            //CMDebug.TextPopupMouse("" + attackDir);
            state = State.Attacking;
            moveDir = Vector3.zero;
            characterBase.PlayAttackAnimation(attackDir, () => state = State.Normal);
            float dashDistance = 5f;
            transform.position += attackDir * dashDistance;
        }
    }

}
