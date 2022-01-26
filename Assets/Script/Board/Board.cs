using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board
{
    public int[,] BoardInfo{get; set;} //ボードのピースの位置を管理する2重配列
    List<int> SetPlayer; //ピースを置いたことのあるプレイヤーリスト
    const int PlayerNum = 2;
    const int AllPiece = 89;

    //シミュレーション中はSetPlayerは必要なし
    public Board(Board Src)
    {
        BoardInfo = Src.CopyBoardInfo();
        SetPlayer = null;
    }

    public Board()
    {
        BoardInfo = new int[ConstList.BoardSize,ConstList.BoardSize];
        SetPlayer = new List<int>();
    }

    //ピースを置けない場合はfalseを返し、置ける場合はtrueを返す。
    //初めて置く場合は頂点が接している条件を満たせないので、別関数での確認
    public bool IsPossibleSetPiece(List<int[]> PieceDesign, int[] Pivot, int PlayerNum)
    {
        List<int[]> NewPieceDesign = new List<int[]>();
        foreach (int[] Point in PieceDesign)
        {
            int[] TmpPoint = new int[]{0, 0};

            TmpPoint[0] = Point[0] + Pivot[0];
            TmpPoint[1] = Point[1] + Pivot[1];
            NewPieceDesign.Add(TmpPoint);
        }
        return IsPossibleSetPiece(NewPieceDesign, PlayerNum);
    }

    public bool IsPossibleSetPiece(List<int[]> PieceDesign, int PlayerNum)
    {
        foreach (int[] Point in PieceDesign)
        {
            if (!CheckBaseRule(Point, PlayerNum))
                return false;
        }
        bool IsPossible = false;
        foreach (int[] Point in PieceDesign)
        {
            if (SetPlayer != null && !SetPlayer.Contains(PlayerNum + 1))
            {
                IsPossible = CheckStartRule(Point, PlayerNum);
            }
            else
            {
                IsPossible = CheckApplicationRule(Point, PlayerNum);
            }
            if (IsPossible)
                break;
        }
        if (!IsPossible)
            return false;
        return true;
    }

    //ピースを置いたことにするため、BoardInfoに書き込む。
    public void SetPiece(List<int[]> PieceDesign, int[] Pivot, int PlayerNum)
    {
        List<int[]> NewPieceDesign = new List<int[]>();
        foreach (int[] Point in PieceDesign)
        {
            int[] TmpPoint = new int[]{0, 0};

            TmpPoint[0] = Point[0] + Pivot[0];
            TmpPoint[1] = Point[1] + Pivot[1];
            NewPieceDesign.Add(TmpPoint);
        }
        SetPiece(NewPieceDesign, PlayerNum);
    }

    public void SetPiece(List<int[]> PieceDesign, int PlayerNum)
    {
        foreach (int[] Point in PieceDesign)
        {
            BoardInfo[Point[0], Point[1]] = PlayerNum + 1;
        }
        if (SetPlayer != null && !SetPlayer.Contains(PlayerNum + 1))
            SetPlayer.Add(PlayerNum + 1);
    }

    //スタート時はプレイヤーごとのスタート地点が違うので、CheckStartPointsで定義
    //そこに入っているかを確認
    bool CheckStartRule(int[] Point, int PlayerNum)
    {
        int[][] CheckStartPoints = new int[][]{new int[]{0, 0}, new int[]{13, 13}};
        if (Point[0] == CheckStartPoints[PlayerNum][0] && Point[1] == CheckStartPoints[PlayerNum][1])
            return true;
        return false;
    }

    //頂点同士が接しているか確認
    public bool CheckApplicationRule(int[] Point, int PlayerNum)
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

    //基本的なルールに則しているか確認
    //重なっていない、ボードからはみ出ていない、自分のピースの辺が接していない
    public bool CheckBaseRule(int[] Point, int PlayerNum)
    {
        if (!Utils.IsPieceInBoard(Point))
            return false;
        if (BoardInfo[Point[0], Point[1]] != 0)
            return false;
        if (!CheckAdjacentRule(Point, PlayerNum))
            return false;
        return true;
    }

    public bool CheckAdjacentRule(int[] Point, int PlayerNum)
    {
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

    public int[,] CopyBoardInfo()
    {
        int[,] ResBoardInfo = new int[ConstList.BoardSize,ConstList.BoardSize];
        
        for (int IndexY = 0; IndexY < ConstList.BoardSize; IndexY++)
        {
            for (int IndexX = 0; IndexX < ConstList.BoardSize; IndexX++)
            {
                ResBoardInfo[IndexY, IndexX] = BoardInfo[IndexY, IndexX];
            }
        }
        return ResBoardInfo;
    }

    public List<int> CopySetPlayer()
    {
        List<int> ResSetPlayer = new List<int>();
        
        foreach (int Player in SetPlayer)
        {
            ResSetPlayer.Add(Player);
        }
        return ResSetPlayer;
    }

    public List<int> CheckWinPlayer()
    {
        int[] PlayerScores = new int[PlayerNum];
        List<int> Winner = new List<int>();
        
        foreach (int Piece in BoardInfo)
        {
            if (Piece == 0)
                continue;
            PlayerScores[Piece - 1] += 1;
        }
        int MaxScore =  PlayerScores.Max();
        int Index = 0;
        foreach (int PlayerScore in PlayerScores)
        {
            if (MaxScore == PlayerScore)
                Winner.Add(Index);
            Index++;
        }
        return Winner;
    }

    //デバッグ用
    public void PrintBoardInfo(int[,] Board)
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
