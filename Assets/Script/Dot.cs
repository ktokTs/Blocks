using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour
{
    [SerializeField]
    Material[] Materials;

    Vector3 SpawnPoint;
    Vector3 DotPoint;
    int PlayerNum;
    const int MaxPlayer = 2;

    void Start()
    {
        SpawnPoint = new Vector3(-0.065f, 0.3f, 0.065f);
        DotPoint = this.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject NewPiece = Instantiate((GameObject)Resources.Load("P3-2"));
            NewPiece.transform.position = SpawnPoint;
            SetMaterialToChild(NewPiece, Materials[PlayerNum % 2]);
            if (++PlayerNum == MaxPlayer)
                PlayerNum = 0;
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

    void MoveSpawnPoint(Vector3 diff)
    {
        this.transform.position += diff;
        SpawnPoint += diff;
    }

    void SetMaterialToChild(GameObject Obj, Material Material)
    {
        foreach (Transform Child in Obj.transform)
        {
            Child.GetComponent<Renderer>().material = Material;
        }
    }
}
