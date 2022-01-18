using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PieceDesign
{
    static Dictionary<string, int[,]> DesignDict;

    public static List<int[]> ReturnDesign(GameObject ParentObj)
    {
        Vector3 Pivot = Vector3.zero;
        List<int[]> res = new List<int[]>();
        res.Add(new int[]{0,0});
        foreach (Transform ChildObj in ParentObj.transform)
        {
            if (ChildObj.gameObject.name == "Piece")
                Pivot = ChildObj.transform.position;
                break;
        }
        foreach (Transform ChildObj in ParentObj.transform)
        {
            if (ChildObj.gameObject.name == "Piece")
                continue;
            Vector3 Pos = (ChildObj.transform.position - Pivot) * 100;
            int[] ChildPoint = new int[2];
            ChildPoint[1] = (int)(Mathf.Round(Pos.z));
            ChildPoint[0] = (int)(Mathf.Round(Pos.x));
            res.Add(ChildPoint);
        }
        return res;
    }
}
