using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class BoardPTest
{
    object[] TestCaseSetPiece = new object[]
    {
        new object[]
        {
            "SuccessTest",
            true,
            new List<int[]>
            {
                new int[]{2, 0},
                new int[]{0, 1},
                new int[]{13, 11},
                new int[]{12, 13},
                new int[]{2, 10},
            },
            new int[,]
            {
              {0,1,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {1,0,0,0,0,0,0,0,0,0,1,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,1},
              {0,0,0,0,0,0,0,0,0,0,0,1,0,0},
            },
        },
        new object[]
        {
            "FailTest",
            false,
            new List<int[]>
            {
                new int[]{2, 1},
                new int[]{0, 1},
                new int[]{13, 11},
                new int[]{12, 13},
                new int[]{2, 10},
            },
            new int[,]
            {
              {0,1,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,1,0,0,0},//
              {1,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,1},
              {0,0,0,0,0,0,0,0,0,0,0,1,0,0},
            },
        },
        new object[]
        {
            "IsNotPieceInBoardTest",
            false,
            new List<int[]>
            {
                new int[]{2, -1},
            },
            new int[,]
            {
            },
        },
    };

    [UnityTest]
    public IEnumerator BoardPTestSetPiece()
    {
        foreach (object[] TestCase in TestCaseSetPiece)
        {
            Debug.Log((string)TestCase[0]);
            bool ExBool = (bool)TestCase[1];
            List<int[]> Input = (List<int[]>)TestCase[2];
            int[,] ExBoard = (int[,])TestCase[3];

            SceneManager.LoadScene("Game");
            yield return null;
            GameObject BoardObj = GameObject.Find ("Board");
            Board BoardScript = BoardObj.GetComponent<Board>();

            bool Res = BoardScript.SetPiece(Input);
            if (Res == false)
            {
                Assert.AreEqual(ExBool, Res);
                continue;
            }
            Assert.AreEqual(ExBool, CompareBoard(ExBoard, BoardScript.BoardInfo));
        }
        yield return null;
    }



    object[] TestCaseSetPieceWithPivot = new object[]
    {
        new object[]
        {
            "SuccessTest",
            true,
            new List<int[]>
            {
                new int[]{0, 0},
                new int[]{0, 1},
            },
            new int[]{2, 1},
            new int[,]
            {
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,1,1,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            },
        },
        new object[]
        {
            "FailTest",
            false,
            new List<int[]>
            {
                new int[]{0, 0},
                new int[]{0, 1},
            },
            new int[]{2, 1},
            new int[,]
            {
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,1,1,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
              {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            },
        },
    };

    [UnityTest]
    public IEnumerator BoardPTestSetPieceWithPivot()
    {
        foreach (object[] TestCase in TestCaseSetPieceWithPivot)
        {
            Debug.Log((string)TestCase[0]);
            bool ExBool = (bool)TestCase[1];
            List<int[]> Input = (List<int[]>)TestCase[2];
            int[] Pivot = (int[])TestCase[3];
            int[,] ExBoard = (int[,])TestCase[4];

            SceneManager.LoadScene("Game");
            yield return null;
            GameObject BoardObj = GameObject.Find ("Board");
            Board BoardScript = BoardObj.GetComponent<Board>();

            bool Res = BoardScript.SetPiece(Input, Pivot);
            if (Res == false)
            {
                Assert.AreEqual(ExBool, Res);
                continue;
            }
            Assert.AreEqual(ExBool, CompareBoard(ExBoard, BoardScript.BoardInfo));
        }
        yield return null;
    }

    bool CompareBoard(int[,] Ex, int[,] Act)
    {
        int IndexY = 0;
        for (; IndexY < ConstList.BoardSize; IndexY++)
        {
            int IndexX = 0;
            for (; IndexX < ConstList.BoardSize; IndexX++)
            {
                if (Ex[IndexY, IndexX] != Act[IndexY, IndexX])
                {
                    Debug.Log(IndexX + "," + IndexY + " Ex:" + Ex[IndexY, IndexX] + " Act:" + Act[IndexY, IndexX]);
                    return false;
                }
            }
        }
        return true;
    }

    void PrintBoardInfo(int[,] Board)
    {
        int x = 0;
        for (; x < ConstList.BoardSize ;x++)
        {
            string PrintList = "";
            int y = 0;
            for (; y < ConstList.BoardSize ;y++)
            {
                PrintList += (Board[y, x]) + " ";
            }
            Debug.Log(PrintList);
        }
    }
}
