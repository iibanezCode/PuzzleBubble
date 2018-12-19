using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighborDetection : MonoBehaviour {

    public void CheckNeighbors() {

       Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, gameObject.GetComponent<CircleCollider2D>().radius);
        List<GameObject> neighbors = new List<GameObject>();
        int i = 0;
        if(hitColliders == null) {
            return;
        }
        while (i < hitColliders.Length) {
            if (hitColliders[i].gameObject.tag.Equals(gameObject.tag)) {
                neighbors.Add(hitColliders[i].gameObject.transform.parent.gameObject);
            }
            i++;
        }

        if ( neighbors.Count >= 2) {
            foreach(GameObject g in neighbors) {
                //matarbola
            }
        }

    }
}
