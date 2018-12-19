using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour {

    private GameObject posDetector;
    private GameObject neighborDetector;

    private Vector3 dir = Vector3.zero;
    private bool Trigger = false;

    private void Awake() {
        posDetector = transform.Find("PositionCollider").gameObject;
        neighborDetector = transform.Find("NeighborDetector").gameObject;
    }

    public void Update() {
       gameObject.transform.Translate(dir * 2 * Time.deltaTime);    
    }

    public void shoot(Vector3 dir) {       
        this.dir = dir;
        posDetector.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Trigger de la bola entra en:");
        if(other.tag == "Ball" || other.tag == "Techo") {
            Debug.Log("Ha entrado en Bola y techo");
            dir = Vector3.zero;
            if (other.tag == "Ball") {
                //gameObject.transform.position = posDetector.GetComponent<BallPositionDetector>().LastCellPosition;
                neighborDetector.GetComponent<NeighborDetection>().enabled = true;
                neighborDetector.GetComponent<NeighborDetection>().CheckNeighbors();
            }
        }

        if(other.tag == "Pared") {
            Debug.Log("Ha entrado en pared");
            dir = new Vector3(-dir.x, dir.y, dir.z);
        }
    }
}
