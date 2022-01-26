using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using System;


public static class NPC
{
    const int MaxPlayer = 2;
    const int MaxLoopCount = 2;
    const int ChangeTurnCount_EvaluateMethod = 22;
    const int ExtractionCount = 100;
    const int CanUsePiece5 = 7;
    const int CanUsePiece4 = 10;
    const int CanUsePiece3 = 13;
    const int CanUsePiece2 = 15;
    const int CanUsePiece1 = 17;
    static List<int> CPUNum = new List<int>{0};

    static long ExecTime;

    //置けるピースがあるか検索を行う。
    //ピースの種類(先頭から何番目のピースか)
    // 反転
    //  回転
    //   Y軸の位置
    //    X軸の位置
    // の順でループを行っている。置ける場合にはループの情報を引き渡す
    public static object[] GetInstruction(List<List<GameObject>> AllPieceList, Board BoardInfo, int Turn, int PlayerNum)
    {
        List<List<PieceInfo>> NewAllPieceList = new List<List<PieceInfo>>();

        foreach (List<GameObject> PlayerPieceList in AllPieceList)
        {
            List<PieceInfo> NewPlayerPieceList = new List<PieceInfo>();
            foreach (GameObject Piece in PlayerPieceList)
            {
                PieceInfo PieceInfo = Piece.GetComponent<Piece>().PieceInfo;
                PieceInfo ClonePieceInfo = new PieceInfo(PieceInfo);
                NewPlayerPieceList.Add(ClonePieceInfo);
            }
            NewAllPieceList.Add(NewPlayerPieceList);
        }
        return GetInstruction(NewAllPieceList, BoardInfo, Turn, PlayerNum);
    }

    // CPUNumにPlayerNumが含まれているかどうかで処理方法を変えている
    public static object[] GetInstruction(List<List<PieceInfo>> AllPieceList, Board BoardInfo, int Turn, int PlayerNum)
    {
        List<object[]> AllInstructions = GetAllInstructions(AllPieceList[PlayerNum], BoardInfo, Turn, PlayerNum);

        if (AllInstructions == null || AllInstructions.Count == 0)
            return null;

        if (CPUNum.Contains(PlayerNum))
        {
            return Evaluate(AllInstructions, BoardInfo, PlayerNum, Turn);
        }
        if (Turn < ChangeTurnCount_EvaluateMethod)
            return Evaluate(AllInstructions, BoardInfo, PlayerNum, Turn);
        else
            return SimulationEvaluate(AllPieceList, AllInstructions, BoardInfo, PlayerNum, Turn);
    }

    // モンテカルロ法の始まり部分
    // 全ての手を見ると処理に時間がかかるので、ランダムに10手を評価。ExtractionCountで10を定義している
    // AllPieceList：全プレイヤーの残りピース　AllInstructions：全ての手
    // Boardクラス：　ボードの状態　PlayerNum：プレイヤー番号　Turn：ターン数
    static object[] SimulationEvaluate(List<List<PieceInfo>> AllPieceList, List<object[]> AllInstructions, Board BoardInfo, int PlayerNum, int Turn)
    {
        List<int> Scores = new List<int>();
        List<object[]> NewAllInstructions = new List<object[]>();

        if (AllInstructions.Count == 0)
            return null;
        AllInstructions = AllInstructions.OrderBy(i => Guid.NewGuid()).ToList();
        if (AllInstructions.Count > ExtractionCount)
        {
            for(int Index = 0; Index < ExtractionCount; Index++)
                NewAllInstructions.Add(AllInstructions[Index]);
        }
        else
            NewAllInstructions = AllInstructions;

        foreach (object[] Instruction in NewAllInstructions)
        {
            int PieceIndex = (int)Instruction[0];
            int IndexY = (int)Instruction[3];
            int IndexX = (int)Instruction[4];
            List<int[]> PieceDesign = (List<int[]>)Instruction[5];
            Board NewBoardInfo = new Board(BoardInfo);
            List<List<PieceInfo>> NewAllPieceList = CloneAllPieceList(AllPieceList);

            NewBoardInfo.SetPiece(PieceDesign, new int[]{IndexY, IndexX}, PlayerNum);
            NewAllPieceList[PlayerNum].Remove(NewAllPieceList[PlayerNum][PieceIndex]);

            Scores.Add(SimulationWhileGameFinish(NewAllPieceList, BoardInfo, PlayerNum, Turn));
        }
        //UnityEngine.Debug.Log(Scores.Max());
        
        return NewAllInstructions[Scores.IndexOf(Scores.Max())];
    }

    static List<List<PieceInfo>> CloneAllPieceList(List<List<PieceInfo>> AllPieceList)
    {
        List<List<PieceInfo>> NewAllPieceList = new List<List<PieceInfo>>();

        foreach(List<PieceInfo> PlayerPieceInfoList in AllPieceList)
        {
            List<PieceInfo> NewPieceList = new List<PieceInfo>();
            foreach(PieceInfo PieceInfo in PlayerPieceInfoList)
            {
                NewPieceList.Add(new PieceInfo(PieceInfo));
            }
            NewAllPieceList.Add(NewPieceList);
        }
        return NewAllPieceList;
    }

