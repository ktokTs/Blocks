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
    List<int> FinishPlayerList;
    bool IsEndGame;
    int Turn = 0;

    //スタート時に動く関数
    void Start()
    {
        BoardScript = new Board();
        PiecePivot = new int[]{0, 0};
        AllPieceList = new List<List<GameObject>>();
        SpawnPoint = new Vector3(-0.065f, 0.05f, 0.065f);
        DotPoint = this.transform.position;
        CreatePieces(new Vector3(0.12f, 0.1f, 0.15f), 0);
        CreatePieces(new Vector3(-0.36f, 0.1f, 0.15f), 1);
        ChangeControlPiece(-1);
        FinishPlayerList = new List<int>();
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
        if (IsEndGame)
            return ;
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
//        if (Input.GetKeyDown(KeyCode.X))
//            NPC.IsPossibleAnySetPiece();
        if (Input.GetKeyDown(KeyCode.C))
        {
            object[] Instruction = NPC.GetInstruction(AllPieceList, BoardScript, Turn, PlayerNum);
            if (Instruction == null)
            {
                Pass();
                return ;
            }
            ExecInstruction(Instruction);
        }
        if (Input.GetKeyDown(KeyCode.P))
            Pass();
        if (Input.GetKeyDown(KeyCode.V))
            BoardScript.PrintBoardInfo(BoardScript.BoardInfo);
        if (Input.GetKeyDown(KeyCode.O))
            WhileFinishGamePlayCPU();
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
        bool IsSpace = BoardScript.IsPossibleSetPiece(ControlPieceScript.PieceInfo.Design, PiecePivot, PlayerNum);
        if (!IsSpace)
            return false;
        BoardScript.SetPiece(ControlPieceScript.PieceInfo.Design, PiecePivot, PlayerNum);

        ControlPiece.transform.position = this.SpawnPoint + new Vector3(0f, -0.01f, 0);
        ControlPiece.GetComponent<Rigidbody>().useGravity  = true;
        ControlPieceScript.IsSet = true;
        AllPieceList[PlayerNum].Remove(ControlPiece);
        ControlPiece = null;
        ChangePlayer();
        return true;
    }

    void ChangePlayer()
    {
        Turn++;
        if (ControlPiece != null)
            ControlPiece.GetComponent<Piece>().ReturnWaitPoint();
        if (FinishPlayerList.Count == MaxPlayer)
        {
            EndGame();
            return ;
        }
        IncreasePlayerNum();
        ControlPiece = null;
        if (FinishPlayerList.Contains(PlayerNum))
            ChangePlayer();
        PieceCount = 1;
        ChangeControlPiece(-1);
    }

    void EndGame()
    {
        List<int> Winner = new List<int>();
        Winner = BoardScript.CheckWinPlayer();

        if (Winner.Count != 1)
            Debug.Log("Draw");
        else
            Debug.Log(Winner[0] + " Win");
        
/*      List<int> PlayerScore = new List<int>();
        PlayerNum = 1;
        foreach (List<GameObject> PlayerPiece in AllPieceList)
        {
            int Score = 0;
            foreach (GameObject Piece in PlayerPiece)
            {
                Score += Piece.GetComponent<Piece>().PieceInfo.PieceCount;
            }
            Debug.Log("Player" + PlayerNum + ": " + Score);
            PlayerScore.Add(Score);
            PlayerNum++;
        } */
        IsEndGame = true;
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
        ControlPiece.GetComponent<Piece>().PieceInfo.DebugLogPieceList();
    }

    public void SetMaterialToChild(GameObject Obj, Material Material)
    {
        foreach (Transform Child in Obj.transform)
        {
            Child.GetComponent<Renderer>().material = Material;
        }
    }

/*     //置けるピースがあるか検索を行う。
    //ピースの種類(先頭から何番目のピースか)
    // 反転
    //  回転
    //   Y軸の位置
    //    X軸の位置
    // の順でループを行っている。置ける場合にはループの情報を引き渡す
    public List<object[]> IsPossibleAnySetPiece()
    {
        List<GameObject> PieceList = AllPieceList[PlayerNum];
        List<object[]> PossibleSetPieceList = new List<object[]>();

        int PieceIndex = -1;
        int PossibleSetPieceListCount = 0;
        foreach (GameObject Piece in PieceList)
        {
            Piece ControlPieceScript = Piece.GetComponent<Piece>();
            List<int[]> CurrentPieceDesign = ControlPieceScript.PieceInfo.CopyPieceDesign();
            
            PieceIndex++;
            if (SkipPieceCount(ControlPieceScript.PieceInfo.PieceCount, Turn, PossibleSetPieceListCount))
                continue ;
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
            Debug.Log(Piece.name + " " + PieceIndex);
        }
        return PossibleSetPieceList;
    }

    bool SkipPieceCount(int PieceCount, int Turn, int ListCount)
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
    } */

    //一旦原点まで基点を戻し、Instructionの指示に沿って回転や移動を行い、SetPieceを行う。
    void ExecInstruction(object[] Instruction)
    {
        int PieceIndex = (int)Instruction[0];
        int ReverseCount = (int)Instruction[1];
        int RotateCount = (int)Instruction[2];
        int IndexY = (int)Instruction[3];
        int IndexX = (int)Instruction[4];
        List<int[]> PieceDesign = (List<int[]>)Instruction[5];

/*         Debug.Log
        (
            PieceIndex + "\n" + 
            "ReverseCount = " + ReverseCount + "\n" + 
            "RotateCount = " + RotateCount + "\n" + 
            IndexY + ", " + IndexX
        );
        PieceInfo.DebugLogPieceList(PieceDesign); */
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
        if (!SetPiece())
        {
            Debug.Log("Error !!");
            IsEndGame = true;
        }
    }

    void Pass()
    {
        FinishPlayerList.Add(PlayerNum);
        //Debug.Log("Add " + PlayerNum);
        ChangePlayer();
    }

    void WhileFinishGamePlayCPU()
    {
        while(!IsEndGame)
        {
            object[] Instruction = NPC.GetInstruction(AllPieceList, BoardScript, Turn, PlayerNum);
            if (Instruction == null)
            {
                Pass();
                continue ;
            }
            ExecInstruction(Instruction);
        }
    }
}
