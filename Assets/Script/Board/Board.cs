using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int[,] BoardInfo{get; set;}

    void Start()
    {
        BoardInfo = new int[ConstList.BoardSize,ConstList.BoardSize];
    }

    void Update()
    {
    }

    public bool SetPiece(List<int[]> PieceDesign, int[] Pivot)
    {
        foreach (int[] Point in PieceDesign)
        {
            Point[0] += Pivot[0];
            Point[1] += Pivot[1];
        }
        return SetPiece(PieceDesign);
    }

    public bool SetPiece(List<int[]> PieceDesign)
    {
        foreach (int[] Point in PieceDesign)
        {
            if (!Utils.IsPieceInBoard(Point))
                return false;
        }
        foreach (int[] Point in PieceDesign)
        {
            BoardInfo[Point[0], Point[1]] = 1;
        }
        return true;
    }
}
