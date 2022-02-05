using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClientToServer;
using System;
using UnityEngine.SceneManagement;

public class Client : BothSide
{
    private ClientConnector cc;

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

    void Update()
    {
        
    }

    private IEnumerator DelayBinds()
    {
        yield return new WaitForSeconds(2);
        cc = Connector.instance as ClientConnector;
        // bind events
        cc.OnConnected += Connected;
        cc.OnSetChar += SetChar;
        cc.OnStartGame += StartGame;
        cc.OnTourChange += TourStatus;
        cc.OnServerDisconnect += ServerDisconnected;
        cc.OnReady += PlayerIsReady;
        GameState.instance.EndGame += EndGame;
        Application.quitting += Disconnect;

        Connect();
    }

    // Calls funcs

    public void Connect()
    {
        Connect data = new Connect();
        data.nickname = MultiplayerOptions.instance.Nickname;
        Connector.instance.SerializeAndSend(data, MultiplayerOptions.instance.GetIpEndPoint());
        
    }

    public void Disconnect()
    {
        Data data = new Data();
        data.type = DataType.disconnect;
        Connector.instance.SerializeAndSend(data, MultiplayerOptions.instance.GetIpEndPoint());
    }

    public override void SetChar(int x, int y)
    {
        CharPosition charPosition = new CharPosition();
        charPosition.charPosition = new Vector2Int(x, y);
        Connector.instance.SerializeAndSend(charPosition, MultiplayerOptions.instance.GetIpEndPoint());
    }
    
    public override void SetReady()
    {
        Data data = new Data();
        data.type = DataType.ready;
        Debug.Log(MultiplayerOptions.instance.GetIpEndPoint());
        Connector.instance.SerializeAndSend(data, MultiplayerOptions.instance.GetIpEndPoint());
    }

    // server events

    private void Connected(string name)
    {
        UIController.instance.ShowStatus(Status.waitingForReady);
        UIController.instance.SetLastEvent("Connected");
        UIController.instance.SetNickname(name);
    }

    private void SetChar(Vector2Int v2, CharType charT)
    {
        GameState.instance.ForceSetChar(v2.x, v2.y, charT);
        UIController.instance.SetLastEvent("Set char");
    }

    private void StartGame(CharType charT)
    {
        UIController.instance.ShowStatus(Status.game);
        UIController.instance.SetYouPlayAs(charT);
        GameState.instance.RestartGame();
        UIController.instance.SetLastEvent("Started game");
    }

    private void TourStatus(ApplicationType tour)
    {
        UIController.instance.SetCurrentPlayer(tour);
        UIController.instance.SetLastEvent("Tour");
    }

    private void ServerDisconnected()
    {
        UIController.instance.SetLastEvent("Server disconnected");
        Debug.Log("Server disconnected");
        SceneManager.LoadSceneAsync(0);
    }

    private void PlayerIsReady()
    {
        // change on UI
        UIController.instance.SetLastEvent("Player ready");
    }

    // show ready button

    private void EndGame(CharType arg1, Vector2Int arg2, LineDirection arg3)
    {
        UIController.instance.ShowStatus(Status.waitingForReady);
    }

}
