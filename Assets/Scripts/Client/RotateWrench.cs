using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWrench : MonoBehaviour {

    public Transform wrenchTransform;
	
    public void key1() {
        wrenchTransform.rotation = Quaternion.Euler(0, 0, -25);
    }
    public void key2() {
        wrenchTransform.rotation = Quaternion.Euler(0, 0, -75);
    }
    public void key3() {
        wrenchTransform.rotation = Quaternion.Euler(0, 0, -120);
    }
    public void key4() {
        wrenchTransform.rotation = Quaternion.Euler(0, 0, -180);
    }
    public void key5() {
        wrenchTransform.rotation = Quaternion.Euler(0, 0, -220);
    }
    public void key6() {
        wrenchTransform.rotation = Quaternion.Euler(0, 0, -270);
    }
    public void key7() {
        wrenchTransform.rotation = Quaternion.Euler(0, 0, -305);
    }
    public void key8() {
        wrenchTransform.rotation = Quaternion.Euler(0, 0, -360);
    }
}
