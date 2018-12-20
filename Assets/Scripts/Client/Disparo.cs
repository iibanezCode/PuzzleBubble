using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

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
    private bool sinBolas = false;

    // Use this for initialization
    public void InitializeBalls(string[] balls,int connectionId) {

        ballPos = transform.Find("BallPositioner");
        nextBallPos = transform.Find("NextBallPositioner");
        hiddenBallPos = transform.Find("HiddenBallPositioner");

        bolas = new List<GameObject>();

        for (int i = 0; i < 8; i++) {
            bolas.Add(Instantiate(bolasPrefabs[int.Parse(balls[i])], hiddenBallPos));
            bolas[i].transform.localPosition = Vector3.zero;
            bolas[i].GetComponent<BallMovement>().cnnId = connectionId;
        }


        SetupBolas();
        SetupBolas();
        SetupBolas();

    }

    private void SetupBolas() {
        if (!sinBolas) {
            if (nextBallPos.childCount == 0) {
                if (!bolas[nextBallIndex].activeSelf) {
                    bolas[nextBallIndex].SetActive(true);
                    bolas[nextBallIndex].GetComponent<BallMovement>().Animated = false;
                }
                bolas[nextBallIndex].transform.parent = nextBallPos;
                bolas[nextBallIndex].transform.localPosition = Vector3.zero;
                nextBallIndex++;
            }

            if (ballPos.childCount == 0) {

                bolas[ballIndex].transform.parent = ballPos;
                bolas[ballIndex].transform.localPosition = Vector3.zero;
                bola = bolas[ballIndex];
                bola.GetComponent<CircleCollider2D>().enabled = true;
                ballIndex++;
            }

            if (nextBallIndex >= 8 && ballIndex >= 8) {
                sinBolas = true;
            }
        }
    }



    public void Disparar() {
        if (ballIndex != 10 && nextBallIndex != 10 && bola != null) {
            if (bola.transform.parent != null) {
                bola.transform.parent = null;
            }
            bola.GetComponent<BallMovement>().shoot(ArrowEnd.position - ArrowOrigin.position);
            bola = null;


            Invoke("SetupBolas", 1);
            Invoke("SetupBolas", 1);
            Invoke("SetupBolas", 1);


        }

    }

    private int GenerateBolas() {
        return UnityEngine.Random.Range(0, 3);
    }
}
