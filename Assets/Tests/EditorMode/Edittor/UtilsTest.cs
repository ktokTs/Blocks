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

   [Test]
   public void UtilsTestComparePoint1()
   {
       Assert.AreEqual(true, 
       Utils.ComparePiecePoint(new int[]{1, 3}, new int[]{1, 3}));
   } 

   [Test]
   public void UtilsTestComparePoint2()
   {
       Assert.AreEqual(false, 
       Utils.ComparePiecePoint(new int[]{1, 3}, new int[]{3, 1}));
   } 


   static object[] TestCase = 
   new object[] {
       new object[]
       {
           new int[]{1, 3},
           new int[]{1, 3},
           true
       },
 
       new object[]
       {
           new int[]{1, 3},
           new int[]{3, 1},
           false
       },
   };
   
   [TestCaseSource(nameof(TestCase))]
   public void UtilsTestComparePoint3(int[] Ex, int[] Act, bool ExBool)
   {
       Assert.AreEqual(ExBool, Utils.ComparePiecePoint(Ex, Act));
   } 
}
