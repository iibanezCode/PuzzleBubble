using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Client : MonoBehaviour {

    public bool logging = false;

    private const int MAX_CONNECTION = 100;
    private int port = 5701;
    private int hostId;
    private int webHostId;
    private int reliableChannel;
    private int unReliableChannel;
    private float connectionTime;
    private int connectionId;
    private bool isConnected;
    private bool isStarted = false;
    private bool movido = false;
    private byte error;

    private string playerName;
    private int ourClientId;

    public Transform jugador1, jugador2;

    public List<Player> jugadores = new List<Player>();
    public GameObject playerPrefab;
    public float velocidad;
    public GameObject canvas1;
    public GameObject canvas2;
    public GameObject puntosP1;
    public GameObject puntosP2;
    public GameObject Timer;

    public GameObject[] ballPrefabs;
    public GameObject coin;
    private string[] player1Balls;
    private string[] player2Balls;

    private bool finJuego = false;
    private bool Jugador1Instanciado = false;
    private bool Jugador2Instanciado = false;

    public GameObject WinnerSprite, LoserSprite;

    public void Awake() {
        Screen.SetResolution(1024, 768, false);
        Application.targetFrameRate = 60;
    }

    public void Connect() {

        string pName = GameObject.Find("NameInput").GetComponent<InputField>().text;

        if (pName == "") {
            Debug.Log("Debes escribir un nombre");
            return;
        }

        playerName = pName;

        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unReliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

        hostId = NetworkTransport.AddHost(topo, 0);
        connectionId = NetworkTransport.Connect(hostId, "127.0.0.1", port, 0, out error);

        connectionTime = Time.time;
        isConnected = true;
    }


    void Update() {

        if (!isConnected) {
            return;
        }

        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        switch (recData) {
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                ToLog("receiving: " + msg);
                string[] splitData = msg.Split('|');

                switch (splitData[0]) {
                    case "ASKNAME":
                        player1Balls = splitData[splitData.Length - 2].Split('.');
                        player2Balls = splitData[splitData.Length - 1].Split('.');
                        OnAskName(splitData);
                        break;

                    case "CNN":
                       // Debug.Log("dato del cnn " + splitData[1] + " segundo valor " + splitData[2]);
                        //player1Balls = splitData[3].Split('.');
                        //player2Balls = splitData[4].Split('.');
                        for (int i = 1; i < splitData.Length; i++) {
                            string[] players = splitData[i].Split('%');
                            SpawnPlayer(players[0], int.Parse(players[1]));
                        }
                        //SpawnPlayer(splitData[1], int.Parse(splitData[2]));
                        break;
                    case "MOVE":
                        jugadores.Find(x => x.playerName == splitData[1]).avatar.transform.Find("Arrow").GetComponent<MoveArrow>().TurnArrow(int.Parse(splitData[2]));
                        jugadores.Find(x => x.playerName == splitData[1]).avatar.transform.Find("PJTurn").GetComponent<Animator>().SetFloat("Speed", float.Parse(splitData[2]));
                        movido = false;
                        break;
                    case "SHOOT":
                        jugadores.Find(x => x.playerName == splitData[1]).avatar.transform.Find("PJ").GetComponent<Animator>().SetTrigger("BlowTrigger");
                        jugadores.Find(x => x.playerName == splitData[1]).avatar.GetComponent<Disparo>().Disparar();
                        break;
                    case "TIME":
                        Timer.GetComponent<Text>().text = splitData[1];
                        break;
                    case "PUNTOS":
                        /* if (jugadores.Find(x => x.playerName == splitData[1]).Equals(jugadores[0])) {
                             int puntos = int.Parse(puntosP1.GetComponent<Text>().text.Split(':')[1]) + int.Parse(splitData[2]);
                             puntosP1.GetComponent<Text>().text = "Puntos:" + puntos;
                         } else if (jugadores.Find(x => x.playerName == splitData[1]).Equals(jugadores[1])) {
                             int puntos = int.Parse(puntosP2.GetComponent<Text>().text.Split(':')[1]) + int.Parse(splitData[2]);
                             puntosP2.GetComponent<Text>().text = "Puntos:" + puntos;
                         }*/
                        if (splitData[1].Equals("1")) {
                            int puntos = int.Parse(puntosP1.GetComponent<Text>().text.Split(':')[1]) + int.Parse(splitData[2]);
                            puntosP1.GetComponent<Text>().text = "Puntos:" + puntos;
                        }
                        else {
                            int puntos = int.Parse(puntosP2.GetComponent<Text>().text.Split(':')[1]) + int.Parse(splitData[2]);
                            puntosP2.GetComponent<Text>().text = "Puntos:" + puntos;
                        }
                        break;
                    case "FINJUEGO":
                        finJuego = true;
                        break;
                    case "WINNER":
                        WinnerSprite.SetActive(true);
                        break;
                    case "LOSER":
                        LoserSprite.SetActive(true);
                        break;

                    default:
                        ToLog("Mensaje Invalido" + msg);
                        break;

                }
                break;
        }
        if (!finJuego) {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && movido == false) {
                Send("ARROWLEFT|" + playerName, reliableChannel);
                movido = true;
            } else if (Input.GetKeyDown(KeyCode.RightArrow) && movido == false) {
                Send("ARROWRIGHT|" + playerName, reliableChannel);
                movido = true;
            } else if (Input.GetKeyUp(KeyCode.LeftArrow)) {
                Send("STOPARROW|" + playerName, reliableChannel);
                movido = false;
            } else if (Input.GetKeyUp(KeyCode.RightArrow)) {
                Send("STOPARROW|" + playerName, reliableChannel);
                movido = false;
            } else if (Input.GetKeyDown(KeyCode.Space)) {
                Send("SHOOT|" + playerName, reliableChannel);
            } else if (Input.GetKeyDown(KeyCode.P)) {
                Send("PUNTOS|" + connectionId + "|100", reliableChannel);
            }
        } else {
            Send("SCORE|0|" + puntosP1.GetComponent<Text>().text.Split(':')[1] + "|" + "1|" + puntosP2.GetComponent<Text>().text.Split(':')[1], reliableChannel);
        }



    }

    private void OnAskName(string[] data) {

        //Id del player
        ourClientId = int.Parse(data[1]);

        //Enviar el nombre al servidor
        Send("NAMEIS|" + playerName, reliableChannel);
        Debug.Log("Entro al ONASKNAME");
        //enviar datos al resto de jugadores
       /* for (int i = 2; i < data.Length - 2; i++) {
            string[] d = data[i].Split('%');
            SpawnPlayer(d[0], int.Parse(d[1]));
        }*/
    }

    private void Send(string message, int channelId) {

        byte[] msg = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostId, connectionId, channelId, msg, message.Length * sizeof(char), out error);
    }

    public void Colision(string message) {

        byte[] msg = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostId, connectionId, reliableChannel, msg, message.Length * sizeof(char), out error);
    }

    private void SpawnPlayer(string playerName, int cnnId) {

        GameObject PnlJ1 = canvas2.transform.Find("PnlGame").Find("PnlJ1").gameObject;
        GameObject PnlJ2 = canvas2.transform.Find("PnlGame").Find("PnlJ2").gameObject;

       // if (cnnId == ourClientId) {
            canvas1.SetActive(false);
            canvas2.SetActive(true);

            PnlJ1.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width / 2);
            PnlJ2.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width / 2);

       // }

        Player p = new Player();
        if (cnnId % 2 != 0 && !Jugador1Instanciado) {

            p.avatar = Instantiate(playerPrefab, jugador1.position, Quaternion.identity);
            p.avatar.transform.parent = jugador1;
            p.avatar.GetComponent<Disparo>().InitializeBalls(player1Balls, p.connectId);

            GameObject HexGridP1 = PnlJ1.transform.Find("HexGridJ1").gameObject;
            HexGridP1.GetComponent<HexGrid>().PrepareGrid();
            HexCell[] cellsP1 = HexGridP1.GetComponent<HexGrid>().Cells;

            for (int i = 67; i > (cellsP1.Length - 15); i--) {

                GameObject ball = Instantiate(ballPrefabs[int.Parse(player1Balls[67 - i])], cellsP1[i].gameObject.transform);
                ball.transform.localPosition = Vector3.zero;
                ball.transform.localScale = new Vector3(270, 270, 270);
                ball.GetComponent<CircleCollider2D>().enabled = true;
                ball.transform.Find("NeighborDetector").gameObject.GetComponent<NeighborDetection>().enabled = false;
            }
            ////////////////////////////////////////////////////////////////////////////////////////
            GameObject mCoin = Instantiate(coin, cellsP1[13].transform);
            mCoin.transform.localPosition = Vector3.zero;
            mCoin.transform.localScale = new Vector3(270, 270, 270);
            mCoin.GetComponent<CircleCollider2D>().enabled = true;
            mCoin.transform.Find("NeighborDetector").gameObject.GetComponent<NeighborDetection>().enabled = false;

            mCoin = Instantiate(coin, cellsP1[14].transform);
            mCoin.transform.localPosition = Vector3.zero;
            mCoin.transform.localScale = new Vector3(270, 270, 270);
            mCoin.GetComponent<CircleCollider2D>().enabled = true;
            mCoin.transform.Find("NeighborDetector").gameObject.GetComponent<NeighborDetection>().enabled = false;

            ////////////////////////////////////////////////////////////////////////////////////////
            p.avatar.transform.localPosition = new Vector3(0, 0, 0);
            p.avatar.transform.localScale = new Vector3(270, 270, 1);
            p.playerName = playerName;
            p.connectId = cnnId;
            p.dirArrow = new Vector3(0, 0, 0);
            jugadores.Add(p);
            Jugador1Instanciado = true;


        } else if (cnnId % 2 == 0 && !Jugador2Instanciado) {

            p.avatar = Instantiate(playerPrefab, jugador2.position, Quaternion.identity);
            p.avatar.transform.parent = jugador2;
            p.avatar.GetComponent<Disparo>().InitializeBalls(player2Balls, p.connectId);

            GameObject HexGridP2 = PnlJ2.transform.Find("HexGridJ2").gameObject;
            HexGridP2.GetComponent<HexGrid>().PrepareGrid();
            HexCell[] cellsP2 = HexGridP2.GetComponent<HexGrid>().Cells;

            for (int i = 67; i > (cellsP2.Length - 15); i--) {

                GameObject ball2 = Instantiate(ballPrefabs[int.Parse(player2Balls[67 - i])], cellsP2[i].gameObject.transform);
                ball2.transform.localPosition = Vector3.zero;
                ball2.transform.localScale = new Vector3(270, 270, 270);
                ball2.GetComponent<CircleCollider2D>().enabled = true;
                ball2.transform.Find("NeighborDetector").gameObject.GetComponent<NeighborDetection>().enabled = false;
            }
            ////////////////////////////////////////////////////////////////////////////////////////
            GameObject mCoin = Instantiate(coin, cellsP2[13].transform);
            mCoin.transform.localPosition = Vector3.zero;
            mCoin.transform.localScale = new Vector3(270, 270, 270);
            mCoin.GetComponent<CircleCollider2D>().enabled = true;
            mCoin.transform.Find("NeighborDetector").gameObject.GetComponent<NeighborDetection>().enabled = false;

            mCoin = Instantiate(coin, cellsP2[14].transform);
            mCoin.transform.localPosition = Vector3.zero;
            mCoin.transform.localScale = new Vector3(270, 270, 270);
            mCoin.GetComponent<CircleCollider2D>().enabled = true;
            mCoin.transform.Find("NeighborDetector").gameObject.GetComponent<NeighborDetection>().enabled = false;

            ////////////////////////////////////////////////////////////////////////////////////////

            p.avatar.transform.localPosition = new Vector3(0, 0, 0);
            p.avatar.transform.localScale = new Vector3(270, 270, 1);
            p.playerName = playerName;
            p.connectId = cnnId;
            p.dirArrow = new Vector3(0, 0, 0);
            jugadores.Add(p);
            Jugador2Instanciado = true;
        }
       
    }

    public int getClienteId() {
        return connectionId;
    }
    public void SendScoredPoints() {
        Send("PUNTOS|" + connectionId + "|100", reliableChannel);
    }
    public void SendScoredPointsDouble() {
        Send("PUNTOS|" + connectionId + "|200", reliableChannel);
    }

    private void ToLog(string msg) {
        if (logging) {
            Debug.Log(msg);
        }
    }
}

public class Player {
    public string playerName;
    public Vector3 dirArrow;
    public GameObject avatar;
    public int connectId;
}
