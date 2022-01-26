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
    List<int> FinishPlayerList; // ピースを置けなくなったプレイヤーリスト
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

    // F,Gでピース変更。スペースでピース設置
    // WASDでピース移動
    // E,Qで回転
    // Rで反転
    // Pでパス
    // CでCPUに行わせる
    // Oで最後までCPUに行わせる
    void Update()
    {
        if (IsEndGame)
            return ;
        if (Input.GetKeyDown(KeyCode.F))
            ChangeControlPiece(1);
        if (Input.GetKeyDown(KeyCode.G))
            ChangeControlPiece(-1);
        if (Input.GetKeyDown(KeyCode.Space))
            SetPiece();
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

    // プレイヤーを変更する。ついでに終了判定も行ってます。
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

    // ゲームの勝者判定。判定後はIsEndGameをTrueにして操作を受け付けなくする
    void EndGame()
    {
        List<int> Winner = new List<int>();
        Winner = BoardScript.CheckWinPlayer();

        if (Winner.Count != 1)
            Debug.Log("Draw");
        else
            Debug.Log(Winner[0] + " Win");
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

    // ピースの位置を移動させる。
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

    // 親のオブジェクト配下のオブジェクトのマテリアルを変更する
    public void SetMaterialToChild(GameObject Obj, Material Material)
    {
        foreach (Transform Child in Obj.transform)
        {
            Child.GetComponent<Renderer>().material = Material;
        }
    }

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
        for (; PieceCount != 0;)
            ChangeControlPiece(1);

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
