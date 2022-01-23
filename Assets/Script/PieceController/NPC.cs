using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class NPC
{


    //置けるピースがあるか検索を行う。
    //ピースの種類(先頭から何番目のピースか)
    // 反転
    //  回転
    //   Y軸の位置
    //    X軸の位置
    // の順でループを行っている。置ける場合にはループの情報を引き渡す


    public static List<object[]> GetInstruction(List<GameObject> PieceList, Board BoardInfo, int Turn, int PlayerNum)
    {
        List<PieceInfo> NewPieceList = new List<PieceInfo>();

        foreach (GameObject Piece in PieceList)
        {
            PieceInfo PieceInfo = Piece.GetComponent<Piece>().PieceInfo;
            PieceInfo ClonePieceInfo = new PieceInfo(PieceInfo.StartDesign);
            NewPieceList.Add(ClonePieceInfo);
        }
        return GetInstruction(NewPieceList, BoardInfo, Turn, PlayerNum);
    }

    public static List<object[]> GetInstruction(List<PieceInfo> PieceList, Board BoardInfo, int Turn, int PlayerNum)
    {
        List<object[]> PossibleSetPieceList = new List<object[]>();

        int PieceIndex = -1;
        int PossibleSetPieceListCount = 0;
        foreach (PieceInfo Piece in PieceList)
        {
            List<int[]> CurrentPieceDesign = Piece.CopyPieceDesign();
            
            PieceIndex++;
            if (SkipPieceCount(Piece.PieceCount, Turn, PossibleSetPieceListCount))
                continue ;
            for (int ReverseCount = 0; ReverseCount < 2; ReverseCount++)
            {
                for (int RotateCount = 0; RotateCount < 4; RotateCount++)
                {
                    for (int IndexY = 0; IndexY < ConstList.BoardSize; IndexY++)
                    {
                        for (int IndexX = 0; IndexX < ConstList.BoardSize; IndexX++)
                        {
                            bool IsSpace = BoardInfo.IsPossibleSetPiece(CurrentPieceDesign, new int[]{IndexY, IndexX}, PlayerNum);
                            if (IsSpace)
                            {
                                PossibleSetPieceListCount++;
                                PossibleSetPieceList.Add( new object[]
                                {
                                    PieceIndex,
                                    ReverseCount,
                                    RotateCount,
                                    IndexY,
                                    IndexX,
                                    PieceDesign.CopyPieceDesign(CurrentPieceDesign),
                                });
                            }
                        }
                    }
                    PieceDesign.Rotate(CurrentPieceDesign, 1);
                }
                PieceDesign.Reverse(CurrentPieceDesign);
            }
        }
        return PossibleSetPieceList;
    }

    static bool SkipPieceCount(int PieceCount, int Turn, int ListCount)
    {
        if (ListCount < 10)
            return false;
        int SkipTurn = 0;
        switch (PieceCount)
        {
            case 5:
                SkipTurn = 2 * 5;
                break;
            case 4:
                SkipTurn = 2 * 7;
                break;
            case 3:
                SkipTurn = 2 * 9;
                break;
            case 2:
                SkipTurn = 2 * 11;
                break;
            case 1:
                SkipTurn = 2 * 13;
                break;
        }
        if (SkipTurn >= Turn)
            return false;
        return true;
    }

    // Scoresの中に各指示でのスコアを格納
    // 格納したスコアの中から最大値の物をMaxScoresに格納
    // その中からランダムに実行
    public static object[] Evaluate(List<object[]> Instruntions, Board BoardInfo, int PlayerNum, int Turn)
    {
        List<int> Scores = new List<int>();
        Board TmpBoardInfo = new Board(BoardInfo);

        if (Turn < 16 || true)
        {
            foreach (object[] Instruntion in Instruntions)
            {
                int Score;
                int IndexY = (int)Instruntion[3];
                int IndexX = (int)Instruntion[4];
                List<int[]> PieceDesign = (List<int[]>)Instruntion[5];
                BoardInfo = new Board(TmpBoardInfo);

                BoardInfo.SetPiece(PieceDesign, new int[]{IndexY, IndexX}, PlayerNum);
                Score = EvaluateBoardInfo(BoardInfo.BoardInfo, PlayerNum);
                Scores.Add(Score);
            }

            List<int> MaxScores = GetMaxScore(Scores);
            Debug.Log("Max: " + Scores.Max() + ", Count: " + MaxScores.Count + ", InstruntionsCount: " + Instruntions.Count);

            int Index = MaxScores[Random.Range(0, MaxScores.Count)];
            return Instruntions[Index];
        }
        else
        {
            foreach (object[] Instruntion in Instruntions)
            {
            }

        }
        return null;
    }

    // 全スコアの中で最大のスコアのみのリストを作成
    static List<int> GetMaxScore(List<int> Scores)
    {
        int Index = 0;
        int Max = Scores.Max();
        List<int> Res = new List<int>();

        foreach(int Score in Scores)
        {
            if (Score == Max)
            {
                Res.Add(Index);
            }
            Index++;
        }
        return Res;
    }

    // 自分のピースがある位置を順にCalcScoreに渡す
    public static int EvaluateBoardInfo(int[,]  BoardInfo, int PlayerNum)
    {
        int Score = 0;
        for (int IndexY = 0; IndexY < ConstList.BoardSize; IndexY++)
        {
            for (int IndexX = 0; IndexX < ConstList.BoardSize; IndexX++)
            {
                if (BoardInfo[IndexY, IndexX] == PlayerNum + 1)
                {
                    Score += CalcScore(IndexY, IndexX, PlayerNum);
                }
            }
        }
        return Score;
    }

    // 主な評価部分
    // スタート地点から遠く、できるだけ中央に寄っている状態を高く評価を行っている
    static int CalcScore(int IndexY, int IndexX, int PlayerNum)
    {
        switch (PlayerNum)
        {
            case 0:
                break;
            case 1:
                IndexX = ConstList.BoardSize - 1 - IndexX;
                IndexY = ConstList.BoardSize - 1 - IndexY;
                break;
        }
        return IndexY + IndexX - System.Math.Abs(IndexY- IndexX);
    }

    public static void SetPiece(List<int[]> PieceDesign, int PlayerNum, int[,] BoardInfo)
    {
        foreach (int[] Point in PieceDesign)
        {
            BoardInfo[Point[0], Point[1]] = PlayerNum + 1;
        }
    }


    public static int[,] CopyBoardInfo(int[,] BoardInfo)
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

    public static void DebugLogPieceList(List<int[]> Design)
    {
        Debug.Log("====");
        foreach (int[] A in Design)
        {
            Debug.Log(A[0] + " " + A[1]);
        }
    }
}
