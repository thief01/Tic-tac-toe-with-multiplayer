using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public enum Status
{
    waitingForReady,
    waitingForOtherPlayers,
    game,
    restartGame
}

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        if(MultiplayerOptions.instance.Multiplayer == false)
        {
            youPlayAs.gameObject.SetActive(false);
            ShowStatus(Status.game);
        }
        else
        {
            ShowStatus(Status.waitingForReady);
        }
    }

    [SerializeField]
    private TextMeshProUGUI currentPlayer;
    [SerializeField]
    private TextMeshProUGUI youPlayAs;

    [SerializeField]
    private GameObject button;
    [SerializeField]
    private GameObject restartButton;
    [SerializeField]
    private GameObject waitingText;
    [SerializeField]
    private TextMeshProUGUI events;
    [SerializeField]
    private TextMeshProUGUI nickname;

    public void RestartSingleplayer()
    {
        GameState.instance.RestartGame();
        restartButton.SetActive(false);
    }

    public void ShowStatus(Status s)
    {
        switch (s)
        {
            case Status.waitingForReady:
                button.SetActive(true);
                waitingText.SetActive(false);
                restartButton.SetActive(false);
                break;
            case Status.waitingForOtherPlayers:
                button.SetActive(false);
                waitingText.SetActive(true);
                break;
            case Status.game:
                button.SetActive(false);
                waitingText.SetActive(false);
                break;
            case Status.restartGame:
                restartButton.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void SetSingleplayerUIMode()
    {
        ShowStatus(Status.game);
        youPlayAs.gameObject.SetActive(false);
        nickname.gameObject.SetActive(false);
        events.gameObject.SetActive(false);
    }

    public void SetCurrentPlayer(ApplicationType type)
    {
        currentPlayer.text = "Now moving: " + type.ToString();
    }
    
    public void SetCurrentPlayer(CharType charT)
    {
        currentPlayer.text = "Now moving: " + charT.ToString();
    }

    public void SetYouPlayAs(CharType type)
    {
        youPlayAs.text = "Your char: " + type.ToString();
    }

    public void SetReady()
    {
        ShowStatus(Status.waitingForOtherPlayers);
        BothSide.instance.SetReady();
    }

    public void SetLastEvent(string e)
    {
        events.text = "";
        //events.text = e;
    }

    public void SetNickname(string oponent)
    {
        nickname.text = "Opponent nickname: " + oponent;
    }
}
