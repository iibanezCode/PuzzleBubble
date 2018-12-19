using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPositionDetector : MonoBehaviour {

    private Vector3 lastCellPosition;

    public Vector3 LastCellPosition {
        get {
            return lastCellPosition;
        }

        set {
            lastCellPosition = value;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Cell")) {
            lastCellPosition = other.transform.position;
        }
    }
}
