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
        new object[] {
            new object[] {1, 'r',},
            new object[] {1, ' ',},
            new object[] {10, 'd',},
            new object[] {13, 's',},
            new object[] {1, ' ',},
        },
        new object[]
        {
            true,
            true,
        }
    };

    [UnityTest]
    public IEnumerator PieceDesignPTestCheckPieceDesign_PieceList()
    {
        //foreach (object[] TestCase in TestCaseCheckPieceDesign_PieceList)
        //{
            object[] TestCase = TestCaseCheckPieceDesign_PieceList;
            SceneManager.LoadScene("Game");
            yield return null;
            PieceController PCS = GameObject.Find("Dot").GetComponent<PieceController>();
            object[] ExecList = (object[])TestCase[0];
            object[] ExBoolList = (object[])TestCase[1];

            int ExBoolIndex = 0;
            foreach (object[] Instruction in ExecList)
            {
                int LoopCount = (int)Instruction[0];
                char ExecChar = (char)Instruction[1];
                
                for (int Count = 0; Count < LoopCount; Count++)
                {
                    bool Res = Exec(ExecChar, PCS);
                    if (ExecChar == ' ')
                        Assert.AreEqual((bool)ExBoolList[ExBoolIndex++], Res);
                }
            }

            yield return null;
        //}
    }

    bool Exec(char Char, PieceController PCS)
    {
        Debug.Log("|" + Char + "|");
        if (Char == 'f')
            PCS.ChangeControlPiece(1);
        if (Char == 'g')
            PCS.ChangeControlPiece(-1);
        if (Char == ' ')
        {
            return PCS.SetPiece();
        }
        if (Char == 'r')
            PCS.PieceReverse();
        if (Char == 'e')
            PCS.PieceRotate(1);
        if (Char == 'q')
            PCS.PieceRotate(-1);
        if (Char == 'w')
            PCS.MoveSpawnPoint(new Vector3(0f, 0f, 0.01f));
        if (Char == 's')
            PCS.MoveSpawnPoint(new Vector3(0f, 0f, -0.01f));
        if (Char == 'd')
            PCS.MoveSpawnPoint(new Vector3(0.01f, 0f, 0f));
        if (Char == 'a')
            PCS.MoveSpawnPoint(new Vector3(-0.01f, 0f, 0f));
        if (Char == 'z')
            PCS.ShowPieceInfo();
        return false;
    }
}
