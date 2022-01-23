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
            new List<int[]>{new int[]{0, 0}, new int[]{0, 1}, new int[]{0, 2}},
        },
        new object[]
        {
            false,
            "P3-2",
            new List<int[]>{new int[]{0, 0}, new int[]{0, 1}, new int[]{0, 3}},
        },
        new object[]
        {
            true,
            "P4-3",
            new List<int[]>{new int[]{0, 0}, new int[]{-1, 0}, new int[]{-1, 1}, new int[]{0, 1}},
        },
        new object[]
        {
            true,
            "P5-6",
            new List<int[]>{new int[]{0, 0}, new int[]{0, 1}, new int[]{-1, 1}, new int[]{1, 1}, new int[]{-1, 2}},
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
            DebugLogPieceList(Script.PieceInfo.Design);

            Assert.AreEqual((bool)TestCase[0], Utils.ComparePiecePointList((List<int[]>)TestCase[2], Script.PieceInfo.Design));
            yield return null;
        }
    }

    
    static object[] TestCaseRotate = 
    new object[] {
        new object[]
        {
            true,
            "P3-2",
            new List<int>{1},
            new List<int[]>{new int[]{0, 0}, new int[]{1, 0}, new int[]{2, 0}},
        },
        new object[]
        {
            true,
            "P3-2",
            new List<int>{-1},
            new List<int[]>{new int[]{0, 0}, new int[]{-1, 0}, new int[]{-2, 0}},
        },

        new object[]
        {
            true,
            "P5-6",
            new List<int>{1},
            new List<int[]>{new int[]{0, 0}, new int[]{1, 0}, new int[]{1, 1}, new int[]{1, -1}, new int[]{2, 1}},
        },
        new object[]
        {
            true,
            "P3-2",
            new List<int>{1, 1},
            new List<int[]>{new int[]{0, 0}, new int[]{0, -1}, new int[]{0, -2}},
        },
    };

    [UnityTest]
    public IEnumerator PieceDesignPTestRotate()
    {
        foreach (object[] TestCase in TestCaseRotate)
        {
            Debug.Log("TestCase" + (string)TestCase[1]);
            bool ExBool = (bool)TestCase[0];
            List<int> RotateList = (List<int>)TestCase[2];
            List<int[]> ExPieceList = (List<int[]>)TestCase[3];
            PrintList(RotateList);

            GameObject NewPiece = Object.Instantiate((GameObject)Resources.Load((string)TestCase[1]));
            NewPiece.transform.position = new Vector3(0f, 0.3f, 0f);
            yield return null;
            Piece Script = NewPiece.GetComponent<Piece>();

            foreach (int Rotate in RotateList)
            {
                Script.Rotate(Rotate);
            }
            
            Debug.Log("Ex");
            DebugLogPieceList(ExPieceList);
            Debug.Log("Act");
            DebugLogPieceList(Script.PieceInfo.Design);

            Assert.AreEqual(ExBool, Utils.ComparePiecePointList(ExPieceList, Script.PieceInfo.Design));
            yield return null;
        }
    }

    static object[] TestCaseReverseRotate = 
    new object[] {
        new object[]
        {
            true,
            "P3-2",
            new List<int>{1, 2},
            new List<int[]>{new int[]{0, 0}, new int[]{-1, 0}, new int[]{-2, 0}},
        },
        new object[]
        {
            true,
            "P5-1",
            new List<int>{1, 2},
            new List<int[]>{new int[]{0, 0}, new int[]{-1, 0}, new int[]{-2, 0}, new int[]{-3, 0}, new int[]{-3, 1}},
        },
        new object[]
        {
            true,
            "P5-3",
            new List<int>{},
            new List<int[]>{new int[]{0, 0}, new int[]{0, 1}, new int[]{0, 2}, new int[]{-1, 1}, new int[]{-1, 2}},
        },
        new object[]
        {
            true,
            "P5-3",
            new List<int>{2},
            new List<int[]>{new int[]{0, 0}, new int[]{0, 1}, new int[]{0, 2}, new int[]{1, 1}, new int[]{1, 2}},
        },
        new object[]
        {
            true,
            "P5-3",
            new List<int>{2, 1},
            new List<int[]>{new int[]{0, 0}, new int[]{1, 0}, new int[]{2, 0}, new int[]{1, -1}, new int[]{2, -1}},
        },
    };

    [UnityTest]
    public IEnumerator PieceDesignPTestReverseRotatee()
    {
        foreach (object[] TestCase in TestCaseReverseRotate)
        {
            Debug.Log("TestCase" + (string)TestCase[1]);
            bool ExBool = (bool)TestCase[0];
            List<int> RotateList = (List<int>)TestCase[2];
            List<int[]> ExPieceList = (List<int[]>)TestCase[3];
            PrintList(RotateList);

            GameObject NewPiece = Object.Instantiate((GameObject)Resources.Load((string)TestCase[1]));
            NewPiece.transform.position = new Vector3(0f, 0.3f, 0f);
            yield return null;
            Piece Script = NewPiece.GetComponent<Piece>();

            foreach (int Rotate in RotateList)
            {
                if (Rotate == 2)
                    Script.Reverse();
                else
                    Script.Rotate(Rotate);
            }
            
            Debug.Log("Ex");
            DebugLogPieceList(ExPieceList);
            Debug.Log("Act");
            DebugLogPieceList(Script.PieceInfo.Design);

            Assert.AreEqual(ExBool, Utils.ComparePiecePointList(ExPieceList, Script.PieceInfo.Design));
            yield return null;
        }
    }


    
    static object[] TestCaseReverse = 
    new object[] {
        new object[]
        {
            true,
            "P3-2",
            new List<int[]>{new int[]{0, 0}, new int[]{0, 1}, new int[]{0, 2}},
            new List<int[]>{new int[]{0, 0}, new int[]{0, 1}, new int[]{0, 2}},
        },
        new object[]
        {
            true,
            "P4-2",
            new List<int[]>{new int[]{0, 0}, new int[]{0, 1}, new int[]{0, 2}, new int[]{-1, 1}},
            new List<int[]>{new int[]{0, 0}, new int[]{0, 1}, new int[]{0, 2}, new int[]{1, 1}},
        },
    };

    [UnityTest]
    public IEnumerator PieceDesignPTestReverse()
    {
        foreach (object[] TestCase in TestCaseReverse)
        {
            Debug.Log("TestCase" + (string)TestCase[1]);
            bool ExBool = (bool)TestCase[0];
            List<int[]> ExBeforePieceList = (List<int[]>)TestCase[2];
            List<int[]> ExPieceList = (List<int[]>)TestCase[3];

            GameObject NewPiece = Object.Instantiate((GameObject)Resources.Load((string)TestCase[1]));
            NewPiece.transform.position = new Vector3(0f, 0.3f, 0f);
            yield return null;
            Piece Script = NewPiece.GetComponent<Piece>();

            DebugLogPieceList(ExBeforePieceList);
            DebugLogPieceList(Script.PieceInfo.Design);
            Assert.AreEqual(true, Utils.ComparePiecePointList(ExBeforePieceList, Script.PieceInfo.Design));

            Script.Reverse();
            
            Debug.Log("Ex");
            DebugLogPieceList(ExPieceList);
            Debug.Log("Act");
            DebugLogPieceList(Script.PieceInfo.Design);

            Assert.AreEqual(ExBool, Utils.ComparePiecePointList(ExPieceList, Script.PieceInfo.Design));
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

    void PrintList(List<int> Array)
    {
        foreach (int A in Array)
        {
            Debug.Log(A + ", ");
        }
    }
}
