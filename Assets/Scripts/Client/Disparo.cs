using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disparo : MonoBehaviour {

    private Transform ballPos;
    private Transform nextBallPos;
    private Transform hiddenBallPos;
    public Transform ArrowOrigin;
    public Transform ArrowEnd;
    public List<GameObject> bolasPrefabs;
    private GameObject bola;
    private GameObject proximaBola;
    private List<GameObject> bolas;

    private int nextBallIndex = 0;
    private int ballIndex = 0;

    public float velocidad = 1;

	// Use this for initialization
	void Start () {

        ballPos = transform.Find("BallPositioner");
        nextBallPos = transform.Find("NextBallPositioner");
        hiddenBallPos = transform.Find("HiddenBallPositioner");

        bolas = new List<GameObject>();
        for(int i = 0; i <10; i++) {
            bolas.Add(Instantiate(bolasPrefabs[GenerateBolas()], hiddenBallPos));
            bolas[i].transform.localPosition = Vector3.zero;
        }

        
        SetupBolas();
        SetupBolas();
        SetupBolas();

    }

    private void SetupBolas() {

        if(nextBallPos.childCount == 0) {
            Debug.Log("Setupeando nextBall");
            bolas[nextBallIndex].transform.parent = nextBallPos;
            bolas[nextBallIndex].transform.localPosition = Vector3.zero;
            nextBallIndex++;
        }
        
        if (ballPos.childCount == 0) {
            Debug.Log("Setupeando Ball");
            bolas[ballIndex].transform.parent = ballPos;
            bolas[ballIndex].transform.localPosition = Vector3.zero;
            bola = bolas[ballIndex];
            ballIndex++;
        }

        if(nextBallIndex == 10) {
            nextBallIndex = 0;
        }
        if (ballIndex == 10) {
            ballIndex = 0;
        }


    }

    public void Disparar() {
        if (ballIndex != 10 && nextBallIndex != 10) {
            bola.transform.parent = null;
            bola.GetComponent<BallMovement>().shoot(ArrowEnd.position - ArrowOrigin.position);
            bola = null;


            
                SetupBolas();
                SetupBolas();
                SetupBolas();

        }

    }

    private int GenerateBolas() {
        return UnityEngine.Random.Range(0, 3);
    }
}
