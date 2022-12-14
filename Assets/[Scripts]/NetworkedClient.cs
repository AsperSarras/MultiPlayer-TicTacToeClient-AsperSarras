using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{
    public static NetworkedClient Instance { get; private set; }//singleton

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;

    //Data
    public StateEnum PlayerState = StateEnum.LOBY;
    public string UserName = "";
    public bool isLogedIn = false;
    public string roomName = "";
    public bool waitingForPlayer = false;

    public bool TurnPlayer = false;
    public string EnemyName = "";
    public string PlayerSign = "";
    public string EnemySign = "";
    public int TurnMove = -1;
    public bool newMove = false;
    public int winCheck = -1;

    public int ChatCheck = -1;

    public int serverMessage = -1;

    public bool ErrorCheck = false;
    public string ErrorMessage = "";

    public bool ExpectatorUpdate = false;
    public string ExpectatorData = "";

    public string GameReplayName = "";
    public string GameReplayData = "";

    public bool GamePlayNotificationCheck = false;
    public string GamePlayNotificationMessage = "";

    public bool ReplayUpdate = false;
    public string replayData = "";
    public bool ReplayPlaying = false;

    private void Awake()//singleton
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(gameObject);
        //Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isConnected)
        {
            Connect();
        }    

        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }

    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.100.109", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);
            }
        }
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
        //GameSingleton.Instance.isLogedIn = false;
    }

    public bool IsConnected()
    {
        return isConnected;
    }

    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        string[] data = msg.Split(",");
        int i = int.Parse(data[0]);
        Debug.Log(i);
        switch (i)
        {
            case 0:
                ErrorCheck = true;
                ErrorMessage = "User already created";
                break;
            case 1:
                Debug.Log("Created");
                break;
            case 2:
                Debug.Log("Loged");
                isLogedIn = true;
                PlayerState = StateEnum.LOBY;
                break;
            case 3:
                ErrorCheck = true;
                ErrorMessage = "Password Incorrect";
                break;
            case 4:
                ErrorCheck = true;
                ErrorMessage = "User not Created";
                break;
            case 5:
                Debug.Log("JoinedGame");
                break;
            case 6:
                Debug.Log("CreatedGame");
                PlayerState = StateEnum.WAITING;
                break;
            case 7:
                Debug.Log("LogOut");
                isLogedIn = false;
                break;
            case 8:
                Debug.Log("Cancel Waiting");
                PlayerState = StateEnum.LOBY;
                break;
            case 9:                             //Data: 1 = Going first or not. 2 = Enemy player name
                Debug.Log("Starting Game");
                PlayerState = StateEnum.GAME;
                if(int.Parse(data[1]) == 1)
                {
                    TurnPlayer = true;
                    PlayerSign = "O";
                    EnemySign = "X";
                }
                else
                {
                    TurnPlayer = false;
                    EnemySign = "O";
                    PlayerSign = "X";
                }
                EnemyName = data[2];
                break;
            case 10:
                Debug.Log("NextTurn");
                TurnMove = int.Parse(data[1]);
                newMove = true;
                break;
            case 11:
                if (int.Parse(data[1]) == 1)
                {
                    Debug.Log("YouWin");
                    winCheck = int.Parse(data[1]);
                }
                else if (int.Parse(data[1]) == 0)
                {
                    Debug.Log("YouLose");
                    winCheck = int.Parse(data[1]);
                }
                else if (int.Parse(data[1]) == 2)
                {
                    Debug.Log("Draw");
                    winCheck = int.Parse(data[1]);
                }
                break;
            case 12:
                Debug.Log("GoBackToLobby");
                PlayerState = StateEnum.LOBY;
                break;
            case 13:
                Debug.Log("Replay");
                winCheck = -1;
                TurnMove = -1;
                serverMessage = 1;
                break;
            case 14:
                ErrorCheck = true;
                ErrorMessage = "User already Loged in";
                Debug.Log(ErrorMessage);
                break;
            case 15:
                Debug.Log("Enemy Left Game");
                serverMessage = 2;
                break;
            case 16:
                ErrorCheck = true;
                ErrorMessage = "Room Players Full";
                break;
            case 17:
                ChatCheck = int.Parse(data[1]);
                break;
            case 18:
                //UpdateExpectator
                ExpectatorData = msg;
                ExpectatorUpdate = true;
                break;
            case 19:
                ErrorCheck = true;
                ErrorMessage = "Room Not Found";
                break;
            case 20:
                ErrorCheck = true;
                ErrorMessage = "Game Has Not Started";
                break;
            case 21: //ExpectateGame
                PlayerState = StateEnum.GAME;
                //ExpectatorCheck = true;
                PlayerSign = "O";
                EnemySign = "X";
                break;
            case 22: //ReplaySaved
                GamePlayNotificationMessage = "Replay Saved";
                GamePlayNotificationCheck = true;
                break;
            case 23: //ReplaySavedFailed
                GamePlayNotificationMessage = "Replay with that name already exists";
                GamePlayNotificationCheck = true;
                break;
            case 24:
                replayData = "";
                for (int ReplayI = 1; ReplayI < data.Length; ReplayI++)
                {
                    if (ReplayI == 1)
                    {
                        replayData = data[ReplayI];
                    }
                    else
                    {
                        replayData = replayData + "," + data[ReplayI];
                    }
                }
                ReplayUpdate = true;
                break;
            case 25:
                for (int ReplayGame = 1; ReplayGame < data.Length; ReplayGame++)
                {
                    if (ReplayGame == 1)
                    {
                        GameReplayData = data[ReplayGame];
                    }
                    else
                    {
                        GameReplayData = GameReplayData + "," + data[ReplayGame];
                    }
                }
                ReplayPlaying = true;
                PlayerState = StateEnum.GAME;
                break;
        }
    }

}