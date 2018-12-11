using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveArrow : MonoBehaviour {

    private Transform transform;
    private int amount = 0;

	// Use this for initialization
	void Start () {
        transform = gameObject.GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        if (transform != null)
        transform.Rotate(new Vector3(0, 0, 1), amount);
    }
    public void TurnArrow(int amount) {
        this.amount = amount;
    }
}