    // 勝敗が決まるまでランダムに手を実行する
    // 勝ち：+2　引き分け：+1　負け：+0　のスコアとする
    // AllInstructions = GetAllInstructions(...)は全ての手を取得する（Turnに応じてある程度手を捨てる）
    // MaxLoopCountがその手でのシミュレーション回数
    static int SimulationWhileGameFinish(List<List<PieceInfo>> AllPieceList, Board BoardInfo, int PlayerNum, int Turn)
    {
        int MyPlayerNum = PlayerNum;
        int Score = 0;

        PlayerNum = IncreasePlayerNum(PlayerNum);
        for (int LoopCount = 0; LoopCount < MaxLoopCount; LoopCount++)
        {
            Board NewBoardInfo = new Board(BoardInfo);
            List<List<PieceInfo>> NewAllPieceList = CloneAllPieceList(AllPieceList);

            while (true)
            {
                int PassPlayerCount = 0;
                for (int PlayerCount = 0; PlayerCount < MaxPlayer; PlayerCount++)
                {
                    Turn++;
                    if (Turn > 100)
                    break;

                    List<object[]> AllInstructions = GetRandomInstruct(NewAllPieceList[PlayerNum], NewBoardInfo, Turn, PlayerNum);
                    //List<object[]> AllInstructions = GetAllInstructions(NewAllPieceList[PlayerNum], NewBoardInfo, Turn, PlayerNum);

                    object[] Instruction;
                    if (AllInstructions.Count == 0)
                    {
                        Instruction = null;
                        PassPlayerCount++;
                    }
                    else
                    {
                        Instruction = AllInstructions[UnityEngine.Random.Range(0, AllInstructions.Count - 1)];
                        NewBoardInfo.SetPiece((List<int[]>)Instruction[5], new int[]{(int)Instruction[3], (int)Instruction[4]}, PlayerNum);
                        NewAllPieceList[PlayerNum].Remove(NewAllPieceList[PlayerNum][(int)Instruction[0]]);
                    }
                    PlayerNum = IncreasePlayerNum(PlayerNum);
                }
                if (PassPlayerCount == MaxPlayer)
                    break;
            }

            List<int> Winner = NewBoardInfo.CheckWinPlayer(); // そのボードの勝者をList<int>型で返す。同率の場合は要素数が2以上となる
            if (Winner.Count != 1)
                Score += 1;
            if (Winner.Contains(MyPlayerNum))
                Score += 2;
            else
                Score += 0;
        }
        return Score;
    }

    static int IncreasePlayerNum(int PlayerNum)
    {
        PlayerNum += 1;
        if (PlayerNum == MaxPlayer)
            PlayerNum = 0;
        return PlayerNum;
    }

    static List<object[]> GetRandomInstruct(List<PieceInfo> PieceList, Board BoardInfo, int Turn, int PlayerNum)
    {
        List<object[]> PossibleSetPieceList = new List<object[]>();
        List<int[]> PossibleSetPiecePointList = GetPossibleSetPiecePointList(BoardInfo, PlayerNum);
        PossibleSetPiecePointList = PossibleSetPiecePointList.OrderBy(i => Guid.NewGuid()).ToList();

        int Count = 0;
        var IndexList = Enumerable.Range(0, PieceList.Count - 1);
        IndexList = IndexList.OrderBy(i => Guid.NewGuid()).ToArray();
        foreach (int[] PossibleSetPiecePoint in PossibleSetPiecePointList)
        {
            foreach (int Index in IndexList)
            {
                PieceInfo CurrentPiece = new PieceInfo(PieceList[Index]);

                for(int ReverseCount = 0; ReverseCount < 2; ReverseCount++)
                {
                    for(int RotateCount = 0; RotateCount < 4; RotateCount++)
                    {
                        if (Count++ > 1000)
                        {
                            UnityEngine.Debug.Log(Count); //処理時間が長すぎることがあるから
                            return PossibleSetPieceList;
                        }
                        RandomInstruct_IsPossibleSetPiece
                        (
                            PossibleSetPieceList,
                            PossibleSetPiecePoint,
                            CurrentPiece,
                            Index,
                            BoardInfo,
                            ReverseCount,
                            RotateCount,
                            PlayerNum
                        );
                        CurrentPiece.Rotate(1);
                    }
                    CurrentPiece.Reverse();
                }
                if (PossibleSetPieceList.Count != 0)
                    return PossibleSetPieceList;
            }
        }
        return PossibleSetPieceList;
    }

