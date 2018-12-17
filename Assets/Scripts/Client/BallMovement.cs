using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour {

    private Vector3 dir = Vector3.zero;
    private bool Trigger = false;

    public void Update() {
       gameObject.transform.Translate(dir * 2 * Time.deltaTime);    
    }

    public void shoot(Vector3 dir) {       
        this.dir = dir;        
    }
}
