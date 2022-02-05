using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        gameState = FindObjectOfType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition *= 3.3f;
                int x = (int)mousePosition.x;
                int y = (int)mousePosition.y;

                BothSide.instance.SetChar(x, y);
            }
            
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            UIController.instance.SetReady();
        }
    }
}
