using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class PlayTest
{
    static object[] TestCaseCheckPieceDesign_PieceList = 
    new object[] {
        new object[]
        {
            true,
            "P3-2",
            new List<int[]>{new int[]{0, 0}, new int[]{1, 0}, new int[]{2, 0}},
        },
    };

    [UnityTest]
    public IEnumerator PieceDesignPTestCheckPieceDesign_PieceList()
    {
        SceneManager.LoadScene("Game");
        yield return null;

        PieceController PCS = GameObject.Find("Dot").GetComponent<PieceController>();
        bool Res = PCS.SetPiece();
        Assert.AreEqual(false, Res);
        PCS.MoveSpawnPoint(new Vector3(0f, 0f, -0.01f));
        Res = PCS.SetPiece();
        Assert.AreEqual(true, Res);
        Res = PCS.SetPiece();
        Assert.AreEqual(false, Res);


        PCS.MoveSpawnPoint(new Vector3(0f, 0f, -0.01f));
        PCS.MoveSpawnPoint(new Vector3(0f, 0f, -0.01f));
        PCS.MoveSpawnPoint(new Vector3(0f, 0f, -0.01f));
        PCS.MoveSpawnPoint(new Vector3(0f, 0f, -0.01f));
        Res = PCS.SetPiece();
        Assert.AreEqual(true, Res);

        yield return null;
    }
}
