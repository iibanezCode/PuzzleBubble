using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Server : MonoBehaviour {

    private const int MAX_CONNECTION = 100;
    private int port = 5701;

    private int hostId;
    private int webHostId;

    private int reliableChannel;
    private int unReliableChannel;

    private bool isStarted = false;
    private byte error;

    public GameObject LoggingText;
    private int logCounter = 0;
    private float time = 0;

    private List<ServerClient> clients = new List<ServerClient>();
    private string Player1Balls;
    private string Player2Balls;
    // Use this for initialization
    void Awake() {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unReliableChannel = cc.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);
        hostId = NetworkTransport.AddHost(topo, port, null);
        webHostId = NetworkTransport.AddWebsocketHost(topo, port, null);

        isStarted = true;

        ToLog("Server Started Succesfully");

        GenerateBalls();

    }

    private void GenerateBalls() {

        Player1Balls = "";
        Player2Balls = "";

        for (int i = 0; i < 15; i++) {

            Player1Balls += Random.Range(1, 4) + ".";
            Player2Balls += Random.Range(1, 4) + ".";
            Debug.Log(Player1Balls);
        }

        Player1Balls = Player1Balls.Substring(0, Player1Balls.Length - 1);
        Player2Balls = Player2Balls.Substring(0, Player2Balls.Length - 1);
        ToLog(Player1Balls + " --->1");
        ToLog(Player2Balls + " --->2");

    }

    // Update is called once per frame
    void Update() {
        if (!isStarted) {
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
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                ToLog("Player" + connectionId + " has connected.");
                OnConnection(connectionId);
                break;

            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                ToLog("Message -> " + connectionId + ": " + msg);
                string[] splitData = msg.Split('|');
                switch (splitData[0]) {
                    case "NAMEIS":
                        OnNameIs(connectionId, splitData[1]);
                        break;

                    case "ARROWLEFT":
                        Send("MOVE|" + splitData[1] + "|1", reliableChannel, clients);
                        break;

                    case "ARROWRIGHT":
                        Send("MOVE|" + splitData[1] + "|-1", reliableChannel, clients);
                        break;

                    case "STOPARROW":
                        Send("MOVE|" + splitData[1] + "|0", reliableChannel, clients);
                        break;

                    case "SHOOT":
                        Send("SHOOT|" + splitData[1], reliableChannel, clients);
                        break;

                    case "PUNTOS":
                        Send("PUNTOS|" + splitData[1] + "|" + splitData[2], reliableChannel, clients);
                        break;
                    case "SCORE":
                        if (int.Parse(splitData[2]) > int.Parse(splitData[4])) {
                            Send("WINNER", reliableChannel, 1);
                            Send("LOSER", reliableChannel, 2);

                        } else {
                            Send("WINNER", reliableChannel, 2);
                            Send("LOSER", reliableChannel, 1);
                        }
                        break;
                }
                break;

            case NetworkEventType.DisconnectEvent:
                ToLog("Player" + connectionId + " has disconnected");
                for (int i = 0; i < clients.Count; i++) {
                    if (clients[i].connectionId == connectionId) {
                        clients.RemoveAt(i);
                    }
                }
                break;
        }
        if (clients.Count >= 2) {
            int lastTime = (int)time;
            time += Time.deltaTime;
            if ((int)time != lastTime) {
                Send("TIME|" + (int)time, reliableChannel, clients);
            }
            if ((int)time == 60) {
                Send("FINJUEGO|", reliableChannel, clients);
            }
        }
    }
    private void OnConnection(int cnnId) {
        //Añadir a la lista
        ServerClient c = new ServerClient();
        c.connectionId = cnnId;
        c.playerName = "TEMP";
        clients.Add(c);

        //Despues de añadir el cliente al servidor
        //mandamos un cliente a los clientes
        string msg = "ASKNAME|" + cnnId + "|";

        foreach (ServerClient sc in clients)
            msg += sc.playerName + "%" + sc.connectionId + '|';

        msg = msg.Trim('|');
        ToLog("Sent to all clients -> " + msg);
        msg +="|" + Player1Balls + "|" + Player2Balls; 

        Send(msg, reliableChannel, cnnId);

    }
    private void Send(string message, int channelId, int cnnId) {

        List<ServerClient> c = new List<ServerClient>();
        c.Add(clients.Find(x => x.connectionId == cnnId));
        Send(message, channelId, c);
    }

    private void Send(string message, int channelId, List<ServerClient> c) {

        ToLog("Sending: " + message);
        byte[] msg = Encoding.Unicode.GetBytes(message);
        foreach (ServerClient sc in c) {
            NetworkTransport.Send(hostId, sc.connectionId, channelId, msg, message.Length * sizeof(char), out error);
        }
    }
    private void OnNameIs(int cnnId, string playerName) {

        clients.Find(x => x.connectionId == cnnId).playerName = playerName;
        string msg = "";
        foreach (ServerClient sc in clients) {
            msg += "|" + sc.playerName + "%" + sc.connectionId;
        }

        Send("CNN" + msg , reliableChannel, clients);
    }

    private void ToLog(string msg) {
        if (logCounter >= 18) {
            LoggingText.GetComponent<Text>().text = msg;
            logCounter = 1;
        } else {
            LoggingText.GetComponent<Text>().text = LoggingText.GetComponent<Text>().text + "\n" + msg;
            logCounter++;
        }
    }
}


public class ServerClient {
    public int connectionId;
    public string playerName;
    public int id;
    public int puntos;
}
