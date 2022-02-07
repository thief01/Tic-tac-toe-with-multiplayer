using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.SceneManagement;

public class Server : BothSide
{
    private ApplicationType tour;
    private GameStatus status;

    private bool hostIsReady = false;
    private bool clientIsReady = false;

    private CharType serverPlayAs;

    private ServerConnector sc;

    private bool connected;

    private ApplicationType lastStarted = ApplicationType.client;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayBinds());
    }

    // Update is called once per frame
    void Update()
    {
        if(status == GameStatus.waiting)
        {
            if(clientIsReady && hostIsReady)
            {
                UIController.instance.ShowStatus(Status.game);
                RestartGame();
            }
        }
    }

    private IEnumerator DelayBinds()
    {
        yield return new WaitForSeconds(2);
        sc = Connector.instance as ServerConnector;
        // events
        sc.OnConnected += OnConnected;
        sc.OnDisconnect += OnDisconnected;
        sc.OnReady += OnReady;
        sc.OnSetChar += OnSetChar;
        // game events
        GameState.instance.EndGame += OnEndGame;
        GameState.instance.OnSetted += OnSettedChar;
        Application.quitting += ServerDisconnected;
    }

    // call funcs

    public override void SetChar(int x, int y)
    {
        if (status != GameStatus.waiting)
        {
            if (tour == ApplicationType.host)
            {
                GameState.instance.SetChar(x, y);
            }
        }
    }

    public override void SetReady()
    {
        if (connected)
        {
            hostIsReady = true;
            PlayerIsReady();
        }
        else
        {
            UIController.instance.SetLastEvent("No one connected");
        }
    }

    // start game

    private void RestartGame()
    {
        status = GameStatus.playing;
        StartTourLogic();
        hostIsReady = false;
        clientIsReady = false;
        GameState.instance.RestartGame();
        UIController.instance.SetYouPlayAs(serverPlayAs);
        UIController.instance.SetCurrentPlayer(tour);
        StartGame();
        UIController.instance.SetLastEvent("Start game");
        TourStatus();
    }

    private void StartTourLogic()
    {
        if(lastStarted == ApplicationType.host)
        {
            lastStarted = ApplicationType.client;
            tour = ApplicationType.client;
            serverPlayAs = CharType.cross;
        }
        else
        {
            lastStarted = ApplicationType.host;
            tour = ApplicationType.host;
            serverPlayAs = CharType.circle;
        }
    }

    // client Events

    private void OnConnected(IPEndPoint ip, string name)
    {
        connected = true;
        MultiplayerOptions.instance.IPToConnect = ip.Address;
        MultiplayerOptions.instance.Port = ip.Port;
        Connected();
        UIController.instance.SetLastEvent("Connected");
        UIController.instance.SetNickname(name);
    }

    private void OnDisconnected()
    {
        connected = false;
        SceneManager.LoadSceneAsync(0);
    }

    private void OnReady()
    {
        clientIsReady = true;
        UIController.instance.SetLastEvent("Client ready");
    }

    private void OnSetChar(Vector2Int pos)
    {
        if (status != GameStatus.waiting)
        {
            if (tour == ApplicationType.client)
            {
                GameState.instance.SetChar(pos.x, pos.y); ;
            }
            UIController.instance.SetLastEvent("Client set char");
        }
    }

    // call events for client

    private void Connected()
    {
        ServerToClient.Connected c = new ServerToClient.Connected();
        c.nickname = MultiplayerOptions.instance.Nickname;
        sc.SerializeAndSend(c, MultiplayerOptions.instance.GetIpEndPoint());
    }

    private void StartGame()
    {
        ServerToClient.StartGame startGame = new ServerToClient.StartGame();
        startGame.youPlayAs = CharType.cross;
        if (serverPlayAs == CharType.cross)
            startGame.youPlayAs = CharType.circle;
        sc.SerializeAndSend(startGame, MultiplayerOptions.instance.GetIpEndPoint());
    }

    private void SetChar(CharType charT, Vector2Int pos)
    {
        ServerToClient.CharPosition charPosition = new ServerToClient.CharPosition();
        charPosition.charType = charT;
        charPosition.charPosition = pos;
        sc.SerializeAndSend(charPosition, MultiplayerOptions.instance.GetIpEndPoint());
    }

    private void TourStatus()
    {
        ServerToClient.TourStatus d = new ServerToClient.TourStatus();
        d.who = tour;
        sc.SerializeAndSend(d, MultiplayerOptions.instance.GetIpEndPoint());
    }

    private void ServerDisconnected()
    {
        ServerToClient.Data d = new ServerToClient.Data();
        d.type = ServerToClient.DataType.serverDisconnect;
        sc.SerializeAndSend(d, MultiplayerOptions.instance.GetIpEndPoint());
    }

    private void PlayerIsReady()
    {
        ServerToClient.Data d = new ServerToClient.Data();
        d.type = ServerToClient.DataType.onReady;
        sc.SerializeAndSend(d, MultiplayerOptions.instance.GetIpEndPoint());
    }

    // game state controll

    private void OnSettedChar(CharType charT, Vector2Int pos)
    {
        UIController.instance.SetLastEvent(tour.ToString());
        if (tour == ApplicationType.host)
        {
            tour = ApplicationType.client;
        }
        else
        {
            tour = ApplicationType.host;
        }
        UIController.instance.SetYouPlayAs(serverPlayAs);
        UIController.instance.SetCurrentPlayer(tour);
        SetChar(charT, pos);
        TourStatus();
    }

    private void OnEndGame(CharType charT, Vector2Int pos, LineDirection lr)
    {
        status = GameStatus.waiting;
        UIController.instance.ShowStatus(Status.waitingForReady);
        clientIsReady = false;
        hostIsReady = false;
    }
}
