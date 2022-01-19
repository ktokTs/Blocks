using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int[,] BoardInfo{get; set;} //ボードのピースの位置を管理する2重配列
    List<int> SetPlayer = new List<int>();

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
            if (!CheckBaseRule(Point, PlayerNum))
                return false;
        }
        bool IsPossible = false;
        foreach (int[] Point in PieceDesign)
        {
            if (!SetPlayer.Contains(PlayerNum + 1))
            {
                IsPossible = CheckStartRule(Point, PlayerNum);
            }
            else
            {
                IsPossible = CheckApplicationRule(Point, PlayerNum);
            }
            Debug.Log(Point[0] + " " + Point[1] + " " + IsPossible + " " + !SetPlayer.Contains(PlayerNum + 1));
            if (IsPossible)
                break;
        }
        if (!IsPossible)
            return false;
        foreach (int[] Point in PieceDesign)
        {
            BoardInfo[Point[0], Point[1]] = PlayerNum + 1;
        }
        if (!SetPlayer.Contains(PlayerNum + 1))
            SetPlayer.Add(PlayerNum + 1);
        return true;
    }


    bool CheckStartRule(int[] Point, int PlayerNum)
    {
        int[][] CheckStartPoints = new int[][]{new int[]{0, 0}, new int[]{13, 13}};
        if (Point[0] == CheckStartPoints[PlayerNum][0] && Point[1] == CheckStartPoints[PlayerNum][1])
            return true;
        return false;
    }

    bool CheckApplicationRule(int[] Point, int PlayerNum)
    {
        int[][] CheckDiagonalPoints = new int[][]{new int[]{1, 1}, new int[]{-1, 1}, new int[]{1, -1}, new int[]{-1, -1}};
        bool IsPossible = false;
        foreach(int[] CheckPoint in CheckDiagonalPoints)
        {
            CheckPoint[1] += Point[1];
            CheckPoint[0] += Point[0];
            if (!Utils.IsPieceInBoard(CheckPoint))
                continue;
            if (BoardInfo[CheckPoint[0], CheckPoint[1]] == PlayerNum + 1)
                IsPossible = true;
        }
        return IsPossible;
    }
    bool CheckBaseRule(int[] Point, int PlayerNum)
    {
        if (!Utils.IsPieceInBoard(Point))
            return false;
        if (BoardInfo[Point[0], Point[1]] != 0)
            return false;
        int[][] CheckAdjacentPoints = new int[][]{new int[]{1, 0}, new int[]{-1, 0}, new int[]{0, 1}, new int[]{0, -1}};
        foreach(int[] CheckPoint in CheckAdjacentPoints)
        {
            CheckPoint[1] += Point[1];
            CheckPoint[0] += Point[0];
            if (!Utils.IsPieceInBoard(CheckPoint))
                continue;
            if (BoardInfo[CheckPoint[0], CheckPoint[1]] == PlayerNum + 1)
                return false;
        }
        return true;
    }

    //デバッグ用
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
