using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActuateBlower : MonoBehaviour {

    public GameObject blower;

    public void blowBlower() {
        blower.GetComponent<Animator>().SetTrigger("Blow");
    }
}
