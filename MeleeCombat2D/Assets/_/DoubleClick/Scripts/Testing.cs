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

public class Testing : MonoBehaviour {

    private const float DOUBLE_CLICK_TIME = .2f;

    [SerializeField] private Player player;

    private float lastClickTime;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= DOUBLE_CLICK_TIME) {
                // Double click!
                CMDebug.TextPopupMouse("Double click!");
                player.DashTo(UtilsClass.GetMouseWorldPosition());
            } else {
                // Normal click!
                CMDebug.TextPopupMouse("Normal click!");
                player.MoveTo(UtilsClass.GetMouseWorldPosition());
            }

            lastClickTime = Time.time;
        }
    }

}
