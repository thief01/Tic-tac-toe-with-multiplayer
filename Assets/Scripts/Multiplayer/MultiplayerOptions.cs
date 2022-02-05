using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

[System.Serializable]
public enum ApplicationType
{
    host,
    client
}

public enum GameStatus
{
    waiting,
    playing
}


public class MultiplayerOptions
{
    public bool Multiplayer { get; set; }

    public ApplicationType Type { get; set; }

    public IPAddress IPToConnect { get; set; }

    public int Port { get; set; }

    public string Nickname { get; set; }

    public static MultiplayerOptions instance;

    public MultiplayerOptions()
    {
        if (instance == null)
            instance = this;
    }

    public IPEndPoint GetIpEndPoint()
    {
        return new IPEndPoint(IPToConnect, Port);
    }
}
