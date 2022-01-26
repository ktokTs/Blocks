using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceInfo
{
    // 待機場所設定
    public List<int[]> StartDesign{get; set;}
    public List<int[]> Design{get; set;}
    public int PieceCount{get; set;}

    // 反転・回転において形状が代わり、再計算が必要かを示す
    // [反転数,回転数]で定義。1の場合は再計算が必要
    int[,] effectiveList = new int[2, 4]{{1, 0, 0, 0}, {0, 0, 0, 0}};
    public int[,] EffectiveList
    {
        get {return effectiveList;}
        private set {effectiveList = value;}
    }

    public PieceInfo(List<int[]> Design)
    {
        this.Design = Design;
        StartDesign = CopyPieceDesign();
        PieceCount = Design.Count;
        SetEffectiveList();
    }

    public PieceInfo(PieceInfo PieceInfo)
    {
        this.Design = PieceInfo.CopyPieceDesign();
        StartDesign = CopyPieceDesign();
        PieceCount = PieceInfo.PieceCount;
        this.EffectiveList = PieceInfo.EffectiveList;
    }

    // 反転・回転で再計算が必要かを確認する
    // 回転後の状態を見ることを考えると、回転前のピースの各位置に基点を移動させ、回転前の
    // リストの位置が完全に一致するかを見ている
    void SetEffectiveList()
    {
        List<List<int[]>> EffectivePieceDesignList = new List<List<int[]>>();

        for (int ReverseCount = 0; ReverseCount < 2; ReverseCount++)
        {
            for (int RotateCount = 0; RotateCount < 4; RotateCount++)
            {
                if (!IsEffectiveDesign(Design, EffectivePieceDesignList))
                    EffectiveList[ReverseCount, RotateCount] = 0;
                else
                {
                    EffectiveList[ReverseCount, RotateCount] = 1;
                    EffectivePieceDesignList.Add(CopyPieceDesign());
                }
                Rotate(1);
            }
            Reverse();
        }
    }

    bool IsEffectiveDesign(List<int[]> Design, List<List<int[]>> EffectivePieceDesignList)
    {
        bool IsEffective = true;

        if (EffectivePieceDesignList.Count == 0)
            return true;

        foreach (List<int[]> EffectivePieceDesign in EffectivePieceDesignList)
        {
            foreach(int[] Pivot in EffectivePieceDesign)
            {
                List<int[]> TmpPieceDesign = CopyPieceDesign();
                MovePivot(TmpPieceDesign, Pivot);
                if (Utils.ComparePiecePointList(EffectivePieceDesign, TmpPieceDesign))
                {
                    IsEffective = false;
                    break;
                }
            }
            if (!IsEffective)
                break;
        }
        return IsEffective;
    }

    // ピースの基点を変更する
    public void MovePivot(List<int[]> Design, int[] Pivot)
    {
        foreach (int[] Point in Design)
        {
            Point[0] += Pivot[0];
            Point[1] += Pivot[1];
        }
    }

    //Designの配列のディープコピーを作成
    public List<int[]> CopyPieceDesign()
    {
        List<int[]> Res = new List<int[]>();
        foreach (int[] Piece in Design)
        {
            int Len = Piece.Length;
            int[] TmpPoint = new int[Len];
            for (int Index = 0; Index < Len; Index++)
            {
                TmpPoint[Index] = Piece[Index];
            }
            Res.Add(TmpPoint);
        }
        return Res;
    }

    public void ReturnWaitPoint()
    {
        Design = PieceDesign.CopyPieceDesign(StartDesign);
    }

    public void Rotate(int Dir)
    {
        PieceDesign.Rotate(Design, Dir);
    }

    public void Reverse()
    {
        PieceDesign.Reverse(Design);
    }

    //デバッグ用
    public void DebugLogPieceList()
    {
        Debug.Log("====");
        foreach (int[] A in Design)
        {
            Debug.Log(A[0] + " " + A[1]);
        }
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
