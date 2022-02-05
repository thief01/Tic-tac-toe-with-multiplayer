using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ClientToServer
{
    [Serializable]
    public enum DataType
    {
        emtyp,
        connect,
        disconnect,
        setChar,
        ready
    }

    [Serializable]
    public class Data
    {
        public DataType type;
    }

    [Serializable]
    public class CharPosition : Data
    {
        public CharPosition()
        {
            type = DataType.setChar;
        }

        public Vector2Int charPosition;
    }

    [Serializable]
    public class Connect : Data
    {
        public string nickname;

        public Connect()
        {
            type = DataType.connect;
        }
    }
}

namespace ServerToClient
{
    [Serializable]
    public enum DataType
    {
        empty,
        connected,
        startGame,
        setChar,
        tourChange,
        serverDisconnect,
        onReady
    }

    [Serializable]
    public class Data // connected
    {
        public DataType type;
    }
    
    [Serializable]
    public class Connected : Data
    {
        public string nickname;
        public Connected()
        {
            type = DataType.connected;
        }
    }

    [Serializable]
    public class StartGame : Data // startGame
    {
        public StartGame()
        {
            type = DataType.startGame;
        }

        public CharType youPlayAs;
    }

    [Serializable]
    public class CharPosition : Data // setChar
    {
        public CharPosition()
        {
            type = DataType.setChar;
        }

        public CharType charType;

        public Vector2Int charPosition;
    }

    [Serializable]
    public class TourStatus : Data // tour change
    {
        public TourStatus()
        {
            type = DataType.tourChange;
        }

        public ApplicationType who;
    }
}