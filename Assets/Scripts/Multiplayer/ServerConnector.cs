using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;

public class ServerConnector : Connector
{
    /// server events
    public event Action<IPEndPoint, string> OnConnected = delegate { }; // same in client
    public event Action OnDisconnect = delegate { };
    public event Action<Vector2Int> OnSetChar = delegate { }; // same but add CharType
    public event Action OnReady = delegate { }; // same

    // Start is called before the first frame update
    void Start()
    {
        InitSocket(1000);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void ReceivedData(string str, IPEndPoint from)
    {        
        UnityMainThread.wkr.AddJob(() =>
        {
            ClientToServer.Data data = JsonUtility.FromJson<ClientToServer.Data>(str);
            UIController.instance.SetLastEvent(data.type.ToString());
            switch (data.type)
            {
                case ClientToServer.DataType.connect:
                    ClientToServer.Connect d = JsonUtility.FromJson<ClientToServer.Connect>(str);
                    OnConnected(from, d.nickname);
                    break;
                case ClientToServer.DataType.disconnect:
                    OnDisconnect();
                    break;
                case ClientToServer.DataType.setChar:
                    ClientToServer.CharPosition ch = JsonUtility.FromJson<ClientToServer.CharPosition>(str);
                    OnSetChar(ch.charPosition);
                    break;
                case ClientToServer.DataType.ready:
                    OnReady();
                    break;
                default:
                    break;
            }
        });
    }
}
