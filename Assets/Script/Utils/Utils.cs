using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utils
{
    // 配列の中身が順不同で同じか確認
    public static bool CompareArray<T>(T[] Ex, T[] Act) where T : IComparable
    {
        if (Ex.Length != Act.Length)
            return false;
        int Len = Ex.Length;
        Array.Sort(Ex);
        Array.Sort(Act);
        for (; Len > 0; Len--)
        {
            if (!Ex[Len - 1].Equals(Act[Len - 1]))
                return false;
        }
        return true;
    }

    // ピースの位置が順不同で同じか確認
    public static bool ComparePiecePointList(List<int[]> Ex, List<int[]> Act)
    {
        if (Ex.Count != Act.Count)
            return false;
        if (CheckPiecePointDuplication(Act) || CheckPiecePointDuplication(Ex))
            return false;
        int Len = Ex.Count;
        foreach (int[] a in Act)
        {
            bool Res = false;
            foreach (int[] b in Ex)
            {
                if (ComparePiecePoint(a, b))
                    Res = true;
            }
            if (Res == false)
                return false;
        }
        return true;
    }

    // 配列の2つ目までの要素が等しいか
    public static bool ComparePiecePoint<T>(T[] Ex, T[] Act) where T : IComparable
    {
        if (Ex[0].Equals(Act[0]))
        {
            if (Ex[1].Equals(Act[1]))
                return true;
        }
        return false;
    }

    // ピースの重複が発生していないか
    public static bool CheckPiecePointDuplication(List<int[]> Array)
    {
        int IndexA = 0;
        foreach (int[] A in Array)
        {
            int IndexB = -1;
            foreach (int[] B in Array)
            {
                IndexB++;
                if (IndexA >= IndexB)
                    continue;
                if (ComparePiecePoint(A,B))
                    return true;
            }
            IndexA++;
        }
        return false;
    }

    // Pointがボードの中に入っているか
    public static bool IsPieceInBoard(int[] Point)
    {
        int PointX = Point[0];
        int PointY = Point[1];
        if ((0 <= PointX && PointX < ConstList.BoardSize) &&
        (0 <= PointY && PointY < ConstList.BoardSize))
            return true;
        return false;
    }
}
