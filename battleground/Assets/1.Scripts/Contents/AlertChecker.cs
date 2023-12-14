using System;
using System.Collections;
using System.Collections.Generic;
using FC;
using UnityEngine;

public class AlertChecker : MonoBehaviour {
    [Range(0,50)]public float alertRadius;
    public int extraWave = 1;

    public LayerMask alertMask = TagAndLayer.LayerMsking.Enemy;
    private Vector3 current;
    private bool alert;

    private void Start() {
        InvokeRepeating("PingAlert", 1, 1);
    }

    private void AlertNearBy(Vector3 origin, Vector3 target, int wave = 0) {
        if (wave > this.extraWave) {
            return;
        }

        Collider[] targetsInViewRadius = Physics.OverlapSphere(origin, alertRadius, alertMask);

        foreach (Collider obj in targetsInViewRadius) {
            obj.SendMessageUpwards("AlertCallBack", target, SendMessageOptions.DontRequireReceiver);
            AlertNearBy(obj.transform.position, target, wave+1);
        }
    }

    public void RootAlertNearBy(Vector3 origin) {
        current = origin;
        alert = true;
    }

    void PingAlert() {
        if (alert) {
            alert = false;
            AlertNearBy(current, current);
        }
    }
}