    static void RandomInstruct_IsPossibleSetPiece(List<object[]> PossibleSetPieceList, int[] PossibleSetPiecePoint, PieceInfo CurrentPiece, int Index, Board BoardInfo, int ReverseCount, int RotateCount, int PlayerNum)
    {
        if (CurrentPiece.EffectiveList[ReverseCount, RotateCount] == 0)
        {
            return;
        }
        foreach(int[] PiecePoint in CurrentPiece.Design)
        {
            PieceInfo TmpCurrentPiece = new PieceInfo(CurrentPiece);

            TmpCurrentPiece.MovePivot(TmpCurrentPiece.Design, new int[]{-PiecePoint[0], -PiecePoint[1]});

            if (BoardInfo.IsPossibleSetPiece(TmpCurrentPiece.Design, PossibleSetPiecePoint, PlayerNum))
            {
                    PossibleSetPieceList.Add( new object[]
                    {
                        Index,
                        ReverseCount,
                        RotateCount,
                        PossibleSetPiecePoint[0] - PiecePoint[0],
                        PossibleSetPiecePoint[1] - PiecePoint[1],
                        PieceDesign.CopyPieceDesign(CurrentPiece.Design),
                    });
            }
        }
    }

    static List<int[]> GetPossibleSetPiecePointList(Board BoardInfo, int PlayerNum)
    {
        List<int[]> PossibleSetPiecePointList = new List<int[]>();;

        for (int IndexY = 0; IndexY < ConstList.BoardSize; IndexY++)
        {
            for (int IndexX = 0; IndexX < ConstList.BoardSize; IndexX++)
            {
                bool IsSpace = BoardInfo.IsPossibleSetPiece(new List<int[]>{new int[]{0, 0}}, new int[]{IndexY, IndexX}, PlayerNum);
                if (IsSpace)
                    PossibleSetPiecePointList.Add(new int[]{IndexY, IndexX});
            }
        }
        return PossibleSetPiecePointList;
    }

    static List<object[]> GetAllInstructions(List<PieceInfo> PieceList, Board BoardInfo, int Turn, int PlayerNum)
    {
        List<object[]> PossibleSetPieceList = new List<object[]>();
        int PieceIndex = -1;
        int PossibleSetPieceListCount = 0;

        foreach (PieceInfo Piece in PieceList)
        {
            PieceInfo CurrentPiece = new PieceInfo(Piece);
            
            PieceIndex++;
            if (SkipPieceCount(Piece.PieceCount, Turn, PossibleSetPieceListCount))
                continue ;
            for (int ReverseCount = 0; ReverseCount < 2; ReverseCount++)
            {
                for (int RotateCount = 0; RotateCount < 4; RotateCount++)
                {
                    GetAllInstructions_IsPossibleSetPiece
                    (
                        PossibleSetPieceList,
                        CurrentPiece,
                        BoardInfo,
                        PieceIndex,
                        ReverseCount,
                        RotateCount,
                        PlayerNum
                    );
                    CurrentPiece.Rotate(1);
                }
                CurrentPiece.Reverse();
            }
        }
        return PossibleSetPieceList;
    }

    static void GetAllInstructions_IsPossibleSetPiece(List<object[]> PossibleSetPieceList, PieceInfo CurrentPiece, Board BoardInfo, int PieceIndex, int ReverseCount, int RotateCount, int PlayerNum)
    {
        if (CurrentPiece.EffectiveList[ReverseCount, RotateCount] == 0)
            return;
        for (int IndexY = 0; IndexY < ConstList.BoardSize; IndexY++)
        {
            for (int IndexX = 0; IndexX < ConstList.BoardSize; IndexX++)
            {
                bool IsSpace = BoardInfo.IsPossibleSetPiece(CurrentPiece.Design, new int[]{IndexY, IndexX}, PlayerNum);
                if (IsSpace)
                {
                    PossibleSetPieceList.Add( new object[]
                    {
                        PieceIndex,
                        ReverseCount,
                        RotateCount,
                        IndexY,
                        IndexX,
                        PieceDesign.CopyPieceDesign(CurrentPiece.Design),
                    });
                }
            }
        }
    }

    static bool SkipPieceCount(int PieceCount, int Turn, int ListCount)
    {
        if (ListCount < 10)
            return false;
        int SkipTurn = 0;
        switch (PieceCount)
        {
            case 5:
                SkipTurn = MaxPlayer * CanUsePiece1;
                break;
            case 4:
                SkipTurn = MaxPlayer * CanUsePiece2;
                break;
            case 3:
                SkipTurn = MaxPlayer * CanUsePiece3;
                break;
            case 2:
                SkipTurn = MaxPlayer * CanUsePiece4;
                break;
            case 1:
                SkipTurn = MaxPlayer * CanUsePiece5;
                break;
        }
        if (SkipTurn >= Turn)
            return false;
        return true;
    }

    // Scoresの中に各指示でのスコアを格納
    // 格納したスコアの中から最大値の物をMaxScoresに格納
    // その中からランダムに実行
    // Instruction：全ての手、　BoardInfo：ボードの状態、　PlayerNum：誰ののターンか　Turn：ターン数
    public static object[] Evaluate(List<object[]> Instruntions, Board BoardInfo, int PlayerNum, int Turn)
    {
        List<int> Scores = new List<int>();
        Board TmpBoardInfo = new Board(BoardInfo);
        if (Instruntions == null || Instruntions.Count == 0)
            return null;

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
        int Index = MaxScores[UnityEngine.Random.Range(0, MaxScores.Count - 1)];
        return Instruntions[Index];
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
}
