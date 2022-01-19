using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConstList
{
    private static int boardSize = 14;
    public static int BoardSize
    {
        get
        {
            return boardSize;
        }
    }

    private static string[] pieceList = 
    new string[]{
        "P5-1",
        "P5-2",
        "P5-3",
        "P5-4",
        "P5-5",
        "P5-6",
        "P5-7",
        "P5-8",
        "P5-9",
        "P5-10",
        "P5-11",
        "P5-12",
        "P4-1",
        "P4-2",
        "P4-3",
        "P4-4",
        "P4-5",
        "P3-1",
        "P3-2",
        "P2-1",
        "P1-1",
    };
    public static string[] PieceList
    {
        get
        {
            return pieceList;
        }
    }
}
