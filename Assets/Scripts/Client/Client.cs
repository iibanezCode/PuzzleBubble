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
                        OnAskName(splitData);
                        break;
                        
                    case "CNN":
                        Debug.Log("dato del cnn " + splitData[1] + " segundo valor " + splitData[2]);
                        SpawnPlayer(splitData[1], int.Parse(splitData[2]));
                        break;
                    case "MOVE":
                        jugadores.Find(x => x.playerName == splitData[1]).avatar.transform.Find("Arrow").GetComponent<MoveArrow>().TurnArrow(int.Parse(splitData[2]));
                        jugadores.Find(x => x.playerName == splitData[1]).avatar.transform.Find("PJTurn").GetComponent<Animator>().SetFloat("Speed",float.Parse(splitData[2]));
                        movido = false;
                        break;
                    case "SHOOT":
                        jugadores.Find(x => x.playerName == splitData[1]).avatar.transform.Find("PJ").GetComponent<Animator>().SetTrigger("BlowTrigger");
                        jugadores.Find(x => x.playerName == splitData[1]).avatar.GetComponent<Disparo>().Disparar();
                        break;
                    default:
                        ToLog("Mensaje Invalido" + msg);
                        break;

                }
                break;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && movido == false) {
            Send("ARROWLEFT|" + playerName, reliableChannel);
            movido = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && movido == false) {
            Send("ARROWRIGHT|" + playerName, reliableChannel);
            movido = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            Send("STOPARROW|" + playerName, reliableChannel);
            movido = false;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) {
            Send("STOPARROW|" + playerName, reliableChannel);
            movido = false;
        }
        else if(Input.GetKeyDown(KeyCode.Space)) {
            Send("SHOOT|" + playerName, reliableChannel);
        }

    }

    private void OnAskName(string[] data) {
        
        //Id del player
        ourClientId = int.Parse(data[1]);

        //Enviar el nombre al servidor
        Send("NAMEIS|" + playerName, reliableChannel);
        Debug.Log("Entro al ONASKNAME");
        //enviar datos al resto de jugadores
        for (int i = 2; i < data.Length - 1; i++) {
            string[] d = data[i].Split('%');
            SpawnPlayer(d[0], int.Parse(d[1]));
        }
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
        
        if (cnnId == ourClientId) {
            canvas1.SetActive(false);
            canvas2.SetActive(true);

            canvas2.transform.Find("PnlGame").Find("PnlJ1").gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width/2);
            canvas2.transform.Find("PnlGame").Find("PnlJ2").gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width / 2);

        }

        Player p = new Player();
        if (cnnId % 2 != 0) {
            p.avatar = Instantiate(playerPrefab, jugador1.position, Quaternion.identity);
            p.avatar.transform.parent = jugador1;
        }
        else {
            p.avatar = Instantiate(playerPrefab, jugador2.position, Quaternion.identity);
            p.avatar.transform.parent = jugador2;
        }

        p.avatar.transform.localPosition = new Vector3(0, 0, 0);
        p.avatar.transform.localScale = new Vector3(70, 70, 1);
        p.playerName = playerName;
        p.connectId = cnnId;
        p.dirArrow = new Vector3(0, 0, 0);
        jugadores.Add(p);

    }

    public int getClienteId() {
        return connectionId;
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
