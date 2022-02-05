using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.Text;
using TMPro;

public enum Panel
{
    mainmenu,
    multiplayer,
    options
}

public class MenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField]
    private GameObject main;
    [SerializeField]
    private GameObject multiplayer;
    [SerializeField]
    private GameObject options;
    [SerializeField]
    private GameObject controller;
    [SerializeField]
    private TMP_InputField nicknameField;
    [SerializeField]
    private TMP_InputField ipAddresField;
    
    // Start is called before the first frame update
    void Start()
    {
        new MultiplayerOptions();
        SetPanel(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSingleplayer()
    {
        controller.AddComponent<BothSide>();
        DontDestroyOnLoad(controller);
        SceneManager.LoadSceneAsync(1);
    }

    public void StartMultiplayer(bool server)
    {
        MultiplayerOptions.instance.Multiplayer = true;
        MultiplayerOptions.instance.Nickname = nicknameField.text;

        MultiplayerOptions.instance.Type = server ? ApplicationType.host : ApplicationType.client;
        //MultiplayerOptions.instance.IPToConnect = new IPAddress(new byte[]{ 127,0,0,1 });
        if(!server)
            MultiplayerOptions.instance.IPToConnect = IPAddress.Parse(ipAddresField.text);
        MultiplayerOptions.instance.Port = server ? 1001 : 1000;
        if(server)
        {
            controller.AddComponent<ServerConnector>();
            controller.AddComponent<Server>();
        }
        else
        {
            controller.AddComponent<ClientConnector>();
            controller.AddComponent<Client>();
        }
        DontDestroyOnLoad(controller);
        SceneManager.LoadSceneAsync(1);
    }

    public void SetPanel(int p)
    {
        main.SetActive(false);
        multiplayer.SetActive(false);
        options.SetActive(false);
        switch ((Panel)p)
        {
            case Panel.mainmenu:
                main.SetActive(true);
                break;
            case Panel.multiplayer:
                multiplayer.SetActive(true);
                break;
            case Panel.options:
                options.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
