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
    int PieceCount = 0;
    GameObject ControlPiece; //操作中のピース

    //スタート時に動く関数
    void Start()
    {
        AllPieceList = new List<List<GameObject>>();
        SpawnPoint = new Vector3(-0.065f, 0.05f, 0.065f);
        DotPoint = this.transform.position;
        CreatePieces(new Vector3(0.12f, 0.1f, 0.15f), 0);
        CreatePieces(new Vector3(-0.36f, 0.1f, 0.15f), 1);
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
        if (Input.GetKeyDown(KeyCode.W))
            MoveSpawnPoint(new Vector3(0f, 0f, 0.01f));
        if (Input.GetKeyDown(KeyCode.S))
            MoveSpawnPoint(new Vector3(0f, 0f, -0.01f));
        if (Input.GetKeyDown(KeyCode.D))
            MoveSpawnPoint(new Vector3(0.01f, 0f, 0f));
        if (Input.GetKeyDown(KeyCode.A))
            MoveSpawnPoint(new Vector3(-0.01f, 0f, 0f));
    }

    //ピースを置く。重力はオンにする。
    //置いたピースはAllPieceListから削除する
    void SetPiece()
    {
        if (ControlPiece == null)
            return;
        ControlPiece.GetComponent<Rigidbody>().useGravity  = true;
        ControlPiece.GetComponent<Piece>().IsSet = true;
        AllPieceList[PlayerNum].Remove(ControlPiece);
        IncreasePlayerNum();
        ControlPiece = null;
        PieceCount = 1;
        ChangeControlPiece(-1);
    }

    //操作するピースを変更する。引数で次のピースか前のピースか決めている。
    void ChangeControlPiece(int Dir)
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

    void IncreasePlayerNum()
    {
        PlayerNum += 1;
        if (PlayerNum == MaxPlayer)
            PlayerNum = 0;
    }

    void MoveSpawnPoint(Vector3 diff)
    {
        if (ControlPiece == null)
            return ;
        this.transform.position += diff;
        SpawnPoint += diff;
        ControlPiece.transform.position = this.SpawnPoint;
    }

    void SetMaterialToChild(GameObject Obj, Material Material)
    {
        foreach (Transform Child in Obj.transform)
        {
            Child.GetComponent<Renderer>().material = Material;
        }
    }
}
