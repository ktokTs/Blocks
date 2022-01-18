using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UtilsTest
{
    // A Test behaves as an ordinary method
    [TestCase(new int[]{1, 3}, new int[]{1, 3}, true)]
    [TestCase(new int[]{1, 3}, new int[]{3, 1}, true)]
    [TestCase(new int[]{1, 3}, new int[]{3, 1, 2}, false)]
    [TestCase(new int[]{2, 1, 3, 2}, new int[]{3, 1, 2}, false)]
    [TestCase(new int[]{1, 1}, new int[]{1}, false)]
    [TestCase(new int[]{1, 1}, new int[]{1, 1}, true)]
    [TestCase(new int[]{1, 1}, new int[]{1, 1}, true)]
    public void UtilsTestCompareArrayT(int[] Ex, int[] Act, bool ExBool)
    {
        Assert.AreEqual(ExBool, Utils.CompareArray(Ex, Act));
    }

    static object[] TestCaseComparePiecePointList = 
    new object[] {
        new object[]
        {
            true,
            new List<int[]>{new int[]{1, 3}, new int[]{5, 3}},
            new List<int[]>{new int[]{1, 3}, new int[]{5, 3}},
        },

        new object[]
        {
            true,
            new List<int[]>{new int[]{1, 3}, new int[]{5, 3}},
            new List<int[]>{new int[]{5, 3}, new int[]{1, 3}},
        },

        new object[]
        {
            false,
            new List<int[]>{new int[]{1, 3}, new int[]{5, 3}},
            new List<int[]>{new int[]{1, 3}, new int[]{5, 3}, new int[]{2, 3}},
        }
    };

    [TestCaseSource(nameof(TestCaseComparePiecePointList))]
    public void UtilsTestComparePiecePointList(bool Res, List<int[]> Ex, List<int[]> Act)
    {
        Assert.AreEqual(Res, Utils.ComparePiecePointList(Ex, Act));
    }

    static object[] TestCaseCheckPiecePointDuplication = 
    new object[] {
        new object[]
        {
            false,
            new List<int[]>{new int[]{1, 3}, new int[]{5, 3}},
        },

        new object[]
        {
            true,
            new List<int[]>{new int[]{5, 3}, new int[]{5, 3}},
        },

        new object[]
        {
            false,
            new List<int[]>{new int[]{1, 3}, new int[]{5, 3}, new int[]{2, 3}},
        },

        new object[]
        {
            true,
            new List<int[]>{new int[]{1, 3}, new int[]{5, 3}, new int[]{1, 3}},
        }
    };

    [TestCaseSource(nameof(TestCaseCheckPiecePointDuplication))]
    public void UtilsTestCheckPiecePointDuplication(bool ExBool, List<int[]> Array)
    {
        Assert.AreEqual(ExBool, Utils.CheckPiecePointDuplication(Array));
    }

    [TestCase(new int[]{1, 3}, new int[]{1, 3}, true)]
    [TestCase(new int[]{1, 3}, new int[]{3, 1}, false)]
    public void UtilsTestComparePoint(int[] Ex, int[] Act, bool ExBool)
    {
        Assert.AreEqual(ExBool, Utils.ComparePiecePoint(Ex, Act));
    }

    [TestCase(true, new int[]{0, 0})]
    [TestCase(false, new int[]{-1, 0})]
    [TestCase(true, new int[]{13, 0})]
    [TestCase(false, new int[]{14, 0})]
    public void UtilsTestIsPieceInBoard(bool ExBool, int[] Input)
    {
        bool Res = Utils.IsPieceInBoard(Input);
        Assert.AreEqual(ExBool, Res);
    }
}
