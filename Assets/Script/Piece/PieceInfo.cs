using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceInfo
{
    public List<int[]> StartDesign;
    public List<int[]> Design{get; set;}
    public int PieceCount;
    // Start is called before the first frame update
    public PieceInfo(List<int[]> Design)
    {
        this.Design = Design;
        StartDesign = CopyPieceDesign();
        PieceCount = Design.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Design = CopyPieceDesign();
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
