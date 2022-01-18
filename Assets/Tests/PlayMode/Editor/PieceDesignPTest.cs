using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PieceDesignPTest
{
    static object[] TestCaseCheckPieceDesign_PieceList = 
    new object[] {
        new object[]
        {
            true,
            "P3-2",
            new List<int[]>{new int[]{0, 0}, new int[]{1, 0}, new int[]{2, 0}},
        },
        new object[]
        {
            false,
            "P3-2",
            new List<int[]>{new int[]{0, 0}, new int[]{1, 0}, new int[]{3, 0}},
        },
        new object[]
        {
            true,
            "P4-3",
            new List<int[]>{new int[]{0, 0}, new int[]{1, 0}, new int[]{1, 1}, new int[]{0, 1}},
        },
        new object[]
        {
            true,
            "P5-6",
            new List<int[]>{new int[]{0, 0}, new int[]{1, 0}, new int[]{1, 1}, new int[]{1, -1}, new int[]{2, 1}},
        },
    };

    [UnityTest]
    public IEnumerator PieceDesignPTestCheckPieceDesign_PieceList()
    {
        foreach (object[] TestCase in TestCaseCheckPieceDesign_PieceList)
        {
            Debug.Log("TestCase" + (string)TestCase[1]);

            GameObject NewPiece = Object.Instantiate((GameObject)Resources.Load((string)TestCase[1]));
            NewPiece.transform.position = new Vector3(0f, 0.3f, 0f);
            yield return null;
            Piece Script = NewPiece.GetComponent<Piece>();
            
            Debug.Log("Ex");
            DebugLogPieceList((List<int[]>)TestCase[2]);
            Debug.Log("Act");
            DebugLogPieceList(Script.Design);

            Assert.AreEqual((bool)TestCase[0], Utils.ComparePiecePointList((List<int[]>)TestCase[2], Script.Design));
            yield return null;
        }
    }

    void DebugLogPieceList(List<int[]> Array)
    {
        foreach (int[] A in Array)
        {
            Debug.Log(A[0] + " " + A[1]);
        }
    }
}
