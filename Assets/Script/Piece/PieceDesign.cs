using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PieceDesign
{
    //基点としているピースの名前はPieceとなるようにしている。それ以外は(Clone)がついている
    public static List<int[]> ReturnDesign(GameObject ParentObj)
    {
        Vector3 Pivot = Vector3.zero;
        List<int[]> res = new List<int[]>();
        res.Add(new int[]{0,0});
        foreach (Transform ChildObj in ParentObj.transform)
        {
            if (ChildObj.gameObject.name == "Piece")
                Pivot = ChildObj.transform.position;
                break;
        }
        foreach (Transform ChildObj in ParentObj.transform)
        {
            if (ChildObj.gameObject.name == "Piece")
                continue;
            Vector3 Pos = (ChildObj.transform.position - Pivot) * 100;
            int[] ChildPoint = new int[2];
            ChildPoint[0] = -(int)(Mathf.Round(Pos.z));
            ChildPoint[1] = (int)(Mathf.Round(Pos.x));
            res.Add(ChildPoint);
        }
        return res;
    }

    // ピースの反転
    public static List<int[]> Reverse(List<int[]> Design)
    {
        foreach (int[] Piece in Design)
        {
            Piece[0] = -Piece[0];
        }
        return Design;
    }

    // ピースの回転。Dirで回転方向を決める
    public static List<int[]> Rotate(List<int[]> Design, int Dir)
    {
        foreach (int[] Piece in Design)
        {
            int[] Tmp = new int[]{Piece[0], Piece[1]};
            Piece[1] = -1 * Dir *Tmp[0];
            Piece[0] = 1 * Dir *Tmp[1];
        }
        return Design;
    }

    // ピースの形のDeepCopyを作成
    public static List<int[]> CopyPieceDesign(List<int[]> Design)
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
}
