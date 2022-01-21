using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public List<int[]> Design{get; set;}
    public Vector3 WaitPoint{get; set;} //ピースの待機場所
    List<int[]> StartDesign;
    int RotateAngle;
    int ReverseAngle;
    public bool IsSet; //置かれた後のピースか判断 あまり必要ないかも
    void Start()
    {
        Design = PieceDesign.ReturnDesign(this.gameObject);
        StartDesign = CopyPieceDesign(Design);
    }

    void Update()
    {
    }

    //ボードで設定されているピース待機場所に戻る
    public void ReturnWaitPoint()
    {
        DebugLogPieceList(StartDesign);
        Design = CopyPieceDesign(StartDesign);
        RotateAngle = 0;
        ReverseAngle = 0;
        transform.rotation = Quaternion.AngleAxis(0, new Vector3(0, 1, 0));
        this.transform.position = WaitPoint;
    }

    //ピースの回転。反転中は回転方向を逆にしています。回転方向は引数の±で判断
    public void Rotate(int Dir)
    {
        PieceDesign.Rotate(Design, Dir);
        if (ReverseAngle != 0)
            Dir *= -1;
        RotateAngle += 90 * Dir;
        Quaternion TmpQ;
        TmpQ = Quaternion.AngleAxis(RotateAngle, new Vector3(0, 1, 0));
        transform.rotation = Quaternion.AngleAxis(ReverseAngle, new Vector3(1, 0, 0)) * TmpQ;
        if (RotateAngle >= 360 || RotateAngle <= -360)
            RotateAngle = 0;
    }

    //ピースの反転
    public void Reverse()
    {
        PieceDesign.Reverse(Design);
        ReverseAngle += 180;
        Quaternion TmpQ;
        TmpQ = Quaternion.AngleAxis(RotateAngle, new Vector3(0, 1, 0));
        transform.rotation = Quaternion.AngleAxis(ReverseAngle, new Vector3(1, 0, 0)) * TmpQ;
        if (ReverseAngle >= 360)
            ReverseAngle = 0;
    }

    //Designの配列のディープコピーを作成
    public List<int[]> CopyPieceDesign(List<int[]> Design)
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

    //デバッグ用
    public void DebugLogPieceList()
    {
        Debug.Log("====");
        foreach (int[] A in Design)
        {
            Debug.Log(A[0] + " " + A[1]);
        }
    }
    public void DebugLogPieceList(List<int[]> Design)
    {
        Debug.Log("====");
        foreach (int[] A in Design)
        {
            Debug.Log(A[0] + " " + A[1]);
        }
    }
}
