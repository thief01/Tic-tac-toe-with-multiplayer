using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BothSide : MonoBehaviour
{
    public static BothSide instance;
    private bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        StartCoroutine(DelayBind());
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            UIController.instance.SetCurrentPlayer(GameState.instance.CurrentMove());
        }
        
    }

    private IEnumerator DelayBind()
    {
        yield return new WaitForSeconds(0.5f);
        UIController.instance.SetSingleplayerUIMode();
        GameState.instance.EndGame += OnEndGame;
        active = true;
    }

    public virtual void SetChar(int x, int y)
    {
        GameState.instance.SetChar(x, y);
    }

    public virtual void SetReady()
    {

    }

    private void OnEndGame(CharType arg1, Vector2Int arg2, LineDirection arg3)
    {
        UIController.instance.ShowStatus(Status.restartGame);
    }

}
