using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighborDetection : MonoBehaviour {

    public GameObject explosion;
    public bool coined = false;

    public void CheckNeighbors() {
        
       Collider2D[] hitColliders = Physics2D.OverlapCircleAll(gameObject.transform.position, gameObject.GetComponent<CircleCollider2D>().radius);
        Debug.Log("CheckingNeghbors" + hitColliders.Length +" NAME OF TE OBJECT= " +transform.parent.gameObject.name);

        List<GameObject> neighbors = new List<GameObject>();
        int i = 0;
        if(hitColliders == null) {
            return;
        }
        while (i < hitColliders.Length) {
            if (hitColliders[i].gameObject.tag.Equals(gameObject.tag) || hitColliders[i].gameObject.tag.Equals("Coin")) {
                neighbors.Add(hitColliders[i].gameObject.transform.parent.gameObject);
            }
            i++;
        }

        if ( neighbors.Count > 2) {
            foreach(GameObject g in neighbors) {
                ///////////////////////////////////////
                if (g.transform.Find("NeighborDetector").gameObject.tag == "Coin") {
                    
                   GameObject e = Instantiate(explosion, g.transform.parent);
                    e.transform.localScale = new Vector3(70, 70, 70);
                    e.transform.localPosition = Vector3.zero;
                    coined = true;
                }
                //////////////////////////////////////
                g.SetActive(false);
                     
            }
            gameObject.transform.parent.gameObject.SetActive(false);
            //////////////////////////////////////////////////////
            if (coined) {
                GameObject.Find("Client").GetComponent<Client>().SendScoredPointsDouble();
                coined = false;
            }
            else {
                GameObject.Find("Client").GetComponent<Client>().SendScoredPoints();
            }
           ////////////////////////////////////////////////////
        }

    }
}
