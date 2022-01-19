using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int[,] BoardInfo{get; set;} //ボードのピースの位置を管理する2重配列

    void Start()
    {
        BoardInfo = new int[ConstList.BoardSize,ConstList.BoardSize];
    }

    void Update()
    {
    }

    public bool SetPiece(List<int[]> PieceDesign, int[] Pivot, int PlayerNum)
    {
        List<int[]> NewPieceDesign = new List<int[]>();
        foreach (int[] Point in PieceDesign)
        {
            int[] TmpPoint = new int[]{0, 0};

            TmpPoint[0] = Point[0] + Pivot[0];
            TmpPoint[1] = Point[1] + Pivot[1];
            NewPieceDesign.Add(TmpPoint);
        }
        return SetPiece(NewPieceDesign, PlayerNum);
    }

    //ピースを置く。置けない場合はfalseを返し、置けた場合はBoardInfoに書き込んで、trueを返す。
    public bool SetPiece(List<int[]> PieceDesign, int PlayerNum)
    {
        foreach (int[] Point in PieceDesign)
        {
            if (!Utils.IsPieceInBoard(Point))
                return false;
            if (BoardInfo[Point[0], Point[1]] != 0)
                return false;
        }
        foreach (int[] Point in PieceDesign)
        {
            BoardInfo[Point[0], Point[1]] = PlayerNum + 1;
        }
        return true;
    }

    void PrintBoardInfo(int[,] Board)
    {
        int y = 0;
        string PrintList = "\n";
        for (; y < ConstList.BoardSize ;y++)
        {
            int x = 0;
            for (; x < ConstList.BoardSize ;x++)
            {
                PrintList += (Board[y, x]) + " ";
            }
            PrintList += "\n";
        }
        Debug.Log(PrintList);
    }
}
