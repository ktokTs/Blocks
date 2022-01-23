using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Vector3 WaitPoint{get; set;} //ピースの待機場所
    int RotateAngle;
    int ReverseAngle;
    public bool IsSet; //置かれた後のピースか判断 あまり必要ないかも
    public PieceInfo PieceInfo;
    void Start()
    {
        PieceInfo = new PieceInfo(PieceDesign.ReturnDesign(this.gameObject));
    }

    void Update()
    {
    }

    //ボードで設定されているピース待機場所に戻る
    public void ReturnWaitPoint()
    {
        PieceInfo.ReturnWaitPoint();
        RotateAngle = 0;
        ReverseAngle = 0;
        transform.rotation = Quaternion.AngleAxis(0, new Vector3(0, 1, 0));
        this.transform.position = WaitPoint;
    }

    //ピースの回転。反転中は回転方向を逆にしています。回転方向は引数の±で判断
    public void Rotate(int Dir)
    {
        PieceInfo.Rotate(Dir);
        //
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
        PieceInfo.Reverse();
        ReverseAngle += 180;
        Quaternion TmpQ;
        TmpQ = Quaternion.AngleAxis(RotateAngle, new Vector3(0, 1, 0));
        transform.rotation = Quaternion.AngleAxis(ReverseAngle, new Vector3(1, 0, 0)) * TmpQ;
        if (ReverseAngle >= 360)
            ReverseAngle = 0;
    }
}
