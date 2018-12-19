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
        
        if(other.tag == "Ball" || other.tag == "Techo") {
            
            dir = Vector3.zero;
            if (other.tag == "Ball") {
                Debug.Log("Ha entrado en Bola");
                gameObject.transform.position = posDetector.GetComponent<BallPositionDetector>().LastCellPosition;
                neighborDetector.SetActive(true);
                neighborDetector.GetComponent<NeighborDetection>().enabled = true;
                neighborDetector.GetComponent<NeighborDetection>().CheckNeighbors();
                GetComponent<Animator>().SetTrigger("Pop");
            }
        }

        if(other.tag == "Pared") {
            dir = new Vector3(-dir.x, dir.y, dir.z);
        }
    }
}
