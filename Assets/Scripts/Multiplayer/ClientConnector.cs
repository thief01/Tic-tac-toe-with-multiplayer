using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;

public class ClientConnector : Connector
{
    /// client events
    public event Action<string> OnConnected = delegate { };
    public event Action<Vector2Int, CharType> OnSetChar = delegate { };
    public event Action OnReady = delegate { };
    public event Action<CharType> OnStartGame = delegate { };
    public event Action<ApplicationType> OnTourChange = delegate { };
    public event Action OnServerDisconnect = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        InitSocket(1001);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void ReceivedData(string str, IPEndPoint ip)
    {
        UnityMainThread.wkr.AddJob(() =>
        {
            ServerToClient.Data data = JsonUtility.FromJson<ServerToClient.Data>(str);

            switch (data.type)
            {
                case ServerToClient.DataType.connected:
                    ServerToClient.Connected conn = JsonUtility.FromJson<ServerToClient.Connected>(str);
                    OnConnected(conn.nickname);
                    break;
                case ServerToClient.DataType.startGame:
                    ServerToClient.StartGame startGame = JsonUtility.FromJson<ServerToClient.StartGame>(str);
                    OnStartGame(startGame.youPlayAs);
                    break;
                case ServerToClient.DataType.setChar:
                    ServerToClient.CharPosition charPosition = JsonUtility.FromJson<ServerToClient.CharPosition>(str);
                    OnSetChar(charPosition.charPosition, charPosition.charType);
                    break;
                case ServerToClient.DataType.tourChange:
                    ServerToClient.TourStatus tourStatus = JsonUtility.FromJson<ServerToClient.TourStatus>(str);
                    OnTourChange(tourStatus.who);
                    break;
                case ServerToClient.DataType.serverDisconnect:
                    OnServerDisconnect();
                    break;
                case ServerToClient.DataType.onReady:
                    OnReady();
                    break;
                default:
                    break;
            }
        });
    }
}
