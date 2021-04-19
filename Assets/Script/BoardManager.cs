using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Script;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }
    private bool[,] allowedMoves { set; get; }
    public Chessman[,] Chessmans { set; get; }
    public Chessman selectedChessman;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = -4.0f;
    public List<GameObject> chessPrefabs;
    private List<GameObject> activeChessMan;

    private Quaternion orientation = Quaternion.Euler(0, 180, 0);

    private int selectionX = -1;
    private int selectionY = -1;

    private void Start()
    {
        Instance = this;
        SpawnAllChessmans();
    }
    private void Update()
    {
        UpdateSelection();
        DrawChessboard();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedChessman == null)
                {
                    //select the chessman
                    SelectChessman(selectionX, selectionY);
                }
                else
                {
                    //move the chessman
                    MoveChessman(selectionX, selectionY);
                    
                }
                
            }
        }
    }

  
    //selecting chessman
    private void SelectChessman(int x, int y)
    {
        if (Chessmans[x, y] == null)
        {
            return; // return
        }

        bool hasAtleastOneMove = false;
        allowedMoves = Chessmans[x, y].PossibleMove();
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (allowedMoves[i, j])
                    hasAtleastOneMove = true;

        if (!hasAtleastOneMove)
            return;

        selectedChessman = Chessmans[x, y];
        
        BoardHighlights.Instance.HighLightAllowedMoves(allowedMoves);
    }
    //move to next tapped poistion
    private void MoveChessman(int x, int y)
    {
        if (allowedMoves[x, y])
        {

            Chessman c = Chessmans[x, y];
            if (c != null)
            {

                // Capture a piece
                activeChessMan.Remove(c.gameObject);
                Destroy(c.gameObject);

            }

            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null; //we move
            selectedChessman.transform.position = GetTileCenter(x, y + 1);
            selectedChessman.SetPosition(x, y);
            Chessmans[x, y] = selectedChessman;
           
        }
        
        BoardHighlights.Instance.Hidehighlights();
        selectedChessman = null; // unselect if click on place doesnt make sense
    }
    //get tap position and tile 
    private void UpdateSelection()
    {
        if (!Camera.main)
        {
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            Debug.Log((int)hit.point.z + 3);
            selectionX = (int)Math.Ceiling(hit.point.x) + 3;
            selectionY = (int)Math.Ceiling(hit.point.z) + 3;
        }

        else
        {
            selectionX = -1;
            selectionY = -1;
        }



    }
    //tile arrangement
    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;

        return origin;
    }

    private void SpawnAllChessmans()
    {
        activeChessMan = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        

       
        // Rooks
        SpawChessMan(0, UnityEngine.Random.Range(0, 7), UnityEngine.Random.Range(0, 7));
        

        //Bishops
        SpawChessMan(1, UnityEngine.Random.Range(0, 7), UnityEngine.Random.Range(0, 7));
        

        //Knights
        SpawChessMan(2, UnityEngine.Random.Range(0, 7), UnityEngine.Random.Range(0, 7));
        




    }
    private void DrawChessboard()
    {

        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);

            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);

            }

        }

        // Draw the selection
        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));


            Debug.DrawLine(
              Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
              Vector3.forward * (selectionY) + Vector3.right * (selectionX + 1));

        }

    }

    private void SpawChessMan(int index, int x, int y)
    {
        GameObject go = Instantiate(chessPrefabs[index], GetTileCenter(x, y + 1), orientation) as GameObject;
        go.transform.SetParent(transform);

        Chessmans[x, y] = go.GetComponent<Chessman>(); // Index out of range exception

        Chessmans[x, y].SetPosition(x, y);
        activeChessMan.Add(go);
    }

    
}
