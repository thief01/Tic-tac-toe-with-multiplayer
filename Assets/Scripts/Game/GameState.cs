using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharType
{
    empty,
    cross,
    circle
}

public enum LineDirection
{
    horizontal,
    vertical,
    diagonally,
    reverseDiagonally
}

public class GameState : MonoBehaviour
{
    private bool gameEnded = false;

    public static GameState instance;

    private const int SIZE_X = 34;
    private const int SIZE_Y = 34;
    private const int IN_ROW_TO_WIN = 5;

    public event Action<CharType, Vector2Int> OnSetted = delegate { };
    public event Action<CharType, Vector2Int, LineDirection> EndGame = delegate { };

    [SerializeField]
    private GameObject crossPrefab;
    [SerializeField]
    private GameObject circlePrefab;
    [SerializeField]
    private Transform graphicParrent;
    [SerializeField]
    private GameObject line;

    private List<GameObject> settedchars = new List<GameObject>();

    private CharType[,] map = new CharType[SIZE_X, SIZE_Y];
    private CharType tour = CharType.circle;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        EndGame += WinAnimate;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public CharType CurrentMove()
    {
        return tour;
    }

    public void RestartGame()
    {
        tour = CharType.circle;
        gameEnded = false;
        foreach (GameObject g in settedchars)
        {
            Destroy(g);
        }
        for(int i=0; i<SIZE_X; i++)
        {
            for(int j=0; j<SIZE_Y; j++)
            {
                map[i, j] = CharType.empty;
            }
        }
        line.GetComponent<Animator>().SetTrigger("reset");
    }

    public void SetChar(int x, int y)
    {
        if (gameEnded)
            return;
        if (!(x > -1 && y > -1 && x < SIZE_X && y < SIZE_Y))
            return;
        //Debug.Log("Set at: " + x + " " + y);
        if (map[x, y] == CharType.empty)
        {
            map[x, y] = tour;
            GameObject g;

            OnSetted(tour, new Vector2Int(x,y));
            if (tour == CharType.cross)
            {
                g = Instantiate(crossPrefab, graphicParrent);
                tour = CharType.circle;
            }
            else
            {
                g = Instantiate(circlePrefab, graphicParrent);
                tour = CharType.cross;
            }
            if (g != null)
            {
                g.transform.position = new Vector3(ConvertNumberToPosition(x), ConvertNumberToPosition(y), 0);
                settedchars.Add(g);
            }
            
            CheckMap();
        }
    }

    public void ForceSetChar(int x, int y, CharType charT)
    {
        tour = charT;
        SetChar(x, y);
    }

    private float ConvertNumberToPosition(int num)
    {
        return num * 0.32f + 0.16f - num * 0.02f;
    }

    private void CheckMap()
    {
        for (int i = 0; i < SIZE_X - IN_ROW_TO_WIN; i++)
        {
            for (int j = 0; j < SIZE_Y - IN_ROW_TO_WIN; j++)
            {
                if (CheckHorizontal(i, j))
                {
                    EndGame(map[i, j], new Vector2Int(i, j), LineDirection.horizontal);
                    line.transform.position = new Vector3(ConvertNumberToPosition(i), ConvertNumberToPosition(j));
                    line.transform.rotation = Quaternion.Euler(0, 0, 0);
                    line.GetComponent<Animator>().SetTrigger("hor");
                }
                if (CheckVertical(i, j))
                {
                    EndGame(map[i, j], new Vector2Int(i, j), LineDirection.vertical);
                    line.transform.position = new Vector3(ConvertNumberToPosition(i), ConvertNumberToPosition(j));
                    line.transform.rotation = Quaternion.Euler(0, 0, 90);
                    line.GetComponent<Animator>().SetTrigger("hor");
                }
                if (CheckDiagonally(i, j))
                {
                    EndGame(map[i, j], new Vector2Int(i, j), LineDirection.diagonally);
                    line.transform.position = new Vector3(ConvertNumberToPosition(i), ConvertNumberToPosition(j));
                    line.transform.rotation = Quaternion.Euler(0, 0, 45);
                    line.GetComponent<Animator>().SetTrigger("diag");
                }
                if (CheckReverseDiagonally(i, j))
                {
                    EndGame(map[i + IN_ROW_TO_WIN - 1, j], new Vector2Int(i, j), LineDirection.reverseDiagonally);
                    line.transform.position = new Vector3(ConvertNumberToPosition(i + 4), ConvertNumberToPosition(j));
                    line.transform.rotation = Quaternion.Euler(0, 0, -45 - 180);
                    line.GetComponent<Animator>().SetTrigger("diag");
                }
            }
        }
    }

    private bool CheckHorizontal(int x, int y)
    {
        CharType first = map[x, y];
        if (first == CharType.empty)
            return false;
        for (int i = 1; i < IN_ROW_TO_WIN; i++)
        {
            if (map[x + i, y] != first)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckVertical(int x, int y)
    {
        CharType first = map[x, y];
        if (first == CharType.empty)
            return false;
        for (int i = 1; i < IN_ROW_TO_WIN; i++)
        {
            if (map[x, y + i] != first)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckDiagonally(int x, int y)
    {
        CharType first = map[x, y];
        if (first == CharType.empty)
            return false;
        for (int i = 1; i < IN_ROW_TO_WIN; i++)
        {
            if (map[x + i, y + i] != first)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckReverseDiagonally(int x, int y)
    {
        if (x + IN_ROW_TO_WIN - 1 >= SIZE_X)
            return false;
        CharType first = map[x + IN_ROW_TO_WIN - 1, y];
        if (first == CharType.empty)
            return false;
        for (int i = 1; i < IN_ROW_TO_WIN; i++)
        {
            if (first != map[x + IN_ROW_TO_WIN-1 - i, y + i])
            {
                return false;
            }
        }
        return true;
    }

    private void WinAnimate(CharType ch, Vector2Int pos, LineDirection lineDirection)
    {
        //Debug.Log(ch);
        gameEnded = true;
    }
}
