using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceController : MonoBehaviour
{
    [SerializeField]
    Material[] Materials;

    Vector3 SpawnPoint;
    Vector3 DotPoint;
    int PlayerNum;
    const int MaxPlayer = 2;
    List<List<GameObject>> AllPieceList; //ピースオブジェクトのリスト
    int PieceCount = 1;
    GameObject ControlPiece; //操作中のピース
    Board BoardScript;
    int[] PiecePivot; //現在操作中のピースの原点の位置

    //スタート時に動く関数
    void Start()
    {
        BoardScript = GameObject.Find("Board").GetComponent<Board>();
        PiecePivot = new int[]{0, 0};
        AllPieceList = new List<List<GameObject>>();
        SpawnPoint = new Vector3(-0.065f, 0.05f, 0.065f);
        DotPoint = this.transform.position;
        CreatePieces(new Vector3(0.12f, 0.1f, 0.15f), 0);
        CreatePieces(new Vector3(-0.36f, 0.1f, 0.15f), 1);
        ChangeControlPiece(-1);
    }

    //プレイヤー分のピースを生成する。初期状態では重力無し
    //AllPieceListに生成したピースのオブジェクトを保管しておく
    //PieceWaitPoint：ピースの生成位置。基本的にここでピースは待機する
    void CreatePieces(Vector3 PieceWaitPoint, int MaterialNum)
    {
        List<GameObject> PlayerPieceList = new List<GameObject>();
        foreach(string PieceName in ConstList.PieceList)
        {
            GameObject NewPiece = Instantiate((GameObject)Resources.Load(PieceName));
            NewPiece.GetComponent<Rigidbody>().useGravity  = false;
            NewPiece.transform.position = PieceWaitPoint;
            NewPiece.GetComponent<Piece>().WaitPoint = PieceWaitPoint;
            PlayerPieceList.Add(NewPiece);
            PieceWaitPoint = StepPieceWaitPoint(PieceWaitPoint);
            SetMaterialToChild(NewPiece, Materials[MaterialNum]);
        }
        AllPieceList.Add(PlayerPieceList);
    }

    //ピース生成中、ピース間の距離分移動
    Vector3 StepPieceWaitPoint(Vector3 WaitPoint)
    {
        WaitPoint += new Vector3(0f, 0f, -0.06f);
        if (WaitPoint.z < -0.15f)
        {
            WaitPoint.x += 0.06f;
            WaitPoint.z = 0.15f;
        }
        return WaitPoint;
    }

    //F,Gでピース変更。スペースでピース設置
    //WASDでピース移動
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ChangeControlPiece(1);
        if (Input.GetKeyDown(KeyCode.G))
            ChangeControlPiece(-1);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetPiece();
        }
        if (Input.GetKeyDown(KeyCode.R))
            PieceReverse();
        if (Input.GetKeyDown(KeyCode.E))
            PieceRotate(1);
        if (Input.GetKeyDown(KeyCode.Q))
            PieceRotate(-1);
        if (Input.GetKeyDown(KeyCode.W))
            MoveSpawnPoint(new Vector3(0f, 0f, 0.01f));
        if (Input.GetKeyDown(KeyCode.S))
            MoveSpawnPoint(new Vector3(0f, 0f, -0.01f));
        if (Input.GetKeyDown(KeyCode.D))
            MoveSpawnPoint(new Vector3(0.01f, 0f, 0f));
        if (Input.GetKeyDown(KeyCode.A))
            MoveSpawnPoint(new Vector3(-0.01f, 0f, 0f));
        if (Input.GetKeyDown(KeyCode.Z))
            ShowPieceInfo();
        if (Input.GetKeyDown(KeyCode.X))
            IsPossibleAnySetPiece();
        if (Input.GetKeyDown(KeyCode.C))
        {
            object[] Instruction = IsPossibleAnySetPiece();
            if (Instruction == null)
                return;
            ExecInstruction(Instruction);
        }
    }

    //ピースを置く。重力はオンにする。
    //置いたピースはAllPieceListから削除する
    //PieceCountはリセット。その後ChangeControlPieceで一番最初のピースを持たせる
    public bool SetPiece()
    {
        Piece ControlPieceScript;
        if (ControlPiece == null)
            return false;
        ControlPieceScript = ControlPiece.GetComponent<Piece>();
        bool IsSpace = BoardScript.IsPossibleSetPiece(ControlPieceScript.Design, PiecePivot, PlayerNum);
        if (!IsSpace)
            return false;
        BoardScript.SetPiece(ControlPieceScript.Design, PiecePivot, PlayerNum);

        ControlPiece.GetComponent<Rigidbody>().useGravity  = true;
        ControlPieceScript.IsSet = true;
        AllPieceList[PlayerNum].Remove(ControlPiece);
        IncreasePlayerNum();
        ControlPiece = null;
        PieceCount = 1;
        ChangeControlPiece(-1);
        return true;
    }

    //操作するピースを変更する。引数で次のピースか前のピースか決めている。
    //操作から離れたピースは最初の生成位置に戻る
    public void ChangeControlPiece(int Dir)
    {
        int MaxPiece = (AllPieceList[PlayerNum].Count);
        if (MaxPiece == 0)
            return;
        if (ControlPiece != null)
            ControlPiece.GetComponent<Piece>().ReturnWaitPoint();
        PieceCount += Dir;
        if (PieceCount >= MaxPiece)
            PieceCount = 0;
        if (PieceCount <= -1)
            PieceCount = MaxPiece - 1;
        ControlPiece = AllPieceList[PlayerNum][PieceCount];
        ControlPiece.transform.position = this.SpawnPoint;
    }

    public void IncreasePlayerNum()
    {
        PlayerNum += 1;
        if (PlayerNum == MaxPlayer)
            PlayerNum = 0;
    }

    public void PieceReverse()
    {
        ControlPiece.GetComponent<Piece>().Reverse();
    }

    public void PieceRotate(int Dir)
    {
        ControlPiece.GetComponent<Piece>().Rotate(Dir);
    }

    public void MoveSpawnPoint(Vector3 diff)
    {
        if (ControlPiece == null)
            return ;

        int PointY = PiecePivot[0] - (int)(Mathf.Round(diff.z * 100));
        int PointX = PiecePivot[1] + (int)(Mathf.Round(diff.x * 100));
        if (PointX < 0 || ConstList.BoardSize <= PointX || 
        PointY < 0 || ConstList.BoardSize <= PointY)
            return;
        this.transform.position += diff;
        SpawnPoint += diff;
        diff *= 100;
        PiecePivot[0] -= (int)(Mathf.Round(diff.z));
        PiecePivot[1] += (int)(Mathf.Round(diff.x));
        ControlPiece.transform.position = this.SpawnPoint;
    }

    public void ShowPieceInfo()
    {
        ControlPiece.GetComponent<Piece>().DebugLogPieceList();
    }

    public void SetMaterialToChild(GameObject Obj, Material Material)
    {
        foreach (Transform Child in Obj.transform)
        {
            Child.GetComponent<Renderer>().material = Material;
        }
    }

    //置けるピースがあるか検索を行う。
    //ピースの種類(先頭から何番目のピースか)
    // 反転
    //  回転
    //   Y軸の位置
    //    X軸の位置
    // の順でループを行っている。置ける場合にはループの情報を引き渡す
    public object[] IsPossibleAnySetPiece()
    {
        List<GameObject> PieceList = AllPieceList[PlayerNum];

        int PieceIndex = 0;
        foreach (GameObject Piece in PieceList)
        {
            Piece ControlPieceScript = Piece.GetComponent<Piece>();
            List<int[]> CurrentPieceDesign = ControlPieceScript.CopyPieceDesign(ControlPieceScript.Design);
            for (int ReverseCount = 0; ReverseCount < 2; ReverseCount++)
            {
                for (int RotateCount = 0; RotateCount < 4; RotateCount++)
                {
                    for (int IndexY = 0; IndexY < ConstList.BoardSize; IndexY++)
                    {
                        for (int IndexX = 0; IndexX < ConstList.BoardSize; IndexX++)
                        {
                            bool IsSpace = BoardScript.IsPossibleSetPiece(CurrentPieceDesign, new int[]{IndexY, IndexX}, PlayerNum);
                            if (IsSpace)
                            {
                                return new object[]
                                {
                                    PieceIndex,
                                    ReverseCount,
                                    RotateCount,
                                    IndexY,
                                    IndexX,
                                };
                            }
                        }
                    }
                    PieceDesign.Rotate(CurrentPieceDesign, 1);
                }
                PieceDesign.Reverse(CurrentPieceDesign);
            }
            PieceIndex++;
        }
        return null;
    }

    //一旦原点まで基点を戻し、Instructionの指示に沿って回転や移動を行い、SetPieceを行う。
    void ExecInstruction(object[] Instruction)
    {
        int PieceIndex = (int)Instruction[0];
        int ReverseCount = (int)Instruction[1];
        int RotateCount = (int)Instruction[2];
        int IndexY = (int)Instruction[3];
        int IndexX = (int)Instruction[4];

        /* Debug.Log
        (
            PieceIndex + "\n" + 
            "ReverseCount = " + ReverseCount + "\n" + 
            "RotateCount = " + RotateCount + "\n" + 
            IndexY + ", " + IndexX
        ); */
        for (int Count = 0; Count < ConstList.BoardSize; Count++)
            MoveSpawnPoint(new Vector3(0f, 0f, 0.01f));
        for (int Count = 0; Count < ConstList.BoardSize; Count++)
            MoveSpawnPoint(new Vector3(-0.01f, 0f, 0f));
        for (int Count = 0; Count < PieceIndex; Count++)
            ChangeControlPiece(1);
        for (int Count = 0; Count < ReverseCount; Count++)
            PieceReverse();
        for (int Count = 0; Count < RotateCount; Count++)
            PieceRotate(1);
        for (int Count = 0; Count < IndexY; Count++)
            MoveSpawnPoint(new Vector3(0f, 0f, -0.01f));
        for (int Count = 0; Count < IndexX; Count++)
            MoveSpawnPoint(new Vector3(0.01f, 0f, 0f));
        SetPiece();
    }
}
