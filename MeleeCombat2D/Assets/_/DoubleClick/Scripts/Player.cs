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
using UnityEngine;
using V_AnimationSystem;
using CodeMonkey.Utils;

/*
 * Player movement with TargetPosition
 * */
public class Player : MonoBehaviour {
    
    private const float SPEED = 40f;
    
    [SerializeField] private Transform pfDashEffect;

    private Player_Base playerBase;
    private Vector3 targetPosition;

    private void Awake() {
        playerBase = gameObject.GetComponent<Player_Base>();
    }

    private void Update() {
        HandleMovement();
        
        /*
        if (Input.GetMouseButtonDown(0)) {
            MoveTo(UtilsClass.GetMouseWorldPosition());
        }
        if (Input.GetMouseButtonDown(1)) {
            MoveTo(UtilsClass.GetMouseWorldPosition());
            DashTo(UtilsClass.GetMouseWorldPosition());
        }
        */
    }

    private void HandleMovement() {
        if (Vector3.Distance(transform.position, targetPosition) > 1f) {
            Vector3 moveDir = (targetPosition - transform.position).normalized;

            playerBase.PlayMoveAnim(moveDir);
            transform.position += moveDir * SPEED * Time.deltaTime;
        } else {
            playerBase.PlayIdleAnim();
        }
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
        
    public void MoveTo(Vector3 targetPosition) {
        targetPosition.z = 0f;
        this.targetPosition = targetPosition;
    }

    public void DashTo(Vector3 dashPosition) {
        float dashDistance = 50f;
        Vector3 dashDir = (dashPosition - GetPosition()).normalized;
        Transform dashEffectTransform = Instantiate(pfDashEffect, GetPosition(), Quaternion.identity);
        dashEffectTransform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dashDir));
        float dashEffectWidth = 30f;
        dashEffectTransform.localScale = new Vector3(dashDistance / dashEffectWidth, 1f, 1f);
        transform.position += dashDir * dashDistance;
    }

}
